using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NavMeshController : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agente;

    // --- Variables de Velocidad y Dificultad ---
    public float velocidadOriginal; // Esta será la velocidad base antes del multiplicador
    public float aceleracionOriginal; // ¡NUEVA! Aceleración base del NavMeshAgent
    public float velocidadAngularOriginal; // ¡NUEVA! Velocidad Angular base del NavMeshAgent

    public float factorReduccionVelocidad = 0.5f;
    public float duracionReduccionVelocidad = 3f;
    private bool velocidadReducida = false;
    private const string DifficultyPrefKey = "GameDifficulty";

    // Define los multiplicadores de velocidad, aceleración y velocidad angular para cada nivel de dificultad
    // Los arrays deben tener la misma longitud que las opciones de tu Dropdown (Fácil, Normal, Difícil)
    private float[] speedMultipliers = { 1.0f, 2.0f, 3.0f }; // Multiplicadores para la velocidad
    private float[] accelerationMultipliers = { 1.0f, 2.0f, 3.0f }; // ¡NUEVO! Multiplicadores para la aceleración
    private float[] angularSpeedMultipliers = { 1.0f, 2.0f, 3.0f }; // ¡NUEVO! Multiplicadores para la velocidad angular

    // --- Resto de tus variables ---
    private Dictionary<GameObject, int> toquesJugador = new Dictionary<GameObject, int>();
    public string tagJugador = "Player";
    public string nombreEscenaMenu = "MenuScene";
    public GameObject pantallaGameOver;
    public Text textoGameOver; // Si es TextMeshPro, debería ser TMP_Text

    private bool juegoPerdido = false;
    private bool juegoGanado = false;

    // --- Variables de Audio SFX ---
    public AudioSource audioSource;
    public AudioClip sonidoColisionEnemiga;
    public AudioClip sonidoPerdidaKamikaze;

    // --- Referencia a la Música de Fondo ---
    public AudioSource musicaDeFondoSource;


    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            // Inicializamos las velocidades originales con los valores del NavMeshAgent en el Inspector.
            // Esto asegura que los multiplicadores se apliquen sobre tus valores base deseados.
            velocidadOriginal = agente.speed;
            aceleracionOriginal = agente.acceleration; // ¡NUEVO!
            velocidadAngularOriginal = agente.angularSpeed; // ¡NUEVO!

            AplicarDificultad();
        }
        else
        {
            Debug.LogError("NavMeshController: No se encontró el componente NavMeshAgent en este GameObject.");
        }

        if (audioSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource para SFX en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }
        if (musicaDeFondoSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource de Música de Fondo en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }

        // Asegurarse de inicializar el diccionario para este GameObject
        if (!toquesJugador.ContainsKey(gameObject))
        {
            toquesJugador.Add(gameObject, 0);
        }

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(false);
        }
    }

    void Update()
    {
        // Solo perseguir si el juego NO ha terminado (ni perdido ni ganado)
        if (!juegoPerdido && !juegoGanado && objetivo != null && agente != null && agente.enabled && agente.isOnNavMesh)
        {
            agente.SetDestination(objetivo.position);
            // Mostrar la velocidad actual del NavMeshAgent en cada frame para verificar cambios
            Debug.Log($"Velocidad ACTUAL del agente {gameObject.name}: {agente.speed} | Accel: {agente.acceleration} | Ang. Speed: {agente.angularSpeed}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Solo reaccionar a colisiones si el juego NO ha terminado
        if (!juegoPerdido && !juegoGanado && collision.gameObject.CompareTag(tagJugador))
        {
            if (audioSource != null && sonidoColisionEnemiga != null)
            {
                audioSource.PlayOneShot(sonidoColisionEnemiga);
            }
            else if (audioSource == null) { Debug.LogWarning("NavMeshController: AudioSource es nulo, no se puede reproducir sonido de colisión."); }
            else if (sonidoColisionEnemiga == null) { Debug.LogWarning("NavMeshController: AudioClip de colisión es nulo, no se puede reproducir."); }

            // Asegurarse de que el GameObject esté en el diccionario
            if (!toquesJugador.ContainsKey(gameObject))
            {
                toquesJugador.Add(gameObject, 0);
            }
            toquesJugador[gameObject]++;
            Debug.Log(gameObject.name + ": Ha tocado al jugador " + toquesJugador[gameObject] + " veces.");

            if (toquesJugador[gameObject] >= 2)
            {
                Debug.Log(gameObject.name + ": Ha tocado al jugador 2 veces. Autodestruyéndose y notificando pérdida.");
                TerminarJuegoPorKamikaze();
                Destroy(gameObject);
            }
            else if (!velocidadReducida)
            {
                StartCoroutine(ReducirVelocidadTemporalmente());
            }
        }
    }

    IEnumerator ReducirVelocidadTemporalmente()
    {
        velocidadReducida = true;
        if (agente != null)
        {
            agente.speed *= factorReduccionVelocidad; // Reduce la velocidad del NavMeshAgent
            // No reducimos Accel y Angular Speed aquí para que la ralentización sea instantánea y perceptible
        }

        yield return new WaitForSeconds(duracionReduccionVelocidad);

        if (agente != null && !juegoPerdido && !juegoGanado) // Asegurarse de no restaurar velocidad si el juego terminó
        {
            // Restaura la velocidad, aceleración y velocidad angular a los valores de la dificultad
            agente.speed = velocidadOriginal * GetCurrentDifficultyMultiplier(speedMultipliers);
            agente.acceleration = aceleracionOriginal * GetCurrentDifficultyMultiplier(accelerationMultipliers); // ¡NUEVO!
            agente.angularSpeed = velocidadAngularOriginal * GetCurrentDifficultyMultiplier(angularSpeedMultipliers); // ¡NUEVO!
        }
        velocidadReducida = false;
    }

    void TerminarJuegoPorKamikaze()
    {
        juegoPerdido = true; // Marca el juego como perdido por kamikaze

        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        if (audioSource != null && sonidoPerdidaKamikaze != null)
        {
            audioSource.PlayOneShot(sonidoPerdidaKamikaze);
        }
        else if (audioSource == null) { Debug.LogWarning("NavMeshController: AudioSource es nulo, no se puede reproducir sonido de pérdida por kamikaze."); }
        else if (sonidoPerdidaKamikaze == null) { Debug.LogWarning("NavMeshController: AudioClip de pérdida por kamikaze es nulo, no se puede reproducir."); }

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            if (textoGameOver != null)
            {
                textoGameOver.text = "¡Has perdido por un Kamikaze!";
            }
        }
        else
        {
            Debug.LogError("Error: No se ha asignado la pantalla de Game Over en el Inspector.");
        }

        if (objetivo != null)
        {
            PlayerController playerController = objetivo.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
                Debug.Log("Movimiento del jugador desactivado.");
            }
            else
            {
                Debug.LogWarning("Advertencia: No se encontró el script PlayerController en el jugador.");
            }
        }

        // Detener a todos los agentes NavMesh existentes al perder
        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            if (agenteEnemigo != null && agenteEnemigo.enabled) // Asegurarse de que el objeto no haya sido destruido y esté activo
            {
                agenteEnemigo.isStopped = true;
                // También reseteamos su aceleración y velocidad angular para que se detengan "en seco"
                agenteEnemigo.acceleration = 0;
                agenteEnemigo.angularSpeed = 0;
            }
        }
    }

    // Nuevo método para ser llamado desde PlayerController cuando el juego se gana
    public void NotificarJuegoGanado()
    {
        juegoGanado = true; // Establece la bandera de ganado
        // Detener a todos los enemigos también
        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            if (agenteEnemigo != null && agenteEnemigo.enabled) // Asegurarse de que el objeto no haya sido destruido ya y esté activo
            {
                agenteEnemigo.isStopped = true;
                // También reseteamos su aceleración y velocidad angular
                agenteEnemigo.acceleration = 0;
                agenteEnemigo.angularSpeed = 0;
            }
        }
    }

    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // --- MÉTODO PARA APLICAR LA DIFICULTAD ---
    void AplicarDificultad()
    {
        if (agente == null)
        {
            Debug.LogError("NavMeshController: NavMeshAgent no está asignado al intentar aplicar dificultad.");
            return;
        }

        int savedDifficultyIndex = PlayerPrefs.GetInt(DifficultyPrefKey, 0);

        if (savedDifficultyIndex < 0 || savedDifficultyIndex >= speedMultipliers.Length)
        {
            Debug.LogWarning($"NavMeshController: Índice de dificultad guardado ({savedDifficultyIndex}) es inválido. Usando dificultad por defecto (A).");
            savedDifficultyIndex = 0;
        }

        // Aplicar los multiplicadores a las propiedades del NavMeshAgent
        agente.speed = velocidadOriginal * speedMultipliers[savedDifficultyIndex];
        agente.acceleration = aceleracionOriginal * accelerationMultipliers[savedDifficultyIndex]; // ¡NUEVO!
        agente.angularSpeed = velocidadAngularOriginal * angularSpeedMultipliers[savedDifficultyIndex]; // ¡NUEVO!

        Debug.Log($"NavMeshController ({gameObject.name}): Dificultad aplicada: Multiplicador x{speedMultipliers[savedDifficultyIndex]} (Velocidad), x{accelerationMultipliers[savedDifficultyIndex]} (Accel), x{angularSpeedMultipliers[savedDifficultyIndex]} (Ang. Speed). Velocidad final del NavMeshAgent: {agente.speed}, Accel: {agente.acceleration}, Ang. Speed: {agente.angularSpeed}");
    }

    // Método helper para obtener el multiplicador, ahora recibe el array de multiplicadores
    private float GetCurrentDifficultyMultiplier(float[] multipliersArray)
    {
        int savedDifficultyIndex = PlayerPrefs.GetInt(DifficultyPrefKey, 0);
        if (savedDifficultyIndex < 0 || savedDifficultyIndex >= multipliersArray.Length)
        {
            savedDifficultyIndex = 0;
        }
        return multipliersArray[savedDifficultyIndex];
    }
}
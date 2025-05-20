using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Mant�n este para otros elementos UI si los usas
using TMPro; // Necesario para TextMeshPro

public class NavMeshController : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agente;

    // --- Variables de Velocidad y Dificultad ---
    public float velocidadOriginal; // Esta ser� la velocidad base antes del multiplicador
    public float aceleracionOriginal; // Aceleraci�n base del NavMeshAgent
    public float velocidadAngularOriginal; // Velocidad Angular base del NavMeshAgent

    public float factorReduccionVelocidad = 0.5f;
    public float duracionReduccionVelocidad = 3f;
    private bool velocidadReducida = false;
    private const string DifficultyPrefKey = "GameDifficulty";

    // Define los multiplicadores de velocidad, aceleraci�n y velocidad angular para cada nivel de dificultad
    private float[] speedMultipliers = { 1.0f, 2.0f, 3.0f }; // Multiplicadores para la velocidad
    private float[] accelerationMultipliers = { 1.0f, 2.0f, 3.0f }; // Multiplicadores para la aceleraci�n
    private float[] angularSpeedMultipliers = { 1.0f, 2.0f, 3.0f }; // Multiplicadores para la velocidad angular

    // --- Resto de tus variables ---
    private Dictionary<GameObject, int> toquesJugador = new Dictionary<GameObject, int>();
    public string tagJugador = "Player";
    public string nombreEscenaMenu = "MenuScene";
    public GameObject pantallaGameOver;
    public TMP_Text textoGameOver; // �CORREGIDO a TMP_Text para consistencia con tu uso de TMPro!

    private bool juegoPerdido = false;
    private bool juegoGanado = false;

    // --- Variables de Audio SFX ---
    public AudioSource audioSource;
    public AudioClip sonidoColisionEnemiga;
    public AudioClip sonidoPerdidaKamikaze;

    // --- Referencia a la M�sica de Fondo ---
    public AudioSource musicaDeFondoSource;

    // --- NUEVA VARIABLE: Prefab del Sistema de Part�culas ---
    // Aseg�rate de arrastrar tu Prefab de part�culas aqu� desde la carpeta Assets.
    public GameObject particulasColisionPrefab;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            // Inicializamos las velocidades originales con los valores del NavMeshAgent en el Inspector.
            // Esto asegura que los multiplicadores se apliquen sobre tus valores base deseados.
            velocidadOriginal = agente.speed;
            aceleracionOriginal = agente.acceleration;
            velocidadAngularOriginal = agente.angularSpeed;

            AplicarDificultad(); // Aplica la dificultad al inicio
        }
        else
        {
            Debug.LogError("NavMeshController: No se encontr� el componente NavMeshAgent en este GameObject.");
        }

        if (audioSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource para SFX en el Inspector. Arrastra el GameObject 'AudioPlayer' aqu�.");
        }
        if (musicaDeFondoSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource de M�sica de Fondo en el Inspector. Arrastra el GameObject 'AudioPlayer' aqu�.");
        }

        // --- Verificaci�n del Prefab de Part�culas ---
        if (particulasColisionPrefab == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el Prefab de Part�culas de Colisi�n en el Inspector. Crea un Particle System, config�ralo y arr�stralo como Prefab aqu�.");
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
        // Solo perseguir si el juego NO ha terminado (ni perdido ni ganado) y el agente es v�lido
        if (!juegoPerdido && !juegoGanado && objetivo != null && agente != null && agente.enabled && agente.isOnNavMesh)
        {
            agente.SetDestination(objetivo.position);
            // El Debug.Log de velocidad actual ya lo ten�as, lo dejo si lo necesitas para depuraci�n
            // Debug.Log($"Velocidad ACTUAL del agente {gameObject.name}: {agente.speed} | Accel: {agente.acceleration} | Ang. Speed: {agente.angularSpeed}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Solo reaccionar a colisiones si el juego NO ha terminado y el objeto colisionado es el jugador
        if (!juegoPerdido && !juegoGanado && collision.gameObject.CompareTag(tagJugador))
        {
            // Reproducir sonido de colisi�n
            if (audioSource != null && sonidoColisionEnemiga != null)
            {
                audioSource.PlayOneShot(sonidoColisionEnemiga);
            }
            else if (audioSource == null) { Debug.LogWarning("NavMeshController: AudioSource es nulo, no se puede reproducir sonido de colisi�n."); }
            else if (sonidoColisionEnemiga == null) { Debug.LogWarning("NavMeshController: AudioClip de colisi�n es nulo, no se puede reproducir."); }

            // --- Instanciar y Reproducir Part�culas de Colisi�n (NUEVO) ---
            if (particulasColisionPrefab != null)
            {
                // Obtener el punto de contacto de la colisi�n para posicionar las part�culas
                // Collision.contacts[0].point devuelve el primer punto de contacto de la colisi�n.
                Vector3 puntoDeContacto = collision.contacts[0].point;

                // Instanciar el prefab de part�culas en la posici�n del contacto
                GameObject particulasInstancia = Instantiate(particulasColisionPrefab, puntoDeContacto, Quaternion.identity);

                // Obtener el componente ParticleSystem de la instancia creada
                ParticleSystem ps = particulasInstancia.GetComponent<ParticleSystem>();

                if (ps != null)
                {
                    ps.Play(); // Asegurarse de que el sistema de part�culas se reproduzca
                    // Destruir el GameObject de las part�culas despu�s de que haya terminado su duraci�n principal
                    Destroy(particulasInstancia, ps.main.duration);
                }
                else
                {
                    Debug.LogWarning("NavMeshController: El Prefab de part�culas de colisi�n no tiene un componente ParticleSystem.");
                    // Si no tiene ParticleSystem, destr�yelo de todas formas despu�s de un corto tiempo
                    Destroy(particulasInstancia, 2f);
                }
            }
            // --- Fin de la l�gica de Part�culas ---


            // Asegurarse de que el GameObject est� en el diccionario antes de acceder a �l
            if (!toquesJugador.ContainsKey(gameObject))
            {
                toquesJugador.Add(gameObject, 0);
            }
            toquesJugador[gameObject]++;
            Debug.Log(gameObject.name + ": Ha tocado al jugador " + toquesJugador[gameObject] + " veces.");

            if (toquesJugador[gameObject] >= 2)
            {
                Debug.Log(gameObject.name + ": Ha tocado al jugador 2 veces. Autodestruy�ndose y notificando p�rdida.");
                TerminarJuegoPorKamikaze();
                Destroy(gameObject); // Destruye el enemigo kamikaze
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
            // Reduce la velocidad del NavMeshAgent. Solo la velocidad para un efecto instant�neo de lentitud.
            agente.speed *= factorReduccionVelocidad;
        }

        yield return new WaitForSeconds(duracionReduccionVelocidad);

        // Restaurar la velocidad, aceleraci�n y velocidad angular si el juego no ha terminado
        if (agente != null && !juegoPerdido && !juegoGanado)
        {
            agente.speed = velocidadOriginal * GetCurrentDifficultyMultiplier(speedMultipliers);
            agente.acceleration = aceleracionOriginal * GetCurrentDifficultyMultiplier(accelerationMultipliers);
            agente.angularSpeed = velocidadAngularOriginal * GetCurrentDifficultyMultiplier(angularSpeedMultipliers);
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
        else if (audioSource == null) { Debug.LogWarning("NavMeshController: AudioSource es nulo, no se puede reproducir sonido de p�rdida por kamikaze."); }
        else if (sonidoPerdidaKamikaze == null) { Debug.LogWarning("NavMeshController: AudioClip de p�rdida por kamikaze es nulo, no se puede reproducir."); }

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            if (textoGameOver != null)
            {
                textoGameOver.text = "�Has perdido por un Kamikaze!";
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
                playerController.enabled = false; // Desactiva el movimiento del jugador
                Debug.Log("Movimiento del jugador desactivado.");
            }
            else
            {
                Debug.LogWarning("Advertencia: No se encontr� el script PlayerController en el jugador.");
            }
        }

        // Detener a todos los agentes NavMesh existentes al perder
        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            if (agenteEnemigo != null && agenteEnemigo.enabled)
            {
                agenteEnemigo.isStopped = true;
                agenteEnemigo.acceleration = 0; // Detenerlos de forma m�s abrupta
                agenteEnemigo.angularSpeed = 0;
            }
        }
    }

    // M�todo para ser llamado desde PlayerController cuando el juego se gana
    public void NotificarJuegoGanado()
    {
        juegoGanado = true; // Establece la bandera de ganado
        // Detener a todos los enemigos tambi�n
        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            if (agenteEnemigo != null && agenteEnemigo.enabled)
            {
                agenteEnemigo.isStopped = true;
                agenteEnemigo.acceleration = 0;
                agenteEnemigo.angularSpeed = 0;
            }
        }
    }

    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // --- M�TODO PARA APLICAR LA DIFICULTAD ---
    void AplicarDificultad()
    {
        if (agente == null)
        {
            Debug.LogError("NavMeshController: NavMeshAgent no est� asignado al intentar aplicar dificultad.");
            return;
        }

        int savedDifficultyIndex = PlayerPrefs.GetInt(DifficultyPrefKey, 0);

        if (savedDifficultyIndex < 0 || savedDifficultyIndex >= speedMultipliers.Length)
        {
            Debug.LogWarning($"NavMeshController: �ndice de dificultad guardado ({savedDifficultyIndex}) es inv�lido. Usando dificultad por defecto (0).");
            savedDifficultyIndex = 0;
        }

        // Aplicar los multiplicadores a las propiedades del NavMeshAgent
        agente.speed = velocidadOriginal * speedMultipliers[savedDifficultyIndex];
        agente.acceleration = aceleracionOriginal * accelerationMultipliers[savedDifficultyIndex];
        agente.angularSpeed = velocidadAngularOriginal * angularSpeedMultipliers[savedDifficultyIndex];

        // Debug.Log para verificar los valores aplicados (lo ten�as, lo dejo si es para depuraci�n)
        // Debug.Log($"NavMeshController ({gameObject.name}): Dificultad aplicada: Multiplicador x{speedMultipliers[savedDifficultyIndex]} (Velocidad), x{accelerationMultipliers[savedDifficultyIndex]} (Accel), x{angularSpeedMultipliers[savedDifficultyIndex]} (Ang. Speed). Velocidad final del NavMeshAgent: {agente.speed}, Accel: {agente.acceleration}, Ang. Speed: {agente.angularSpeed}");
    }

    // M�todo helper para obtener el multiplicador, ahora recibe el array de multiplicadores
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
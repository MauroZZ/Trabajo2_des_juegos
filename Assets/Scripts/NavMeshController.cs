using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavMeshController : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agente;
    public float velocidadOriginal;
    public float factorReduccionVelocidad = 0.5f;
    public float duracionReduccionVelocidad = 3f;
    private bool velocidadReducida = false;
    private Dictionary<GameObject, int> toquesJugador = new Dictionary<GameObject, int>();
    public string tagJugador = "Player";
    public string nombreEscenaMenu = "MenuScene";
    public GameObject pantallaGameOver;
    public Text textoGameOver;

    // Cambiar 'juegoTerminado' por 'juegoPerdido' para mayor claridad
    private bool juegoPerdido = false; // Indica si se perdió por kamikaze o tiempo
    private bool juegoGanado = false; // Agrega una bandera para saber si el juego ya fue ganado

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
            velocidadOriginal = agente.speed;
        }

        if (audioSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource para SFX en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }
        if (musicaDeFondoSource == null)
        {
            Debug.LogError("NavMeshController: No se ha asignado el AudioSource de Música de Fondo en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }

        toquesJugador[gameObject] = 0;

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(false);
        }
    }

    void Update()
    {
        // Solo perseguir si el juego NO ha terminado (ni perdido ni ganado)
        if (!juegoPerdido && !juegoGanado && objetivo != null && agente != null)
        {
            agente.SetDestination(objetivo.position);
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
            agente.speed *= factorReduccionVelocidad;
        }

        yield return new WaitForSeconds(duracionReduccionVelocidad);

        if (agente != null)
        {
            agente.speed = velocidadOriginal;
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

        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            agenteEnemigo.isStopped = true;
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
            if (agenteEnemigo != null) // Asegurarse de que el objeto no haya sido destruido ya
            {
                agenteEnemigo.isStopped = true;
                // Opcional: destruir enemigos o deshabilitar su componente
                // Destroy(agenteEnemigo.gameObject);
            }
        }
    }


    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
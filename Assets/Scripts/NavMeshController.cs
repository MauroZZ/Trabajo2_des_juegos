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

    private bool juegoTerminado = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            velocidadOriginal = agente.speed;
        }

        toquesJugador[gameObject] = 0;

        // Asegurarse de que la pantalla de Game Over esté desactivada al inicio
        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(false);
        }
    }

    void Update()
    {
        if (!juegoTerminado && objetivo != null && agente != null)
        {
            agente.SetDestination(objetivo.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!juegoTerminado && collision.gameObject.CompareTag(tagJugador))
        {
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
        juegoTerminado = true;

        // Mostrar la pantalla de Game Over
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

        // Desactivar el movimiento del jugador
        if (objetivo != null)
        {
            PlayerController playerController = objetivo.GetComponent<PlayerController>(); // Obtener el script PlayerController
            if (playerController != null)
            {
                playerController.enabled = false; // Desactivar el script
                Debug.Log("Movimiento del jugador desactivado.");
            }
            else
            {
                Debug.LogWarning("Advertencia: No se encontró el script PlayerController en el jugador.");
            }
        }

        // Detener a todos los enemigos (opcional)
        NavMeshAgent[] todosAgentes = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agenteEnemigo in todosAgentes)
        {
            agenteEnemigo.isStopped = true;
        }
    }

    // Método para el botón de reinicio en la pantalla de Game Over
    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
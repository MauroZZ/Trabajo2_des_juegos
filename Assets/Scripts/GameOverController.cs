using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public string nombreEscenaMenu = "MenuScene"; // Aseg�rate de que coincida con el nombre de tu escena del men�

    public void ReiniciarPartida()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Carga la escena actual (el juego)
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // Puedes a�adir otras funciones aqu� si lo necesitas
}
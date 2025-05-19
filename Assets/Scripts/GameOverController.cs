using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public string nombreEscenaMenu = "MenuScene"; // Asegúrate de que coincida con el nombre de tu escena del menú

    public void ReiniciarPartida()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Carga la escena actual (el juego)
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // Puedes añadir otras funciones aquí si lo necesitas
}
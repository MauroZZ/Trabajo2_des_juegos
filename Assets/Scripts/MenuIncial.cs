using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuIncial : MonoBehaviour
{
    public GameObject optionsPanel;
    public TMP_InputField playerNameInputField;

    private const string PlayerNamePrefKey = "PlayerName"; // Clave para guardar/cargar el nombre

    void Start()
    {
        // Asegurarse de que el Panel de Opciones est� desactivado al inicio
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        // Cargar el nombre guardado al iniciar el men�
        CargarNombreJugador();
    }

    public void jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    // --- M�todos para la pantalla de Opciones ---

    public void AbrirOpciones()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            CargarNombreJugador(); // Recargar el nombre por si se cambi� en otra sesi�n
        }
    }

    public void CerrarOpciones()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            // Si el bot�n "Aplicar Nombre" ya llama a GuardarNombreJugador(), puedes optar por no guardarlo aqu� tambi�n
            // GuardarNombreJugador(); // Esto guarda al cerrar, decide si lo quieres o si solo el bot�n "Aplicar" guarda
        }
    }

    public void GuardarNombreJugador()
    {
        if (playerNameInputField != null)
        {
            // Guarda el nombre del jugador
            PlayerPrefs.SetString(PlayerNamePrefKey, playerNameInputField.text);
            PlayerPrefs.Save(); // Guarda los PlayerPrefs inmediatamente

            // Mensaje de �xito en la consola
            Debug.Log($"Nombre '{playerNameInputField.text}' guardado correctamente.");
        }
        else
        {
            Debug.LogError("MenuInicial: playerNameInputField no est� asignado. No se pudo guardar el nombre.");
        }
    }

    private void CargarNombreJugador()
    {
        if (playerNameInputField != null)
        {
            string savedName = PlayerPrefs.GetString(PlayerNamePrefKey, ""); // Carga el nombre, por defecto vac�o
            playerNameInputField.text = savedName; // Muestra el nombre cargado en el InputField
            Debug.Log("Nombre del jugador cargado: " + savedName);
        }
        else
        {
            Debug.LogError("MenuInicial: playerNameInputField no est� asignado al cargar.");
        }
    }
}
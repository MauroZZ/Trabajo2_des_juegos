using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuIncial : MonoBehaviour
{
    public GameObject optionsPanel;
    public TMP_InputField playerNameInputField;

    // --- Nuevas variables para la dificultad ---
    public TMP_Dropdown difficultyDropdown; // Asigna tu Dropdown de dificultad en el Inspector
    private const string DifficultyPrefKey = "GameDifficulty"; // Clave para guardar la dificultad

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
        // Cargar la dificultad guardada al iniciar el men�
        CargarDificultad(); // Aseg�rate de que este m�todo existe y se llama aqu�
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
            CargarDificultad();   // Recargar la dificultad por si se cambi� en otra sesi�n
        }
    }

    public void CerrarOpciones()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            // Si el bot�n "Aplicar Nombre" ya llama a GuardarNombreJugador(), puedes optar por no guardarlo aqu� tambi�n
            // GuardarNombreJugador(); // Esto guarda al cerrar, decide si lo quieres o si solo el bot�n "Aplicar" guarda
            GuardarDificultad(); // Guardamos la dificultad al cerrar las opciones (si no se ha guardado ya con el OnValueChanged)
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

    // --- M�todos para la Dificultad ---

    public void GuardarDificultad()
    {
        if (difficultyDropdown != null)
        {
            // Guarda el �ndice seleccionado (0 para A, 1 para B, 2 para C)
            int selectedDifficultyIndex = difficultyDropdown.value;
            PlayerPrefs.SetInt(DifficultyPrefKey, selectedDifficultyIndex);
            PlayerPrefs.Save(); // Guarda los PlayerPrefs inmediatamente

            string selectedOptionText = difficultyDropdown.options[selectedDifficultyIndex].text;

            // Mensaje de confirmaci�n en la consola
            Debug.Log($"Dificultad seleccionada: '{selectedOptionText}'. Se ha guardado el ajuste de dificultad.");

            // Puedes a�adir l�gica aqu� para mostrar un mensaje en pantalla al usuario, si lo deseas.
            // Por ejemplo: ShowMessage("Dificultad ajustada a " + selectedOptionText);
        }
        else
        {
            Debug.LogError("MenuInicial: difficultyDropdown no est� asignado. No se pudo guardar la dificultad.");
        }
    }

    private void CargarDificultad()
    {
        if (difficultyDropdown != null)
        {
            // Carga el �ndice guardado, por defecto 0 (Opci�n A)
            int savedDifficultyIndex = PlayerPrefs.GetInt(DifficultyPrefKey, 0);

            // Asegurarse de que el �ndice sea v�lido para las opciones del Dropdown
            if (savedDifficultyIndex < 0 || savedDifficultyIndex >= difficultyDropdown.options.Count)
            {
                Debug.LogWarning($"Dificultad guardada ({savedDifficultyIndex}) fuera de rango. Estableciendo a 'A'.");
                savedDifficultyIndex = 0; // Por defecto a la primera opci�n si es inv�lido
            }

            difficultyDropdown.value = savedDifficultyIndex; // Establece la opci�n del Dropdown
            string loadedOptionText = difficultyDropdown.options[savedDifficultyIndex].text;
            Debug.Log($"Dificultad cargada: '{loadedOptionText}' (�ndice: {savedDifficultyIndex}).");
        }
        else
        {
            Debug.LogError("MenuInicial: difficultyDropdown no est� asignado al cargar.");
        }
    }
}
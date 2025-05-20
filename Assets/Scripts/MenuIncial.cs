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
        // Asegurarse de que el Panel de Opciones esté desactivado al inicio
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        // Cargar el nombre guardado al iniciar el menú
        CargarNombreJugador();
        // Cargar la dificultad guardada al iniciar el menú
        CargarDificultad(); // Asegúrate de que este método existe y se llama aquí
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

    // --- Métodos para la pantalla de Opciones ---

    public void AbrirOpciones()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            CargarNombreJugador(); // Recargar el nombre por si se cambió en otra sesión
            CargarDificultad();   // Recargar la dificultad por si se cambió en otra sesión
        }
    }

    public void CerrarOpciones()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            // Si el botón "Aplicar Nombre" ya llama a GuardarNombreJugador(), puedes optar por no guardarlo aquí también
            // GuardarNombreJugador(); // Esto guarda al cerrar, decide si lo quieres o si solo el botón "Aplicar" guarda
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

            // Mensaje de éxito en la consola
            Debug.Log($"Nombre '{playerNameInputField.text}' guardado correctamente.");
        }
        else
        {
            Debug.LogError("MenuInicial: playerNameInputField no está asignado. No se pudo guardar el nombre.");
        }
    }

    private void CargarNombreJugador()
    {
        if (playerNameInputField != null)
        {
            string savedName = PlayerPrefs.GetString(PlayerNamePrefKey, ""); // Carga el nombre, por defecto vacío
            playerNameInputField.text = savedName; // Muestra el nombre cargado en el InputField
            Debug.Log("Nombre del jugador cargado: " + savedName);
        }
        else
        {
            Debug.LogError("MenuInicial: playerNameInputField no está asignado al cargar.");
        }
    }

    // --- Métodos para la Dificultad ---

    public void GuardarDificultad()
    {
        if (difficultyDropdown != null)
        {
            // Guarda el índice seleccionado (0 para A, 1 para B, 2 para C)
            int selectedDifficultyIndex = difficultyDropdown.value;
            PlayerPrefs.SetInt(DifficultyPrefKey, selectedDifficultyIndex);
            PlayerPrefs.Save(); // Guarda los PlayerPrefs inmediatamente

            string selectedOptionText = difficultyDropdown.options[selectedDifficultyIndex].text;

            // Mensaje de confirmación en la consola
            Debug.Log($"Dificultad seleccionada: '{selectedOptionText}'. Se ha guardado el ajuste de dificultad.");

            // Puedes añadir lógica aquí para mostrar un mensaje en pantalla al usuario, si lo deseas.
            // Por ejemplo: ShowMessage("Dificultad ajustada a " + selectedOptionText);
        }
        else
        {
            Debug.LogError("MenuInicial: difficultyDropdown no está asignado. No se pudo guardar la dificultad.");
        }
    }

    private void CargarDificultad()
    {
        if (difficultyDropdown != null)
        {
            // Carga el índice guardado, por defecto 0 (Opción A)
            int savedDifficultyIndex = PlayerPrefs.GetInt(DifficultyPrefKey, 0);

            // Asegurarse de que el índice sea válido para las opciones del Dropdown
            if (savedDifficultyIndex < 0 || savedDifficultyIndex >= difficultyDropdown.options.Count)
            {
                Debug.LogWarning($"Dificultad guardada ({savedDifficultyIndex}) fuera de rango. Estableciendo a 'A'.");
                savedDifficultyIndex = 0; // Por defecto a la primera opción si es inválido
            }

            difficultyDropdown.value = savedDifficultyIndex; // Establece la opción del Dropdown
            string loadedOptionText = difficultyDropdown.options[savedDifficultyIndex].text;
            Debug.Log($"Dificultad cargada: '{loadedOptionText}' (Índice: {savedDifficultyIndex}).");
        }
        else
        {
            Debug.LogError("MenuInicial: difficultyDropdown no está asignado al cargar.");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Mantén este para otros elementos UI si los usas, aunque para Text será TMPro
using TMPro; // ¡NUEVO! Necesario para trabajar con TextMeshPro

public class PlayerController : MonoBehaviour
{
    // --- Variables de Movimiento y Control del Jugador ---
    public float horizontalMove;
    public float verticalMove;
    public CharacterController player;
    public float PlayerSpeed = 5f;
    public float gravity = 9.8f;
    public float jumpHeight = 2f;

    private Vector3 playerInput;
    private Vector3 moveDirection;
    private Vector3 verticalVelocity;

    // --- Variables de la Cámara ---
    public Camera MainCamera;
    [Tooltip("Altura de la cámara sobre el jugador.")]
    public float cameraHeight = 10f;
    [Tooltip("Suavidad con la que la cámara sigue al jugador (0-1).")]
    [Range(0f, 1f)]
    public float cameraFollowSpeed = 0.1f;
    private Vector3 cameraTargetPosition;

    // --- Variables de Temporizador y Game Over ---
    public float tiempoDeJuego = 15f;
    private float tiempoRestante;
    public GameObject pantallaGameOver;
    public string mensajeGameOverPorTiempo = "¡Tiempo agotado!";
    public TMP_Text textoGameOver;

    public TMP_Text temporizadorDisplay;

    private bool juegoTerminadoPorTiempo = false;

    // --- Variables de Audio SFX ---
    public AudioSource audioSource;
    public AudioClip sonidoSalto;
    public AudioClip sonidoPerdidaTiempo;

    // --- Referencia a la Música de Fondo ---
    public AudioSource musicaDeFondoSource;

    // --- Variables para la Victoria ---
    public GameObject pantallaVictoria; // El panel principal de victoria
    public TMP_Text tiempoFinalText; // El TMP_Text para el tiempo
    public TMP_Text nombreJugadorFinalText; // El TMP_Text para el nombre del jugador

    private float tiempoInicioJuego;
    private bool juegoGanado = false;

    // ¡CLAVE CORREGIDA! DEBE COINCIDIR CON LA DE MenuInicial.cs
    private const string NombreJugadorPrefKey = "PlayerName"; //


    void Start()
    {
        player = GetComponent<CharacterController>();

        if (MainCamera == null)
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                Debug.LogError("PlayerController: No se encontró la Main Camera en la escena. Asegúrate de que una cámara tenga el tag 'MainCamera'.");
                enabled = false;
                return;
            }
        }

        if (audioSource == null)
        {
            Debug.LogError("PlayerController: No se ha asignado el AudioSource para SFX en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }
        if (musicaDeFondoSource == null)
        {
            Debug.LogError("PlayerController: No se ha asignado el AudioSource de Música de Fondo en el Inspector. Arrastra el GameObject 'AudioPlayer' aquí.");
        }

        if (MainCamera != null)
        {
            MainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        tiempoRestante = tiempoDeJuego;
        tiempoInicioJuego = Time.time;

        if (temporizadorDisplay == null)
        {
            Debug.LogError("PlayerController: No se ha asignado el TemporizadorDisplay (TMP_Text UI) en el Inspector.");
        }
        ActualizarTemporizadorDisplay();

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(false);
        }
        else
        {
            Debug.LogError("PlayerController: No se ha asignado la pantalla de Game Over en el PlayerController.");
        }

        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(false);
        }
        else
        {
            Debug.LogError("PlayerController: No se ha asignado la pantalla de Victoria en el PlayerController.");
        }

        // Verificar asignación de textos de victoria
        if (tiempoFinalText == null) Debug.LogError("PlayerController: No se ha asignado 'Tiempo Final Text' para la pantalla de victoria.");
        if (nombreJugadorFinalText == null) Debug.LogError("PlayerController: No se ha asignado 'Nombre Jugador Final Text' para la pantalla de victoria.");
    }

    void Update()
    {
        if (!juegoTerminadoPorTiempo && !juegoGanado)
        {
            horizontalMove = Input.GetAxis("Horizontal");
            verticalMove = Input.GetAxis("Vertical");

            playerInput = new Vector3(horizontalMove, 0, verticalMove);
            playerInput = Vector3.ClampMagnitude(playerInput, 1);

            moveDirection = playerInput * PlayerSpeed;

            if (moveDirection.magnitude > 0.1f)
            {
                player.transform.LookAt(player.transform.position + new Vector3(moveDirection.x, 0f, moveDirection.z));
            }

            PlayerSkills();
            ApplyGravity();

            Vector3 finalMove = moveDirection + verticalVelocity;
            player.Move(finalMove * Time.deltaTime);

            if (MainCamera != null)
            {
                cameraTargetPosition = new Vector3(transform.position.x, cameraHeight, transform.position.z);
                MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, cameraTargetPosition, cameraFollowSpeed);
            }

            tiempoRestante -= Time.deltaTime;
            ActualizarTemporizadorDisplay();

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                ActualizarTemporizadorDisplay();
                TerminarJuegoPorTiempo();
            }
        }
    }

    void ApplyGravity()
    {
        if (player.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
        else
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }
    }

    public void PlayerSkills()
    {
        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);

            if (audioSource != null && sonidoSalto != null)
            {
                audioSource.PlayOneShot(sonidoSalto);
            }
            else if (audioSource == null) { Debug.LogWarning("PlayerController: AudioSource para SFX es nulo, no se puede reproducir sonido de salto."); }
            else if (sonidoSalto == null) { Debug.LogWarning("PlayerController: AudioClip de salto es nulo, no se puede reproducir."); }
        }
    }

    void ActualizarTemporizadorDisplay()
    {
        if (temporizadorDisplay != null)
        {
            int segundos = Mathf.CeilToInt(tiempoRestante);
            temporizadorDisplay.text = "Tiempo: " + segundos.ToString();
        }
    }

    void TerminarJuegoPorTiempo()
    {
        juegoTerminadoPorTiempo = true;
        Debug.Log("¡Tiempo agotado! Game Over.");

        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        if (audioSource != null && sonidoPerdidaTiempo != null)
        {
            audioSource.PlayOneShot(sonidoPerdidaTiempo);
        }
        else if (audioSource == null) { Debug.LogWarning("PlayerController: AudioSource para SFX es nulo, no se puede reproducir sonido de pérdida por tiempo."); }
        else if (sonidoPerdidaTiempo == null) { Debug.LogWarning("PlayerController: AudioClip de pérdida por tiempo es nulo, no se puede reproducir."); }

        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true); //
            if (textoGameOver != null)
            {
                textoGameOver.text = mensajeGameOverPorTiempo;
            }
        }

        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Meta") && !juegoGanado && !juegoTerminadoPorTiempo)
        {
            Debug.Log("¡Meta alcanzada! Has ganado.");
            TerminarJuegoPorVictoria();
        }
    }

    void TerminarJuegoPorVictoria()
    {
        juegoGanado = true;
        enabled = false; // Desactiva este script para detener el movimiento y la lógica del juego

        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        float tiempoTranscurrido = Time.time - tiempoInicioJuego;
        Debug.Log("Tiempo utilizado: " + tiempoTranscurrido.ToString("F2") + " segundos.");

        // Carga el nombre del jugador usando la clave corregida
        string nombreJugador = PlayerPrefs.GetString(NombreJugadorPrefKey, "Jugador Desconocido"); //
        Debug.Log("DEBUG (PlayerController): Nombre cargado para victoria: " + nombreJugador); // Log para verificar la carga

        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(true); // Activa el panel de victoria

            if (tiempoFinalText != null)
            {
                tiempoFinalText.text = "Tiempo: " + tiempoTranscurrido.ToString("F2") + " segundos";
            }
            if (nombreJugadorFinalText != null)
            {
                nombreJugadorFinalText.text = "Jugador: " + nombreJugador; // Asigna el nombre al texto
            }
        }
        else
        {
            Debug.LogError("PlayerController: La pantalla de victoria (GameObject) no está asignada en el Inspector.");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

    // --- Variables de la C�mara ---
    public Camera MainCamera;
    [Tooltip("Altura de la c�mara sobre el jugador.")]
    public float cameraHeight = 10f;
    [Tooltip("Suavidad con la que la c�mara sigue al jugador (0-1).")]
    [Range(0f, 1f)]
    public float cameraFollowSpeed = 0.1f;
    private Vector3 cameraTargetPosition;

    // --- Variables de Temporizador y Game Over ---
    public float tiempoDeJuego = 15f;
    private float tiempoRestante;
    public GameObject pantallaGameOver; // Panel/Canvas de Game Over
    public string mensajeGameOverPorTiempo = "�Tiempo agotado!";
    public TMP_Text textoGameOver;      // Texto TMP_Text en la pantalla de Game Over
    public TMP_Text temporizadorDisplay;

    private bool juegoTerminadoPorTiempo = false;
    private bool juegoPerdidoPorLava = false; // �NUEVA BANDERA! Para la muerte por lava

    // --- Variables de Audio SFX ---
    public AudioSource audioSource;
    public AudioClip sonidoSalto;
    public AudioClip sonidoPerdidaTiempo;
    public AudioClip sonidoMuerteLava; // �NUEVO! AudioClip para la muerte por lava

    // --- Referencia a la M�sica de Fondo ---
    public AudioSource musicaDeFondoSource;

    // --- Variables para la Victoria ---
    public GameObject pantallaVictoria; // El panel principal de victoria
    public TMP_Text tiempoFinalText; // El TMP_Text para el tiempo
    public TMP_Text nombreJugadorFinalText; // El TMP_Text para el nombre del jugador

    private float tiempoInicioJuego;
    private bool juegoGanado = false;

    // �CLAVE CORREGIDA! DEBE COINCIDIR CON LA DE MenuInicial.cs
    private const string NombreJugadorPrefKey = "PlayerName";

    void Start()
    {
        player = GetComponent<CharacterController>();

        if (MainCamera == null)
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                Debug.LogError("PlayerController: No se encontr� la Main Camera en la escena. Aseg�rate de que una c�mara tenga el tag 'MainCamera'.");
                enabled = false;
                return;
            }
        }

        if (audioSource == null)
        {
            Debug.LogError("PlayerController: No se ha asignado el AudioSource para SFX en el Inspector. Arrastra el GameObject 'AudioPlayer' aqu�.");
        }
        if (musicaDeFondoSource == null)
        {
            Debug.LogError("PlayerController: No se ha asignado el AudioSource de M�sica de Fondo en el Inspector. Arrastra el GameObject 'AudioPlayer' aqu�.");
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

        // Verificar asignaci�n de textos de victoria
        if (tiempoFinalText == null) Debug.LogError("PlayerController: No se ha asignado 'Tiempo Final Text' para la pantalla de victoria.");
        if (nombreJugadorFinalText == null) Debug.LogError("PlayerController: No se ha asignado 'Nombre Jugador Final Text' para la pantalla de victoria.");

        // Inicializar banderas de juego perdido
        juegoTerminadoPorTiempo = false;
        juegoPerdidoPorLava = false;
        juegoGanado = false;
    }

    void Update()
    {
        // Solo permitir el movimiento y la l�gica del juego si NO se ha perdido/ganado
        if (!juegoTerminadoPorTiempo && !juegoGanado && !juegoPerdidoPorLava) // �ACTUALIZADO!
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
        juegoTerminadoPorTiempo = true; // Establece la bandera de Game Over por tiempo
        Debug.Log("�Tiempo agotado! Game Over.");

        // Detener la m�sica de fondo
        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        // Reproducir sonido de p�rdida de tiempo
        if (audioSource != null && sonidoPerdidaTiempo != null)
        {
            audioSource.PlayOneShot(sonidoPerdidaTiempo);
        }
        else if (audioSource == null) { Debug.LogWarning("PlayerController: AudioSource para SFX es nulo, no se puede reproducir sonido de p�rdida por tiempo."); }
        else if (sonidoPerdidaTiempo == null) { Debug.LogWarning("PlayerController: AudioClip de p�rdida por tiempo es nulo, no se puede reproducir."); }

        // Activar pantalla de Game Over
        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            if (textoGameOver != null)
            {
                textoGameOver.text = mensajeGameOverPorTiempo; // Mensaje espec�fico para el Game Over por tiempo
            }
        }

        // Desactivar movimiento del jugador y detener enemigos
        DesactivarMovimientoJugadorYEnemigos();
    }

    // --- L�gica de Detecci�n de Colisi�n (Trigger) ---
    void OnTriggerEnter(Collider other)
    {
        // Si el juego no ha terminado por tiempo ni por victoria ni por lava
        if (!juegoTerminadoPorTiempo && !juegoGanado && !juegoPerdidoPorLava)
        {
            if (other.CompareTag("Meta"))
            {
                Debug.Log("�Meta alcanzada! Has ganado.");
                TerminarJuegoPorVictoria();
            }
            else if (other.CompareTag("Lava")) // �NUEVO! Detecci�n de la lava
            {
                Debug.Log("�El jugador ha ca�do en la lava! Activando Game Over.");
                MorirPorLava();
            }
        }
    }

    // --- NUEVO M�TODO: Muerte por Lava ---
    void MorirPorLava()
    {
        juegoPerdidoPorLava = true; // Establece la bandera de Game Over por lava
        Debug.Log("�Jugador muerto por lava! Game Over.");

        // Detener la m�sica de fondo
        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        // Reproducir sonido de muerte por lava
        if (audioSource != null && sonidoMuerteLava != null) // Usamos el nuevo AudioClip
        {
            audioSource.PlayOneShot(sonidoMuerteLava);
        }
        else if (audioSource == null) { Debug.LogWarning("PlayerController: AudioSource para SFX es nulo, no se puede reproducir sonido de muerte por lava."); }
        else if (sonidoMuerteLava == null) { Debug.LogWarning("PlayerController: AudioClip de muerte por lava es nulo, no se puede reproducir."); }

        // Activar pantalla de Game Over
        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            if (textoGameOver != null)
            {
                textoGameOver.text = "�Ca�ste en la lava!"; // Mensaje espec�fico para la lava
            }
        }

        // Desactivar movimiento del jugador y detener enemigos
        DesactivarMovimientoJugadorYEnemigos();
    }


    void TerminarJuegoPorVictoria()
    {
        juegoGanado = true;
        Debug.Log("�Juego ganado!");

        if (musicaDeFondoSource != null)
        {
            musicaDeFondoSource.Stop();
        }

        float tiempoTranscurrido = Time.time - tiempoInicioJuego;
        Debug.Log("Tiempo utilizado: " + tiempoTranscurrido.ToString("F2") + " segundos.");

        string nombreJugador = PlayerPrefs.GetString(NombreJugadorPrefKey, "Jugador Desconocido");
        Debug.Log("DEBUG (PlayerController): Nombre cargado para victoria: " + nombreJugador);

        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(true);

            if (tiempoFinalText != null)
            {
                tiempoFinalText.text = "Tiempo: " + tiempoTranscurrido.ToString("F2") + " segundos";
            }
            if (nombreJugadorFinalText != null)
            {
                nombreJugadorFinalText.text = "Jugador: " + nombreJugador;
            }
        }
        else
        {
            Debug.LogError("PlayerController: La pantalla de victoria (GameObject) no est� asignada en el Inspector.");
        }

        // Desactivar movimiento del jugador y detener enemigos
        DesactivarMovimientoJugadorYEnemigos();
    }

    // --- NUEVO M�TODO CONSOLIDADO para detener el juego ---
    void DesactivarMovimientoJugadorYEnemigos()
    {
        // Desactiva el script PlayerController para detener el movimiento del jugador
        enabled = false;
        Debug.Log("Movimiento del jugador desactivado.");

        // Opcional: si el CharacterController debe deshabilitarse o el Rigidbody (si tuvieras uno)
        if (player != null)
        {
            player.enabled = false;
        }

        // Detener a todos los agentes NavMesh (enemigos)
        UnityEngine.AI.NavMeshAgent[] todosAgentes = FindObjectsOfType<UnityEngine.AI.NavMeshAgent>();
        foreach (UnityEngine.AI.NavMeshAgent agenteEnemigo in todosAgentes)
        {
            if (agenteEnemigo != null && agenteEnemigo.enabled)
            {
                agenteEnemigo.isStopped = true;
                agenteEnemigo.acceleration = 0;
                agenteEnemigo.angularSpeed = 0;
                Debug.Log($"Enemigo {agenteEnemigo.gameObject.name} detenido.");
            }
        }
    }


    // M�todo para reiniciar el juego, asociado a un bot�n UI.
    // Deber�a ser p�blico para ser llamado desde un bot�n.
    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Carga la escena actual para reiniciar
    }

    // M�todo para volver al men�, asociado a un bot�n UI.
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuScene"); // Aseg�rate de que "MenuScene" sea el nombre correcto de tu escena de men�
    }
}
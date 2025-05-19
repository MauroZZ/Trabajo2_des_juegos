using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalMove;
    public float verticalMove;
    public CharacterController player;
    public float PlayerSpeed = 5f;
    public float gravity = 9.8f; // Valor positivo para la aceleraci�n hacia abajo
    public float jumpHeight = 2f; // Altura del salto
    [Tooltip("Altura de la c�mara sobre el jugador.")]
    public float cameraHeight = 10f;
    [Tooltip("Suavidad con la que la c�mara sigue al jugador (0-1).")]
    [Range(0f, 1f)]
    public float cameraFollowSpeed = 0.1f;

    private Vector3 playerInput;
    private Vector3 moveDirection;
    private Vector3 verticalVelocity;
    public Camera MainCamera;
    private Vector3 cameraTargetPosition;

    void Start()
    {
        player = GetComponent<CharacterController>();

        if (MainCamera == null)
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                Debug.LogError("No se encontr� la Main Camera en la escena.");
                enabled = false;
            }
        }

        // Establecer la rotaci�n inicial de la c�mara (cenital)
        if (MainCamera != null)
        {
            MainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        playerInput = new Vector3(horizontalMove, 0, verticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        // Movimiento del jugador (sin depender de la rotaci�n de la c�mara)
        moveDirection = playerInput * PlayerSpeed;

        // Rotaci�n del personaje (opcional, si quieres que mire hacia donde se mueve)
        if (moveDirection.magnitude > 0.1f)
        {
            player.transform.LookAt(player.transform.position + new Vector3(moveDirection.x, 0f, moveDirection.z));
        }

        // Aplicar habilidades del jugador (incluido el salto)
        PlayerSkills();

        // Aplicar gravedad
        ApplyGravity();

        // Combinar el movimiento horizontal y vertical
        Vector3 finalMove = moveDirection + verticalVelocity;

        player.Move(finalMove * Time.deltaTime);

        // Movimiento de la c�mara
        if (MainCamera != null)
        {
            // Calcular la posici�n objetivo de la c�mara
            cameraTargetPosition = new Vector3(transform.position.x, cameraHeight, transform.position.z);

            // Mover la c�mara suavemente hacia la posici�n objetivo
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, cameraTargetPosition, cameraFollowSpeed);
        }
    }

    void ApplyGravity()
    {
        if (player.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // Peque�a fuerza hacia abajo para asegurar contacto con el suelo
        }
        else
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }
    }

    public void PlayerSkills()
    {
        // Detectar la entrada del salto (barra espaciadora)
        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            // Calcular la velocidad vertical necesaria para alcanzar la altura de salto deseada
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    void FixedUpdate()
    {
        // FixedUpdate se usa generalmente para f�sicas, el movimiento con CharacterController
        // suele ser m�s preciso en Update.
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalMove;
    public float verticalMove;
    public CharacterController player;
    public float PlayerSpeed = 5f;
    public float gravity = 9.8f; // Valor positivo para la aceleración hacia abajo
    public float jumpHeight = 2f; // Altura del salto

    private Vector3 playerInput;
    private Vector3 moveDirection;
    private Vector3 verticalVelocity;
    public Camera MainCamera;
    private Vector3 camForward;
    private Vector3 camRight;

    void Start()
    {
        player = GetComponent<CharacterController>();

        if (MainCamera == null)
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                Debug.LogError("No se encontró la Main Camera en la escena.");
                enabled = false;
            }
        }
    }

    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        playerInput = new Vector3(horizontalMove, 0, verticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        camDirection();

        // Calcula la dirección del movimiento horizontal relativa a la cámara
        moveDirection = playerInput.x * camRight + playerInput.z * camForward;
        moveDirection = moveDirection.normalized * PlayerSpeed;

        // Rotación del personaje
        if (moveDirection.magnitude > 0.1f)
        {
            player.transform.LookAt(player.transform.position + moveDirection);
        }

        // Aplicar habilidades del jugador (incluido el salto)
        PlayerSkills();

        // Aplicar gravedad
        ApplyGravity();

        // Combinar el movimiento horizontal y vertical
        Vector3 finalMove = moveDirection + verticalVelocity;

        player.Move(finalMove * Time.deltaTime);
    }

    void camDirection()
    {
        if (MainCamera != null)
        {
            camForward = MainCamera.transform.forward;
            camRight = MainCamera.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward = camForward.normalized;
            camRight = camRight.normalized;
        }
    }

    void ApplyGravity()
    {
        if (player.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // Pequeña fuerza hacia abajo para asegurar contacto con el suelo
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
        // FixedUpdate se usa generalmente para físicas, el movimiento con CharacterController
        // suele ser más preciso en Update.
    }
}
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         // El Transform del jugador al que la cámara seguirá. ¡ASIGNAR EN EL INSPECTOR!
    [Tooltip("Distancia de la cámara al jugador.")]
    public float distance = 8.0f;
    [Tooltip("Altura de la cámara sobre el jugador.")]
    public float height = 4.0f;
    [Tooltip("Suavidad del movimiento de la cámara (0-1).")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;
    [Tooltip("Velocidad de rotación de la cámara (grados por segundo).")]
    public float rotationSpeed = 100f;
    [Tooltip("Umbral de entrada para actualizar la rotación de la cámara.")]
    public float rotationThreshold = 0.1f;

    [Tooltip("Offset de altura aplicado al objetivo.")]
    public Vector3 targetOffset = new Vector3(0f, 1.862f, 0f);
    [Tooltip("Ángulo vertical deseado de la cámara (en grados).")]
    public float fixedAngleVertical = 26.04f;

    private Vector3 velocity;
    private float currentRotationY = 0f;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        if (target != null)
        {
            currentRotationY = target.eulerAngles.y; // Inicializar la rotación Y de la cámara con la del jugador
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado un objetivo (jugador) a la cámara.");
            return;
        }

        // Solo actualizar la rotación Y de la cámara si hay una entrada horizontal o vertical significativa
        if (Mathf.Abs(horizontalInput) > rotationThreshold || Mathf.Abs(verticalInput) > rotationThreshold)
        {
            currentRotationY = Mathf.LerpAngle(currentRotationY, target.eulerAngles.y, rotationSpeed * Time.deltaTime);
        }

        // Calcular la posición deseada
        Quaternion rotation = Quaternion.Euler(fixedAngleVertical, currentRotationY, 0f);
        Vector3 desiredPosition = target.position + targetOffset - rotation * Vector3.forward * distance;

        // Suavizar el movimiento de la posición de la cámara
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;

        // Hacer que la cámara mire al jugador
        transform.LookAt(target.position + targetOffset);
    }
}
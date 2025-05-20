using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         // El Transform del jugador al que la c�mara seguir�. �ASIGNAR EN EL INSPECTOR!
    [Tooltip("Distancia de la c�mara al jugador.")]
    public float distance = 8.0f;
    [Tooltip("Altura de la c�mara sobre el jugador.")]
    public float height = 4.0f;
    [Tooltip("Suavidad del movimiento de la c�mara (0-1).")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;
    [Tooltip("Velocidad de rotaci�n de la c�mara (grados por segundo).")]
    public float rotationSpeed = 100f;
    [Tooltip("Umbral de entrada para actualizar la rotaci�n de la c�mara.")]
    public float rotationThreshold = 0.1f;

    [Tooltip("Offset de altura aplicado al objetivo.")]
    public Vector3 targetOffset = new Vector3(0f, 1.862f, 0f);
    [Tooltip("�ngulo vertical deseado de la c�mara (en grados).")]
    public float fixedAngleVertical = 26.04f;

    private Vector3 velocity;
    private float currentRotationY = 0f;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        if (target != null)
        {
            currentRotationY = target.eulerAngles.y; // Inicializar la rotaci�n Y de la c�mara con la del jugador
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
            Debug.LogWarning("No se ha asignado un objetivo (jugador) a la c�mara.");
            return;
        }

        // Solo actualizar la rotaci�n Y de la c�mara si hay una entrada horizontal o vertical significativa
        if (Mathf.Abs(horizontalInput) > rotationThreshold || Mathf.Abs(verticalInput) > rotationThreshold)
        {
            currentRotationY = Mathf.LerpAngle(currentRotationY, target.eulerAngles.y, rotationSpeed * Time.deltaTime);
        }

        // Calcular la posici�n deseada
        Quaternion rotation = Quaternion.Euler(fixedAngleVertical, currentRotationY, 0f);
        Vector3 desiredPosition = target.position + targetOffset - rotation * Vector3.forward * distance;

        // Suavizar el movimiento de la posici�n de la c�mara
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;

        // Hacer que la c�mara mire al jugador
        transform.LookAt(target.position + targetOffset);
    }
}
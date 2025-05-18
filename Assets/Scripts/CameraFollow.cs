using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objetivo al que la cámara sigue (el tanque)
    public Vector3 offset = new Vector3(0f, 5f, -30f);
    public LayerMask collisionMask; // Máscara de capas para detectar las paredes

    public float followSpeed = 10f;
    public float rotationSpeed = 20f;
    public float verticalRotationSpeed = 500f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    public float collisionOffset = 0.2f; // Pequeño offset para evitar que la cámara se meta justo en la pared

    public Transform tankTransform;

    float currentRotationY;
    float currentRotationX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentRotationX = transform.eulerAngles.x;

        // Asegurarse de que el tankTransform esté asignado
        if (tankTransform == null && target != null)
        {
            tankTransform = target; // Si el target es el tanque, usarlo también para la rotación
            Debug.LogWarning("Tank Transform no asignado. Usando el Target como Tank Transform.");
        }
        else if (tankTransform == null)
        {
            Debug.LogError("Tank Transform no asignado. El tanque no rotará con la cámara.");
        }
    }

    void LateUpdate()
    {
        if (target == null || tankTransform == null) return;

        // Obtener rotación con mouse horizontal
        float horizontalInput = Input.GetAxis("Mouse X");
        currentRotationY += horizontalInput * rotationSpeed * Time.deltaTime;

        // Obtener rotación con mouse vertical
        float verticalInput = Input.GetAxis("Mouse Y");
        currentRotationX -= verticalInput * verticalRotationSpeed * Time.deltaTime;
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);

        // Crear rotación combinada (vertical y horizontal) para la cámara
        Quaternion cameraRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);

        // Calcular la posición deseada basada en la rotación combinada
        Vector3 desiredPosition = target.position + cameraRotation * offset;

        // Realizar un Raycast desde la posición deseada hacia el objetivo
        RaycastHit hit;
        Vector3 finalPosition = desiredPosition;
        if (Physics.Linecast(target.position, desiredPosition, out hit, collisionMask))
        {
            // Si el Raycast golpea algo (una pared), la posición final de la cámara será el punto de impacto
            finalPosition = hit.point + hit.normal * collisionOffset;
        }

        // Mover cámara suavemente a la posición final (ya sea la deseada o la ajustada por colisión)
        transform.position = Vector3.Lerp(transform.position, finalPosition, followSpeed * Time.deltaTime);

        // Mirar al objetivo (mantener la cámara apuntando al tanque)
        transform.LookAt(target);

        // Rotar el tanque horizontalmente para que coincida con la rotación horizontal de la cámara
        Quaternion tankRotationY = Quaternion.Euler(0f, currentRotationY, 0f);
        tankTransform.rotation = tankRotationY;
    }
}
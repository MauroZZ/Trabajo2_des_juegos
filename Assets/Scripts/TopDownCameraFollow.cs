using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;         // El Transform del jugador. ¡ASIGNAR EN EL INSPECTOR!
    public float height = 20f;      // Altura constante de la cámara.
    public float followSpeed = 5f;  // Suavidad con la que la cámara sigue al jugador.

    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado un objetivo (jugador) a la cámara.");
            return;
        }

        // Calcular la posición objetivo de la cámara: misma X y Z que el jugador, altura constante
        targetPosition = new Vector3(target.position.x, height, target.position.z);

        // Mover la cámara suavemente hacia la posición objetivo
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Asegurar que la cámara mire hacia abajo (rotación fija)
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;         // El Transform del jugador. �ASIGNAR EN EL INSPECTOR!
    public float height = 20f;      // Altura constante de la c�mara.
    public float followSpeed = 5f;  // Suavidad con la que la c�mara sigue al jugador.

    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado un objetivo (jugador) a la c�mara.");
            return;
        }

        // Calcular la posici�n objetivo de la c�mara: misma X y Z que el jugador, altura constante
        targetPosition = new Vector3(target.position.x, height, target.position.z);

        // Mover la c�mara suavemente hacia la posici�n objetivo
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Asegurar que la c�mara mire hacia abajo (rotaci�n fija)
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
using UnityEngine;

public class gameMusicController : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager no está inicializado.");
            return;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Intentar obtener AudioSource si no está asignado
            if (audioSource == null)
            {
                Debug.LogWarning("No se encontró AudioSource en el objeto.");
                return;
            }
        }

        audioSource.volume = AudioManager.Instance.volume;
        audioSource.Play();
    }
}

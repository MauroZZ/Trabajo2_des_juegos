using UnityEngine;

public class gameMusicController : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager no est� inicializado.");
            return;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Intentar obtener AudioSource si no est� asignado
            if (audioSource == null)
            {
                Debug.LogWarning("No se encontr� AudioSource en el objeto.");
                return;
            }
        }

        audioSource.volume = AudioManager.Instance.volume;
        audioSource.Play();
    }
}

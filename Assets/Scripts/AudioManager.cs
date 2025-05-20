using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton
    public float volume = 1f; // Volumen global

    private void Awake()
    {
        // Asegurarse de que solo haya una instancia de AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Destruir duplicados
        }
    }

    public void SetVolume(float value)
    {
        volume = value;
    }
}

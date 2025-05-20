using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioController : MonoBehaviour
{
    public AudioSource audioSource; // Asegúrate de que esto esté asignado en el Inspector
    public Slider volumeSlider; // Asegúrate de que esto esté asignado en el Inspector

    void Start()
    {
        Debug.Log("AudioSource: " + audioSource);
        Debug.Log("VolumeSlider: " + volumeSlider);

        if (volumeSlider != null)
        {
            // Sincroniza el slider con el volumen global al iniciar
            volumeSlider.value = AudioManager.Instance.volume;
            // Añadir listener para actualizar volumen al cambiar el slider
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        else
        {
            Debug.LogWarning("Volume Slider no está asignado en el inspector.");
        }
        if (audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.volume;
        }
        else
        {
            Debug.LogWarning("AudioSource no está asignado en el inspector.");
        }
    }

    public void OnVolumeChanged(float value)
    {
        // Actualizar volumen global y volumen del audio source
        AudioManager.Instance.SetVolume(value);
        if (audioSource != null)
        {
            audioSource.volume = value;
        }
    }

    public void PlayMusic()
    {
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}

using UnityEngine;
using System.Collections;

public class SoundBreaker : MonoBehaviour
{
    [Header("Fonte de Áudio da Música")]
    public AudioSource musicSource;

    [Header("Tempo do Fade (segundos)")]
    public float fadeDuration = 1.5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return; // evita repetir o fade
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (musicSource != null)
            StartCoroutine(FadeOutMusic());
        else
            Debug.LogWarning("Nenhum AudioSource foi atribuído no inspector!");
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // opcional: restaura volume original
    }
}


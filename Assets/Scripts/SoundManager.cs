using System.Collections;
using UnityEngine;

public class SoundManager : PortableBeep
{
    public static SoundManager Instance;

    [SerializeField] private AudioReverbFilter reverbFilter;
    [SerializeField] private AudioSource glitchSource;
    public GameObject portableBeepPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayLoseSound()
    {
        StartCoroutine(LoseSound());
    }

    private IEnumerator LoseSound()
    {
        PlayBeep(2);
        yield return new WaitForSeconds(0.2f);
        PlayBeep(2, true, 0.8f);
    }

    public void PlayGlitchSound(float fadeDuration)
    {
        StartCoroutine(GlitchSound(fadeDuration));
    }

    private IEnumerator GlitchSound(float fadeDuration)
    {
        float timeElapsed = 0f;
        glitchSource.volume = 0.2f;
        glitchSource.Play();

        while (glitchSource.volume < 1)
        {
            glitchSource.volume = Mathf.Lerp(0.2f, 1, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }

    public void StopGlitchSound()
    {
        glitchSource.Stop();
    }

    //public void ToggleBeepReverb(bool value)
    //{
    //    reverbFilter.enabled = value;
    //}
}

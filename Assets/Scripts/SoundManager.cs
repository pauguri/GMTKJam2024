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

    public void PlayGlitchSound()
    {
        StartCoroutine(GlitchSound());
    }

    private IEnumerator GlitchSound()
    {
        float timeElapsed = 0f;
        glitchSource.volume = 0;
        glitchSource.Play();

        while (glitchSource.volume < 1)
        {
            glitchSource.volume = Mathf.Lerp(0, 1, timeElapsed / 2f);
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

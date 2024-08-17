using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource beepSource;
    [SerializeField] private AudioClip[] beeps;

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

    public void PlayBeep(int clip, bool forcePitch = false, float pitch = 1f)
    {
        beepSource.clip = beeps[clip];
        beepSource.pitch = forcePitch ? pitch : 1;
        beepSource.Play();
    }
}

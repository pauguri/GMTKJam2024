using UnityEngine;

public class PortableBeep : MonoBehaviour
{
    public AudioSource beepSource;
    public AudioClip[] beeps;

    public void PlayBeep(int clip, bool forcePitch = false, float pitch = 1f, bool ignoreReverb = false)
    {
        beepSource.clip = beeps[clip];
        beepSource.pitch = forcePitch ? pitch : 1;
        beepSource.bypassReverbZones = ignoreReverb;
        beepSource.Play();
    }
}

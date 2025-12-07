using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource EnterAudio;
    public AudioSource ExitAudio;

    public void PlayEnterAudio()
    {
        EnterAudio.Play();
    }
    public void PlayExitAudio()
    {
        ExitAudio.Play();
    }
}

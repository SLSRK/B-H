using UnityEngine;


public class MusicController : MonoBehaviour
{
    public AudioSource AS;
    public AudioClip[] Music;
    private bool pause;

    void Update()
    {
        if (!AS.isPlaying && !pause)
        {
            /*if(AS.resource == Music[0])
            {
                SetMenuClip();
            }
            else
            {
                SetGameClip();
            }*/
            AS.Play();
        }
    }

    public void SetMenuClip()
    {
        AS.resource = Music[0];
        AS.Play();
        pause = false;
    }
    
    public void SetGameClip()
    {
        if (AS.isPlaying) return;
        var muz = Music[UnityEngine.Random.Range(1, Music.Length)];
        if (muz == AS.resource) 
        {
            SetGameClip();
            return;
        }
        else
        {
            AS.resource = muz;
            AS.Play();
            pause = false;
        }
    }

    public void PauseClip()
    {
        AS.Pause();
        pause = true;
    }
}

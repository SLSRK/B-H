using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public IGameController GC;

    public Rigidbody BallRG;
    public AudioSource BallSnd;

    public bool OnSurface;
    public bool Fall = false;

    private void Start()
    {
        BallSnd.enabled = StaticData.Sound;
    }

    void FixedUpdate()
    {
        BallRG.WakeUp();
        BallSnd.pitch = BallRG.linearVelocity.magnitude / 2;
        //BallSnd.pitch = BallRG.angularVelocity.magnitude / 6;
        if (Fall && BallSnd.volume != 0)
        {
            BallSnd.volume -= Time.deltaTime;
        }
    }

    public void OnFall()
    {
        Fall = true;
    }

    public void Pause(bool snd)
    {
        if (!snd) BallSnd.enabled = snd;
        else BallSnd.enabled = StaticData.Sound;
    }
}

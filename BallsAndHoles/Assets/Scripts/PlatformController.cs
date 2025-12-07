using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float Maxpoint;

    private float angle;

    private bool PressedL;
    private bool PressedR;
    private bool OnMoved;

    public bool over;

    public GameObject Platform;
    public GameObject Edge1;
    public GameObject Edge2;
    public GameObject MCam;

    public Collider BallCol;

    public AudioSource Sound1;
    public AudioSource Sound2;

    private void Start()
    {
        Sound1.enabled = StaticData.Sound;
        Sound2.enabled = StaticData.Sound;
    }

    void FixedUpdate()
    {
        angle = Platform.transform.eulerAngles.z;
        angle = Mathf.Repeat(angle + 180, 360) - 180;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || PressedR)
        {
            REdgeUp();
        }
        else
        {
            //Sound2.Pause();
            SilentAudio(Sound2);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || PressedL)
        {
            LEdgeUp();
        }
        else
        {
            //Sound1.Pause();
            SilentAudio(Sound1);
        }
        if (over)
        {
            if (angle != 0)
            {
                Platform.transform.Rotate(Vector3.forward * 0.4f * -(angle/ Mathf.Abs(angle)));
                if (Mathf.Abs(angle) < 1) Platform.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            if (transform.position.y > 0)
            {
                transform.position -= transform.up * 2 * Time.deltaTime;
            }
            //SilentAudio(Sound1);
            //SilentAudio(Sound2);
        }
    }

    public void REdgeUp()
    {
        if (!over && angle < 20 && Edge2.transform.position.y < Maxpoint)
        {
            transform.position += transform.up * Time.deltaTime;
            Platform.transform.Rotate(Vector3.forward * 0.2f);
            // if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
            // MCam.transform.position = new Vector3(transform.position.x, Edge1.transform.position.y, Edge1.transform.position.z);
            MCam.transform.position = Vector3.MoveTowards(MCam.transform.position, new Vector3(0, Edge1.transform.position.y, Edge1.transform.position.z), (Vector3.Distance(MCam.transform.position, Edge1.transform.position) - 4) * Time.deltaTime);
            StartAudio(Sound2);
            OnMoved = true;
        }
        else
        {
            SilentAudio(Sound2);
        }
    }

    public void LEdgeUp()
    {
        if (!over && angle > -20 && Edge1.transform.position.y < Maxpoint)
        {
            transform.position += transform.up * Time.deltaTime;
            Platform.transform.Rotate(Vector3.forward * -0.2f);
            MCam.transform.position = Vector3.MoveTowards(MCam.transform.position, new Vector3(0, Edge2.transform.position.y, Edge2.transform.position.z), (Vector3.Distance(MCam.transform.position, Edge1.transform.position) - 4) * Time.deltaTime);
            StartAudio(Sound1);
            OnMoved = true;
        }
        else
        {
            SilentAudio(Sound1);
        }
    }

    public void StartAudio(AudioSource Sound)
    {
        if (!Sound.isPlaying) Sound.Play();
        if (Sound.volume < 1f) Sound.volume += 0.1f;
    }

    public void SilentAudio(AudioSource Sound)
    {
        if (Sound.volume > 0f) Sound.volume -= 0.1f;
        else Sound.Pause();
    }

    public void Pause(bool snd)
    {
        if (!snd)
        {
            Sound1.enabled = snd;
            Sound2.enabled = snd;
        }
        else
        {
            Sound1.enabled = StaticData.Sound;
            Sound2.enabled = StaticData.Sound;
        }
    }

    public void PressL(bool Press)
    {
        PressedL = Press;
    }
    public void PressR(bool Press)
    {
        PressedR = Press;
    }

    public bool Moved()
    {
        return OnMoved;
    }

    public IEnumerator CameraShake(int angle)
    {
        //MCam.transform.Rotate(Vector3.forward * angle);
        //yield return new WaitForSeconds(0.2f);
        //MCam.transform.Rotate(Vector3.forward * -angle);
        for(int i = 0; i < 10; i++)
        {
            MCam.transform.Rotate(Vector3.forward * angle / 10);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            MCam.transform.Rotate(Vector3.forward * -angle / 10);
            yield return new WaitForSeconds(0.015f);
        }
    }
}

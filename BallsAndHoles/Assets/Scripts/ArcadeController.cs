using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using TMPro;

public class ArcadeController : MonoBehaviour
{
    public PlatformController PC;
    public BallController BC;

    public Animation BlurAnim;
    public Animator PauseAnim;
    public Animator TransitionAnim;

    public TextMeshProUGUI PauseProperty;
    public TextMeshProUGUI OverProperty;
    public TextMeshProUGUI SideSign;
    public TextMeshProUGUI TransitionSign;

    public AudioSource[] OtherSounds;

    [HideInInspector]
    public bool over = false;

    public GameObject Wall;
    public GameObject[] WallParts;
    public GameObject[] DynamicParts = new GameObject[6];
    private int currentCount = 0;
    private int prevWall = 100;
    public GameObject SpawnPlace;
    public GameObject Ball;

    public GameObject OverEffect;
    public GameObject PausePanel;
    public GameObject PauseBtn;

    public Material WallSkin;

    private float MaxBallPos;

    void Start()
    {
        for (int i = 0; i < OtherSounds.Length; i++)
        {
            OtherSounds[i].enabled = StaticData.Sound;
        }
        //StaticData.MusicSource.GetComponent<MusicController>().SetGameClip();
        PauseAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        for (int i = 0; i < StaticData.BallSkinsID.Length; i++)
        {
            if (StaticData.BallSkinsID[i] == 2) Ball.GetComponent<MeshRenderer>().material = StaticData.BallSkins[i];
        }
        for (int i = 0; i < StaticData.WallSkinsID.Length; i++) if (StaticData.WallSkinsID[i] == 2) WallSkin = StaticData.WallSkins[i];
        Wall.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = WallSkin;
        for (int i = 0; i < 5; i++)
        {
            int SpawnID = UnityEngine.Random.Range(1,8) * (UnityEngine.Random.value < 0.5f ? -1 : 1);
            if (SpawnID != prevWall) SpawnWall(SpawnID);
            else i--;
        }
    }

    void FixedUpdate()
    {
        if (!over && MaxBallPos < Ball.transform.position.y)
        {
            MaxBallPos = Ball.transform.position.y;
            Physics.gravity = new Vector3(Physics.gravity.x, -20 - (MaxBallPos / 10), Physics.gravity.z);
            if(SpawnPlace.transform.position.y - MaxBallPos < 5)
            {
                bool ready = false;
                while (!ready)
                {
                    float RndVar;
                    int SpawnID;
                    if (MaxBallPos < 20) RndVar = 0.2f;
                    else if (MaxBallPos < 40) RndVar = 0.4f;
                    else if (MaxBallPos < 60) RndVar = 0.6f;
                    else if (MaxBallPos < 80) RndVar = 0.8f;
                    else RndVar = 0.9f;

                    if (UnityEngine.Random.value < RndVar) SpawnID = UnityEngine.Random.Range(9, 21) * (UnityEngine.Random.value < 0.5f ? -1 : 1);
                    else SpawnID = UnityEngine.Random.Range(1, 8) * (UnityEngine.Random.value < 0.5f ? -1 : 1);
                    if(SpawnID != prevWall)
                    {
                        SpawnWall(SpawnID);
                        ready = true;
                        if(MaxBallPos > 150 && DynamicParts[5].transform.name.EndsWith("E(Clone)") && (UnityEngine.Random.value < 0.2f)) StartCoroutine(Shake());
                    }
                }
            }
        }
        else if (!over && MaxBallPos - 2 > Ball.transform.position.y)
        {
            OnOver();
        }
        if (!over)
        {
            SideSign.text = Mathf.RoundToInt(MaxBallPos).ToString("00");
        }
    }

    public void SpawnWall(int i)
    {
        var go = Instantiate(WallParts[Math.Abs(i) - 1], SpawnPlace.transform.position, SpawnPlace.transform.rotation, Wall.transform);
        SpawnPlace = go.transform.GetChild(0).gameObject;
        prevWall = i;
        foreach (Transform child in go.transform)
        {
            GameObject childObject = child.gameObject;
            if (childObject.GetComponent<MeshRenderer>() != null) childObject.GetComponent<MeshRenderer>().material = WallSkin;
        }
        if (i < 0)
        {
            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        }

        if (currentCount < DynamicParts.Length)
        {
            DynamicParts[currentCount] = go;
            currentCount++;
        }
        else
        {
            Destroy(DynamicParts[0]);

            for (int j = 1; j < DynamicParts.Length; j++)
            {
                DynamicParts[j - 1] = DynamicParts[j];
            }

            DynamicParts[DynamicParts.Length - 1] = go;
        }
    }

    public void OnOver()
    {
        over = true;
        PC.over = true;
        StartCoroutine(OnOverDelay());
    }

    IEnumerator Shake()
    {
        int angle = UnityEngine.Random.Range(6, 21) * (UnityEngine.Random.value < 0.5f ? -1 : 1);
        StartCoroutine(PC.CameraShake(angle));
        Physics.gravity = new Vector3(angle, Physics.gravity.y, Physics.gravity.z);
        yield return new WaitForSeconds(0.2f);
        Physics.gravity = new Vector3(0, Physics.gravity.y, Physics.gravity.z);
    }

    IEnumerator OnOverDelay()
    {
        PauseBtn.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        BC.OnFall();
        yield return new WaitForSeconds(0.4f);
        OverEffect.SetActive(true);
        BlurAnim.Play();
        if (MaxBallPos < StaticData.Bestscore)
        {
            OverProperty.text = "Best: " + Mathf.RoundToInt(StaticData.Bestscore).ToString("00");
        }
        else
        {
            StaticData.Bestscore = MaxBallPos;
            OverProperty.text = "New Best!";
            yield return new WaitForSeconds(0.6f);
            OverProperty.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            OverProperty.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            OverProperty.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            OverProperty.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            OverProperty.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            OverProperty.color = Color.white;
        }
    }

    IEnumerator PauseDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        PauseBtn.SetActive(true);
        PC.Pause(true);
        BC.Pause(true);
        Time.timeScale = 1;
    }


    public void Restart()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadAsyncScene(SceneManager.GetActiveScene().name));
    }

    public void Menu()
    {
        TransitionSign.text = "";
        StartCoroutine(LoadAsyncScene("Menu"));
        //StaticData.MusicSource.GetComponent<MusicController>().PauseClip();
    }

    IEnumerator LoadAsyncScene(string lvl)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lvl.ToString());
        asyncLoad.allowSceneActivation = false;
        TransitionAnim.Play("Close");
        yield return new WaitForSecondsRealtime(0.6f);
        asyncLoad.allowSceneActivation = true;
    }

    public void Pause(int Timescale)
    {
        if (Timescale == 0) 
        {
            Time.timeScale = Timescale;
            PC.Pause(false);
            BC.Pause(false);
            if (MaxBallPos < StaticData.Bestscore)
            {
                PauseProperty.text = "Best: " + Mathf.RoundToInt(StaticData.Bestscore).ToString("00");
            }
            else
            {
                PauseProperty.text = "Best: " + Mathf.RoundToInt(MaxBallPos).ToString("00");
            }
        }
        else StartCoroutine(PauseDelay());
    }

    public void Save()
    {
        var data = new LoadSave
        {
            Bestscore = StaticData.Bestscore,
            Levels = StaticData.Levels,
            BallSkinsID = StaticData.BallSkinsID,
            WallSkinsID = StaticData.WallSkinsID,
            Audio = new int[] { StaticData.Sound ? 1 : 0, StaticData.Music ? 1 : 0 }
        };

        PlayerPrefs.SetString("SV", JsonUtility.ToJson(data));
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!over)
        {
            Pause(0);
            PausePanel.SetActive(true);
            PauseAnim.Play("UIAnim1");
            PauseBtn.SetActive(false);
        }
        Save();
    }
}

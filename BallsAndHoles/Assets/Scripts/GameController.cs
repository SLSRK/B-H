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

public class GameController : MonoBehaviour
{
    public PlatformController PC;
    public BallController BC;

    public Animation BlurAnim;
    public Animator PauseAnim;
    public Animator TransitionAnim;

    public TextMeshProUGUI Sign;
    public TextMeshProUGUI PauseProperty;
    public TextMeshProUGUI SideSign;
    public TextMeshProUGUI TransitionSign;

    public AudioSource[] OtherSounds;

    private bool Win = false;
    private bool Lose = false;
    [HideInInspector]
    public bool over = false;
    public int[] StarsRate;

    public GameObject Wall;
    public GameObject Ball;
    public GameObject WinDetector;
    public GameObject WinEffect;
    public GameObject OverEffect;
    private GameObject WinHole;

    public GameObject PausePanel;
    public GameObject PauseBtn;
    public GameObject[] BtnPacks;

    public GameObject[] Stars;
    public GameObject[] PauseStars;

    public Material WallSkin;

    private float LvlTimer;
    private float MaxBallPos;

    void Start()
    {
        for(int i = 0; i < OtherSounds.Length; i++)
        {
            OtherSounds[i].enabled = StaticData.Sound;
        }
        //StaticData.MusicSource.GetComponent<MusicController>().SetGameClip();
        WinHole = WinDetector.transform.parent.gameObject;
        PC.Maxpoint = WinHole.transform.position.y + 0.15f;
        Physics.gravity = new Vector3(0, -20, 0);
        PauseAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        for (int i = 0; i < StaticData.BallSkinsID.Length; i++)
        {
            if (StaticData.BallSkinsID[i] == 2) Ball.GetComponent<MeshRenderer>().material = StaticData.BallSkins[i];
        }
        for (int i = 0; i < StaticData.WallSkinsID.Length; i++) if(StaticData.WallSkinsID[i] == 2) WallSkin = StaticData.WallSkins[i];
        foreach (Transform child in Wall.transform)
        {
            GameObject childObject = child.gameObject;
            MeshRenderer mr = childObject.GetComponent<MeshRenderer>();

            if (mr != null)
            {
                mr.material = WallSkin;

                if (childObject.name == "Sign 1")
                {
                    var mats = mr.materials;
                    if (mats.Length > 3)
                    {
                        mats[2] = WallSkin;
                        mats[3] = WallSkin;
                        mr.materials = mats;
                    }
                }
            }
        }

    }

    void FixedUpdate()
    {
        if(!Win && !Lose && MaxBallPos < Ball.transform.position.y)
        {
            MaxBallPos = Ball.transform.position.y;
        }
        else if(!Win && !Lose && MaxBallPos - 2 > Ball.transform.position.y)
        {
            OnLose();
        }
        if(!over && PC.Moved())
        {
            LvlTimer += Time.deltaTime;
            SideSign.text = LvlTimer.ToString("00");
        }
    }

    public void OnWin()
    {
        Win = true;
        over = true;
        PC.over = true;
        StartCoroutine(OnOverDelay("w"));
        Debug.Log("U Win");
    }
    public void OnLose()
    {
        Lose = true;
        over = true;
        PC.over = true;
        StartCoroutine(OnOverDelay("l"));
        Debug.Log("U Lose");
    }

    IEnumerator OnOverDelay(string state)
    {
        if (Win)
        {
            int stars;
            if (LvlTimer <= StarsRate[0] && LvlTimer >= StarsRate[1])
            {
                Sign.text = "Great!";
                stars = 2;
            }
            else if (LvlTimer <= StarsRate[1])
            {
                Sign.text = "Perfect!";
                stars = 3;
            }
            else
            {
                Sign.text = "Good!";
                stars = 1;
            }

            if (StaticData.Levels[int.Parse(SceneManager.GetActiveScene().name) - 1] < stars) 
            {
                StaticData.Levels[int.Parse(SceneManager.GetActiveScene().name) - 1] = stars; 
            }
            StartCoroutine(StarsDelay(stars));
        }
        PauseBtn.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        WinDetector.GetComponent<BoxCollider>().enabled = false;
        BC.OnFall();
        yield return new WaitForSeconds(0.4f);
        if (state == "w")
        {
            Instantiate(WinEffect, WinHole.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            BtnPacks[1].SetActive(true);
        }
        else if(state == "l")
        {
            Sign.text = "Try again.";
            BtnPacks[0].SetActive(true);
        }
        OverEffect.SetActive(true);
        BlurAnim.Play();

    }

    IEnumerator StarsDelay(int str)
    {
        yield return new WaitForSeconds(2.2f);
        for (int i = 0; i < str; i++)
        {
            Stars[i].SetActive(true);
            yield return new WaitForSeconds(0.3f);
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
    
    public void Next()
    {
        Time.timeScale = 1;
        int current = int.Parse(SceneManager.GetActiveScene().name);
        TransitionSign.text = "Level: " + (current + 1).ToString();
        StartCoroutine(LoadAsyncScene((current + 1).ToString()));
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

        // Wait until the asynchronous scene fully loads
        /*while (!asyncLoad.isDone)
        {
            yield return null;
        }*/
    }

    public void Pause(int Timescale)
    {
        if (Timescale == 0)
        {
            Time.timeScale = Timescale;
            PC.Pause(false);
            BC.Pause(false);

            if (LvlTimer <= StarsRate[1])
            {
                PauseProperty.text = StarsRate[1] + " sec.";
            }
            else if (LvlTimer <= StarsRate[0])
            {
                PauseProperty.text = StarsRate[0] + " sec.";
                PauseStars[2].SetActive(false);
                PauseStars[0].GetComponent<RectTransform>().anchorMin = new Vector2(0.37f, 0.41f);
                PauseStars[0].GetComponent<RectTransform>().anchorMax = new Vector2(0.45f, 0.6f);
                PauseStars[1].GetComponent<RectTransform>().anchorMin = new Vector2(0.29f, 0.41f);
                PauseStars[1].GetComponent<RectTransform>().anchorMax = new Vector2(0.37f, 0.6f);
                Vector2 anchorMin = PauseProperty.rectTransform.anchorMin;
                anchorMin.x = 0.45f;
                PauseProperty.rectTransform.anchorMin = anchorMin;
            }
            else
            {
                PauseProperty.text = "Level " + SceneManager.GetActiveScene().name;
                PauseStars[0].SetActive(false);
                PauseStars[1].SetActive(false);
                PauseStars[2].SetActive(false);
                Vector2 anchorMin = PauseProperty.rectTransform.anchorMin;
                anchorMin.x = 0f;
                PauseProperty.rectTransform.anchorMin = anchorMin;
                PauseProperty.horizontalAlignment = HorizontalAlignmentOptions.Center;
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

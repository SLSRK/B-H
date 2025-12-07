using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MenuController : MonoBehaviour
{
    private static MenuController instance;
    private AsyncOperation loadingSceneOperation;
    public AudioSource Err;
    public AudioSource[] OtherSounds;
    public AudioSource MusicSource;
    public GameObject[] LvlButtons;
    public GameObject SUnlocked;
    public GameObject MUnlocked;
    public GameObject NewSkins;
    public GameObject Bpos;
    public GameObject BallExample;
    public GameObject BL;
    public GameObject BR;
    public GameObject BLocked;
    public GameObject BUnlocked;
    public GameObject BStars;
    public GameObject Wpos;
    public GameObject WallExample;
    public GameObject WL;
    public GameObject WR;
    public GameObject WLocked;
    public GameObject WUnlocked;
    public GameObject WStars;
    public TextMeshProUGUI RecSing;
    public TextMeshProUGUI TransitionSign;
    public TextMeshProUGUI NewSkinsSign;
    public TextMeshProUGUI BReqTextField;
    public TextMeshProUGUI BNameTextField;
    public TextMeshProUGUI WReqTextField;
    public TextMeshProUGUI WNameTextField;
    private string[] BReqText = {
        " ",
        "Level\n10",
        "Arcade\n30",
        "Lvl 33",
        "Arcade\n50", //5
        "Level\n44",
        "Lvl 56",
        "Level\n67",
        "Level\n79",
        "Lvl 90", //10
        "Arcade\n200",
        "Lvl 110",
        "Level\n121",
        "Level\n133",
        "Lvl 144", //15
        "Level\n167",
        "Arcade\n300",
        "Level\n190",
        "Lvl 200",
        "Arcade\n500", //20
    };
    private string[] WReqText = {
        " ",
        "Lvl 21",
        "Arcade\n100",
        "Level\n156",
        "Lvl 179", 
    };
    int BCurrID;
    int WCurrID;
    public List<int> BIDOpened = new List<int>();
    public List<int> WIDOpened = new List<int>();

    private bool[] BallSkinReq;
    private bool[] WallSkinReq;

    public Animator TransitionAnim;
    public Animator NewSkinsAnim;
    void Start()
    {
        if (!StaticData.isOpened) { 
            LoadSaved();
            DontDestroyOnLoad(MusicSource);
            StaticData.MusicSource = MusicSource;
        }
        else
        {
            Destroy(MusicSource.gameObject);
            MusicSource = StaticData.MusicSource;
        }
        StaticData.isOpened = true;
        Application.targetFrameRate = 120;
        Time.timeScale = 1;
        for (int i = 0; i < LvlButtons.Length; i++)
        {
            if(StaticData.Levels[i] != 0)
            {
                LvlButtons[i].transform.GetChild(1).gameObject.SetActive(false);
                var chld = LvlButtons[i].transform.GetChild(0).gameObject;
                chld.SetActive(true);
                for (int j = 0; j < StaticData.Levels[i]; j++)
                {
                    chld.transform.GetChild(j + 1).gameObject.GetComponent<Image>().color = Color.red;
                    chld.transform.GetChild(j + 1).gameObject.GetComponent<Shadow>().enabled = true;
                }
            }
            else
            {
                LvlButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                LvlButtons[i].transform.GetChild(1).gameObject.SetActive(false);
                break;
            }
        }
        if(StaticData.Bestscore != 0) RecSing.text = "Best: " + Mathf.RoundToInt(StaticData.Bestscore).ToString("00");
        else RecSing.text = "Try this!";
        BallExample.transform.position = Bpos.GetComponent<RectTransform>().position;
        WallExample.transform.position = Wpos.GetComponent<RectTransform>().position;

        BallSkinReq = new bool[] {
            true,
        StaticData.Levels[9] >= 1,
        StaticData.Bestscore >= 30,
        StaticData.Levels[32] == 3,
        StaticData.Bestscore >= 50, //5
        StaticData.Levels[43] >= 1,
        StaticData.Levels[55] == 3,
        StaticData.Levels[66] >= 1,
        StaticData.Levels[78] >= 1,
        StaticData.Levels[89] == 3, //10
        StaticData.Bestscore >= 200,
        StaticData.Levels[109] == 3,
        StaticData.Levels[120] >= 1,
        StaticData.Levels[132] >= 1,
        StaticData.Levels[143] == 3, //15
        StaticData.Levels[166] >= 1,
        StaticData.Bestscore >= 300,
        StaticData.Levels[189] >= 1,
        StaticData.Levels[199] == 3,
        StaticData.Bestscore >= 500, //20
        };
        WallSkinReq = new bool[]
        {
            true,
            StaticData.Levels[20] == 3,
            StaticData.Bestscore >= 100,
            StaticData.Levels[155] >= 1,
            StaticData.Levels[178] == 3,
        };

        if (StaticData.BallSkinsID[0] == 0)
        {
            StaticData.BallSkinsID[0] = 2;
            BallExample.GetComponent<MeshRenderer>().material = StaticData.BallSkins[0];
            BCurrID = 0;
            BNameTextField.text = StaticData.BallSkins[0].name;
            BL.GetComponent<Button>().enabled = false;
            BL.GetComponent<Image>().color = Color.grey;
            StaticData.WallSkinsID[0] = 2;
            WallExample.GetComponent<MeshRenderer>().material = StaticData.WallSkins[0];
            WCurrID = 0;
            WNameTextField.text = StaticData.WallSkins[0].name;
            WL.GetComponent<Button>().enabled = false;
            WL.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            for(int i = 0; i < StaticData.BallSkinsID.Length; i++)
            {
                if (StaticData.BallSkinsID[i] == 2)
                {
                    BallExample.GetComponent<MeshRenderer>().material = StaticData.BallSkins[i];
                    BNameTextField.text = StaticData.BallSkins[i].name;
                    BCurrID = i;
                    if (i == 0)
                    {
                        BL.GetComponent<Button>().enabled = false;
                        BL.GetComponent<Image>().color = Color.grey;
                    }
                    else if (i == StaticData.BallSkinsID.Length - 1)
                    {
                        BR.GetComponent<Button>().enabled = false;
                        BR.GetComponent<Image>().color = Color.grey;
                    }
                }
                else if (StaticData.BallSkinsID[i] == 0 && BallSkinReq[i])
                {
                    StaticData.BallSkinsID[i] = 1;
                    BIDOpened.Add(i);
                }
            }
            if (BIDOpened.Count > 0)
            {
                if (BIDOpened.Any(x => x > BCurrID)) BR.transform.GetChild(1).gameObject.SetActive(true);
                if (BIDOpened.Any(x => x < BCurrID)) BL.transform.GetChild(1).gameObject.SetActive(true);
            }
            for (int i = 0; i < StaticData.WallSkinsID.Length; i++)
            {
                if (StaticData.WallSkinsID[i] == 2)
                {
                    WallExample.GetComponent<MeshRenderer>().material = StaticData.WallSkins[i];
                    WNameTextField.text = StaticData.WallSkins[i].name;
                    WCurrID = i;
                    if (i == 0)
                    {
                        WL.GetComponent<Button>().enabled = false;
                        WL.GetComponent<Image>().color = Color.grey;
                    }
                    else if (i == StaticData.WallSkinsID.Length - 1)
                    {
                        WR.GetComponent<Button>().enabled = false;
                        WR.GetComponent<Image>().color = Color.grey;
                    }
                }
                else if (StaticData.WallSkinsID[i] == 0 && WallSkinReq[i])
                {
                    StaticData.WallSkinsID[i] = 1;
                    WIDOpened.Add(i);
                }
            }
            if (WIDOpened.Count > 0)
            {
                if (WIDOpened.Any(x => x > WCurrID)) WR.transform.GetChild(1).gameObject.SetActive(true);
                if (WIDOpened.Any(x => x < WCurrID)) WL.transform.GetChild(1).gameObject.SetActive(true);
            }
            if(BIDOpened.Count + WIDOpened.Count > 0)
            {
                NewSkins.SetActive(true);
                if (BIDOpened.Count + WIDOpened.Count > 1) NewSkinsSign.text = "new skins unlocked";
                else NewSkinsSign.text = "new skin unlocked";
                StartCoroutine(NewSkinsCoroutine());
            }

            SUnlocked.SetActive(StaticData.Sound);
            MUnlocked.SetActive(StaticData.Music);
            Err.enabled = StaticData.Sound;
            for (int i = 0; i < OtherSounds.Length; i++)
            {
                OtherSounds[i].enabled = StaticData.Sound;
            }
            MusicSource.enabled = StaticData.Music;
            //MusicSource.GetComponent<MusicController>().SetMenuClip();
        }
    }

    public void StartLevel(int lvl)
    {
        if (lvl == 1 || StaticData.Levels[lvl - 2] != 0)
        {
            TransitionSign.text = "Level: " + lvl.ToString();
            StartCoroutine(LoadLevelAsync(lvl.ToString()));
        }
        else
        {
            Err.Play();
        }
    }


    private IEnumerator LoadLevelAsync(string lvl)
    {
        //StaticData.MusicSource.GetComponent<MusicController>().PauseClip();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lvl);
        asyncLoad.allowSceneActivation = false;
        TransitionAnim.Play("Close");

        yield return new WaitForSeconds(0.6f);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator NewSkinsCoroutine()
    {
        Image img = NewSkins.GetComponent<Image>();
        Color col = img.color;
        for (int i = 0; i < 10; i++)
        {
            col.a += 0.1f;
            img.color = col;
            yield return new WaitForSeconds(0.04f);
        }
        NewSkinsSign.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        NewSkinsSign.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        NewSkinsSign.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        NewSkinsSign.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        NewSkinsSign.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        NewSkinsSign.color = Color.white;
        yield return new WaitForSeconds(0.8f);
        NewSkinsAnim.Play("UIAnim2");
        for (int i = 0; i < 10; i++)
        {
            col.a -= 0.1f;
            img.color = col;
            yield return new WaitForSeconds(0.04f);
        }
        NewSkins.SetActive(false);
    }

    public void Arcade()
    {
        TransitionSign.text = "Arcade";
        StartCoroutine(LoadLevelAsync("Arcade"));
    }

    public void ScrollBalls(int dir)
    {
        if (BCurrID + dir >= 0 && BCurrID + dir <= StaticData.BallSkinsID.Length - 1) BCurrID += dir;
        if(StaticData.BallSkinsID[BCurrID] != 0)
        {
            for(int i = 0; i < StaticData.BallSkinsID.Length; i++)
            {
                if (StaticData.BallSkinsID[i] == 2) StaticData.BallSkinsID[i] = 1;
            }
            StaticData.BallSkinsID[BCurrID] = 2;
            BallExample.SetActive(true);
            BLocked.SetActive(false);
            BUnlocked.SetActive(true);
            BallExample.GetComponent<MeshRenderer>().material = StaticData.BallSkins[BCurrID];
            BNameTextField.gameObject.SetActive(true);
            BNameTextField.text = StaticData.BallSkins[BCurrID].name;
        }
        else
        {
            BallExample.SetActive(false);
            BLocked.SetActive(true);
            BUnlocked.SetActive(false);
            BNameTextField.gameObject.SetActive(false);
            BReqTextField.text = BReqText[BCurrID];
            if (BReqText[BCurrID].Contains("Lvl"))
            {
                BReqTextField.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.25f);
                BReqTextField.textWrappingMode = TextWrappingModes.NoWrap;
                BStars.SetActive(true);
            }
            else
            {
                BReqTextField.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0f);
                BReqTextField.textWrappingMode = TextWrappingModes.Normal;
                BStars.SetActive(false);
            }
        }


        if (BCurrID == 0)
        {
            BL.GetComponent<Button>().enabled = false;
            BL.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            BL.GetComponent<Button>().enabled = true;
            BL.GetComponent<Image>().color = Color.black;
        }
        if (BCurrID == StaticData.BallSkinsID.Length - 1)
        {
            BR.GetComponent<Button>().enabled = false;
            BR.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            BR.GetComponent<Button>().enabled = true;
            BR.GetComponent<Image>().color = Color.black;
        }
        if(BIDOpened.Count > 0)
        {
            if (BIDOpened.Contains(BCurrID)) BIDOpened.Remove(BCurrID);
            if(!BIDOpened.Any(x => x > BCurrID)) BR.transform.GetChild(1).gameObject.SetActive(false);
            if (!BIDOpened.Any(x => x < BCurrID)) BL.transform.GetChild(1).gameObject.SetActive(false);
        }
        Debug.Log(string.Join(", ", StaticData.BallSkinsID) + " | " + BCurrID);
    }

    public void ScrollWalls(int dir)
    {
        if (WCurrID + dir >= 0 && WCurrID + dir <= StaticData.WallSkinsID.Length - 1) WCurrID += dir;
        if (StaticData.WallSkinsID[WCurrID] != 0)
        {
            for (int i = 0; i < StaticData.WallSkinsID.Length; i++)
            {
                if (StaticData.WallSkinsID[i] == 2) StaticData.WallSkinsID[i] = 1;
            }
            StaticData.WallSkinsID[WCurrID] = 2;
            WallExample.SetActive(true);
            WLocked.SetActive(false);
            WUnlocked.SetActive(true);
            WallExample.GetComponent<MeshRenderer>().material = StaticData.WallSkins[WCurrID];
            WNameTextField.gameObject.SetActive(true);
            WNameTextField.text = StaticData.WallSkins[WCurrID].name;
        }
        else
        {
            WallExample.SetActive(false);
            WLocked.SetActive(true);
            WUnlocked.SetActive(false);
            WNameTextField.gameObject.SetActive(false);
            WReqTextField.text = WReqText[WCurrID];
            if (WReqText[WCurrID].Contains("Lvl"))
            {
                WReqTextField.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.25f);
                WReqTextField.textWrappingMode = TextWrappingModes.NoWrap;
                WStars.SetActive(true);
            }
            else
            {
                WReqTextField.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0f);
                WReqTextField.textWrappingMode = TextWrappingModes.Normal;
                WStars.SetActive(false);
            }
        }


        if (WCurrID == 0)
        {
            WL.GetComponent<Button>().enabled = false;
            WL.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            WL.GetComponent<Button>().enabled = true;
            WL.GetComponent<Image>().color = Color.black;
        }
        if (WCurrID == StaticData.WallSkinsID.Length - 1)
        {
            WR.GetComponent<Button>().enabled = false;
            WR.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            WR.GetComponent<Button>().enabled = true;
            WR.GetComponent<Image>().color = Color.black;
        }
        if (WIDOpened.Count > 0)
        {
            if (WIDOpened.Contains(WCurrID)) WIDOpened.Remove(WCurrID);
            if (!WIDOpened.Any(x => x > WCurrID)) WR.transform.GetChild(1).gameObject.SetActive(false);
            if (!WIDOpened.Any(x => x < WCurrID)) WL.transform.GetChild(1).gameObject.SetActive(false);
        }
        Debug.Log(string.Join(", ", StaticData.WallSkinsID) + " | " + WCurrID);
    }

    public void Sound()
    {
        StaticData.Sound = !StaticData.Sound;
        SUnlocked.SetActive(StaticData.Sound);
        Err.enabled = StaticData.Sound;
        for(int i = 0; i < OtherSounds.Length; i++)
        {
            OtherSounds[i].enabled = StaticData.Sound;
        }
    }

    public void Music()
    {
        StaticData.Music = !StaticData.Music;
        MUnlocked.SetActive(StaticData.Music);
        MusicSource.enabled = StaticData.Music;
    }

    public void OpenLeaderboard()
    {

    }

    public void LoadSaved()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            var data = JsonUtility.FromJson<LoadSave>(PlayerPrefs.GetString("SV"));
            StaticData.Bestscore = data.Bestscore;
            StaticData.Levels = data.Levels;
            StaticData.BallSkinsID = data.BallSkinsID;
            StaticData.WallSkinsID = data.WallSkinsID;
            StaticData.Sound = data.Audio[0] != 0;
            StaticData.Music = data.Audio[1] != 0;
        }
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
        if(StaticData.isOpened)Save();
    }
}

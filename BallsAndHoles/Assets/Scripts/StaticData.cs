using UnityEngine;

public class StaticData : MonoBehaviour
{
    public static bool isOpened;
    public static bool Sound = true;
    public static bool Music = true;
    public static float Bestscore;
    public static int[] Levels = new int[200];

    public static Material[] BallSkins;
    public static int[] BallSkinsID = new int[20];

    public static Material[] WallSkins;
    public static int[] WallSkinsID = new int[5];

    public static AudioSource MusicSource;

    public static AdController adController;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        Skins skins = Resources.Load<Skins>("SkinsData");
        if (skins != null)
        {
            BallSkins = skins.BallSkins;
            WallSkins = skins.WallSkins;
            Debug.Log("SkinsData init");
        }
        else
        {
            Debug.LogError("SkinsData asset not found in Resources folder!");
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Skins", menuName = "Scriptable Objects/Skins")]
public class Skins : ScriptableObject
{
    public Material[] BallSkins;
    public Material[] WallSkins;
}

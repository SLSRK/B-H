using UnityEngine;
using System.Collections;
using System;

public class WinController : MonoBehaviour
{
    public GameController GC;
    private void OnCollisionEnter(Collision collision)
    {
        GC.OnWin();
    }
}

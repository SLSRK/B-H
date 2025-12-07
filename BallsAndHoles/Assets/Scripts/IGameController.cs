using UnityEngine;

public interface IGameController
{
    bool over { get; }
    void OnWin();
}

using System;
using UnityEngine;

public static class EventManager
{
    public static Action OnGameOver;
    public static Action OnGameStart;
    public static Action OnGameWin;
    public static Action CoinCollect;
    public static void ClearAllEvents()
    {
        OnGameOver = null;
        OnGameStart = null;
        OnGameWin = null;
        CoinCollect = null;
    }
}

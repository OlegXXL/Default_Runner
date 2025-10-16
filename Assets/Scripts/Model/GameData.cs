using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static int _coin;
    public static int Coin
    {
        get { return _coin; }
        set { PlayerPrefs.SetInt("Coin", (_coin = value)); }
    }
    static GameData()
    {
        _coin = PlayerPrefs.GetInt("Coin", 0);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject finishPanel;
    public Text counterCoin_txt;
    public GameObject startPanel;
    public GameObject winPanel;
    private int coinCount;
    void OnEnable()
    {
        EventManager.OnGameOver += GameFinish;
        EventManager.OnGameWin += WinGame;
        EventManager.CoinCollect += UpdateCounter;
    }
    public void UpdateCounter()
    {
        coinCount = GameData.Coin;
        counterCoin_txt.text = $"{coinCount}";
    }
    public void StartGame()
    {
        EventManager.OnGameStart?.Invoke();
        startPanel.SetActive(false);
    }
    private void Awake()
    {
        UpdateCounter();
    }
    public void GameFinish()
    {
        finishPanel.SetActive(true);
    }
    public void RestartGame()
    {
        EventManager.ClearAllEvents();
        SceneManager.LoadScene(0);
    }
    public void WinGame()
    {
        winPanel.SetActive(true);
    }
}

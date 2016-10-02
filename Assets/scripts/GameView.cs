using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GameView : GameLogic
{

    public List<GameObject> Hearts = new List<GameObject>();
    public GameObject StartGamePanel;
    public GameObject TopPanel;
    public GameObject Shadow;
    public Text       text;
    public Text       txtPercent;
    public Button     BtnNext;
    public Button     BtnStartGame;
    public Button     BtnShadow;
    public Button     BtnResume;
    public Button     BtnRestart;
    public Button     BtnExit;
    public Button     BtnPause;


    public override void OnEnable()
    {
        //TODO// правильний порядок
        base.OnEnable();
        BtnStartGame.onClick.AddListener(StartGame);
        BtnResume.onClick.AddListener(OnBtnResumeClick);
        BtnNext.onClick.AddListener(OnBtnNextClick);

        BtnRestart.onClick.AddListener(OnBtnRestarClick);
        BtnExit.onClick.AddListener(OnBtnExitClick);
        BtnPause.onClick.AddListener(OnBtnPauseClick);
        //  StartGamePanel.SetActive(true);
        StartGame();
    }
    void OnDisable()
    {
        BtnResume.onClick.RemoveAllListeners();
        BtnRestart.onClick.RemoveAllListeners();
        BtnExit.onClick.RemoveAllListeners();
        BtnPause.onClick.RemoveAllListeners();
        BtnNext.onClick.RemoveAllListeners();

    }

    void OnBtnResumeClick()
    {
        Time.timeScale = 1;
        ShowPause(false);
    }

    void OnBtnRestarClick()
    {
        ShowGameOver(false);
        StartGame();
    }

    void OnBtnExitClick()
    {

    }
    void OnBtnNextClick()
    {

    }


        void OnBtnPauseClick()
    {
        Time.timeScale = 0;
        ShowPause(true);
    }

    void ShowPause(bool active)
    {
        ShowShadow(active);
        BtnRestart.gameObject.SetActive(active);
        BtnExit.gameObject.SetActive(active);
        BtnResume.gameObject.SetActive(active);
    }  

    public override void ShowPercent(float CurrentPercent, float AllPercent)
    {
        base.ShowPercent(CurrentPercent, AllPercent);

        txtPercent.text = string.Format("Progress: {0} / {1}%", (int)CurrentPercent, (int)AllPercent);
    }

    public override void StartGame()
    {
        base.StartGame();

        OffAll();
        StartGamePanel.SetActive(false);
    }

    public override void GameOver()
    {
        base.GameOver();

        ShowGameOver(true);
    }

    private void ShowGameOver(bool active)
    {
        text.text = "You Lose :(";
        text.gameObject.SetActive(active);
        ShowShadow(active);
        BtnRestart.gameObject.SetActive(active);
        BtnExit.gameObject.SetActive(active);
        TopPanel.SetActive(false);
    }

    public override void GameWin()
    {
        base.GameWin();

        ShowGameWin(true);
    }

    private void ShowGameWin(bool active)
    {
        text.text = "You Win :)";
        text.gameObject.SetActive(active);
        ShowShadow(active);
        BtnRestart.gameObject.SetActive(active);
        BtnExit.gameObject.SetActive(active);
        TopPanel.SetActive(false);
    }

    void ShowShadow(bool active)
    {
        Shadow.SetActive(active);
    }

    void OffAll()
    {
        TopPanel.SetActive(true);
        Shadow.SetActive(false);
        text.gameObject.SetActive(false);
        BtnRestart.gameObject.SetActive(false);
        BtnExit.gameObject.SetActive(false);
        BtnResume.gameObject.SetActive(false);
    }

    public override void ShowHeart(int count)
    {
        base.ShowHeart(count);

        for (int i = 0; i < count || i < Hearts.Count; i++)
        {
            if (i < count)
                Hearts[i].gameObject.SetActive(true);
            else
                Hearts[i].gameObject.SetActive(false);

        }
    }


}

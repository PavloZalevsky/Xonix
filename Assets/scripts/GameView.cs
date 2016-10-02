using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameView : GameLogic
{

    public List<GameObject> Hearts = new List<GameObject>();
    public GameObject TopPanel;
    public GameObject Shadow;
    public Text       text;
    public Text       txtPercent;
    public Button     BtnShadow;
    public Button     BtnResume;
    public Button     BtnRestart;
    public Button     BtnExit;
    public Button     BtnPause;


    public override void OnEnable()
    {
        base.OnEnable();

        BtnResume.onClick.AddListener(OnBtnResumeClick);
        BtnRestart.onClick.AddListener(OnBtnRestarClick);
        BtnExit.onClick.AddListener(OnBtnExitClick);
        BtnPause.onClick.AddListener(OnBtnPauseClick);

        OffAll();
    }
    void OnDisable()
    {
        BtnResume.onClick.RemoveAllListeners();
        BtnRestart.onClick.RemoveAllListeners();
        BtnExit.onClick.RemoveAllListeners();
        BtnPause.onClick.RemoveAllListeners();
    }

    void OnBtnResumeClick()
    {
        Time.timeScale = 1;
        ShowPause(false);
    }

    void OnBtnRestarClick()
    {

    }

    void OnBtnExitClick()
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

    public void ShoWinGame(bool active)
    {
        text.text = "You Win!!!";
        text.gameObject.SetActive(active);

    }

    public override void ShowPercent(float CurrentPercent, float AllPercent)
    {
        base.ShowPercent(CurrentPercent, AllPercent);

        txtPercent.text = string.Format("Progress: {0} / {1}%", (int)CurrentPercent, (int)AllPercent);
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



}

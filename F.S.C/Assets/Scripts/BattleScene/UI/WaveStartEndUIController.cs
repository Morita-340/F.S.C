using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WaveStartEndUIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI WaveNumberText;
    [SerializeField]
    TextMeshProUGUI WaveText;
    [SerializeField]
    TextMeshProUGUI WaveClearText;
    [SerializeField]
    Image WarningBack;
    [SerializeField]
    GameObject WarningUp;
    [SerializeField]
    GameObject WarningDown;
    [SerializeField]
    TextMeshProUGUI StageNameText;
    SoundController SCer;
    // Start is called before the first frame update
    void Start()
    {
        SCer = GetComponent<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StageEntryUI(string inputStageName){
        StageNameText.text = inputStageName;
    }
    public void StageEntryUIFade(){
        StartCoroutine(UIFadeGradually(StageNameText));
    }
    public IEnumerator WaveStartUI(int waveNum,bool finalWaveFlag,string inputStageName){
        string waveNumText;
        if(waveNum == 1){
            StageNameText.text = inputStageName;
            waveNumText = waveNum.ToString() + "st";}
        else if(waveNum == 2){waveNumText = waveNum.ToString() + "nd";}
        else if(waveNum == 3){waveNumText = waveNum.ToString() + "rd";}
        else{waveNumText = waveNum.ToString() + "th";}
        if(finalWaveFlag){waveNumText = "Final";}
        //初期値設定
        WaveNumberText.text = waveNumText;
        WaveNumberText.color = new Color(1,1,1,1);
        WaveNumberText.rectTransform.anchoredPosition = new Vector3(-500,0,0);
        WaveText.text = "Wave";
        WaveText.color = new Color(1,1,1,1);
        WaveText.rectTransform.anchoredPosition = new Vector3(500,0,0);
        //アニメーション
        StartCoroutine(UIFadeGradually(WaveNumberText));
        StartCoroutine(UIFadeGradually(WaveText));
        if(waveNum == 1){
            StartCoroutine(UIFadeGradually(StageNameText));
            StartCoroutine(SCer.PlayBGM(0));
        }
        if(finalWaveFlag){
            yield return StartCoroutine(WarningPerfomance());
        }else{
            SCer.PlaySE(0);
        }
        yield return null;
    }
    /// <summary>
    /// UIを止める。動きそのものは呼び出し側のtimeScale=0で止まっている
    /// こちらでは音の制御のみ担当する
    /// </summary>
    public void PauseUI(){
        SCer.PauseBGM();
        SCer.PauseSE();
    }
    public void UnPauseUI(){
        SCer.UnPauseBGM();
        SCer.UnPauseSE();
    }
    public void WaveClearUI(){
        WaveClearText.color = new Color(1,1,1,1);
        StartCoroutine(UIFadeGradually(WaveClearText));
        //クリア音声の再生
        SCer.PlaySE(1);
    }
    private IEnumerator UIFadeGradually(TextMeshProUGUI text){
        float moveTime = 1f;
        text.rectTransform.DOLocalMove(Vector2.zero, moveTime);
        yield return new WaitForSeconds(moveTime+0.2f);
        text.DOFade(0,0.5f);
    }
    private IEnumerator WarningPerfomance(){
        //警告音の再生
        SCer.StopBGM();
        SCer.PlaySE(2);
        //警告背景のスケールを大きくしながら表示
        WarningBack.gameObject.SetActive(true);
        WarningBack.gameObject.transform.DOScaleY(1,0.1f);
        //警告の画像を表示する
        WarningUp.SetActive(true);
        WarningDown.SetActive(true);
        //警告背景のalpha値を大きくしたり小さくしたりする
        DOTween.ToAlpha(
            () => WarningBack.color,
            color => WarningBack.color = color,
            0.4f,
            0.5f
        );
        yield return new WaitForSeconds(0.5f);
        DOTween.ToAlpha(
            () => WarningBack.color,
            color => WarningBack.color = color,
            0.1f,
            0.5f
        );
        yield return new WaitForSeconds(0.5f);
        DOTween.ToAlpha(
            () => WarningBack.color,
            color => WarningBack.color = color,
            0.4f,
            0.5f
        );
        yield return new WaitForSeconds(0.5f);
        DOTween.ToAlpha(
            () => WarningBack.color,
            color => WarningBack.color = color,
            0.1f,
            0.5f
        );
        yield return new WaitForSeconds(0.5f);
        //一定時間経ったら警告背景はスケールを小さくしながら非表示
        WarningBack.gameObject.transform.DOScaleY(0,0.1f);
        //警告画像もほぼ同じタイミングで非表示にする
        WarningUp.SetActive(false);
        WarningDown.SetActive(false);
        yield return new WaitForSeconds(1f);
        WarningBack.gameObject.SetActive(false);
        SCer.FadeSE();
        StartCoroutine(SCer.PlayBGM(1));
    }
    public void BattleEnd(){
        SCer.StopBGM();
    }
}

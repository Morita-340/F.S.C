using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using FSCGeneral;

public class ResultUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ResultSituation;
    [SerializeField] TextMeshProUGUI DefeatPoint;
    [SerializeField] TextMeshProUGUI MaximumCombatPower;
    [SerializeField] TextMeshProUGUI NoDamageBonus;
    [SerializeField] TextMeshProUGUI TotalScore;
    [SerializeField] PlayerSActionFeedBackUIController PAFBUC;
    [SerializeField] GameObject TitleButton;
    private RectTransform rectTransform;
    private string resultSituation;
    private int defeatpoint;
    private int displayDefeatPoint; 
    private int maximumCombatPower;
    private int displayMaximumCombatPower; 
    private int noDamageBonus;
    private int noDamageClearWaveNum = 0;
    private int displayNoDamageBonus; 
    private int totalScore;
    private int displayTotalScore;
    private SoundController SCer;
    // Start is called before the first frame update
    void Start()
    {
        SCer = GetComponent<SoundController>();
        rectTransform = GetComponent<RectTransform>();
        ResultSituation     .text = " ";
        DefeatPoint         .text = " ";
        MaximumCombatPower  .text = " ";
        NoDamageBonus       .text = " ";
        TotalScore          .text = " ";
    }

    // Update is called once per frame
    void Update()
    {
        if(displayDefeatPoint != 0)DefeatPoint.text = displayDefeatPoint.ToString();
        if(displayMaximumCombatPower != 0)MaximumCombatPower.text = displayMaximumCombatPower.ToString();
        if(displayNoDamageBonus != 0)NoDamageBonus.text = displayNoDamageBonus.ToString();
        if(displayTotalScore != 0)TotalScore.text = displayTotalScore.ToString();
    }
    /// <summary>
    /// ゲームマネージャーで呼び出すリザルトUIの表示制御関数
    /// </summary>
    public IEnumerator ResultUI(GSetting.ResultSituation situation){
        //目標値の設定
        bool isClear = IsThisSituationClear(situation);
        if(isClear){StartCoroutine(SCer.PlayBGM(0));}
        else{StartCoroutine(SCer.PlayBGM(1));}
        resultSituation = situation.ToString();
        var resultStatus = PAFBUC.GetPlayerFinalStatus();
        defeatpoint = resultStatus.Item1;
        maximumCombatPower = resultStatus.Item2;
        noDamageBonus = 500*noDamageClearWaveNum;
        totalScore = defeatpoint + maximumCombatPower + noDamageBonus;
        if(PAFBUC == null){yield break;}
        //縦スケールを増やしてウィンドウとして表示させる
        rectTransform.DOScaleY(1,0.5f);
        yield return new WaitUntil(() => transform.localScale.y ==1);
        //結果状況の表示
        yield return new WaitForSeconds(0.3f);
        ResultSituation.text = resultSituation;
        yield return new WaitForSeconds(1f);
        ////撃破ポイントの表示
        SCer.PlaySE(0);
        DOTween.To(() => displayDefeatPoint,(x) => displayDefeatPoint = x,defeatpoint,1f);
        SCer.StopSE();
        yield return new WaitForSeconds(1f);
        ////最大戦闘力の表示
        SCer.PlaySE(0);
        DOTween.To(() => displayMaximumCombatPower,(x) => displayMaximumCombatPower = x,maximumCombatPower,1f);
        SCer.StopSE();
        yield return new WaitForSeconds(1f);
        ////ノーダメージボーナスを表示
        SCer.PlaySE(0);
        DOTween.To(() => displayNoDamageBonus,(x) => displayNoDamageBonus = x, noDamageBonus,1f);
        SCer.StopSE();
        yield return new WaitForSeconds(1f);
        ////最終結果を表示
        SCer.PlaySE(0);
        DOTween.To(() => displayTotalScore,(x) => displayTotalScore = x,totalScore,2f);
        SCer.StopSE();
        yield return new WaitForSeconds(2f);
        //タイトルへ戻るボタンを表示
        TitleButton.GetComponent<WindowTranslateButton>().Executable(true);
        TitleButton.transform.DOScaleX(1,0.5f);
        //表彰演出（congratulation!とか）
        switch (resultSituation){
            default:break;}
    }
    public void noDamageClearWaveNumCountUp(){
        noDamageClearWaveNum ++;
    }
    /// <summary>
    /// 入力のシチュエーションが成功なのか失敗なのかを識別する
    /// </summary>
    /// <param name="resultSituation"></param>
    /// <returns></returns>
    protected bool IsThisSituationClear(GSetting.ResultSituation resultSituation){
        bool isClear;
        switch (resultSituation){
            case GSetting.ResultSituation.AllWaveClear:{isClear = true;break;}
            case GSetting.ResultSituation.PlayerDestroyed:{isClear = false;break;}
            default : isClear = false;break;
        }
        return isClear;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;
using FSCGeneral;

public class TakeOffButton : WindowTranslateButton
{
    bool preparedForTakeOff = false;
    bool preparedForTakeOffFirstTime = true;
    TakeOffPerformanceController TakeOffPC;
    [SerializeField] SoundController MainBGMSCer;
    /// <summary>
    /// Update内で1回だけ実行したい処理のために使用
    /// </summary>
    bool executeOnceFlag = true;
    protected override void Update()
    {
        if (preparedForTakeOffFirstTime)
        {
            if (preparedForTakeOff)
            {
                GetComponent<RectTransform>().DOScaleX(1, 0.5f);
                preparedForTakeOffFirstTime = false;
            }
        }
        base.Update();
    }
    protected override void IconPushedAccepted()
    {
        base.IconPushedAccepted();
    }
    public void SetTranslateScene(GSetting.SceneName sceneName)
    {
        TranslateScene = sceneName;
    }
    protected override IEnumerator SceneTranslate()
    {
        if (executeOnceFlag)
        {
            executeOnceFlag = false;
            yield return StartCoroutine(TakeOffPC.Perform());
            yield return StartCoroutine(base.SceneTranslate());
        }
    }
    public void PreparedForTakeOff()
    {
        preparedForTakeOff = true;
    }
    public void SetPerformanceController(TakeOffPerformanceController inputTakeOffPC)
    {
        TakeOffPC = inputTakeOffPC;
    }
    public bool GetExecuteFlag()
    {
        return executeFlag;
    }
    /// <summary>
    /// シーン遷移のウィンドウ閉じのアニメーションの際にかざしてしまう問題への対処
    /// </summary>
    /// <param name="eventData"></param>
   public override void OnPointerEnter(PointerEventData eventData)
    {
        if (executeOnceFlag)
        {
            CursolSelected = true;
            SCer.PlaySE(0);
        }
    }
    /// <summary>
    /// シーン遷移のウィンドウ閉じのアニメーションの際にかざしてしまう問題への対処
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (executeOnceFlag)
        {
            CursolSelected = false;
        }
    }
}

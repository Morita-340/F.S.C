using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using FSCGeneral;

/// <summary>
/// ステージセレクト画面において、マップ領域に表示するステージのアイコン
/// </summary>
public class StageIcon : GeneralUIIconController
{
    [SerializeField]
    private StageData stageData = new StageData();
    [SerializeField]
    StageSelectManager SSM;
    [SerializeField]
    GeneralFlagManager GFM;
    private float chargeTime = 0;
    private float selectTime = 1;
    protected bool executeOnce = true;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(CursolSelected){chargeTime += Time.deltaTime;}
        else{chargeTime = 0;
            executeOnce = true;}
        if(chargeTime > selectTime){
            SSM.SetSelectedStage(this.stageData);
            GFM.SetStageName(this.stageData.StageName);
            if(executeOnce){
                executeOnce = false;
                SCer.PlaySE(1);
            }
        }
    }
    public override void OnPointerEnter(PointerEventData eventData){
        base.OnPointerEnter(eventData);
        SSM.SetDisplayStageData(stageData);
    }
}
/// <summary>
/// データはここに格納する
/// </summary>
[Serializable]
public class StageData
{
    public string StageName;
    [TextArea]
    public string StageSummary;
    [TextArea]
    public string StageAdvise;
    public bool StageAlreadyCleared = false;
    public GSetting.SceneName scene;
}

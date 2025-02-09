using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

/// <summary>
/// ステージセレクト画面において、マップ領域に表示するステージのアイコン
/// </summary>
public class StageIcon : GeneralUIIconController
{
    [SerializeField]
    private StageData stageData = new StageData();
    [SerializeField]
    StageSelectManager SSM;
    private float chargeTime = 0;
    private float selectTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(CursolSelected){chargeTime += Time.deltaTime;}
        else{chargeTime = 0;}
        if(chargeTime > selectTime){
            SSM.SetSelectedStage(this.stageData);}
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
    public SceneAsset scene;
}

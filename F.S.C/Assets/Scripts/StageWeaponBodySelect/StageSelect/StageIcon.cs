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
    [SerializeField,ReadOnly]
    StageSelectManager SSM;
    [SerializeField, ReadOnly]
    StagePlayerSelectMenuManager SPSMM;
    [SerializeField, ReadOnly]
    GeneralFlagManager GFM;
    private float chargeTime = 0;
    private float selectTime = 1;
    protected bool executeOnce = true;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SSM = FindObjectOfType<StageSelectManager>();
        SPSMM = FindObjectOfType<StagePlayerSelectMenuManager>();
        GFM = FindObjectOfType<GeneralFlagManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0) && CursolSelected)
        {
            SSM.SetSelectedStage(this.stageData);
            SPSMM.ChangeToPlayerSelectMenu();
            GFM.SetStageName(this.stageData.StageName);
            SCer.PlaySE(1);
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

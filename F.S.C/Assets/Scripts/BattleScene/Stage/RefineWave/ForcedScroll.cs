using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "RefineWaveData", menuName = "CreateRefineWaveData/強制スクロールウェーブ")]
public class ForcedScroll : RefineWaveData,ScrollWaveIF
{
    [SerializeField]
    List<RefineEnemyUnitDestroyManagementScript> instantiableEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    [SerializeField, ReadOnly]
    MainCameraController MCC;
    [SerializeField]
    Vector2 ScrollStartPos = Vector2.zero;
    [SerializeField]
    Vector2 ScrollGoalPos = Vector2.one;
    ForcedScrollRuntime FSRT;
    /// <summary>
    /// 生存時間
    /// </summary>
    [SerializeField, Range(1f, 60f)]
    float scrollTime = 30f;
    //色んなルール（変数）を記述
    public override IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS)
    {
        FSRT = ReBSFM.gameObject.AddComponent<ForcedScrollRuntime>();
        waveRuntime = FSRT;
        FSRT.InitialSetting(inputRePUDMS, instantiableEnemyList, GetSelectedWaveShapePreset(),scrollTime);
        return FSRT;
    }
    public void SetMCC(MainCameraController inputMCC)
    {
        Debug.LogAssertion("DDDD");
        FSRT.ScrollFlowCall(inputMCC,ScrollStartPos,ScrollGoalPos);
        MCC = inputMCC;
    }
    public override void DestroyProcess(RefineBattleSceneFlowManager ReBSFM)
    {
        FSRT.DestroyProcess();
        Destroy(FSRT);
    }
    public override string GetGoalText()
    {
        return GoalText;
    }
    public override string GetExplainText()
    {
        return ExplainText;
    }
    public override Vector2 GetExplainTexPos()
    {
        return ExplainTexPos;
    }
}

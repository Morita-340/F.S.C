using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "RefineWaveData", menuName = "CreateRefineWaveData/生存ウェーブ")]
public class Survival : RefineWaveData
{
    [SerializeField]
    List<RefineEnemyUnitDestroyManagementScript> instantiableEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    SurvivalRuntime SRT;
    /// <summary>
    /// 生存時間
    /// </summary>
    [SerializeField, Range(1f, 60f)]
    float survivalTime = 30f;
    //色んなルール（変数）を記述
    public override IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS)
    {
        SRT = ReBSFM.AddComponent<SurvivalRuntime>();
        waveRuntime = SRT;
        SRT.InitialSetting(inputRePUDMS, instantiableEnemyList, GetSelectedWaveShapePreset(),survivalTime);
        return SRT;
    }
    public override void DestroyProcess(RefineBattleSceneFlowManager ReBSFM)
    {
        SRT.DestroyProcess();
        Destroy(SRT);
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

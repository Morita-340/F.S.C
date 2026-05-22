using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 敵全滅を目的とするウェーブ
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "RefineWaveData", menuName = "CreateRefineWaveData/敵全滅ウェーブ")]
public class EliminateAllEnemy : RefineWaveData
{
    [SerializeField]
    List<RefineEnemyUnitDestroyManagementScript> instantiableEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    EliminateAllEnemyRunTime EAERT;
    //色んなルール（変数）を記述
    public override IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS)
    {
        Debug.LogWarning("WWWWWWWW");
        EAERT = ReBSFM.AddComponent<EliminateAllEnemyRunTime>();
        waveRuntime = EAERT;
        EAERT.InitialSetting(inputRePUDMS, instantiableEnemyList,GetSelectedWaveShapePreset());
        return EAERT;
    }
    public override void DestroyProcess(RefineBattleSceneFlowManager ReBSFM)
    {
        EAERT.DestroyProcess();
        Destroy(EAERT);
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

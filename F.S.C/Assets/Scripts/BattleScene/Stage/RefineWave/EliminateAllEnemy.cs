using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EliminateAllEnemy : RefineWaveData
{
    [SerializeField]
    List<RefineEnemyUnitDestroyManagementScript> instantiableEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    EliminateAllEnemyRunTime EAERT;
    //色んなルール（変数）を記述
    public override IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS)
    {
        EAERT = ReBSFM.AddComponent<EliminateAllEnemyRunTime>();
        EAERT.InitialSetting(inputRePUDMS, instantiableEnemyList);
        return EAERT;
    }
    public override void DestroyProcess(RefineBattleSceneFlowManager ReBSFM)
    {
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

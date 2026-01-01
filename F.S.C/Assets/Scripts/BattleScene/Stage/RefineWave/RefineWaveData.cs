using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RefineWaveData : ScriptableObject
{
    [SerializeField]protected string GoalText;
    [SerializeField]protected string ExplainText;
    [SerializeField]protected Vector2 ExplainTexPos;
    public abstract IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS);
    public abstract void DestroyProcess(RefineBattleSceneFlowManager ReBSFM);
    public abstract string GetGoalText();
    public abstract string GetExplainText();
    public abstract Vector2 GetExplainTexPos();
}

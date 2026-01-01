using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EliminateAllEnemyRunTime : IWaveRuntime
{
    public void InitialSetting(RefinePlayerUnitDestroyManagementScript inputRePUDMS, List<RefineEnemyUnitDestroyManagementScript> EUDMSList)
    {
        RePUDMS = inputRePUDMS;
        ReSpS = this.AddComponent<RefineSpawnSystem>();
        SpawnedEnemyList = ReSpS.Spawn(RePUDMS.GetCombatPower(), EUDMSList);
        initialSettingFlag = true;
    }
    protected override void DetermineSuccessORFailure()
    {
        for (int i = 0; i < SpawnedEnemyList.Count; i++)
        {
            if (SpawnedEnemyList[i] != null) { break; }
            //生成した敵が全滅したので
            successFlag = true;
        }
        if (RePUDMS != null)
        {
            
        }
    }
    public override bool GetSuccessFlag() {return successFlag;}
    public override bool GetFailureFlag() {return failureFlag;}
    void Update()
    {
        if(initialSettingFlag)DetermineSuccessORFailure();
    }
}

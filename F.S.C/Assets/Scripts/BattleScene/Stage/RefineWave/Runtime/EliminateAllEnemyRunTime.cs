using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EliminateAllEnemyRunTime : IWaveRuntime
{
    public void InitialSetting(RefinePlayerUnitDestroyManagementScript inputRePUDMS, List<RefineEnemyUnitDestroyManagementScript> EUDMSList, Vector3[] InstPosList)
    {
        RePUDMS = inputRePUDMS;
        ReSpS = this.AddComponent<RefineSpawnSystem>();
        SpawnedEnemyList = ReSpS.Spawn(RePUDMS.GetCombatPower(), EUDMSList, InstPosList);
        initialSettingFlag = true;
    }
    protected override void DetermineSuccessORFailure()
    {
        if(SpawnedEnemyList.All(x => x == null))successFlag = true;
        base.DetermineSuccessORFailure();
    }
    protected override void Update()
    {
        base.Update();
    }
}

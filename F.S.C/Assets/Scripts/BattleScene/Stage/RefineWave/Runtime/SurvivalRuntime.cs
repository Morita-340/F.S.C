using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SurvivalRuntime : IWaveRuntime
{
    [SerializeField, ReadOnly]
    float nowTime = 0;
    float survivalTime = 0;
    BoolEdgeTrigger sucessTrigger;
    public void InitialSetting(RefinePlayerUnitDestroyManagementScript inputRePUDMS, List<RefineEnemyUnitDestroyManagementScript> EUDMSList, Vector3[] InstPosList, float inputSurvivalTime)
    {
        RePUDMS = inputRePUDMS;
        ReSpS = this.AddComponent<RefineSpawnSystem>();
        SpawnedEnemyList = ReSpS.Spawn(RePUDMS.GetCombatPower(), EUDMSList, InstPosList);
        initialSettingFlag = true;
        survivalTime = inputSurvivalTime;
        sucessTrigger = new BoolEdgeTrigger(() => successFlag);
    }
    protected override void DetermineSuccessORFailure()
    {
        for (int i = 0; i < SpawnedEnemyList.Count; i++)
        {
            if (SpawnedEnemyList[i] != null) { break; }
            //生成した敵が全滅したので
            successFlag = true;
        }
        if (nowTime >= survivalTime)
        {
            successFlag = true;
        }
        base.DetermineSuccessORFailure();
    }
    protected override void Update()
    {
        base.Update();
        nowTime += Time.deltaTime;
        //敵全滅と違い、クリア時に敵が残っている可能性があるため、クリア時に敵を消す処理が必要
        if (sucessTrigger.Rising())
        {
            foreach (RefineEnemyUnitDestroyManagementScript ReEUDMS in SpawnedEnemyList)
            {
                Destroy(ReEUDMS?.gameObject);
            }
        }
    }
}

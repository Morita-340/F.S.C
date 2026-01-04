using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWaveRuntime : MonoBehaviour
{
    protected RefineSpawnSystem ReSpS;
    protected bool successFlag = false;
    protected bool failureFlag = false;
    protected bool initialSettingFlag = false;
    protected List<RefineEnemyUnitDestroyManagementScript> SpawnedEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    protected RefinePlayerUnitDestroyManagementScript RePUDMS;
    protected BoolEdgeTrigger sucessTrigger;
    private void Start()
    {
        sucessTrigger = new BoolEdgeTrigger(() => successFlag);
    }
    protected virtual void DetermineSuccessORFailure()
    {
        if (RePUDMS != null)
        {
            if (RePUDMS.GetIsDead())
            {
                failureFlag = true;
            }
        }
    }
    public bool GetSuccessFlag() { return successFlag; }
    public bool GetFailureFlag() { return failureFlag; }
    public void DestroyProcess() { if (GetComponent<RefineSpawnSystem>()) Destroy(ReSpS); }
    protected virtual void Update()
    {
        if (initialSettingFlag) DetermineSuccessORFailure();
        //クリア時に敵が残っている可能性があるため、クリア時に敵を消す処理が必要
        if (sucessTrigger.Rising())
        {
            foreach (RefineEnemyUnitDestroyManagementScript ReEUDMS in SpawnedEnemyList)
            {
                Destroy(ReEUDMS?.gameObject);
            }
        }
    }
}

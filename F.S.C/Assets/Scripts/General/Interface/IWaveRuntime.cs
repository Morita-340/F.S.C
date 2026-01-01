using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWaveRuntime:MonoBehaviour
{
    protected RefineSpawnSystem ReSpS;
    protected bool successFlag = false;
    protected bool failureFlag = false;
    protected bool initialSettingFlag = false;
    protected List<RefineEnemyUnitDestroyManagementScript> SpawnedEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
    protected RefinePlayerUnitDestroyManagementScript RePUDMS;
    protected abstract void DetermineSuccessORFailure();
    public abstract bool GetSuccessFlag();
    public abstract bool GetFailureFlag();
}

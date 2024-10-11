using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamagedDebugger : MonoBehaviour
{
    [SerializeField]
    UnitBase unitBase;
    [SerializeField]
    UnitBase anotherUnitBase;
    public void DebugDamaged(){
        unitBase.DebugDamaged();
    }
    public void DebugDamagedToMultiUnit(){
        unitBase.DebugDamaged();
        if(anotherUnitBase != null){anotherUnitBase.DebugDamaged();}
        else if(anotherUnitBase == unitBase){Debug.LogWarning("同じユニットを設定しています");}
        else{Debug.LogWarning("Another Unit is not registered or Something Incident has happened !");}
    }
}

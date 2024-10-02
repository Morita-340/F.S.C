using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamagedDebugger : MonoBehaviour
{
    [SerializeField]
    UnitBase unitBase;
    public void DebugDamaged(){
        unitBase.DebugDamaged();
    }
}

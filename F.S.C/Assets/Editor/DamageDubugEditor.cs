using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(UnitDamagedDebugger))]
public class DamageDubugEditor : Editor
{
    public override void OnInspectorGUI(){
        UnitDamagedDebugger UDD= (UnitDamagedDebugger)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("1 Point Damage To This Unit!")){
            Debug.LogWarning("ダメージ付与のデバッグ用チート発動！" + UDD.name);
            UDD.DebugDamaged();
        }
        if(GUILayout.Button("1 Point Damage To Multi Units!")){
            Debug.LogWarning("複数ユニットにダメージ付与のデバッグ用チートを発動！" + UDD.name);
            UDD.DebugDamagedToMultiUnit();
        }
    }
}

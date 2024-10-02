using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//こういうのはエディタ拡張の一種である。
[CustomEditor(typeof(UnitDamagedDebugger))]
public class DamageDubugEditor : Editor
{
    public override void OnInspectorGUI(){
        UnitDamagedDebugger UDD= (UnitDamagedDebugger)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("1 Point Damage!")){
            Debug.LogWarning("ダメージ付与のデバッグ用チート発動！");
            UDD.DebugDamaged();
        }
    }
}

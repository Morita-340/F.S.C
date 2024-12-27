using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "WaveData",menuName = "CreateWaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField,ReadOnly]
    int combatPower = 0;
    public List<GameObject> waveEnemyList = new List<GameObject>();
    /// <summary>
    /// 複数機体が同時に出現するウェーブの総戦闘力を計測する
    /// </summary>
    /// <returns></returns>
    public int GetCombatPower(){
        combatPower = 0;
        foreach(GameObject waveEnemy in waveEnemyList){
            combatPower += waveEnemy.GetComponent<EnemyUnitDestroyManagementScript>().GetCombatPower();
        }
        Debug.Log("CombatPower :"+combatPower);
        return combatPower;
    }
    /// <summary>
    /// 複数機体が同時に出現するウェーブの総戦闘力を計測する
    /// </summary>
    /// <returns></returns>
    [ContextMenu("CaluculateCombatPower")]
    private void GetCombatPowerOnInspector(){
        combatPower = 0;
        foreach(GameObject waveEnemy in waveEnemyList){
            combatPower += waveEnemy.GetComponent<EnemyUnitDestroyManagementScript>().CaluculateCombatPower();
        }
        Debug.Log("CombatPower :"+combatPower);
    }
    public List<GameObject> GetWaveEnemyList(){
        return waveEnemyList;
    }
}

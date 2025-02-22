using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "WaveData",menuName = "CreateWaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField,ReadOnly]
    private int combatPower = 0;
    [SerializeField]
    private GSetting.WaveShapePreset waveShapePreset;
    public List<GameObject> waveEnemyList = new List<GameObject>();
    /// <summary>
    /// 前から順に座標を利用していく。改行単位で1ウェーブの敵機数を増やす（3-6-9-12,4-8-12）
    /// </summary>
    private Vector3[][] InstPosPreSet = new Vector3[9][]{//0-1 画面上のx座標,画面上のy座標,-180-180回転角
        //四隅に出現
        new Vector3[12]{new Vector3(0.1f,0.9f,-120),new Vector3(0.1f,0.1f,-60),new Vector3(0.9f,0.9f,120),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0)},
        //上から塊で出現
        new Vector3[12]{new Vector3(0.5f,0.9f,180),new Vector3(0.1f,0.9f,-120),new Vector3(0.9f,0.9f,120),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //下から塊で出現
        new Vector3[12]{new Vector3(0.5f,0.1f,0),new Vector3(0.1f,0.1f,-60),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //右から塊で出現
        new Vector3[12]{new Vector3(0.9f,0.5f,90),new Vector3(0.9f,0.9f,120),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左から塊で出現
        new Vector3[12]{new Vector3(0.1f,0.5f,-90),new Vector3(0.1f,0.9f,-120),new Vector3(0.1f,0.1f,-60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //上下から出現
        new Vector3[12]{new Vector3(0.3f,0.9f,180),new Vector3(0.3f,0.1f,0),new Vector3(0.7f,0.9f,180),new Vector3(0.7f,0.1f,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左右から出現
        new Vector3[12]{new Vector3(0.9f,0.3f,90),new Vector3(0.1f,0.3f,-90),new Vector3(0.9f,0.7f,90),new Vector3(0.1f,0.7f,-90),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //右上と左下から出現
        new Vector3[12]{new Vector3(0.7f,0.9f,120),new Vector3(0.3f,0.1f,-60),new Vector3(0.9f,0.7f,120),new Vector3(0.1f,0.3f,-60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左上と右下から出現
        new Vector3[12]{new Vector3(0.3f,0.9f,-120),new Vector3(0.9f,0.3f,60),new Vector3(0.1f,0.7f,-120),new Vector3(0.7f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
    };
    /// <summary>
    /// Vector3だが、（0-1 画面上のx座標,画面上のy座標,-180-180回転角）を割り当てている
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetSelectedWaveShapePreset(){
        return InstPosPreSet[(int)waveShapePreset];
    }
    /// <summary>
    /// 複数機体が同時に出現するウェーブの総戦闘力を計測する
    /// </summary>
    /// <returns></returns>
    public int GetCombatPower(){
        combatPower = 0;
        foreach(GameObject waveEnemy in waveEnemyList){
            combatPower += waveEnemy.GetComponent<EnemyUnitDestroyManagementScript>().CaluculateCombatPower();
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
            EnemyUnitDestroyManagementScript EUDMS = waveEnemy.GetComponent<EnemyUnitDestroyManagementScript>();
            combatPower += EUDMS.CaluculateCombatPower();
        }
        Debug.Log("CombatPower :"+combatPower);
    }
    public List<GameObject> GetWaveEnemyList(){
        return waveEnemyList;
    }
}

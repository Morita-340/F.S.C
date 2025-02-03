using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "WaveDataBase",menuName = "CreateWaveDataBase")]
public class WaveDataBase : ScriptableObject
{
    [SerializeField]private List<WaveData> WaveDataList = new List<WaveData>();
    /// <summary>
    /// プレイヤーの戦闘力をもとに適切なウェーブを自動で選択する。
    /// </summary>
    /// <param name="playerCombatPower">プレイヤーの戦闘力</param>
    /// <param name="combatPowerRange">許容されるプレイヤーの戦闘力との振れ幅。戦闘力の1/6以下は認めない（戦闘力がデカいのに戦闘力5毎に実装とかは面倒くさすぎるし意味がないから）</param>
    /// <param name="offSet">戦闘力の振れ幅の中央値。0ならPower+-(Range/2)が範囲となり、offSetの値だけプレイヤーの戦闘力からずれる</param>
    /// <returns></returns>
    public WaveData GetAppropriateWaveData(int playerCombatPower,int combatPowerRange,int offSet){
        Debug.Log("WDB" + playerCombatPower);
        //初期値の設定
        if(playerCombatPower <= 0){Debug.LogWarning("playerCombatPowerInput is incorrect");playerCombatPower = 1;}
        if(playerCombatPower/6 > combatPowerRange){Debug.LogWarning("combatPowerRangeInput is incorrect");combatPowerRange = playerCombatPower/6;}
        else if(playerCombatPower < combatPowerRange){combatPowerRange = playerCombatPower;}
        if(offSet < -combatPowerRange){offSet = -combatPowerRange;}
        else if(offSet > combatPowerRange){offSet = combatPowerRange;}
        int minimumValue = playerCombatPower + offSet - combatPowerRange;
        int maximumValue = playerCombatPower + offSet + combatPowerRange;
        List<WaveData> AppropriateWaveList = new List<WaveData>();
        foreach(WaveData waveData in WaveDataList){
            Debug.Log("WDB" + waveData.GetCombatPower());
            if(waveData.GetCombatPower() >= minimumValue && waveData.GetCombatPower() <= maximumValue){
                AppropriateWaveList.Add(waveData);
            }
        }
        if(AppropriateWaveList.Count == 0){Debug.LogWarning("No AppropriateWave! Change Search Wave Way!"+playerCombatPower +" "+combatPowerRange +" "+offSet+" "+minimumValue+" "+maximumValue);}
        int randValue = UnityEngine.Random.Range(0, AppropriateWaveList.Count-1);
        WaveData SelectWave = AppropriateWaveList[randValue];
        return SelectWave;
    }
}

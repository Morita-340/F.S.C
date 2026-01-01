using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

[Serializable]
[CreateAssetMenu(fileName = "BattleStageData", menuName = "CreateBattleStageData")]
public class BattleStageData : ScriptableObject
{
    [SerializeField]
    private List<RefineWaveData> WaveDataList = new List<RefineWaveData>();
    [SerializeField]
    private string StageName;
    [SerializeField]
    private string StageSummary;
    [SerializeField]
    private string StageAdvise;
    [SerializeField]
    private bool StageAlreadyCleared = false;
    [SerializeField]
    private GSetting.SceneName scene;
    public List<RefineWaveData> GetWaveDataList()
    {
        return WaveDataList;
    }
}

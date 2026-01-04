using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ステージ選択画面における選択状況やアイコン表示などを管理する
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI StageName;
    [SerializeField]
    TextMeshProUGUI StageSummary;
    [SerializeField]
    TextMeshProUGUI StageAdvise;
    [SerializeField]
    TakeOffButton takeOffButton;
    //シーン上でプレイヤーが選択した
    private BattleStageData DisplayStageData;
    private BattleStageData SelectedStageData;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StageName.text = DisplayStageData?.GetStageName();
        StageSummary.text = DisplayStageData?.GetStageSummary();
        StageAdvise.text = DisplayStageData?.GetStageAdvise();
    }
    /// <summary>
    /// カーソルが重なったステージの情報を格納する
    /// </summary>
    /// <param name="displayStageData"></param>
    public void SetDisplayStageData(BattleStageData displayBSD){
        if(displayBSD != null){
            DisplayStageData = displayBSD;
        }
    }
    /// <summary>
    /// ステージを選択した際にここに格納する
    /// </summary>
    /// <param name="SD"></param>
    public void SetSelectedStage(BattleStageData BSD){
        if (BSD != null)
        {
            SelectedStageData = BSD;
            takeOffButton.SetTranslateScene(SelectedStageData.GetSceneName());
        }
    }
}

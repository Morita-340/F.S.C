using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
    GameObject SelectedIcon;
    [SerializeField]
    TakeOffButton takeOffButton;
    //シーン上でプレイヤーが選択した
    private StageData DisplayStageData;
    private StageData SelectedStageData;
    // Start is called before the first frame update
    void Start()
    {
        SelectedIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        StageName.text = DisplayStageData?.StageName;
        StageSummary.text = DisplayStageData?.StageSummary;
        StageAdvise.text = DisplayStageData?.StageAdvise;
        if(DisplayStageData == SelectedStageData){SelectedIcon.SetActive(true);}
        else{SelectedIcon.SetActive(false);}
    }
    /// <summary>
    /// カーソルが重なったステージの情報を格納する
    /// </summary>
    /// <param name="displayStageData"></param>
    public void SetDisplayStageData(StageData displayStageData){
        if(displayStageData != null){
            DisplayStageData = displayStageData;
        }
    }
    /// <summary>
    /// ステージを選択した際にここに格納する
    /// </summary>
    /// <param name="SD"></param>
    public void SetSelectedStage(StageData SD){
        if (SD != null){
            SelectedStageData = SD;
            takeOffButton.SetTranslateScene(SelectedStageData.scene);}
    }
}

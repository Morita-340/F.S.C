using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerSelectManager : MonoBehaviour
{
    [SerializeField]
    PlayerMachineInfoText PMIT;
    [SerializeField]
    PlayerMachineDisplayManager PMDM;
    [SerializeField]
    TakeOffPerformanceController TakeOffPC;
    [SerializeField]
    TakeOffButton takeOffButton;
    [SerializeField,ReadOnly]
    GeneralFlagManager GFM;
    [SerializeField]
    BackToStageSelectButton BTSSButton;
    BodyFlag selectedMachineBody;
    BodyFlag pointedMachineBodyNowSelected;
    BodyFlag pointedMachineBody;

    // Start is called before the first frame update
    void Start()
    {
        GFM = FindObjectOfType<GeneralFlagManager>();
        List<BodyFlag> machineBodyList = GFM.GetBodyList();
        PMDM.DisplayMachine(machineBodyList);
        PMDM.SetTakeOFFButton(takeOffButton);
        takeOffButton.SetPerformanceController(TakeOffPC);
    }

    // Update is called once per frame
    void Update()
    {
        //選んだやつ
        selectedMachineBody = PMDM.GetSelectedBody();
        //かざしたやつ
        pointedMachineBodyNowSelected = PMDM.GetPointedBody();
        if (pointedMachineBodyNowSelected != null)
        {
            if (pointedMachineBody != pointedMachineBodyNowSelected)
            {
                //かざし続けている時の負荷軽減
                //機体情報は呼び出す時に随時計算しているため、毎フレーム呼び出しによる負荷上昇を防ぐため
                pointedMachineBody = pointedMachineBodyNowSelected;
                //機体情報の表示
                PMIT.SetMachine(pointedMachineBody);
            }
        }
        if (selectedMachineBody != null)
        {
            //真ん中のカタパルトに機体が置かれる
            TakeOffPC.SetMachine(selectedMachineBody);
            takeOffButton.PreparedForTakeOff();
            //出撃ボタンがヌルっと出現
            GFM.SetSelectedBody(selectedMachineBody);
            //選択アイコンの表示
        }
        if (takeOffButton.GetExecuteFlag())
        {
            //選択アイコンの移動禁止
            //アイコンを押しても反映無し＋情報変更禁止＋押した時のSE変更
            PMDM.NotAcceptMachineSelecting();
            //画面戻り操作禁止＋押した時のSE変更
            BTSSButton.SetPushAccepted(false);
        }
    }
}

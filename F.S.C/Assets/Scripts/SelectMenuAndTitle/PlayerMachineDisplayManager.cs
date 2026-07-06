using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMachineDisplayManager : MonoBehaviour
{
    /// <summary>
    /// 選択した機体を示すマーク
    /// </summary>
    [SerializeField]
    GameObject selectedMark;
    List<GameObject> DisplayedPlayerMachineImageList = new List<GameObject>();
    List<PlayerMachineIcon> MachineIconList = new List<PlayerMachineIcon>();
    BodyFlag pointedBodyFlag;
    BodyFlag selectedBodyFlag;
    TakeOffButton takeOffButton;
    bool OperationNotAcceptFlag = false;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DisplayedPlayerMachineImageList.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void DisplayMachine(List<BodyFlag> selectableMachineBodyList)
    {
        int displayDataNum = DisplayedPlayerMachineImageList.Count;
        if (DisplayedPlayerMachineImageList.Count > selectableMachineBodyList.Count)
        {
            displayDataNum = selectableMachineBodyList.Count;
        }
        Debug.LogAssertion(DisplayedPlayerMachineImageList.Count + "" + selectableMachineBodyList.Count + "" + displayDataNum);
        for (int i = 0; i < DisplayedPlayerMachineImageList.Count; i++)
        {
            if (i < displayDataNum)
            {
                //機体のスプライトデータを取得
                //子オブジェクトに挿入
                DisplayedPlayerMachineImageList[i].GetComponent<Image>().sprite = selectableMachineBodyList[i].BodySprite;
                DisplayedPlayerMachineImageList[i].GetComponent<Image>().preserveAspect = true;
                //MachineIconコンポーネントを追加
                //コンポーネントの初期設定
                MachineIconList.Add(
                    DisplayedPlayerMachineImageList[i].AddComponent<PlayerMachineIcon>().Initialize(selectableMachineBodyList[i], this)
                );
            }
            else
            {
                //挿入の無かった子オブジェクトは非表示にする
                DisplayedPlayerMachineImageList[i].SetActive(false);
            }
        }
    }
    public bool SetSelectedBodyFlag(BodyFlag bodyFlag, Vector2 inputPositon)
    {
        if (takeOffButton.GetExecuteFlag()) { return false; }
        selectedBodyFlag = bodyFlag;
        selectedMark.GetComponent<RectTransform>().anchoredPosition = inputPositon;
        return true;
    }
    public void SetTakeOFFButton(TakeOffButton inputTakeOFFButton)
    {
        takeOffButton = inputTakeOFFButton;
    }
    public void SetPointedBodyFlag(BodyFlag bodyFlag)
    {
        pointedBodyFlag = bodyFlag;
    }
    /// <summary>
    /// 選んだ機体を渡す
    /// </summary>
    /// <returns></returns>
    public BodyFlag GetSelectedBody()
    {
        return selectedBodyFlag;
    }
    /// <summary>
    /// かざした機体を渡す
    /// </summary>
    /// <returns></returns>
    public BodyFlag GetPointedBody()
    {
        return pointedBodyFlag;
    }
    /// <summary>
    /// 押した時のSE変更
    /// </summary>
    public void NotAcceptMachineSelecting()
    {
        foreach (PlayerMachineIcon icon in MachineIconList)
        {
            icon.SetPushAccepted(false);
        }
    }
}

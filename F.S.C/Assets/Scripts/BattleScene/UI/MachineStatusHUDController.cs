using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FSCGeneral;
using System;
/// <summary>
/// 機体の角度やカーソルとの位置を基に表示位置を調整する
/// 機首方向とかも記す
/// </summary>
public class MachineStatusHUDController : MonoBehaviour
{
    /// <summary>
    /// 無装備時のスプライト　読み取り用
    /// </summary>
    [SerializeField] Sprite NoEquipPartIcon;
    /// <summary>
    /// 鹵獲時のパーツを表示する
    /// </summary>
    [SerializeField] SpriteHUD CapturingPartHUD;
    [SerializeField] SpriteHUD NoCapturableCrossIcon;
    /// <summary>
    /// 色で合体待機・パージ待機・回復待機を見分けられるようにする
    /// </summary>
    [SerializeField] SpriteHUD ModeRing;
    private GSetting.RightPointedMode rightPointedMode = GSetting.RightPointedMode.None;
    /// <summary>
    /// パージ対象のパーツを表示する
    /// </summary>
    [SerializeField] SpriteHUD PurgingPartSelectIcon;
    /// <summary>
    /// PurgingPartSelectIconを重ねる対象のスロット
    /// </summary>
    private PartSlot purgePart;
    /// <summary>
    /// 基本的に子オブジェクトで管理
    /// </summary>
    [SerializeField] PartStatusHUD[] partStatusHUD_List = new PartStatusHUD[8];
    [SerializeField] GameObject Player;
    [SerializeField] RefinePlayerUnitDestroyManagementScript RePUDMS;
    PartSlot[] partSlots = new PartSlot[8];
    [SerializeField, Range(1, 100)]
    int radius = 5;
    [SerializeField] GameObject PlayerFrontDirIcon;
    [SerializeField] GameObject PlayerToCursolDirIcon;
    private Coroutine NoCapturableIconActiveCoroutine;
    private bool rightPointedMode_ISNone = true;
    private BoolEdgeTrigger rightPointedModeTrigger;
    public void SetRightPointedMode(GSetting.RightPointedMode rightPointedMode)
    {
        this.rightPointedMode = rightPointedMode;
        rightPointedMode_ISNone = rightPointedMode == GSetting.RightPointedMode.None;
        switch (rightPointedMode)
        {
            case GSetting.RightPointedMode.None: ModeRing.SetColor(Color.white);break;
            case GSetting.RightPointedMode.RepairMode: ModeRing.SetColor(Color.green); break;
            case GSetting.RightPointedMode.CaptureMode: ModeRing.SetColor(Color.blue); break;
            case GSetting.RightPointedMode.PurgeMode: ModeRing.SetColor(Color.yellow); break;
            default: break;
        }
    }
    private void Start()
    {
        //挙動確認のために一旦作成
        SetPart(RePUDMS.GetPartSlotList());
        SetPlayer(RePUDMS.gameObject);
        //ここまで確認用
        CapturingPartHUD.SetLocalPosition(transform.localPosition + new Vector3(0, radius * 2.5f, 0));
        NoCapturableCrossIcon.SetLocalPosition(transform.localPosition + new Vector3(0, radius * 2.5f, 0));
        PurgingPartSelectIcon.SetLocalPosition(transform.localPosition + new Vector3(0, -radius * 2.5f, 0));
        rightPointedModeTrigger = new BoolEdgeTrigger(() => rightPointedMode_ISNone);
    }
    public void SetPlayer(GameObject gameObject)
    {
        Player = gameObject;
    }
    public void SetPart(PartSlot[] PartSlots)
    {
        this.partSlots = PartSlots;
        for (int i = 0; i < partStatusHUD_List.Length; i++)
        {
            if (partSlots[i].GetPart() == null)
            {
                partStatusHUD_List[i].Init(partSlots[i].SlotFlag(),NoEquipPartIcon);
            }
            else
            {
                partStatusHUD_List[i].Init(partSlots[i].SlotFlag(), partSlots[i].GetPart());
            }
        }
    }
    public void SetCaputuringPart(bool caputuring, Sprite sprite)
    {
        if (!caputuring) { CapturingPartHUD.gameObject.SetActive(false); }
        else
        {
            CapturingPartHUD.gameObject.SetActive(true);
            Debug.LogAssertion(sprite.name);
            CapturingPartHUD.SetSprite(sprite);
        }
    }
    public void SetPurgingPartSelectIcon(bool IconActive, PartSlot partSlot)
    {
        if (!IconActive){ PurgingPartSelectIcon.gameObject.SetActive(false);}
        else
        {
            if(partSlot == null){ return; }
            int a = Array.IndexOf(partSlots, partSlot);
            //エラー処理
            if (a < 0) return;
            PurgingPartSelectIcon.gameObject.SetActive(true);
            PurgingPartSelectIcon.SetLocalPosition(partStatusHUD_List[a].gameObject);
        }
    }
    public void NoCapturableIconActive()
    {
        if (NoCapturableIconActiveCoroutine != null) { StopCoroutine(NoCapturableIconActiveCoroutine); }
        NoCapturableIconActiveCoroutine = StartCoroutine(NoCapturableCrossIcon.UI_PingPongStretch(new Vector3(1.1f, 1.1f, 1.1f), 0, Color.white, false, 1f));
    }
    private void Update()
    {
        this.transform.position = Input.mousePosition;
        rightPointedModeTrigger.Update();
        //8方向の武装アイコンの位置制御
        //PlayerFrontDirIcon.transform.localPosition = Player.transform.rotation * new Vector3(0, radius * 0.5f, 0);
        //PlayerFrontDirIcon.transform.localRotation = Player.transform.rotation;
        ModeRing.transform.localRotation = Player.transform.rotation;
        //PlayerToCursolDirIcon.transform.localPosition = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position)) * new Vector3(0, radius * 0.5f, 0);
        PlayerToCursolDirIcon.transform.localPosition = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position)) * new Vector3(0, radius * 0.5f, 0);
        PlayerToCursolDirIcon.transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position));
        for (int i = 0; i < partStatusHUD_List.Length; i++)
        {
            PartStatusHUD partStatusHUD = partStatusHUD_List[i];
            //(プレイヤーの回転角+offset)*半径+カーソル座標
            Quaternion HUDRotation = Quaternion.Euler(0, 0, Player.transform.rotation.eulerAngles.z + 45 * i);
            partStatusHUD.transform.localPosition = HUDRotation * new Vector3(0, radius, 0);
        }
        //if (rightPointedModeTrigger.Falling())
        //{
        //    Debug.LogAssertion("GGGGGA");
        //    Color ringColor = ModeRing.GetColor();
        //    StartCoroutine(ModeRing.UIStretch(new Vector3(1f,1f,1f),0,new Color(ringColor.r,ringColor.g,ringColor.b,1),true,0.3f));
        //}
        //if (rightPointedModeTrigger.Rising())
        //{
        //    Debug.LogAssertion("GGGGGB");
        //    Color ringColor = ModeRing.GetColor();
        //    StartCoroutine(ModeRing.UIStretch(new Vector3(0.8f,0.8f,0.8f),0,new Color(ringColor.r,ringColor.g,ringColor.b,0),true,0.3f));
        //}
    }
}

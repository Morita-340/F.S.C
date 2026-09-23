using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    /// パージ対象のパーツを表示する
    /// </summary>
    [SerializeField] PartStatusHUD PargingPartHUD;
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
    private void Start()
    {
        //挙動確認のために一旦作成
        SetPart(RePUDMS.GetPartSlotList());
        SetPlayer(RePUDMS.gameObject);
        //ここまで確認用
        CapturingPartHUD.transform.localPosition = transform.localPosition + new Vector3(0, radius * 2f, 0);
        NoCapturableCrossIcon.transform.localPosition = transform.localPosition + new Vector3(0, radius * 2f, 0);
        PargingPartHUD.transform.localPosition = transform.localPosition + new Vector3(0, -radius * 2f, 0);
        PargingPartHUD.Init(false, NoEquipPartIcon);
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
    public void NoCapturableIconActive()
    {
        if(NoCapturableIconActiveCoroutine != null){ StopCoroutine(NoCapturableIconActiveCoroutine); }
        NoCapturableIconActiveCoroutine = StartCoroutine(NoCapturableCrossIcon.UI_PingPongStretch(new Vector3(1.1f,1.1f,1.1f),0,Color.white,false,1f));
    }
    private void Update()
    {
        this.transform.position = Input.mousePosition;
        //8方向の武装アイコンの位置制御
        PlayerFrontDirIcon.transform.localPosition = Player.transform.rotation * new Vector3(0, radius * 0.5f, 0);
        PlayerFrontDirIcon.transform.localRotation = Player.transform.rotation;
        PlayerToCursolDirIcon.transform.localPosition = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position)) * new Vector3(0, radius * 0.5f, 0);
        for (int i = 0; i < partStatusHUD_List.Length; i++)
        {
            PartStatusHUD partStatusHUD = partStatusHUD_List[i];
            //(プレイヤーの回転角+offset)*半径+カーソル座標
            Quaternion HUDRotation = Quaternion.Euler(0, 0, Player.transform.rotation.eulerAngles.z + 45 * i);
            partStatusHUD.transform.localPosition = HUDRotation * new Vector3(0, radius, 0);
        }
    }
}

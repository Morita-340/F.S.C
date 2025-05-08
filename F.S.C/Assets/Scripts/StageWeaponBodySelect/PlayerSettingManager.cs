using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// チャージ武器と機体の選択と反映の管理。シーン遷移時はここから情報を渡す
/// 未開放のものであるかどうかはフラグで確認
/// </summary>
public class PlayerSettingManager : MonoBehaviour
{
    [SerializeField,ReadOnly]
    private GeneralFlagManager GFM;
    [SerializeField]
    private GameObject PlayerSpawnPoint;
    [SerializeField]
    private GameObject EnemySpawnPoint;
    /*機体情報設定の変数*/
    [SerializeField]
    private TextMeshProUGUI SelectedBodyUIText;
    [SerializeField]
    SettingMouseInput SMI;
    private BodyFlag SelectedBody;
    private GameObject InstBody;
    private List<BodyFlag> UnlockedBodyList = new List<BodyFlag>();
    private int selectedBodyIndex;
    /*武器情報設定の変数*/
    [SerializeField]
    private TextMeshProUGUI SelectedWeaponUIText;
    private WeaponFlag SelectedWeapon;
    private List<WeaponFlag> UnlockedWeaponList = new List<WeaponFlag>();
    private int selectedWeaponIndex;
    private bool firstUpdate = true;
    void Awake()
    {
        GFM = FindObjectOfType<GeneralFlagManager>();
        //シングルトンからアンロックされているもののみ抜き出す
        foreach(BodyFlag body in GFM.GetBodyList()){
            if(body.isUnlocked){UnlockedBodyList.Add(body);}
        }
        foreach(WeaponFlag weapon in GFM.GetWeaponList()){
            if(weapon.isUnlocked){UnlockedWeaponList.Add(weapon);}
        }
    }
    void Start()
    {
            if(UnlockedBodyList.Count != 0){
                SelectedBody = UnlockedBodyList[0];
                GFM.SetSelectedBody(SelectedBody);
                InstBody = Instantiate(SelectedBody.SettingBody,PlayerSpawnPoint.transform.position,PlayerSpawnPoint.transform.rotation);
                SMI.SetPlayer(InstBody);
            }
    }
    // Update is called once per frame
    void Update()
    {
        if(firstUpdate){
            firstUpdate = false;
            if(UnlockedWeaponList.Count != 0){
                SelectedWeapon = UnlockedWeaponList[0];
                GFM.SetSelectedWeapon(SelectedWeapon);
                InstBody?.GetComponent<PlayerUnitAttackManagementScript>()?.SetChargeWeapon(SelectedWeapon.Weapon.GetComponent<WeaponBase>());
            }
        }
        //UIの表記変更
        SelectedBodyUIText.text = SelectedBody?.Name;
        SelectedWeaponUIText.text = SelectedWeapon?.Name;
    }
    /// <summary>
    /// 以下矢印ボタンを押すことで変わる
    /// </summary>
    /// <param name="upMode">インデックスを上げるかどうか</param>
    public void ChangeBodyIndex(bool upMode){
        if(UnlockedBodyList.Count == 0){selectedBodyIndex = 0;return;}
        if(InstBody!=null){
            SMI.SetPlayer(null);
            InstBody.GetComponent<AbstractUnitDestroyManagementScript>().DeleteAllRangeMesh();
            Destroy(InstBody);}
        if(upMode){
            selectedBodyIndex ++;
            if(selectedBodyIndex >= UnlockedBodyList.Count){selectedBodyIndex = 0;}
        }
        else{
            selectedBodyIndex --;
            if(selectedBodyIndex < 0){selectedBodyIndex = UnlockedBodyList.Count -1;}
        }
        SelectedBody = UnlockedBodyList[selectedBodyIndex];
        InstBody = Instantiate(SelectedBody.SettingBody,PlayerSpawnPoint.transform.position,PlayerSpawnPoint.transform.rotation);
        SMI.SetPlayer(InstBody);
        GFM.SetSelectedBody(SelectedBody);
    }
    /// <summary>
    /// 以下矢印ボタンを押すことで変わる
    /// </summary>
    /// <param name="upMode">インデックスを上げるかどうか</param>
    public void ChangeWeaponIndex(bool upMode){
        if(UnlockedWeaponList.Count == 0){selectedWeaponIndex = 0;return;}
        if(upMode){
            selectedWeaponIndex ++;
            if(selectedWeaponIndex >= UnlockedWeaponList.Count){selectedWeaponIndex = 0;}
        }
        else{
            selectedWeaponIndex --;
            if(selectedWeaponIndex < 0){selectedWeaponIndex = UnlockedWeaponList.Count -1;}
        }
        SelectedWeapon = UnlockedWeaponList[selectedWeaponIndex];
        Debug.Log("LLL" + SelectedWeapon.Weapon);
        InstBody?.GetComponent<PlayerUnitAttackManagementScript>()?.SetChargeWeapon(SelectedWeapon.Weapon.GetComponent<WeaponBase>());
        GFM.SetSelectedWeapon(SelectedWeapon);
    }
    public GameObject GetSelectedBody()
    {
        return SelectedBody.SettingBody;
    }
    public WeaponBase GetSelectedWeapon()
    {
        return SelectedWeapon.Weapon.GetComponent<WeaponBase>();
    }
}

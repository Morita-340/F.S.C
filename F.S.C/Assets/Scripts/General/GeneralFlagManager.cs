using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ゲーム全体でフラグを保持しておくシングルトンクラス。主に武器と機体のクリア時アンロックとアンロック演出のフラグ
/// </summary>
public sealed class GeneralFlagManager : MonoBehaviour
{
    [SerializeField]
    private List<BodyFlag> BodyList = new List<BodyFlag>(){};
    /// <summary>
    /// 設定画面で決めたプレイヤーの素体の機体と武器をそのまま格納。ゲームシーンで読みだす
    /// </summary>
    [SerializeField,ReadOnly]private GameObject SelectedBody;
    [SerializeField,ReadOnly]private GameObject SelectedWeapon;
    [SerializeField]
    private List<WeaponFlag> WeaponList = new List<WeaponFlag>(){};
    public List<BodyFlag> GetBodyList(){
        return BodyList;
    }
    public List<WeaponFlag> GetWeaponList(){
        return WeaponList;
    }
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    public void SetSelectedBody(BodyFlag body){
        SelectedBody = body.Body;
    }
    public void SetSelectedWeapon(WeaponFlag weapon){
        SelectedWeapon = weapon.Weapon;
    }
    public (GameObject,GameObject) GetSelectedPlayer(){
        return (SelectedBody,SelectedWeapon);
    }
}
/// <summary>
/// 機体と武器のフラグクラス
/// </summary>
[Serializable]
public class BodyFlag
{
    public string Name;
    /// <summary>
    /// アンロック状況
    /// </summary>
    public bool isUnlocked = true;
    /// <summary>
    /// アンロック演出をしたかどうか
    /// </summary>
    public bool alreadyShowUnlockedPerform = false;
    public GameObject SettingBody;
    public GameObject Body;
}
[Serializable]
public class WeaponFlag
{
    public string Name;
    /// <summary>
    /// アンロック状況
    /// </summary>
    public bool isUnlocked = true;
    /// <summary>
    /// アンロック演出をしたかどうか
    /// </summary>
    public bool alreadyShowUnlockedPerform = false;
    public GameObject Weapon;
}
//ステージのフラグクラス

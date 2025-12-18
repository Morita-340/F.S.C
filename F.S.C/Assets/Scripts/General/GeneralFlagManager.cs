using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ゲーム全体でフラグを保持しておくシングルトンクラス。主に武器と機体のクリア時アンロックとアンロック演出のフラグ
/// </summary>
public sealed class GeneralFlagManager : MonoBehaviour
{
    /// <summary>
    /// ゲーム全体の音ボリューム。これを弄れるようにすることでプレイヤーが簡単に音量調整できる
    /// </summary>
    [SerializeField,ReadOnly]
    private float generalSoundVolume = 1;
    [SerializeField]
    private List<BodyFlag> BodyList = new List<BodyFlag>(){};
    /// <summary>
    /// 設定画面で決めたプレイヤーの素体の機体と武器をそのまま格納。ゲームシーンで読みだす
    /// </summary>
    [SerializeField,ReadOnly]private GameObject SelectedBody;
    [SerializeField,ReadOnly]private GameObject SelectedWeapon;
    [SerializeField,ReadOnly]private string stageName;
    [SerializeField]
    //private List<WeaponFlag> WeaponList = new List<WeaponFlag>(){};
    public float GetSoundVolume(){
        return generalSoundVolume;
    }
    /// <summary>
    /// 音量設定関数
    /// </summary>
    /// <param name="volume">入力下限を0とした音量ツマミの角度変化量</param>
    /// <param name="volumeRange">入力上限-入力下限</param>
    public void SetSoundVolume(float volume,float inputVolumeRange){
        float volumeRange = inputVolumeRange;
        if(volume < 0){return;}
        if(volumeRange == 0){volume = 1;}
        Debug.LogWarning(volume + " " + volumeRange);
        //AudioSourceコンポーネントでは0-1で調整するのでそれに合うようにする
        generalSoundVolume = volume/volumeRange;
    }
    public List<BodyFlag> GetBodyList(){
        return BodyList;
    }
    //public List<WeaponFlag> GetWeaponList(){
    //    return WeaponList;
    //}
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    public void SetSelectedBody(BodyFlag body){
        SelectedBody = body.Body;
    }
    //public void SetSelectedWeapon(WeaponFlag weapon){
    //    SelectedWeapon = weapon.Weapon;
    //}
    /// <summary>
    /// 設定画面で選んだ兵装と武器を引き渡し生成に用いる
    /// </summary>
    /// <returns></returns>
    public (GameObject,GameObject) GetSelectedPlayer(){
        return (SelectedBody,SelectedWeapon);
    }
    public void SetStageName(string inputName){
        stageName = inputName;
    }
    public string GetStageName(){
        return stageName;
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
    public GameObject Body;
    public Sprite BodySprite;
    public string disctiption;
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

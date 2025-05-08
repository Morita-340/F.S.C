using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 同一シーン内にあるステージ選択と機体設定画面の切り替えを制御する
/// </summary>
public class StageWeaponBodySelectMenuManager : MonoBehaviour
{
    /// <summary>
    /// 機体設定画面に遷移するためにかざすアイコン
    /// </summary>
    [SerializeField]private GameObject WeaponBodySelectIcon;
    private Image WeaponBodySelectIconImage;
    /// <summary>
    /// ステージ選択画面に遷移するためのアイコン
    /// </summary>
    [SerializeField]private GameObject StageSelectIcon;
    private Image StageSelectIconImage;
    /// <summary>
    /// ステージ選択画面のUI群
    /// </summary>
    [SerializeField]private GameObject StageSelectManagerObject;
    /// <summary>
    /// 機体選択や兵装選択のUI群
    /// </summary>
    [SerializeField]private GameObject PlayerSetting;
    /// <summary>
    /// バトルシーン同様にカーソル周りを拡大表示するUI群
    /// </summary>
    [SerializeField]private GameObject ZoomCameraImage;
    [SerializeField]private GameObject TakeOffButton;
    /// <summary>
    /// カメラそのもの
    /// </summary>
    //[SerializeField]private GameObject ZoomCamera;
    [SerializeField,Range(0f,100f)]private float changeTime = 0f; 
    /// <summary>
    /// プレイヤー設定画面であるかどうか。trueなら機体選択画面の操作をうけつけないように非表示その他云々の処理を実行する。ステージ選択画面は縮ませるだけで操作を受け付けないのでヨシ
    /// </summary>
    [SerializeField,ReadOnly]private bool isPlayerSetting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //初期状態ではステージ選択画面が有効
        StageSelectIcon.SetActive(false);
        StageSelectManagerObject.transform.localScale = new Vector3(1,1,1);
        ZoomCameraImage.transform.localScale = new Vector3(2,0,1);
        PlayerSetting.transform.localScale = new Vector3(0,1,1);
        TakeOffButton.transform.localScale = new Vector3(TakeOffButton.transform.localScale .x,0,1);
        WeaponBodySelectIcon.SetActive(true);

        WeaponBodySelectIcon.TryGetComponent(out WeaponBodySelectIconImage);
        StageSelectIcon.TryGetComponent(out StageSelectIconImage);
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerSetting){
            PlayerSettingMenuActive();
        }else{
            StageSelectMenuActive();
        }
    }
    /// <summary>
    /// 呼び出されると一度だけ実行
    /// 機体選択兵装選択の画面に遷移させるための処理とアニメーションが行われる
    /// </summary>
    void PlayerSettingMenuActive(){
        //かざされたアイコン非表示
        WeaponBodySelectIcon.SetActive(false);
        WeaponBodySelectIconImage.color = new Color(WeaponBodySelectIconImage.color.r,WeaponBodySelectIconImage.color.g,WeaponBodySelectIconImage.color.b,0);
        //ステージ選択画面のUI群のRectTransformXを1sで縮小
        //float SSMCheck = Mathf.Lerp(StageSelectManagerObject.transform.localScale.x,0,changeTime);
        //StageSelectManagerObject.transform.localScale = new Vector3(SSMCheck,1,1);
        if(StageSelectManagerObject.transform.localScale.x > 0)StageSelectManagerObject.transform.DOScaleX(0,1);
        //ズームカメラ登場
        //float ZMCheck = Mathf.Lerp(ZoomCameraImage.transform.localScale.y,2,changeTime);
        //ZoomCameraImage.transform.localScale = new Vector3(2,ZMCheck,1);
        if(ZoomCameraImage.transform.localScale.y < 2)ZoomCameraImage.transform.DOScaleY(2,0.3f);
        //機体と兵装のUI群の表示
        //float PSCheck = Mathf.Lerp(PlayerSetting.transform.localScale.x,1,changeTime);
        //PlayerSetting.transform.localScale = new Vector3(PSCheck,1,1);
        if(PlayerSetting.transform.localScale.x < 1)PlayerSetting.transform.DOScaleX(1,0.3f);
        //出撃ボタンの非表示
        //float TOButtonCheck = Mathf.Lerp(TakeOffButton.transform.localScale.y,0,changeTime);
        //TakeOffButton.transform.localScale = new Vector3(TakeOffButton.transform.localScale .x,TOButtonCheck,1);
        if(TakeOffButton.transform.localScale.y > 0)TakeOffButton.transform.DOScaleY(0,0.3f);
        //もう片方のアイコン表示
        StageSelectIcon.SetActive(true);
        if(StageSelectIconImage.color.a < 1)DOTween.ToAlpha(() => StageSelectIconImage.color,color => StageSelectIconImage.color = color,1f,0.6f);
    }
    /// <summary>
    /// 呼び出されると一度だけ実行
    /// ステージ選択画面に遷移させるための処理とアニメーションが行われる
    /// PlayerSettingMenuActive()の判定反転操作以外を逆順で実施
    /// </summary>
    void StageSelectMenuActive(){
        //かざされたアイコン非表示
        StageSelectIcon.SetActive(false);
        StageSelectIconImage.color = new Color(StageSelectIconImage.color.r,StageSelectIconImage.color.g,StageSelectIconImage.color.b,0);
        //機体と兵装のUI群の非表示
        //float PSCheck = Mathf.Lerp(PlayerSetting.transform.localScale.x,0,changeTime);
        //PlayerSetting.transform.localScale = new Vector3(PSCheck,1,1);
        if(PlayerSetting.transform.localScale.x > 0)PlayerSetting.transform.DOScaleX(0,0.3f);
        //ズームカメラ退場
        //float ZMCheck = Mathf.Lerp(ZoomCameraImage.transform.localScale.y,0,changeTime);
        //ZoomCameraImage.transform.localScale = new Vector3(2,ZMCheck,1);
        if(ZoomCameraImage.transform.localScale.y > 0)ZoomCameraImage.transform.DOScaleY(0,0.3f);
        //ステージ選択画面のUI群のRectTransformXを1sで拡大
        //float SSMCheck = Mathf.Lerp(StageSelectManagerObject.transform.localScale.x,1,changeTime);
        //StageSelectManagerObject.transform.localScale = new Vector3(SSMCheck,1,1);
        if(StageSelectManagerObject.transform.localScale.x < 1)StageSelectManagerObject.transform.DOScaleX(1,1);
        //出撃ボタンの表示
        //float TOButtonCheck = Mathf.Lerp(TakeOffButton.transform.localScale.y,1,changeTime);
        //TakeOffButton.transform.localScale = new Vector3(TakeOffButton.transform.localScale .x,TOButtonCheck,1);
        if(TakeOffButton.transform.localScale.y < 1)TakeOffButton.transform.DOScaleY(1,0.3f);
        //もう片方のアイコン表示
        WeaponBodySelectIcon.SetActive(true);
        if(WeaponBodySelectIconImage.color.a < 1)DOTween.ToAlpha(() => WeaponBodySelectIconImage.color,color => WeaponBodySelectIconImage.color = color,1f,0.6f);
    }
    bool LerpEndCheck(float inputValue,float endValue,float time){
        if(inputValue != endValue){
            Mathf.Lerp(inputValue,endValue,time);
            return false;}
        else{
            return true;}
    }
    public void ChangeisPlayerSetting(){
        Debug.Log("AAA");
        isPlayerSetting = !isPlayerSetting;
    }
}

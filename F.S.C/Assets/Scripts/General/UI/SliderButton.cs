using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderButton : GeneralUIIconController
{
    protected bool executeFlag = false;
    protected float nowChargeTime = 0;
    protected float nowExecuteTime = 0;
    protected float initImageWide;
    [SerializeField,Range(0.5f,2f)]float chargeThreshold = 1;
    [SerializeField]protected RectTransform ChargeSlider;
    [SerializeField]protected RectTransform ExecuteSlider;
    protected override void Start()
    {
        base.Start();
        initImageWide = ChargeSlider.sizeDelta.x;
    }
    protected override void Update()
    {
        ButtonCharge();
        ChargeTimeSlider();
        if(executeFlag){
            ExecuteUISlider();
        }
        base.Update();
    }
    protected void ButtonCharge(){
        //ボタンにカーソルを合わせてチャージしないと進めない仕様
        //ポーズ画面でも用いるため、Time.unscaledDeltaTimeを採用してTime.timeScale=0の環境でもアニメーションが動くようにしている
        if(CursolSelected){
            nowChargeTime += Time.unscaledDeltaTime;
        }
        else{
            nowChargeTime -= Time.unscaledDeltaTime;
            if(nowChargeTime < 0)nowChargeTime = 0;
        }
        if(nowChargeTime > chargeThreshold){
            //ボタンの効力発揮
            executeFlag = true;
        }
    }
    protected void ChargeTimeSlider(){
        //チャージ度合いに応じてボタンの見た目を変える
        float chargeRate = nowChargeTime / chargeThreshold;
        if(chargeRate >1 )chargeRate = 1;
        ChargeSlider.sizeDelta = new Vector2(chargeRate*initImageWide,ChargeSlider.sizeDelta.y);
    }
    void ExecuteUISlider(){
        //チャージ完了なら見た目を更に変える
        nowExecuteTime += Time.unscaledDeltaTime;
        float executeRate = nowExecuteTime/0.5f;
        if(executeRate >1)executeRate = 1;
        ExecuteSlider.sizeDelta = new Vector2(executeRate*initImageWide,ExecuteSlider.sizeDelta.y);
    }
}

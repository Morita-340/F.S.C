using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderButton : GeneralUIIconController
{
    protected bool executeFlag = false;
    protected float nowChargeTime = 0;
    protected float nowExecuteTime = 0;
    protected float initImageWide;
    /// <summary>
    /// スライドアニメーション時のスライド速度を変える変数
    /// 一瞬で色が変わるようにしたいので、かなり大きい値を設定
    /// </summary>
    private float sliderTimeFactor = 20;
    [SerializeField, Range(0.5f, 2f)] float chargeThreshold = 1;
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
        //仕様変更
        //カーソルを合わせるとチャージ
        //→ボタンにカーソルを合わせると一瞬でスライドして色が変わる
        //チャージ終了で実行
        //→クリックして実行
        //仕様変更内容記述終わり
        //ボタンにカーソルを合わせてチャージしないと進めない仕様
        //ポーズ画面でも用いるため、Time.unscaledDeltaTimeを採用してTime.timeScale=0の環境でもアニメーションが動くようにしている
        if (CursolSelected)
        {
            nowChargeTime += Time.unscaledDeltaTime *sliderTimeFactor;
        }
        else
        {
            nowChargeTime -= Time.unscaledDeltaTime *sliderTimeFactor;
            if (nowChargeTime < 0) nowChargeTime = 0;
        }
        //左クリックして実行
        if (Input.GetMouseButtonDown(0))
        {
            //ボタンの効力発揮
            executeFlag = true;
        }
    }
    protected void ChargeTimeSlider()
    {
        //チャージ度合いに応じてボタンの見た目を変える
        float chargeRate = nowChargeTime / chargeThreshold;
        if (chargeRate > 1) chargeRate = 1;
        ChargeSlider.sizeDelta = new Vector2(chargeRate * initImageWide, ChargeSlider.sizeDelta.y);
        //ExecuteSliderがChargeSliderより手前にあるため、実行後はChargeSlider側に専用の操作をする必要はない。
        //（実行後はシーン遷移だったり、UIが消えたりするため自動的にCursolSelectedがfalseになり戻るから）
    }
    void ExecuteUISlider(){
        //チャージ完了なら見た目を更に変える
        nowExecuteTime += Time.unscaledDeltaTime;
        float executeRate = nowExecuteTime/0.5f;
        if(executeRate >1)executeRate = 1;
        ExecuteSlider.sizeDelta = new Vector2(executeRate*initImageWide,ExecuteSlider.sizeDelta.y);
    }
}

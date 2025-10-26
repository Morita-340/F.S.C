using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderButton : GeneralUIIconController
{
    /// <summary>
    /// ユーザーがボタン操作で「実行」に相当する操作を行った場合にtrue。これを使って実行後の処理をこちらで実施する
    /// </summary>
    protected bool executeFlag = false;
    /// <summary>
    /// ユーザーが「実行」できるかどうかを判定するフラグ。ボタンとカーソルが重なっている状態（Scale=0であろうとも）で左クリックをすると即時実行してしまうため、実行を想定しているタイミング以外で実行できないようにする
    /// </summary>
    [SerializeField,ReadOnly]protected bool executableFlag = false;
    protected float nowChargeTime = 0;
    protected float nowExecuteTime = 0;
    protected float initImageWide;
    /// <summary>
    /// スライドアニメーション時のスライド速度を変える変数
    /// 一瞬で色が変わるようにしたいので、かなり大きい値を設定
    /// </summary>
    private float sliderTimeFactor = 20;
    [SerializeField, Range(0.5f, 2f)] float chargeThreshold = 1;
    [SerializeField] protected RectTransform ChargeSlider;
    [SerializeField] protected RectTransform ExecuteSlider;
    protected override void Start()
    {
        base.Start();
        initImageWide = ChargeSlider.sizeDelta.x;
    }
    protected override void Update()
    {
        ButtonCharge();
        ChargeTimeSlider();
        if (executeFlag)
        {
            ExecuteUISlider();
        }
        base.Update();
    }
    protected void ButtonCharge()
    {
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
            nowChargeTime += Time.unscaledDeltaTime * sliderTimeFactor;
        }
        else
        {
            nowChargeTime -= Time.unscaledDeltaTime * sliderTimeFactor;
            if (nowChargeTime < 0) nowChargeTime = 0;
        }
        //左クリックして実行
        if (Input.GetMouseButtonDown(0) && CursolSelected)
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
    void ExecuteUISlider()
    {
        //チャージ完了なら見た目を更に変える
        nowExecuteTime += Time.unscaledDeltaTime;
        float executeRate = nowExecuteTime / 0.5f;
        if (executeRate > 1) executeRate = 1;
        ExecuteSlider.sizeDelta = new Vector2(executeRate * initImageWide, ExecuteSlider.sizeDelta.y);
    }
    /// <summary>
    /// ボタンが実行可能かどうかを切り替える関数。ボタンの表示非表示周りは外部で制御しているため、そこで使えるようにする
    /// </summary>
    /// <param name="flag"></param>
    public virtual void Executable(bool flag)
    {
        executableFlag = flag;
    }
}

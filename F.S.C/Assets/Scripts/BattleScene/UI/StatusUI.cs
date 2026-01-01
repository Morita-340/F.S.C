using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// ゲージと数値でステータスを表示するUI
/// </summary>
public class StatusUI : HUD
{
    [SerializeField]
    TextMeshProUGUI valueText;
    [SerializeField]
    RectTransform guage;
    [SerializeField, Range(1, 200)]
    int maxValue = 100;
    [SerializeField, ReadOnly]
    int goalValue =0;
    [SerializeField, ReadOnly]
    float nowValue =0;
    bool changeFlag = true;
    public void SetValue(int inputValue)
    {
        goalValue = inputValue;
        if (goalValue < 0) goalValue = 0;
    }
    protected override void Start()
    {
        //ゲージがあるあたりの場所（目視で判断）
        base.Start();
        rayPos = (Vector2)thisRectTransform.position + new Vector2(thisRectTransform.sizeDelta.x / 2, -thisRectTransform.sizeDelta.y / 2);
    }
    protected override void Update()
    {
        rayPos = (Vector2)thisRectTransform.position + new Vector2(thisRectTransform.sizeDelta.x / 2, -thisRectTransform.sizeDelta.y / 2);
        //値を変更する際に一度だけ実行
        if (goalValue != nowValue) {
            if (changeFlag)
            {
                //changeFlag = false;
                StartCoroutine(ValueChangeAnim());
            }
        }
        //値の変更が終了した
        if (goalValue == nowValue)
        {
            changeFlag = true;
        }
        base.Update();
    }
    IEnumerator ValueChangeAnim()
    {
        float changeTime = 0.5f;
        //徐々に数値を変化させていく
        DOTween.To(() => nowValue, (x) => nowValue = x, goalValue, changeTime);
        UIStretch(valueText, changeTime);
        valueText.text = Mathf.RoundToInt(nowValue).ToString();
        guage.sizeDelta = new Vector2(nowValue *100/*ゲージのWidth*/ /maxValue, guage.sizeDelta.y);
        yield break;
    }
    IEnumerator UIStretch(TextMeshProUGUI text, float time)
    {
        text.transform.DOScale(new Vector3(1, 0.6f, 0.8f), time / 4);
        yield return new WaitForSeconds(time / 2);
        text.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), time / 4);
    }
}

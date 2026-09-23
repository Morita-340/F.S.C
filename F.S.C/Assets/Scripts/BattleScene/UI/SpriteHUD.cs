using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
/// <summary>
/// ただスプライトをImageコンポーネントで表示するだけのHUD
/// </summary>
public class SpriteHUD : HUD
{
    [SerializeField] Image image;
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
    public void SetImage()
    {
        image.sprite = null;
    }
    public void SetColor(Color color)
    {
        image.color = color;
    }
    /// <summary>
    /// スプライトを引数のサイズ・角度に指定時間で変形・回転。必要なタイミングでのみ呼び出す
    /// </summary>
    /// <param name="scale">変形後のサイズ</param>
    /// <param name="eulerAngleZ">回転後のzオイラー角</param>
    /// <param name="color">スプライトの色</param>
    /// <param name="colorStretchGradually">色が指定時間をかけて変わるか否か。falseなら呼び出し時にすぐ変わる</param>
    /// <param name="time">変形・回転に掛かる時間</param>
    /// <returns></returns>
    public IEnumerator UIStretch(Vector3 scale, float eulerAngleZ,Color color,bool colorStretchGradually, float time)
    {
        transform.DOScale(scale, time);
        transform.DORotate(new Vector3(0, 0, eulerAngleZ), time);
        if(!colorStretchGradually){ image.color = color; }
        else{ image.DOColor(color, time); }
        yield return new WaitForSeconds(time);
    }
    public IEnumerator UI_PingPongStretch(Vector3 scale, float eulerAngleZ,Color color,bool colorStretchGradually, float time)
    {
        Vector3 nowScale = transform.localScale;
        float nowRotationEular = transform.localRotation.eulerAngles.z;
        Color nowColor = image.color;
        yield return StartCoroutine(UIStretch(scale, eulerAngleZ,color,colorStretchGradually, time / 2));
        StartCoroutine(UIStretch(nowScale, nowRotationEular,nowColor,colorStretchGradually, time / 2));
    }
}

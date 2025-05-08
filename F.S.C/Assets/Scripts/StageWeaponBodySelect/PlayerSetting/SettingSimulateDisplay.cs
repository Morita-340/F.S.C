using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SettingSimulateDisplay : MonoBehaviour
{
    /// <summary>
    /// 生成するスプライトの種類
    /// </summary>
    [SerializeField]private List<GameObject> VirtualEffectList;
    private List<GameObject> GenerateVirtualEffectList;
    [SerializeField]private Image BackGroundImage;
    /// <summary>
    /// 生成範囲
    /// </summary>
    ///増加量
    [SerializeField]private int increaseGenerateWidth;
    //下限値
    [SerializeField]private int minGenerateWidth;
    ///増加量
    [SerializeField]private int increaseGenerateHeight;
    //下限値
    [SerializeField]private int minGenerateHeight;
    /// <summary>
    /// 一つ前の処理を行ってから次にスプライトが生成されるまでの時間間隔
    /// </summary>
    private int maxGenerateInterval = 6;
    /// <summary>
    /// 生成サイズ
    /// </summary>
    [SerializeField,Range(1,5)]private float maxGenerateSize;
    /// <summary>
    /// 生成されて消えるまでの時間
    /// </summary>
    [SerializeField,Range(3,10)]private int maxGenerateTime;
    /// <summary>
    /// エフェクトを生成する回数
    /// </summary>
    private int generateNum = 1000;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectGenerateProcess());
        StartCoroutine(AlphaChangeProcess());
    }

    // Update is called once per frame
    void Update()
    {
        //任意の時間間隔、任意の生成サイズ、任意の生成時間、任意の種類のスプライト
        
    }
    /// <summary>
    /// エフェクトを生成する全体の処理
    /// </summary>
    /// <returns></returns>
    IEnumerator EffectGenerateProcess(){
        int nowGenerateNum = 0;
        int multipleGenerateValue = Random.Range(2,5);
        while(nowGenerateNum < generateNum){
            nowGenerateNum ++;
            int generateInterval = Random.Range(2,maxGenerateInterval);
            for(int i = 0;i < multipleGenerateValue;i++){
                StartCoroutine(GenerateAnEffect());
            }
            yield return new WaitForSeconds(generateInterval);
        }
        yield break;
    }
    /// <summary>
    /// スプライトを一つ生成する処理
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateAnEffect(){
        //生成座標
        float generateX = Random.Range(minGenerateWidth,minGenerateWidth + increaseGenerateWidth);
        float generateY = Random.Range(minGenerateHeight,minGenerateHeight + increaseGenerateHeight);
        Vector3 generateCoordinate = new Vector3(generateX,generateY,0);
        //生成サイズ
        float generateSize = Random.Range(1,maxGenerateSize);
        //生成時間
        float generateTime = Random.Range(2,maxGenerateTime);
        //生成するスプライト
        int generateSpriteIndex = Random.Range(0,VirtualEffectList.Count);
        GameObject GenerateEffect = Instantiate(VirtualEffectList[generateSpriteIndex],this.transform);
        GenerateEffect.GetComponent<RectTransform>().anchoredPosition = generateCoordinate;
        GenerateEffect.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
        GenerateEffect.transform.DOScale(generateSize,generateTime/2);
        yield return new WaitForSeconds(generateTime/2);
        GenerateEffect.transform.DOScale(0,generateTime/2);
        yield return new WaitForSeconds(generateTime/2);
        Destroy(GenerateEffect);
    }
    IEnumerator AlphaChangeProcess(){
        int num = 1000;
        int nownum = 0;
        while(nownum < num){
            yield return BackGroundColorAlphaChange();
            nownum++;
        }
    }
    IEnumerator BackGroundColorAlphaChange(){
        yield return new WaitForSeconds(10);
        DOTween.ToAlpha(
            () => BackGroundImage.color,
            color => BackGroundImage.color = color,
            0.1f,
            10f
        );
        yield return new WaitForSeconds(10);
        DOTween.ToAlpha(
            () => BackGroundImage.color,
            color => BackGroundImage.color = color,
            0.3f,
            10f
        );
    }
}

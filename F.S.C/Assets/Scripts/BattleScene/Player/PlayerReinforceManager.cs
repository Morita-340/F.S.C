using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 強化パーツによる機体の強化具合を管理する（現状スラスターによる機動力のみ）
/// </summary>
public class PlayerReinforceManager : MonoBehaviour
{
    /// <summary>
    /// 右方向に取り付けられたスラスター個数
    /// </summary>
    [SerializeField,ReadOnly]
    int rightAdditionalThrusterAmount = 0;
    /// <summary>
    /// 左方向に取り付けられたスラスター個数
    /// </summary>
    [SerializeField,ReadOnly]
    int leftAdditionalThrusterAmount = 0;
    /// <summary>
    /// 上方向に取り付けられたスラスター個数
    /// </summary>
    [SerializeField,ReadOnly]
    int upAdditionalThrusterAmount = 0;
    /// <summary>
    /// 下方向に取り付けられたスラスター個数
    /// </summary>
    [SerializeField,ReadOnly]
    int lowAdditionalThrusterAmount = 0;
    /// <summary>
    /// 入手したパーツ個数に対応する加速や旋回の強化倍率を計算する
    /// </summary>
    /// <param name="inputAmount">鹵獲したパーツ個数</param>
    /// <returns>強化倍率</returns>
    private float GetPartsEfficiency(int inputAmount){
        //input = 0以下の場合は1を返す
        //input = 5で最大値(113/3)を返す ←鹵獲しつつ加速できる上限速度が11.4なのでPUMMSの速度計算式における速度倍率0.3に合うようにした
        //加速倍率を主軸において設計。旋回倍率はいくらでも大丈夫だろうと思ってる。
        //input１増える毎に22増やす
        int amount = inputAmount;
        if(amount < 0){amount = 0;}
        if(amount >= 5){amount = 5;}
        return 1 + amount * (22/3);
    }
    /// <summary>
    /// 引数に指定された方向の変数に対してインクリメント又はデクリメントを行う
    /// </summary>
    /// <param name="direction">0上,1下,2左,3右</param>
    /// <param name="addMode">trueならインクリメント,falseならデクリメント</param>
    public void AddAmount(int direction,bool addMode){
        if(addMode){
            switch(direction){
                case 0:upAdditionalThrusterAmount++;    break;
                case 1:lowAdditionalThrusterAmount++;   break;
                case 2:leftAdditionalThrusterAmount++;  break;
                case 3:rightAdditionalThrusterAmount++; break;
                default:break;
            }
        }else{
            //0以下ならデクリメントしない
            switch(direction){
                case 0:if(upAdditionalThrusterAmount>0)upAdditionalThrusterAmount--;        break;
                case 1:if(lowAdditionalThrusterAmount>0)lowAdditionalThrusterAmount--;      break;
                case 2:if(leftAdditionalThrusterAmount>0)leftAdditionalThrusterAmount--;    break;
                case 3:if(rightAdditionalThrusterAmount>0)rightAdditionalThrusterAmount--;  break;
                default:break;
            }
        }
    }
    //以下上下左右の倍率参照用public関数
    public float GetUpperThrusterEfficiency(){
        return GetPartsEfficiency(upAdditionalThrusterAmount);
    }
    public float GetLowerThrusterEfficiency(){
        return GetPartsEfficiency(lowAdditionalThrusterAmount);
    }
    public float GetLeftThrusterEfficiency(){
        return GetPartsEfficiency(leftAdditionalThrusterAmount);
    }
    public float GetRightThrusterEfficiency(){
        return GetPartsEfficiency(rightAdditionalThrusterAmount);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

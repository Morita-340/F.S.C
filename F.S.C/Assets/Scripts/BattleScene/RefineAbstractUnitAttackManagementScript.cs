using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AUAMSのリファイン版。
/// AUAMS→Unitで管理していたが、複数のUnitをまとめて一つの機能として動かせるようにしたくなった。そこでAPCという役割を追加し、ReAUAMS→APC→Unitという構造で動かす
/// ユニットを機能ごとにパーツという形でオブジェクトにまとめて管理するため、それに合うように作り直し
/// </summary>
public class RefineAbstractUnitAttackManagementScript : MonoBehaviour
{
    [SerializeField]
    private RefineAbstractUnitDestroyManagementScript ReAUDMS;
    [SerializeField,Range(0.1f,10f)]
    private float normalAttackInterval = 1;
    [SerializeField,Range(0.1f,10f)]
    protected float chargeAttackInterval =1;
    [SerializeField]protected List<AbstractPartsController> PartsList;
    bool inTimeRangeOFNormalAttack = true;
    float NextNormalAttack = 0f;
    bool inTimeRangeOFChargeAttack = false;
    float nextChargeAttackTime = 0f;
    [SerializeField,ReadOnly]protected Vector3 TargetPosition = Vector3.zero;
    public virtual bool SetTargetPosition(Vector3 inputTargetPosion,RaycastHit2D enemyHit2D)
    {
        bool rockONFlag = false;
        TargetPosition = inputTargetPosion;
        foreach (AbstractPartsController Parts in PartsList)
        {
            bool flag = Parts.SetTargetPosition(TargetPosition,enemyHit2D);
            if (flag) { rockONFlag = true; }
        }
        return rockONFlag;
    }
    protected virtual void Awake(){
    }
    protected virtual void Start(){
        PartsList = ReAUDMS.GetPartsList();
    }
    /// <summary>
    /// 呼び出されたら一定時間おきに各ユニットの通常攻撃を"毎フレームではなく一度だけ"実行する
    /// </summary>
    /// <param name="time">ロックオンし始めてから経過した時間。ロックオンが外れると0に戻る</param>
    public virtual void NormalAttack(float time)
    {
        Debug.Log("AUAMS normal time" + time + " " + NextNormalAttack);
        if (time < 0) { NextNormalAttack = 0; }
        if (time >= NextNormalAttack)
        {
            Debug.Log("AUAMS normal timeA" + time + " " + NextNormalAttack);
            foreach (AbstractPartsController Parts in PartsList)
            {
                Debug.Log("QQQQ");
                Parts.NormalAttack(TargetPosition);
            }
            NextNormalAttack += normalAttackInterval;
        }
    }
}

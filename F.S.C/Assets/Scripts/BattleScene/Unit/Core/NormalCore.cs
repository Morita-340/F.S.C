using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCore : CoreBase
{
    [SerializeField]
    GameObject RocketBomb;
    [SerializeField]
    GameObject Bomb;
    [SerializeField]
    GameObject FirePositionObj;
    FanRange FR;
    protected override void Start(){
        base.Start();
        FR = this.gameObject.GetComponent<FanRange>();
    }
    public override void NormalAttack(Vector3 TargetPosition)
    {
        if(FR.InRange(TargetPosition)){
            Vector3 FirePosition = FirePositionObj.transform.position;
            Quaternion FireRotation = FirePositionObj.transform.rotation;
            base.NormalAttack(TargetPosition);
            //ロケット弾を前方に射出
            Instantiate(RocketBomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = transform.up + FR.GetTargetDelta();
        }
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(FR.InRange(TargetPosition)){
            Vector3 FirePosition = FirePositionObj.transform.position;
            Quaternion FireRotation = FirePositionObj.transform.rotation;
            base.ChargeAttack(TargetPosition);
            Instantiate(Bomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = transform.up + FR.GetTargetDelta();
        }
    }
}

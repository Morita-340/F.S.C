using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBeamWeaponUnit : WeaponUnitBase
{
    [SerializeField]
    GameObject RocketBomb;
    [SerializeField]
    GameObject Bomb;
    FanRange FR;
    protected override void Start(){
        base.Start();
        FR = this.gameObject.GetComponent<FanRange>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public override void NormalAttack(Vector3 TargetPosition)
    {
        if(FR.InRange(TargetPosition)){
            Vector3 FirePosition = this.transform.position;
            Quaternion FireRotation = this.transform.rotation;
            base.NormalAttack(TargetPosition);
            //ロケット弾を前方に射出
            Instantiate(RocketBomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = transform.up + FR.GetTargetDelta();
        }
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(FR.InRange(TargetPosition)){
            Vector3 FirePosition = this.transform.position;
            Quaternion FireRotation = this.transform.rotation;
            base.ChargeAttack(TargetPosition);
            Instantiate(Bomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = transform.up + FR.GetTargetDelta();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
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
            base.NormalAttack(TargetPosition);
            Vector3 FirePosition = this.transform.position;
            Quaternion FireRotation = this.transform.rotation;
            //ロケット弾を前方に射出
            string tagName = GSetting.ObjTagName.PlayerWeapon1.ToString();
            switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
                case GSetting.ObjTagName.PlayerUnit:{
                    tagName = GSetting.ObjTagName.PlayerWeapon1.ToString();
                    break;}
                case GSetting.ObjTagName.EnemyUnit:{
                    tagName = GSetting.ObjTagName.EnemyWeapon1.ToString();
                    break;
                }
                default:break;

            }
            RocketBomb.tag = tagName;
            Instantiate(RocketBomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = /*transform.up +*/ FR.GetTargetDelta();
        }
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(FR.InRange(TargetPosition)){
            Vector3 FirePosition = this.transform.position;
            Quaternion FireRotation = this.transform.rotation;
            base.ChargeAttack(TargetPosition);
            string tagName = GSetting.ObjTagName.PlayerWeapon1.ToString();
            switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
                case GSetting.ObjTagName.PlayerUnit:{
                    tagName = GSetting.ObjTagName.PlayerWeapon2.ToString();
                    break;}
                case GSetting.ObjTagName.EnemyUnit:{
                    tagName = GSetting.ObjTagName.EnemyWeapon2.ToString();
                    break;
                }
                default:break;

            }
            Bomb.tag= tagName;
            Instantiate(Bomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = /*transform.up +*/ FR.GetTargetDelta();
        }
    }
}

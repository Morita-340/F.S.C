using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using System;

public class AttackUnit : UnitBase
{
    [SerializeField]protected WeaponBase NormalWeapon;
    [SerializeField]protected WeaponBase ChargeWeapon;
    [SerializeField]protected GameObject ReactorLevelUI;
    protected GameObject InstReactorLevelUI;
    protected SpriteRenderer ReactorLevelUISSpRenderer;
    protected FanRange FR;
    // Start is called before the first frame update
    protected override void Start()
    {
        SetWeaponPower();
        FR = this.gameObject.GetComponent<FanRange>();
        if(tag == GSetting.ObjTagName.PlayerUnit.ToString() || tag == GSetting.ObjTagName.EnemyUnit.ToString()){
            InstReactorLevelUI = Instantiate(ReactorLevelUI,this.gameObject.transform);
            InstReactorLevelUI.transform.position = this.transform.position + new Vector3(0,0,-2);
            ReactorLevelUISSpRenderer = InstReactorLevelUI.GetComponent<SpriteRenderer>();
        }
        base.Start();
    }
    public void SetWeaponPower(){
        normalAttackPower = NormalWeapon.GetAttackPower();
        chargeAttackPower = ChargeWeapon.GetAttackPower();
    }
    public override int GetUnitStatus(){
        SetWeaponPower();
        return base.GetUnitStatus();
    }
    // Update is called once per frame
    protected override void Update()
    {
        if(tag == GSetting.ObjTagName.PlayerUnit.ToString() || tag == GSetting.ObjTagName.EnemyUnit.ToString()){
            ReactorLevelUISSpRenderer.color 
            = new Color(attackEfficiency/(int)GSetting.UniqueMagicNumber.AttackEfficiencyONReactorLevel/100,1,1,0.5f);
        }
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
            NormalWeapon.tag = tagName;
            WeaponBase InstWeapon = Instantiate(NormalWeapon,FirePosition,FireRotation);
            InstWeapon.SetAttackEfficiency(attackEfficiency);
            InstWeapon.GetComponent<Rigidbody2D>().velocity = /*transform.up +*/ FR.GetTargetDelta()*10;
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
            ChargeWeapon.tag= tagName;
            WeaponBase InstWeapon = Instantiate(ChargeWeapon,FirePosition,FireRotation);
            InstWeapon.SetAttackEfficiency(attackEfficiency);
            InstWeapon.GetComponent<Rigidbody2D>().velocity = /*transform.up +*/ FR.GetTargetDelta()*10;
        }
    }
}

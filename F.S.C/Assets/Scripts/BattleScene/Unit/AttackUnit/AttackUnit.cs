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
        if(tag == GSetting.ObjTagName.PlayerUnit.ToString() || tag == GSetting.ObjTagName.EnemyUnit.ToString()||tag == GSetting.ObjTagName.DestroyedUnit.ToString()){
            InstReactorLevelUI = Instantiate(ReactorLevelUI,this.gameObject.transform);
            InstReactorLevelUI.transform.position = this.transform.position + new Vector3(0,0,-2);
            ReactorLevelUISSpRenderer = InstReactorLevelUI.GetComponent<SpriteRenderer>();
        }
        base.Start();
    }
    public void SetWeaponPower(){
        normalAttackPower = NormalWeapon?.GetAttackPower()?? 0;
        chargeAttackPower = ChargeWeapon?.GetAttackPower()?? 0;
    }
    public void SetChargeWeapon(WeaponBase Weapon){
        ChargeWeapon = Weapon;
        SetWeaponPower();
    }
    public override int GetUnitStatus(){
        SetWeaponPower();
        return base.GetUnitStatus()/*HPのこと*/ + (normalAttackPower + chargeAttackPower)*(attackEfficiency + EXP);
    }
    /// <summary>
    /// ウェーブの戦闘力を数値化するにあたってリアクターの効果が及ぶ範囲の強化具合を計算するために使用
    /// </summary>
    /// <returns></returns>
    public virtual int GetUnitAttackPower(){
        return normalAttackPower + chargeAttackPower;
    }
    // Update is called once per frame
    protected override void Update()
    {
        if((tag == GSetting.ObjTagName.PlayerUnit.ToString() || tag == GSetting.ObjTagName.EnemyUnit.ToString())&&ReactorLevelUISSpRenderer.enabled){
            ReactorLevelUISSpRenderer.color 
            = new Color(attackEfficiency/*+EXP*//(int)GSetting.UniqueMagicNumber.AttackEfficiencyONReactorLevel/100,1,1,0.5f);
        }
        base.Update();
    }
    public override void NormalAttack(Vector3 TargetPosition)
    {
        if(NormalWeapon == null){return;}
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
            InstWeapon.SetAttackEfficiency(attackEfficiency + EXP);
            WeaponLook(InstWeapon,TargetPosition);
            Vector2 weaponVelocity = (Vector2)FR.GetTargetDelta()*10 *NormalWeapon.GetVelocityEfficiency()+ this.transform.root.GetComponent<Rigidbody2D>().velocity;
            switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
                case GSetting.ObjTagName.PlayerUnit:{
                    break;}
                case GSetting.ObjTagName.EnemyUnit:{
                    weaponVelocity = 0.7f * weaponVelocity;
                    break;
                }
                default:break;
            }
            InstWeapon.GetComponent<Rigidbody2D>().velocity = weaponVelocity;
        }
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(ChargeWeapon == null){return;}
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
            InstWeapon.SetAttackEfficiency(attackEfficiency + EXP);
            WeaponLook(InstWeapon,TargetPosition);
            Vector2 weaponVelocity = (Vector2)FR.GetTargetDelta()*10 *ChargeWeapon.GetVelocityEfficiency()+ this.transform.root.GetComponent<Rigidbody2D>().velocity;
            switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
                case GSetting.ObjTagName.PlayerUnit:{
                    break;}
                case GSetting.ObjTagName.EnemyUnit:{
                    weaponVelocity = 0.7f * weaponVelocity;
                    break;
                }
                default:break;
            }
            InstWeapon.GetComponent<Rigidbody2D>().velocity = weaponVelocity;
        }
    }
    private void WeaponLook(WeaponBase weapon, Vector3 TargetPosition){
        Transform myTransform = weapon.transform;
        Vector2 direction = TargetPosition - myTransform.position;
        myTransform.up = direction;
    }
}

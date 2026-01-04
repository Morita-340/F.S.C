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
    protected FanRange FR;
    protected LineRenderer lineRenderer;
    [SerializeField,ReadOnly]protected Vector3 targetPosition =new Vector3(0,0,0);
    protected Vector3[] positions = new Vector3[]{};
    [SerializeField]
    protected bool isHoming = false;
    protected RaycastHit2D enemyHit2D;
    public bool SetTargetPosition(Vector3 inputTargetPosition, RaycastHit2D inputEnemyHit2D)
    {
        targetPosition = inputTargetPosition;
        enemyHit2D = inputEnemyHit2D;
        return FR.InRockONRange(targetPosition)&&enemyHit2D&&isHoming == true;
    }
    /// <summary>
    /// AUAMS→AttackUnitの攻撃処理がすべての子オブジェクトに対して同時に行われているので、AttackUnit側で攻撃タイミングをずらすことで、弾幕を張ることができ、弾を当てやすくなる
    /// </summary>
    protected float attackTimeOffset;
    protected override void Awake(){
        FR = GetComponent<FanRange>();
        lineRenderer = GetComponent<LineRenderer>();
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        SetWeaponPower();
        if(isPrime){attackTimeOffset = 0;}
        else{attackTimeOffset = UnityEngine.Random.Range(0.1f,0.9f);}
        targetPosition = transform.position;
        //照準までの軌跡の描画初期設定
        positions = new Vector3[]{transform.position,targetPosition};
        if(tag == GSetting.ObjTagName.PlayerUnit.ToString()){lineRenderer.startColor = new Color(0,0,0,0);lineRenderer.endColor = Color.green;}
        if(tag == GSetting.ObjTagName.EnemyUnit.ToString()){lineRenderer.startColor = Color.red;lineRenderer.endColor = new Color(0,0,0,0);}
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
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
    public int GetHP()
    {
        return base.GetUnitStatus();
    }
    public int GetAttackPower()
    {
        //GSetting.RefineDebugAssertinLog(transform, normalAttackPower+"+"+chargeAttackPower+"+"+attackEfficiency+"+"+EXP);
        return (normalAttackPower + chargeAttackPower) * (attackEfficiency + EXP);
    }
    /// <summary>
    /// ウェーブの戦闘力を数値化するにあたってリアクターの効果が及ぶ範囲の強化具合を計算するために使用
    /// </summary>
    /// <returns></returns>
    public virtual int GetUnitAttackPower()
    {
        return normalAttackPower + chargeAttackPower;
    }
    // Update is called once per frame
    protected override void Update()
    {
        if(FR.InRange(targetPosition)){
            lineRenderer.enabled = true;
            positions = new Vector3[]{transform.position,targetPosition};
            lineRenderer.SetPositions(positions);
        }else{lineRenderer.enabled = false;}
        if (FR.InRockONRange(targetPosition)&&enemyHit2D&&isHoming)
        {
            if(tag == GSetting.ObjTagName.PlayerUnit.ToString()){lineRenderer.startColor = new Color(0,0,0,0);lineRenderer.endColor = Color.red;}
            if(tag == GSetting.ObjTagName.EnemyUnit.ToString()){lineRenderer.startColor = Color.blue;lineRenderer.endColor = new Color(0,0,0,0);}
        }
        else
        {
            if(tag == GSetting.ObjTagName.PlayerUnit.ToString()){lineRenderer.startColor = new Color(0,0,0,0);lineRenderer.endColor = Color.green;}
            if(tag == GSetting.ObjTagName.EnemyUnit.ToString()){lineRenderer.startColor = Color.red;lineRenderer.endColor = new Color(0,0,0,0);}
        }
        base.Update();
    }
    public override IEnumerator NormalAttack(Vector3 TargetPosition)
    {
        if(NormalWeapon == null){yield break;}
        Debug.Log("LLLL");
        if(FR.InRange(TargetPosition)){
            yield return base.NormalAttack(TargetPosition);
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
            Vector2 thisVelocity = this.transform.root.GetComponent<Rigidbody2D>().velocity;
            //InstWeapon.SetVelocity(thisVelocity);
            InstWeapon.SetAttackEfficiency(attackEfficiency + EXP);
            WeaponLook(InstWeapon,TargetPosition);
            //if (FR.InRockONRange(targetPosition) && isHoming)
            Vector2 weaponVelocity = (Vector2)FR.GetTargetDelta()*10 *NormalWeapon.GetVelocityEfficiency()+ thisVelocity*0.3f;
            if (FR.InRockONRange(targetPosition))
            {
                Debug.LogWarning("WWW");
                //InstWeapon.SetHoming(enemyHit2D.collider?.gameObject);
                weaponVelocity *= 5;
            }
            yield return new WaitForSeconds(attackTimeOffset);
            switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
                case GSetting.ObjTagName.PlayerUnit:{
                    break;}
                case GSetting.ObjTagName.EnemyUnit:{
                    weaponVelocity = 0.7f * weaponVelocity;
                    break;
                }
                default:break;
            }
            InstWeapon.SetVelocity(weaponVelocity);
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
            InstWeapon.SetVelocity(weaponVelocity);
        }
    }
    private void WeaponLook(WeaponBase weapon, Vector3 TargetPosition){
        Transform myTransform = weapon.transform;
        Vector2 direction = TargetPosition - myTransform.position;
        myTransform.up = direction;
    }
    public virtual void DestroyFRMesh(){
        FR?.DestroyRMM();
    }
}

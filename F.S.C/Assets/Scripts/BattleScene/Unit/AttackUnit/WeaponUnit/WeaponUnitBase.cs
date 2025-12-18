using System.Collections;
using System.Collections.Generic;
using System;
using FSCGeneral;
using UnityEngine;

public class WeaponUnitBase : AttackUnit
{
    [SerializeField]protected GameObject DividableIcon;
    private GameObject icon;
    public override GameObject UnitSetting(string tagName, int layerNum)
    {
        return base.UnitSetting(tagName, layerNum);
    }
    public override int GetUnitStatus()
    {
        if(isPrime){return base.GetUnitStatus();}
        else{return InstHitPoint;}
    }
    public override int GetUnitAttackPower()
    {
        if(isPrime){return base.GetUnitAttackPower();}
        else{return 0;}
    }
    protected override void Awake()
    {
        base.Awake();
        FR.InitialSetting();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        //分離の度にiconが再生成されてしまうため、その前に以前生成したiconを消去しておく
        //毎回Instantiateするのは処理が重くなりそうだが、WeaponUnit全てに対して予めヒエラルキー上でiconを設定しSetActiveを管理するのは面倒くさすぎるのでこちらを採用した
        if(this.transform.childCount > 0){
            foreach(Transform child in this.transform){
                Destroy(child.gameObject);
            }
        }
        icon = Instantiate(DividableIcon,this.gameObject.transform,false);
        if(GetThisUnitData().dividable){
            icon.SetActive(true);
        }else{icon.SetActive(false);}
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(this.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            if(GetThisUnitData().dividable){
                icon.SetActive(true);
            }else{icon.SetActive(false);}
        }
        else{ReactorLevelUISSpRenderer.enabled=true;}
    }
    public override IEnumerator NormalAttack(Vector3 TargetPosition)
    {
        if(isPrime)yield return base.NormalAttack(TargetPosition);
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(isPrime)base.ChargeAttack(TargetPosition);
    }
    public bool IsWCUBenabled()
    {
        //if(WCUB != null){
        //    if(WCUB.tag == tag)ControllUnitPosition = Quaternion.Euler(-transform.rotation.eulerAngles)*(WCUB.transform.position - transform.position);//WCUB.transform.localPosition - transform.localPosition;
        //    ControllUnitPosition.z = 15;
        //}
        //WCUB = GetThisControllUnit(ControllUnitPosition);
        //if(WCUB != null)return WCUB.tag == this.tag;
        //else return false;
        return true;
    }
    protected override void DestroyUnit()
    {
        FR?.DestroyRMM();
        base.DestroyUnit();
    }
    public override void DestroyFRMesh(){
        FR?.DestroyRMM();
    }
}

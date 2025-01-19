using System.Collections;
using System.Collections.Generic;
using System;
using FSCGeneral;
using UnityEngine;

public class WeaponUnitBase : AttackUnit
{
    [SerializeField]protected GameObject DividableIcon;
    [SerializeField]protected WeaponControllUnitBase WCUB;
    [SerializeField]Vector3 ControllUnitPosition;
    private GameObject icon;
    public override GameObject UnitSetting(string tagName, int layerNum)
    {
        return base.UnitSetting(tagName, layerNum);
    }
    public override int GetUnitStatus()
    {
        if(isPrime || WCUB != null){return base.GetUnitStatus();}
        else{return InstHitPoint;}
    }
    public override int GetUnitAttackPower()
    {
        if(isPrime || WCUB != null){return base.GetUnitAttackPower();}
        else{return 0;}
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        //
        if(WCUB != null){
            if(WCUB.tag == tag)ControllUnitPosition = Quaternion.Euler(-transform.rotation.eulerAngles)*(WCUB.transform.position - transform.position);//WCUB.transform.localPosition - transform.localPosition;
            Debug.LogWarning("aaa");
            ControllUnitPosition.z = 15;
        }
        WCUB = GetThisControllUnit(ControllUnitPosition);
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
        Debug.Log(WCUB== null);
        Debug.Log("AAAAAA"+ReactorLevelUISSpRenderer);
        if(WCUB == null){ReactorLevelUISSpRenderer.enabled = false;}
        else{ReactorLevelUISSpRenderer.enabled=true;}

        Vector3 RayPosition = Quaternion.Euler(transform.rotation.eulerAngles)* ControllUnitPosition + transform.position;
        if(ControllUnitPosition != Vector3.zero)Debug.DrawLine(RayPosition, transform.position);
        //WCUB = GetThisControllUnit(ControllUnitPosition);
    }
    protected WeaponControllUnitBase GetThisControllUnit(Vector3 ControllUnitPosition){
        Vector3 RayPosition = Quaternion.Euler(transform.rotation.eulerAngles)* ControllUnitPosition + transform.position;//transform.localRotation.eulerAngles+ transform.parent.rotation.eulerAngles)* ControllUnitPosition) + transform.position;
        foreach(RaycastHit2D raycastHit2D in Physics2D.RaycastAll(RayPosition,Vector3.back)){
        Debug.LogWarning(name +RayPosition +" "+ raycastHit2D.collider.tag + " "+tag);
            if(raycastHit2D.collider.tag == this.tag){
                if(raycastHit2D.collider.gameObject.GetComponent<WeaponControllUnitBase>()){
                    return raycastHit2D.collider.GetComponent<WeaponControllUnitBase>();
                }
            }
        }
        return null;
    }
    public override void NormalAttack(Vector3 TargetPosition)
    {
        if(isPrime||WCUB != null)base.NormalAttack(TargetPosition);
    }
    public override void ChargeAttack(Vector3 TargetPosition)
    {
        if(isPrime||WCUB != null)base.ChargeAttack(TargetPosition);
    }
    public bool IsWCUBenabled(){
        if(WCUB != null){
            if(WCUB.tag == tag)ControllUnitPosition = Quaternion.Euler(-transform.rotation.eulerAngles)*(WCUB.transform.position - transform.position);//WCUB.transform.localPosition - transform.localPosition;
            ControllUnitPosition.z = 15;
        }
        WCUB = GetThisControllUnit(ControllUnitPosition);
        if(WCUB != null)return WCUB.tag == this.tag;
        else return false;
    }
}

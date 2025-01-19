using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FSCGeneral;

public class CoreBase : AttackUnit
{
    /// <summary>
    /// 爆風だけのREMを実装したいので、REMをそのまま使用する
    /// </summary>
    [SerializeField]
    protected CoreEffectManager CEM;
    protected GameObject ReactorEffectPool;
    protected override void Start(){
        ReactorEffectPool = GameObject.Find("ReactorEffectPool");
        CEM.transform.SetParent(ReactorEffectPool.transform);
        CEM.ExplosionActive(false);
        CEM.SetThisCore(this);
        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //if(Input.GetKeyDown(KeyCode.Space) && thisGameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
        //    NormalAttack();
        //}
    }
    /// <summary>
    /// コアが破壊された場合はプレイヤーユニットが完全に消去される
    /// </summary>
    protected override void DestroyUnit()
    {
        CEM.ExplosionActive(true);
        DestroyCore();
    }
    protected void DestroyCore(){
        if(spriteRenderer.isVisible){AUDMS.ThisIsVisible(false);}
        AUDMS.DeleteChildrenDataFromFM();
        //AUDMSの撃墜判定を書き換える処理を行う
        thisGameObject.transform.root.GetComponent<AbstractUnitDestroyManagementScript>().IsDead();
        AUDMS.DestroyProcess(GetThisUnitData());
        if(this.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            this.gameObject.transform.root.gameObject.SetActive(false);
            }
        else{Destroy(this.gameObject.transform.root.gameObject);}
    }
}

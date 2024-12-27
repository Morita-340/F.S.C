using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class CoreBase : UnitBase
{

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
        DestroyCore();
    }
    protected void DestroyCore(){
        if(spriteRenderer.isVisible){AUDMS.ThisIsVisible(false);}
        AUDMS.DeleteChildrenDataFromFM();
        //AUDMSの撃墜判定を書き換える処理を行う
        thisGameObject.transform.root.GetComponent<AbstractUnitDestroyManagementScript>().IsDead();
        if(this.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            this.gameObject.transform.root.gameObject.SetActive(false);
            }
        else{Destroy(this.gameObject.transform.root.gameObject);}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class CoreBase : UnitBase
{

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.Space) && thisGameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            AttackAction();
        }
    }
    /// <summary>
    /// コアが破壊された場合はプレイヤーユニットが完全に消去される
    /// </summary>
    //protected override void DestroyUnit()
    //{
    //    DestroyCore();
    //}
    //protected void DestroyCore(){
    //    Destroy(this.gameObject.transform.root.gameObject);
    //}
}

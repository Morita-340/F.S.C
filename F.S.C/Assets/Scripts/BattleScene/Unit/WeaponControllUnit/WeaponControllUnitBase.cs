using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 兵装制御ユニット。これが制御する武器ユニットにのみ攻撃命令が伝達される。すなわち親子関係をとらずとも兵装ごとに破損状況に応じて攻撃の有無を変えられる。これと制御対象のユニットを結ぶ経路は一つに定めておくこと
/// </summary>
public class WeaponControllUnitBase : UnitBase
{
    //[SerializeField,ReadOnly]
    //protected List<WeaponUnitBase> ControllWeaponUnitList = new List<WeaponUnitBase>();
    public override GameObject UnitSetting(string tagName, int layerNum)
    {
        return base.UnitSetting(tagName, layerNum);
    }
    protected override void Start(){
        base.Start();
    }
    protected override void Update()
    {
        //ControllWeaponUnitList.RemoveAll(weaponUnit => weaponUnit == null);
        base.Update();
    }
    //public bool WeaponUnitExist(){
    //    ControllWeaponUnitList.RemoveAll(weaponUnit => weaponUnit == null);
    //    Debug.Log("WCUB CWUE"+ControllWeaponUnitList.Count);
    //    for(int i = 0; i < ControllWeaponUnitList.Count;i++){
    //        //Debug.Log("WCUB CWUE" +ControllWeaponUnitList[i].name + ControllWeaponUnitList[i]?.tag);
    //        Debug.Log("WCUB CWUE" + ControllWeaponUnitList[i]?.name + ControllWeaponUnitList[i]?.tag + tag);
    //        if(ControllWeaponUnitList[i] != null){
    //            if(ControllWeaponUnitList[i]?.tag == tag){
    //                return true;}
    //        }
    //    }
    //    //制御対象のユニットが存在しない（破壊されているのでnull）または分離している（タグが異なる）
    //    return false;
    //}
}
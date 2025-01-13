using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトルシーンでのシングルトン。シーン中のUnitDataは全てUnitListとしてここにある
/// </summary>
namespace FSCGeneral{
    public sealed class FieldManager
    {
        private static FieldManager FM = new FieldManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private FieldManager(){}
        /// <summary>
        /// 内部アクセス用メソッド
        /// </summary>
        /// <returns>シングルトンの参照</returns>
        public static FieldManager GetInstance(){
            return FM;
        }
        public List<UnitData> UnitList = new List<UnitData>();
        public UnitData SearchUnit(UnitBase serachUnit){
            foreach(UnitData unitData in FM.UnitList){
                if(unitData.ReturnThisUnit()==serachUnit){
                    Debug.Log("LLJ"+unitData.ReturnThisUnit() + serachUnit);
                    return unitData;
                }
            }return new UnitData(null,0);
        }
        /// <summary>
        /// ListのRemoveを使えば同じ処理が出来るのでいらない
        /// </summary>
        /// <param name="willDeleteData"></param>
        public void DeleteData(UnitData willDeleteData){
            foreach(UnitData unitData in FM.UnitList){
                if(unitData == willDeleteData){
                    FM.UnitList.Remove(unitData);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class UnitData
{
    public UnitData(UnitBase thisUnit,int ShapeType){
        //thisUnit = ThisUnit;
        //upperUnit = UpperUnit;
        //downerUnit = DownerUnit;
        //rightUnit = RightUnit;
        //leftUnit = LeftUnit;
        //ShapeType = ShapeTypeNum;
        ThisUnit = thisUnit;
        ShapeTypeNum = ShapeType;
    }
    private UnitData UpperUnit;
    private UnitData DownerUnit;
    private UnitData RightUnit;
    private UnitData LeftUnit;
    //データベースのキー扱い
    private UnitBase ThisUnit;
    private int MaxHitPoint;
    private float AttackPower;
    public bool dividable = false;
    public int ShapeTypeNum;
    //探索時に探索済みであるかを判別してもらう
    public bool AlreadySearch = false;
    public UnitBase ReturnThisUnit(){
        return ThisUnit;
    }
    public List<UnitData> ReturnFourWayLink(){
        List<UnitData> ReturnFourWayLink = new List<UnitData>(){UpperUnit,DownerUnit,RightUnit,LeftUnit};
        return ReturnFourWayLink;
    }
    public void ReRegistFourWayLink(UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        if(upperUnit != null)UpperUnit = upperUnit.GetThisUnitData();
        if(downerUnit != null)DownerUnit = downerUnit.GetThisUnitData();
        if(rightUnit != null)RightUnit = rightUnit.GetThisUnitData();
        if(leftUnit != null)LeftUnit = leftUnit.GetThisUnitData();
    }
    /// <summary>
    /// 自身と紐づけられたUnitのHitPointが0になった場合に呼び出され周囲から自分へのLinkを削除する処理
    /// </summary>
    public void DeleteFourWayLink(){
        if(UpperUnit != null)UpperUnit.DeleteDestroyUnitLink(this);
        if(DownerUnit != null)DownerUnit.DeleteDestroyUnitLink(this);
        if(RightUnit != null)RightUnit.DeleteDestroyUnitLink(this);
        if(LeftUnit != null)LeftUnit.DeleteDestroyUnitLink(this);
    }
    /// <summary>
    /// DeleteFourWayLinkを拡張。データのdividableがfalse即ち分離対象に選ばれていなければdividableなunitとのリンクを相互で削除する
    /// これによりdividableなユニットのリンクをコアから独立させることが出来る
    /// </summary>
    public void DeleteFourWayLinkThatIsNotDividable(){
        if(UpperUnit?.dividable == false){
            UpperUnit.DeleteDestroyUnitLink(this);
            UpperUnit = null;}
        if(DownerUnit?.dividable == false){
            DownerUnit.DeleteDestroyUnitLink(this);
            DownerUnit = null;}
        if(RightUnit?.dividable == false){
            RightUnit.DeleteDestroyUnitLink(this);
            RightUnit = null;}
        if(LeftUnit?.dividable == false){
            LeftUnit.DeleteDestroyUnitLink(this);
            LeftUnit = null;}
    }
    /// <summary>
    /// DeleteFourWayLink内でのみ呼び出され周囲のUnitの自分へのLinkを削除する
    /// </summary>
    /// <param name="DestroyUnitData"></param>
    protected void DeleteDestroyUnitLink(UnitData DestroyUnitData){
        Debug.Log("PUDMS DDUL");
        if(UpperUnit == DestroyUnitData)UpperUnit = null;
        else if(DownerUnit == DestroyUnitData)DownerUnit = null;
        else if(RightUnit == DestroyUnitData)RightUnit = null;
        else if(LeftUnit == DestroyUnitData)LeftUnit = null;
        else {
            if(ThisUnit == null){
                FieldManager FM = FieldManager.GetInstance();
                FM.DeleteData(this);
            }
            else if(ThisUnit.gameObject == null){
                FieldManager FM = FieldManager.GetInstance();
                FM.DeleteData(this);
            }
            else{Debug.LogWarning("DeleteDestroyUnitLink Was Called but DestroyUnitData isNot in" + ThisUnit.gameObject.name);}
        }
        return;
    }
}

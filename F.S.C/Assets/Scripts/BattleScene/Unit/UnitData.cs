using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class UnitData
{
    public UnitData(UnitBase thisUnit,UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit,int ShapeType){
        //thisUnit = ThisUnit;
        //upperUnit = UpperUnit;
        //downerUnit = DownerUnit;
        //rightUnit = RightUnit;
        //leftUnit = LeftUnit;
        //ShapeType = ShapeTypeNum;
        ThisUnit = thisUnit;
        UpperUnit = upperUnit;
        DownerUnit = downerUnit;
        RightUnit = rightUnit;
        LeftUnit = leftUnit;
        ShapeTypeNum = ShapeType;
    }
    private UnitBase UpperUnit;
    private UnitBase DownerUnit;
    private UnitBase RightUnit;
    private UnitBase LeftUnit;
    private UnitBase ThisUnit;
    private int HitPoint;
    private float AttackPower;
    public int ShapeTypeNum;
    public UnitBase ReturnThisUnit(){
        return ThisUnit;
    }
    public List<UnitBase> ReturnFourWayLink(){
        List<UnitBase> ReturnFourWayLink = new List<UnitBase>(){UpperUnit,DownerUnit,RightUnit,LeftUnit};
        return ReturnFourWayLink;
    }
}

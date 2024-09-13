using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField]
    protected GSetting.ShapeType shapeType;
    private string UnitTagName = "PlayerUnit";
    private GameObject thisGameObject;
    private FieldManager FM = FieldManager.GetInstance();
    UnitBase thisUnit;
    // Start is called before the first frame update
    void Awake()
    {
        thisGameObject = this.gameObject;
        thisUnit = this;
    }
    protected void Start(){
        GetAdjacentObjLink();
    }
    protected virtual void GetAdjacentObjLink()
    {
        Debug.Log("LLA");
        //shapetypeで取得するか否かを使い分ける
        //UnitBase rightUnit;
        //Ray RightRay = Camera.main.ScreenPointToRay(this.gameObject.transform.position + new Vector3(1,0,0));
        //if(!Physics.Raycast(RightRay,out hit)){return;}
        //GameObject RightObj = hit.collider.gameObject;
        //if(RightObj.tag != UnitTagName){return;}
        //rightUnit = RightObj.GetComponent<UnitBase>();
        //if(rightUnit == null){Debug.LogAssertion("RightUnit is null;");return;}
        ////
        //UnitBase leftUnit;
        //Ray LeftRay = Camera.main.ScreenPointToRay(this.gameObject.transform.position + new Vector3(-1,0,0));
        //if(!Physics.Raycast(LeftRay,out hit)){return;}
        //GameObject LeftObj = hit.collider.gameObject;
        //if(LeftObj.tag != UnitTagName){return;}
        //leftUnit = LeftObj.GetComponent<UnitBase>();
        //if(leftUnit == null){Debug.LogAssertion("LeftUnit is null;");return;}
        //
        UnitBase upperUnit = null;
        UnitBase downerUnit = null;
        UnitBase rightUnit = null;
        UnitBase leftUnit = null;
        if(shapeType == GSetting.ShapeType.Square){
            upperUnit = GetUpLink();
            downerUnit = GetDownLink();
            rightUnit = GetRightLink();
            leftUnit = GetLeftLink();
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            downerUnit = GetDownLink();
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            downerUnit = GetDownLink();
            rightUnit = GetRightLink();
        }
        RegistData(thisUnit,upperUnit,downerUnit,rightUnit,leftUnit);
    }
    void RegistData(UnitBase thisUnit,UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        Debug.Log("LLA"+shapeType+thisUnit.name+downerUnit);
        UnitData unitData= new UnitData(thisUnit,upperUnit,downerUnit,rightUnit,leftUnit,(int)shapeType);
        FM.UnitList.Add(unitData);
    }
    private UnitBase GetUpLink(){
        UnitBase upperUnit;
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(0,1,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject UpperObj = hit2D.collider.gameObject;
        if(UpperObj.tag != UnitTagName){return null;}
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if(upperUnit == null)Debug.LogAssertion("UpperUnit is null;");
        return upperUnit;
    }
    private UnitBase GetDownLink(){
        UnitBase downerUnit;
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(0,-1,5),new Vector3(0,0,1));
        //
        if(!hit2D){Debug.Log("III");return null;}
        GameObject DownerObj = hit2D.collider.gameObject;
        if(DownerObj.tag != UnitTagName){Debug.Log("IIU");return null;}
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if(downerUnit == null)Debug.LogAssertion("DownerUnit is null;");
        return downerUnit;
    }
    private UnitBase GetRightLink(){
        UnitBase rightUnit;
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(1,0,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject RightObj = hit2D.collider.gameObject;
        if(RightObj.tag != UnitTagName){return null;}
        rightUnit = RightObj.GetComponent<UnitBase>();
        if(rightUnit == null)Debug.LogAssertion("RightUnit is null;");
        return rightUnit;
    }
    private UnitBase GetLeftLink(){
        UnitBase leftUnit;
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(-1,0,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject LeftObj = hit2D.collider.gameObject;
        if(LeftObj.tag != UnitTagName){return null;}
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if(leftUnit == null)Debug.LogAssertion("LeftUnit is null;");
        return leftUnit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void AttackAction(){}
    protected virtual void DestroyUnit(){
        //分離エフェクトを実装する
        Destroy(thisGameObject);
    }
}

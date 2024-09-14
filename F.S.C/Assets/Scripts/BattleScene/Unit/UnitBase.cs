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
    UnitBase upperUnit = null;
    UnitBase downerUnit = null;
    UnitBase rightUnit = null;
    UnitBase leftUnit = null;
    // Start is called before the first frame update
    void Awake()
    {
        thisGameObject = this.gameObject;
        thisUnit = this;
    }
    protected void Start(){
        GetAdjacentObjLink();
        RegistData(thisUnit,upperUnit,downerUnit,rightUnit,leftUnit);
    }
    protected virtual void GetAdjacentObjLink()
    {
        Debug.Log("LLA");
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
        }else Debug.LogAssertion("ShapeType is null!");
    }
    void RegistData(UnitBase thisUnit,UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        Debug.Log("LLA"+shapeType+thisUnit.name+downerUnit);
        UnitData unitData= new UnitData(thisUnit,upperUnit,downerUnit,rightUnit,leftUnit,(int)shapeType);
        FM.UnitList.Add(unitData);
    }
    private UnitBase GetUpLink(){
        UnitBase upperUnit;
        GameObject UpperObj = GetUpperGameObject(UnitTagName);
        if(UpperObj == null){return null;}
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if(upperUnit == null)Debug.LogAssertion("UpperUnit is null;");
        return upperUnit;
    }
    private UnitBase GetDownLink(){
        UnitBase downerUnit;
        GameObject DownerObj = GetDownerGameObject(UnitTagName);
        if(DownerObj == null){return null;}
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if(downerUnit == null)Debug.LogAssertion("DownerUnit is null;");
        return downerUnit;
    }
    private UnitBase GetRightLink(){
        UnitBase rightUnit;
        GameObject RightObj = GetRightGameObject(UnitTagName);
        if(RightObj == null){return null;}
        rightUnit = RightObj.GetComponent<UnitBase>();
        if(rightUnit == null)Debug.LogAssertion("RightUnit is null;");
        return rightUnit;
    }
    private UnitBase GetLeftLink(){
        UnitBase leftUnit;
        GameObject LeftObj = GetLeftGameObject(UnitTagName);
        if(LeftObj == null){return null;}
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if(leftUnit == null)Debug.LogAssertion("LeftUnit is null;");
        return leftUnit;
    }
    private GameObject GetUpperGameObject(string ObjTag){
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(0,1,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject UpperObj = hit2D.collider.gameObject;
        if(UpperObj.tag != ObjTag){return null;}
        else return UpperObj;
    }
    private GameObject GetDownerGameObject(string ObjTag){
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(0,-1,5),new Vector3(0,0,1));
        if(!hit2D){Debug.Log("III");return null;}
        GameObject DownerObj = hit2D.collider.gameObject;
        if(DownerObj.tag != ObjTag){Debug.Log("IIU");return null;}
        else return DownerObj;
    }
    private GameObject GetRightGameObject(string ObjTag){
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(1,0,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject RightObj = hit2D.collider.gameObject;
        if(RightObj.tag != ObjTag){return null;}
        else return RightObj;
    }
    private GameObject GetLeftGameObject(string ObjTag){
        RaycastHit2D hit2D = Physics2D.Raycast(this.gameObject.transform.position + new Vector3(-1,0,5),new Vector3(0,0,1));
        if(!hit2D){return null;}
        GameObject LeftObj = hit2D.collider.gameObject;
        if(LeftObj.tag != ObjTag){return null;}
        else return LeftObj;
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
    public bool IsNotIsolatedUnit(){
        if(shapeType == GSetting.ShapeType.Square){
            if(GetUpperGameObject(UnitTagName) != null)return true;
            else if(GetDownerGameObject(UnitTagName) != null)return true;
            else if(GetRightGameObject(UnitTagName) != null)return true;
            else if(GetLeftGameObject(UnitTagName) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            if(GetDownerGameObject(UnitTagName) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            if(GetDownerGameObject(UnitTagName) != null)return true;
            else if(GetRightGameObject(UnitTagName) != null)return true;
            else return false;
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null "); return false;
    }
    /// <summary>
    /// Unitの周りにPlayerUnitがあるかどうかを判別
    /// </summary>
    /// <returns>隣接するPlayerUnitのUnitBaseのList</returns>
    public List<UnitBase> ReturnAdjacentUnitList(){
        GetAdjacentObjLink();
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        if(upperUnit != null)AdjacentUnitList.Add(upperUnit);
        if(downerUnit != null)AdjacentUnitList.Add(downerUnit);
        if(rightUnit != null)AdjacentUnitList.Add(rightUnit);
        if(leftUnit != null)AdjacentUnitList.Add(leftUnit);
        return AdjacentUnitList;
    }
    public void ReRegistData(){
        GetAdjacentObjLink();
        UnitData unitData = FM.SearchUnit(this);
        unitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
        Debug.Log("JJJ"+thisUnit+upperUnit+downerUnit+rightUnit+leftUnit);
    }
}

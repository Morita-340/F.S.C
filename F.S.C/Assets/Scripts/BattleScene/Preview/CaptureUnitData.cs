using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

class CaptureUnitData
{
    Vector3 target;
    Vector3 ParentRotationEular;
    GameObject previewParentObject;
    List <Vector3> previewUnitPositionList;
    List <Quaternion> previewUnitRotationList;
    List<GSetting.ShapeType> shapeTypeList;
    public CaptureUnitData(Vector3 target,Vector3 ParentRotationEular,GameObject previewParentObject,List <Vector3> previewUnitPositionList,List <Quaternion> previewUnitRotationList,List<GSetting.ShapeType> shapeTypeList){
        this.target = target;
        this.ParentRotationEular = ParentRotationEular;
        this.previewParentObject = previewParentObject;
        this.previewUnitPositionList = previewUnitPositionList;
        this.previewUnitRotationList = previewUnitRotationList;
        this.shapeTypeList = shapeTypeList;
    }
    public Vector3 getTarget(){
        return target;
    }
    public Vector3 GetParentRotationEular(){
        return ParentRotationEular;
    }
    public GameObject GetPreviewParentObject(){
        return previewParentObject;
    }
    public List <Vector3> GetPreviewUnitPositionList(){
        return previewUnitPositionList;
    }
    public List <Quaternion> GetPreviewUnitRotationList(){
        return previewUnitRotationList;
    }
    public List <GSetting.ShapeType> GetShapeTypeList(){
        return shapeTypeList;
    }

}
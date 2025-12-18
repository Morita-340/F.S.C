using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class FanRange : MonoBehaviour
{
    [SerializeField,Range(1f,100f)]
    float rangeRadius = 50f;
    [SerializeField,Range(1f,180f)]
    float rangeRadian = 90f;
    [SerializeField, Range(0f, 1f)]
    float RockOnRangeRate = 0;
    Vector3 TargetDelta = new Vector3(0f, 0f,0f);
    [SerializeField]GameObject RMM;
    [SerializeField,ReadOnly]RangeMeshManager GeneRMM;
    GameObject RangeMeshPool;
    public bool InRange(Vector3 TargetPosition){
        var TargetInfo = CaluculateTargetDistanceANDAngle(TargetPosition);
        //距離を計測
        float unit2TargetDistance = TargetInfo.Item1;
        //角度を計測
        float unit2TargetAngle = TargetInfo.Item2;
        if(unit2TargetDistance <= rangeRadius && unit2TargetAngle <= rangeRadian){
            return true;
        }else{return false;}
    }
    public bool InRockONRange(Vector3 TargetPosition){
        var TargetInfo = CaluculateTargetDistanceANDAngle(TargetPosition);
        //距離を計測
        float unit2TargetDistance = TargetInfo.Item1;
        //角度を計測
        float unit2TargetAngle = TargetInfo.Item2;
        if(unit2TargetDistance <= rangeRadius * RockOnRangeRate && unit2TargetAngle <= rangeRadian){
            return true;
        }else{return false;}
    }
    private (float, float) CaluculateTargetDistanceANDAngle(Vector3 TargetPosition)
    {
        TargetDelta = TargetPosition - this.transform.position;
        TargetDelta.z = 0;
        //距離を計測
        float unit2TargetDistance = TargetDelta.magnitude;
        //角度を計測
        float unit2TargetAngle = Vector3.Angle(this.transform.up, TargetDelta);
        return (unit2TargetDistance,unit2TargetAngle);
    }
    /// <summary>
    /// InRange()がtrueの時のみこれも同時に返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTargetDelta()
    {
        return TargetDelta / TargetDelta.magnitude;
    }
    void Update(){
    }
    public void InitialSetting(){
        RangeMeshPool = GameObject.Find(GSetting.UniqueObjectName.RangeMeshPool.ToString());
        if(GeneRMM == null){
            GeneRMM = Instantiate(RMM).GetComponent<RangeMeshManager>();
        }
        GeneRMM.transform.SetParent(RangeMeshPool.transform);
        GeneRMM.SetRadiusAndRadianAndFanRange(rangeRadius,rangeRadian,this);
    }
    public void DestroyRMM(){
        if(GeneRMM != null){
            DestroyImmediate(GeneRMM.gameObject);
        }
    }
    public float GetRangeRadius(){
        return rangeRadius;
    }
}
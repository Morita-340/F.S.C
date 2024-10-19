using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FanRange : MonoBehaviour
{
    [SerializeField,Range(1f,100f)]
    float rangeRadius = 50f;
    [SerializeField,Range(1f,180f)]
    float rangeRadian = 90f;
    Vector3 TargetDelta = new Vector3(0f, 0f,0f);
    //private bool inRange = false;
    public bool InRange(Vector3 TargetPosition){
        TargetDelta = TargetPosition - this.transform.position;
        //距離を計測
        float unit2TargetDistance = TargetDelta.magnitude;
        //角度を計測
        float unit2TargetAngle = Vector3.Angle(this.transform.up,TargetDelta);
        if(unit2TargetDistance <= rangeRadius && unit2TargetAngle <= rangeRadian){
            return true;
        }else{return false;}
    }
    /// <summary>
    /// InRange()がtrueの時のみこれも同時に返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTargetDelta(){
        return TargetDelta;
    }
}
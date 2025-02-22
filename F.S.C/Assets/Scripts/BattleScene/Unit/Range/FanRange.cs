using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRange : MonoBehaviour
{
    [SerializeField,Range(1f,100f)]
    float rangeRadius = 50f;
    [SerializeField,Range(1f,180f)]
    float rangeRadian = 90f;
    Vector3 TargetDelta = new Vector3(0f, 0f,0f);
    [SerializeField]
    LineRenderer line;
    //private bool inRange = false;
    public bool InRange(Vector3 TargetPosition){
        TargetDelta = TargetPosition - this.transform.position;
        TargetDelta.z = 0;
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
        return TargetDelta/TargetDelta.magnitude;
    }
    void Start(){
        // コンポーネント追加
        line = gameObject.GetComponent<LineRenderer>();
        // 幅を0.1f
        line.startWidth = 0.1f;
        // ラインの色を黒
        line.material.color = Color.black;
    }
    void Update(){
        //DrawFan();
    }
    void DrawFan(){
        // オブジェクト前方
        var direction = transform.forward;
        // 扇型になるような位置情報を格納するためのリスト
        var positions = new List<Vector3>();
        // 始点として自身の位置を追加
        positions.Add(transform.position);
        // 0を中央としてangle角分ループさせる
        for(int i = -(int)rangeRadian; i < rangeRadian; i++)
        {
            // 意味は分からないです、分かる人に聞いて下さい
            var rot = Quaternion.AngleAxis(i, direction);
            // 上記を使って良い感じに位置を設定します
            var position = rot * transform.up * rangeRadius + transform.position;
            // リストに追加
            positions.Add(position);
        }
        // 終点として自身の位置を追加
        positions.Add(transform.position);
        // ラインレンダラーコンポーネントに上記コードによる扇形に必要な数を設定
        line.positionCount = positions.Count;
        // ラインを引く扇形の位置情報を与える
        line.SetPositions(positions.ToArray());
    }
    public float GetRangeRadius(){
        return rangeRadius;
    }
}
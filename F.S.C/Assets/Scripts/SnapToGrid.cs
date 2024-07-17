using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public Transform gridOrigin; // グリッドの起点（中心）オブジェクト
    [SerializeField]
    Transform Origin;
    public float cellSize; // グリッドのセルのサイズ（正方形）
    public bool snapRotation = true; // 回転もスナップするかどうか

    void Update()
    {
        SnapObjectToGrid();
    }

    void SnapObjectToGrid()
    {
        if (gridOrigin == null)
        {
            Debug.LogWarning("Grid origin is not assigned.");
            return;
        }

        // グリッドのローカル座標に変換
        Vector3 localPosition = gridOrigin.InverseTransformPoint(Origin.transform.position);

        // 位置のスナップ
        float snappedX = Mathf.Round(localPosition.x / cellSize) * cellSize;
        float snappedY = Mathf.Round(localPosition.y / cellSize) * cellSize;
        float snappedZ = Mathf.Round(localPosition.z / cellSize) * cellSize;

        Vector3 snappedLocalPosition = new Vector3(snappedX, snappedY, snappedZ);
        
        // グリッドのワールド座標に変換
        transform.position = gridOrigin.TransformPoint(snappedLocalPosition);

        // 回転のスナップ（オプション）
        if (snapRotation)
        {
            Vector3 eulerRotation = Origin.transform.rotation.eulerAngles;
            //float snappedRotationX = Mathf.Round(eulerRotation.x / 90) * 90;
            //float snappedRotationY = Mathf.Round(eulerRotation.y / 90) * 90;
            //float snappedRotationZ = Mathf.Round(eulerRotation.z / 90) * 90;
            float snappedRotationZ = gridOrigin.transform.eulerAngles.z % 90 + Mathf.Floor(eulerRotation.z / 90) * 90;
            //(eulerRotation.z+90) / 90
            Debug.Log("a"+snappedRotationZ+"b"+gridOrigin.transform.eulerAngles.z+"c"+(Mathf.Floor(eulerRotation.z / 90) * 90));
            //this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
            this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
        }
    }
}

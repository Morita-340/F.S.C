using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnitSimulaterの回転と移動を制御しているだけ
/// このクラスもといコンポーネントはオブジェクトにアタッチして2つのTransformを設定することで、グリッドの起点（中心）オブジェクト（プレイヤー）からマス目の等間隔に補正対象のオブジェクトのプレビュー用オブジェクトを動的に設置することが可能
/// </summary>
public class SnapToGrid : MonoBehaviour
{
    public Transform gridOrigin; // グリッドの起点（中心）オブジェクト[SerializeField]に書き換えても問題ない
    [SerializeField]
    Transform Origin;//プレビューで補正する対象のオブジェクト
    public float cellSize; // グリッドのセルのサイズ（正方形）
    public bool snapRotation = true; // 回転もスナップするかどうか

    void Update()
    {
        //プレビュー表示範囲に入ったら、入ったタイミングに一度だけ対応する子オブジェクトを作成して設定を済ませる。範囲から出るか設置するかキャンセルをすることで子オブジェクトを消去する
        SnapObjectToGrid();
    }
    /// <summary>
    /// このオブジェクトを動的にグリッド補正、角度補正する
    /// </summary>
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
        //float snappedZ = Mathf.Round(localPosition.z / cellSize) * cellSize;

        Vector3 snappedLocalPosition = new Vector3(snappedX, snappedY, 7);
        
        // グリッドのワールド座標に変換
        transform.position = gridOrigin.TransformPoint(snappedLocalPosition);

        // 回転のスナップ（オプション）
        if (snapRotation)
        {
            Vector3 eulerRotation = Origin.transform.rotation.eulerAngles;
            //float snappedRotationX = Mathf.Round(eulerRotation.x / 90) * 90;
            //float snappedRotationY = Mathf.Round(eulerRotation.y / 90) * 90;
            //float snappedRotationZ = Mathf.Round(eulerRotation.z / 90) * 90;
            float snappedRotationZ = gridOrigin.transform.eulerAngles.z % 90 + Mathf.Round(eulerRotation.z/90) * 90;
            //(eulerRotation.z+90) / 90
            //Debug.Log("a"+snappedRotationZ+"b"+gridOrigin.transform.eulerAngles.z+"c"+(Mathf.Floor(eulerRotation.z / 90) * 90));
            //this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
            this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
        }
    }
    /// <summary>
    /// クリックしてプレビューを初めて表示する際にもグリッド補正が必要なのでこのメソッドを呼び出す
    /// </summary>
    /// <param name="position">グリッド補正後の座標</param>
    /// <returns></returns>
    public Vector3 SnapPositon(Vector3 position){
        Vector3 localPosition = position;

        // 位置のスナップ
        float snappedX = Mathf.Round(localPosition.x / cellSize) * cellSize;
        float snappedY = Mathf.Round(localPosition.y / cellSize) * cellSize;
        //float snappedZ = Mathf.Round(localPosition.z / cellSize) * cellSize;

        Vector3 snappedLocalPosition = new Vector3(snappedX, snappedY, 7);
        
        // グリッドのワールド座標に変換
        return gridOrigin.TransformPoint(snappedLocalPosition);
    }
    /// <summary>
    /// クリックしてプレビューを初めて表示する際にもグリッド補正が必要なのでこのメソッドを呼び出す
    /// The first time you click to display the preview, you also need to correct the grid, so call this method
    /// </summary>
    /// <param name="rotation">グリッド補正後の回転角</param>
    /// <returns></returns>
    public Quaternion SnapRotation(Quaternion rotation){
        Vector3 eulerRotation = rotation.eulerAngles;
        //float snappedRotationX = Mathf.Round(eulerRotation.x / 90) * 90;
        //float snappedRotationY = Mathf.Round(eulerRotation.y / 90) * 90;
        //float snappedRotationZ = Mathf.Round(eulerRotation.z / 90) * 90;
        float snappedRotationZ = gridOrigin.transform.eulerAngles.z % 90 + Mathf.Floor(eulerRotation.z/90) * 90;
        //(eulerRotation.z+90) / 90
        //Debug.Log("a"+snappedRotationZ+"b"+gridOrigin.transform.eulerAngles.z+"c"+(Mathf.Floor(eulerRotation.z / 90) * 90));
        //this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
            
        return Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
    }
    public Vector3 SnapRotation(Vector3 rotation){
        Vector3 eulerRotation = rotation;
        //float snappedRotationX = Mathf.Round(eulerRotation.x / 90) * 90;
        //float snappedRotationY = Mathf.Round(eulerRotation.y / 90) * 90;
        //float snappedRotationZ = Mathf.Round(eulerRotation.z / 90) * 90;
        float snappedRotationZ = gridOrigin.transform.eulerAngles.z % 90 + Mathf.Floor(eulerRotation.z/90) * 90;
        //(eulerRotation.z+90) / 90
        //Debug.Log("a"+snappedRotationZ+"b"+gridOrigin.transform.eulerAngles.z+"c"+(Mathf.Floor(eulerRotation.z / 90) * 90));
        //this.gameObject.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, snappedRotationZ);
            
        return new Vector3(eulerRotation.x, eulerRotation.y, snappedRotationZ);
    }
}

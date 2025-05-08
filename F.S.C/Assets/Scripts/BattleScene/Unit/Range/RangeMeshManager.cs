using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeMeshManager : MonoBehaviour
{
    private float rangeRadius = 50f;
    private float rangeRadian = 90f;
    private LineRenderer line;
    private MeshFilter meshFilter;
    [SerializeField,ReadOnly]private FanRange thisFR;
    List<FanRange> thisFRNullCheckList = new List<FanRange>();
    // Start is called before the first frame update
    void Start()
    {
        // コンポーネント追加
        line = GetComponent<LineRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        // 幅を0.1f
        line.startWidth = 0.1f;
        // ラインの色を黒
        line.material.color = Color.black;
    }

    // Update is called once per frame
    void Update(){
        if(thisFR == null){gameObject.SetActive(false);}
        else{
            gameObject.SetActive(true);
            this.transform.position = thisFR.transform.position;
            this.transform.rotation = thisFR.transform.rotation;
            DrawFan();
        }
    }
    void DrawFan(){
        // オブジェクト前方
        Vector3 direction = transform.forward;
        // 扇型になるような位置情報を格納するためのリスト
        List<Vector3> positions = new List<Vector3>();

        List<Vector3> meshPositions = new List<Vector3>();
        meshPositions.Add(Vector3.zero);
        // 0を中央としてangle角分ループさせる
        for(int i = -(int)rangeRadian; i < rangeRadian; i++)
        {
            //頂点の角度変更
            Quaternion rot = Quaternion.AngleAxis(i, direction);
            // 上記を使って位置を設定
            Vector3 position = rot * transform.up * rangeRadius + transform.position;
            // リストに追加
            positions.Add(position);

            Quaternion meshRot = Quaternion.Euler(0,0,i);
            Vector3 meshPosition = meshRot * Vector3.up * rangeRadius;// + thisFR.transform.position;
            meshPositions.Add(meshPosition);
        }
        // 終点として自身の位置を追加
        positions.Add(thisFR.transform.position);
        // ラインレンダラーコンポーネントに上記コードによる扇形に必要な数を設定
        //line.positionCount = positions.Count;
        // ラインを引く扇形の位置情報を与える
        //line.SetPositions(positions.ToArray());

        meshPositions.Add(thisFR.transform.position);
        List<int> triangles = new List<int>();
        int meshPositionsCount = meshPositions.Count;
        if(meshPositionsCount % 3 == 1){meshPositionsCount -= 1;}
        else if(meshPositionsCount % 3 == 2){meshPositionsCount -= 2;}
        // 三角形の定義
        for (int i = 1; i < meshPositionsCount - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = meshPositions.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.RecalculateNormals();
    }
    public void SetRadiusAndRadianAndFanRange(float inputRadius,float inputRadian,FanRange fanRange){
        rangeRadius = inputRadius;
        rangeRadian = inputRadian;
        thisFR = fanRange;
        //thisFRNullCheckList.Add(thisFR);
    }
}

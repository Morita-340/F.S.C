using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// マウスカーソルによる入力はここで管理する
/// </summary>
public class MouseCursorControllScript : MonoBehaviour
{
    [SerializeField]
    GameObject UnitSimulater;
    SnapToGrid snapToGrid;
    PlayerUnitSimulateScript playerUnitSimulateScript;
    [SerializeField]
    GameObject PreviewUnit;
    GameObject ParentObject = null;
    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        snapToGrid = UnitSimulater.GetComponent<SnapToGrid>();
        playerUnitSimulateScript = UnitSimulater.GetComponent<PlayerUnitSimulateScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(Input.mousePosition);
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        Debug.Log("bbbZ" + target);
        CursorDragClick(wheelInput);
    }
    /// <summary>
    /// Unitを右クリックしたら離すまでドラッグし続ける
    /// </summary>
    void CursorDragClick(float wheelInput){
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        //Debug.Log(hit2D.collider.gameObject.name);
        List<Vector3> positionList = new List<Vector3>();
        List<Quaternion> rotationList = new List<Quaternion>();
        if(Input.GetMouseButtonDown(0)){
            ParentObject = hit2D.collider.gameObject.transform.root.gameObject;
            if(hit2D.collider.tag == "DestroyedUnit"/*撃破後のUnitであることを識別できる何かをフラグに持ってくる*/){
                //子オブジェクトのtransformを取得する
                for(int i = 0;i < ParentObject.transform.childCount;i++){
                        //Transform child = ParentObject.transform.GetChild(i);
                    Vector3 position = ParentObject.transform.GetChild(i).localPosition;
                    Quaternion rotation = ParentObject.transform.GetChild(i).localRotation;
                    //position = snapToGrid.SnapPositon(position);
                    //rotation = snapToGrid.SnapRotation(rotation);
                    positionList.Add(position);
                    rotationList.Add(rotation);
                        //child.position = snapToGrid.SnapPositon(child.position);
                        //child.rotation = snapToGrid.SnapRotation(child.rotation);
                        //ChildrenList.Add(child);
                }
                Debug.Log("bbbA" + UnitSimulater.transform.position);
                for(int i = 0;i < positionList.Count;i++){
                    GameObject Preview = Instantiate(PreviewUnit,positionList[i],rotationList[i]);
                    Preview.transform.SetParent(UnitSimulater.transform,false);
                }
                //InstantiateでSimulaterの子オブジェクトとしてprafabのプレビュー用オブジェクトを配置する
                /*
                foreach(Transform child in ChildrenList){
                Instantiate(PrviewUnit,child.position,child.rotation,UnitSimulater.transform);
                }
                */
                //PlayerUnitSimulaterがプレビュー表示するためにオブジェクト情報をコピーしてSimulater側で複製する

                //Rotationは0、座標は原点中心とする
                Debug.Log("bbbA"+ ParentObject.transform.position);
            }
        }
        if(Input.GetMouseButton(0)){
            Debug.Log("bbbB"+ ParentObject.name + ParentObject.transform.position);
            Debug.Log("aaa");
            if(ParentObject == null){
                Debug.LogWarning("Clicked Parent Object is null!");
            }
            else{
                EnemyUnitManagementScript enemyUnitManagementScript = ParentObject.GetComponent<EnemyUnitManagementScript>();
                if(enemyUnitManagementScript == null){
                    Debug.LogWarning("enemyUnitManagementScript is null" + ParentObject.name);
                }else{
                //親オブジェクトのスクリプトEnemyUnitManagementScriptにアクセス
                enemyUnitManagementScript.MovePosition(target);
                //マウスに追従するようにするMovePosition関数を呼び出す
                enemyUnitManagementScript.Spin(wheelInput);
                Debug.Log("ccc");
                }
            }
        }
        if(Input.GetMouseButtonUp(0)){
            //Simulaterの複製を削除する
            for(int i = 0;i < UnitSimulater.transform.childCount;i++){
                Destroy(UnitSimulater.transform.GetChild(i).gameObject);
            }
            //PlayerUnitに複製する
            ParentObject = null;
            Debug.Log("ddd");
        }
    }
    /// <summary>
    /// マウスホイールでドラッグ中のオブジェクトを回転させる
    /// EnemyUnitManagementScript.Spinで対応させたので現在不要
    /// </summary>
    void CursorRotate(){

    }
}

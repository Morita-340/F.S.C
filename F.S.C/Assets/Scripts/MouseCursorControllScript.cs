using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorControllScript : MonoBehaviour
{
    GameObject ParentObject = null;
    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(Input.mousePosition);
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        CursorDragClick(wheelInput);
    }
    /// <summary>
    /// Unitを右クリックしたら離すまでドラッグし続ける
    /// </summary>
    void CursorDragClick(float wheelInput){
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        if(Input.GetMouseButton(0)){
            Debug.Log("aaa");
            if(hit2D.collider/*撃破後のUnitであることを識別できる何かをフラグに持ってくる*/){
                ParentObject = hit2D.collider.gameObject.transform.root.gameObject;
                Debug.Log("bbb");
            }
            if(ParentObject != null){
                EnemyUnitManagementScript enemyUnitManagementScript = ParentObject.GetComponent<EnemyUnitManagementScript>();
                //親オブジェクトのスクリプトEnemyUnitManagementScriptにアクセス
                enemyUnitManagementScript.MovePosition(target);
                //マウスに追従するようにするMovePosition関数を呼び出す
                enemyUnitManagementScript.Spin(wheelInput);
                Debug.Log("ccc");
            }
        }
        if(Input.GetMouseButtonUp(0)){
            ParentObject = null;
            Debug.Log("ddd");
        }
    }
    /// <summary>
    /// マウスホイールでドラッグ中のオブジェクトを回転させる
    /// </summary>
    void CursorRotate(){

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// EnemyUnitの移動と攻撃の制御を行う
/// </summary>
public class EnemyUnitMoveManagementScript : MonoBehaviour
{
    MainCameraController MainCamera;
    Vector2 PlayerVector;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerVector = transform.position -MainCamera.GetPlayer().transform.position;
        if(PlayerVector.x > 90 || PlayerVector.x < -90){MoveAroundPlayer(0);}
        if(PlayerVector.y >40 || PlayerVector.y < -40){MoveAroundPlayer(1);}
    }
    /// <summary>
    /// カメラは常にプレイヤーを追うので、敵が画面外の離れた場所に行ってしまった際の詰み防止
    /// </summary>
    private void MoveAroundPlayer(int moveMode){
        if (MainCamera != null){
            Vector2 PlayerVector = this.transform.position - MainCamera.GetPlayer().transform.position;
            if(moveMode == 0){//x方向の移動
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3( - PlayerVector.x,PlayerVector.y,0)*0.95f;
            }else if(moveMode == 1){
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3(PlayerVector.x, - PlayerVector.y,0)*0.95f;
            }else{Debug.LogWarning("Invalid moveMode");}
        }
    }
}

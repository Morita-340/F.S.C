using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// EnemyUnitの移動と攻撃の制御を行う
/// </summary>
public class EnemyUnitMoveManagementScript : MonoBehaviour
{
    [SerializeField]bool Setting = false;
    private MainCameraController MainCamera;
    private Vector2 PlayerVector;
    protected List<GameObject> DiscoveredObjectList = new List<GameObject>();
    [SerializeField]protected Rigidbody2D rb2d;
    protected bool playerFlag = false;
    [SerializeField,ReadOnly]protected GameObject Player;
    protected StateMachine stateMachine;
    protected EnemyUnitAttackManagementScript EUAMS;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraController>();
        EUAMS = GetComponent<EnemyUnitAttackManagementScript>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(!Setting){
        PlayerRecognize();
        PlayerVector = transform.position -MainCamera.GetPlayer().transform.position;
            if(PlayerVector.x > 95 || PlayerVector.x < -95){MoveAroundPlayer(0);}
            if(PlayerVector.y >70 || PlayerVector.y < -70){MoveAroundPlayer(1);}
        }
    }
    /// <summary>
    /// カメラは常にプレイヤーを追うので、敵が画面外の離れた場所に行ってしまった際の詰み防止
    /// </summary>
    private void MoveAroundPlayer(int moveMode){
        if (MainCamera != null){
            Vector2 PlayerVector = this.transform.position - MainCamera.GetPlayer().transform.position;
            if(moveMode == 0){//x方向の移動
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3( - PlayerVector.x,PlayerVector.y,0)*0.95f;
            }else if(moveMode == 1){//y方向の移動
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3(PlayerVector.x, - PlayerVector.y,0)*0.95f;
            }else{Debug.LogWarning("Invalid moveMode");}
        }
    }
    /// <summary>
    /// プレイヤーの認識はここ
    /// </summary>
    private void PlayerRecognize(){
        if(DiscoveredObjectList.Count != 0){
            foreach(GameObject gameObject in DiscoveredObjectList){
                if(gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
                    Player = gameObject;
                    EUAMS.SetTargetPosition(Player.transform.position);
                    break;
                }
            }
        }else{Player = null;}
    }
    public void SetInputObjList(List<GameObject> ObjectList){
        DiscoveredObjectList = ObjectList;
        //Debug.LogWarning("DiscoveredObjectList Count"+DiscoveredObjectList.Count);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class RefineMouseInput : MonoBehaviour
{
    [SerializeField]GameObject UnitSimulater;
    [SerializeField]GameObject PreviewUnit;
    [SerializeField,ReadOnly]GameObject PlayerUnit;
    [SerializeField,Range(0f,2f)]float firstAttackInterval = 0.5f;
    [SerializeField,ReadOnly]protected SoundController SCer;
    private RefinePlayerUnitAttackManagementScript RePUAMS;
    private RefinePlayerUnitDestroyManagementScript RePUDMS;
    private PlayerUnitSimulateScript playerUnitSimulateScript;
    private DestroyedUnitManagementScript DUMS;
    private SnapToGrid snapToGrid;
    private List<GameObject> previewObjectList = new List<GameObject>(); 
    private List<Vector3> positionList = new List<Vector3>();
    private List<Quaternion> rotationList = new List<Quaternion>();
    private List<GSetting.ShapeType> shapeTypeList = new List<GSetting.ShapeType>();
    private List<UnitBase> UnitBaseList = new List<UnitBase>();
    //private List<WeaponUnitBase> DivideUnitList = new List<WeaponUnitBase>();
    [SerializeField,ReadOnly]private Vector3 target;
    private float WheelInput = 0;
    private float chargeAttackTimer = 0;
    private float normalAttackTimer = 0;
    private bool isRegistered = false;
    // Start is called before the first frame update
    void Start()
    {
        //シーン内のプレイヤーが単一であるため
        PlayerUnit = FindObjectOfType<RefinePlayerUnitDestroyManagementScript>().gameObject;
        InitialSetting();
        SCer = GetComponent<SoundController>();
    }
    void InitialSetting(){
        UnitSimulater = PlayerUnit.transform.Find("UnitSimulater").gameObject;
        //Cursor.visible = false;
        snapToGrid = UnitSimulater.GetComponent<SnapToGrid>();
        playerUnitSimulateScript = UnitSimulater.GetComponent<PlayerUnitSimulateScript>();
        RePUAMS = PlayerUnit.GetComponent<RefinePlayerUnitAttackManagementScript>();
        RePUDMS = PlayerUnit.GetComponent<RefinePlayerUnitDestroyManagementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelInput += Input.GetAxis("Mouse ScrollWheel");
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        if(!RePUDMS.GetIsDead()){CursorControll(WheelInput);}
        RePUAMS?.SetTargetPosition(target);
    }
    /// <summary>
    /// カーソルを動かすことで入力をする処理まとめ
    /// </summary>
    private void CursorControll(float wheelInput){
        //ChargeIcon.ChangeCircleRange(chargeAttackTimer + firstAttackInterval,RePUAMS,target);
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        NormalAttack();
        var Hit2DList = RockOn(ray);
        UnitCapture(Hit2DList.Item2,wheelInput);
    }
    private void NormalAttack(){
        if(Input.GetMouseButton(0)){
            RePUAMS?.NormalAttack(normalAttackTimer);
            normalAttackTimer += Time.deltaTime;
        }else{
            //カーソルを合わせてもすぐには発射しないようにしてクールタイムを無視した連射を防ぐ
            normalAttackTimer = -firstAttackInterval;
        }
    }
    /// <summary>
    /// カーソルをEnemyUnitとDestroyedUnitPlayerUnitとPlayerUnitにかざすと認識できるようにする
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    private (RaycastHit2D,RaycastHit2D,RaycastHit2D) RockOn(Ray ray){
        //Update関数内で実行するためかざされている間は連続で登録されないようにするためのフラグ
        bool enemyHitFirst = true;
        bool destroyedHitFirst = true;
        bool playerHitFirst = true;
        //オブジェクトを登録するためのraycasthit2d
        RaycastHit2D DestroyedHit2D = new RaycastHit2D();
        RaycastHit2D EnemyHit2D = new RaycastHit2D();
        RaycastHit2D PlayerHit2D = new RaycastHit2D();
        foreach(RaycastHit2D hit2D in Physics2D.RaycastAll((Vector2)ray.origin, (Vector2)ray.direction)){
            if(hit2D){
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), hit2D.collider.tag,true)){
                    case GSetting.ObjTagName.EnemyUnit:{
                        if(enemyHitFirst){EnemyHit2D = hit2D;enemyHitFirst = false;}
                        break;}
                    case GSetting.ObjTagName.EnemyWeapon1:{
                        if(enemyHitFirst){EnemyHit2D = hit2D;enemyHitFirst = false;}
                        break;}
                    case GSetting.ObjTagName.DestroyedUnit:{
                        if(destroyedHitFirst){DestroyedHit2D = hit2D;destroyedHitFirst = false;}
                        break;}
                    case GSetting.ObjTagName.PlayerUnit:{
                        if(playerHitFirst){PlayerHit2D = hit2D;playerHitFirst = false;}
                        break;}
                    default:break;
                }
            }else{
                enemyHitFirst = true;
                destroyedHitFirst = true;
                playerHitFirst = true;
            }
            // 両方のヒットがすでに割り当てられたら、ループを終了
            if (DestroyedHit2D && EnemyHit2D && PlayerHit2D)
            {
                Debug.Log("MCCS PlundeAndRockOn");
                break;
            }
        }
        return (EnemyHit2D,DestroyedHit2D,PlayerHit2D);
    }
    /// <summary>
    /// 破壊済みユニットを鹵獲する処理
    /// かざすと選択可能→右クリックで一つだけキャプチャ→マウスホイールで場所を選んで右クリックで場所決定→マウスホイールで向きを選んで右クリックで向き決定
    /// 以上の右クリックによる決定は、キャプチャ操作と場所決定だけホイールクリックでキャンセル可能
    /// </summary>
    private void UnitCapture(RaycastHit2D DestroyedHit2D,float wheelInput){
    }
}

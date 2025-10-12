using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using Unity.VisualScripting;

public class MouseInput : MonoBehaviour
{
    [SerializeField]GameObject UnitSimulater;
    [SerializeField]GameObject PreviewUnit;
    [SerializeField,ReadOnly]GameObject PlayerUnit;
    [SerializeField]ChargeCursolIconController ChargeIcon;
    [SerializeField,Range(0f,2f)]float firstAttackInterval = 0.5f;
    [SerializeField,ReadOnly]protected SoundController SCer;
    private PlayerUnitAttackManagementScript PUAMS;
    private PlayerUnitDestroyManagementScript PUDMS;
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
        PlayerUnit = FindObjectOfType<PlayerUnitDestroyManagementScript>().gameObject;
        InitialSetting();
        SCer = GetComponent<SoundController>();
    }
    void InitialSetting(){
        UnitSimulater = PlayerUnit.transform.Find("UnitSimulater").gameObject;
        //Cursor.visible = false;
        snapToGrid = UnitSimulater.GetComponent<SnapToGrid>();
        playerUnitSimulateScript = UnitSimulater.GetComponent<PlayerUnitSimulateScript>();
        PUAMS = PlayerUnit.GetComponent<PlayerUnitAttackManagementScript>();
        PUDMS = PlayerUnit.GetComponent<PlayerUnitDestroyManagementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelInput += Input.GetAxis("Mouse ScrollWheel");
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        if(!PUDMS.GetIsDead()){CursorControll(WheelInput);}
        PUAMS?.SetTargetPosition(target);
    }
    /// <summary>
    /// カーソルを動かすことで入力をする処理まとめ
    /// </summary>
    private void CursorControll(float wheelInput){
        //ChargeIcon.ChangeCircleRange(chargeAttackTimer + firstAttackInterval,PUAMS,target);
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        NormalAttack();
        var Hit2DList = RockOn(ray);
        Targetting(Hit2DList.Item1);
        UnitCapture(Hit2DList.Item2,wheelInput);
    }
    private void NormalAttack(){
        if(Input.GetMouseButton(0)){
            PUAMS?.NormalAttack(normalAttackTimer);
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
    /// チャージ攻撃を放つための捕捉時間を計算
    /// </summary>
    /// <param name="EnemyHit2D"></param>
    private void Targetting(RaycastHit2D EnemyHit2D){
        if(EnemyHit2D && PUDMS.GetUnitCore().InCoreRange(target)){
            chargeAttackTimer += Time.deltaTime;
            ChargeIcon.ChargeCommand(chargeAttackTimer /*+ firstAttackInterval*/,PUAMS,true);
        }else{
            //カーソルを合わせてもすぐには発射しないようにしてクールタイムを無視した連射を防ぐ
            chargeAttackTimer = 0;
            ChargeIcon.ChargeCommand(chargeAttackTimer /*+ firstAttackInterval*/,PUAMS,false);
        }
    }
    /// <summary>
    /// 破壊済みユニットを鹵獲する処理
    /// かざすと登録（一つのみ）
    /// 登録中に左クリックで設置が可能、ダブルクリックで登録解除
    /// </summary>
    private void UnitCapture(RaycastHit2D DestroyedHit2D,float wheelInput){
        Debug.Log("MI UC" + isRegistered);
        //Rayを照射した瞬間即ちかざしたタイミングで一度だけDestroyedUnitを登録しPreviewを作成する
        if(DestroyedHit2D){
            if(isRegistered == false){
                //isRegisteredはここでのみtrueとする
                isRegistered = true;
                SCer.PlaySE(0);
                GameObject ParentObject = DestroyedHit2D.collider.gameObject.transform.parent.gameObject;
                //Previewを動かすために補正対象として登録する
                //snapToGrid.Origin = ParentObject.transform;
                //子オブジェクトのlocalPositionとlocalRotationを取得してリストに格納
                for(int i = 0;i < ParentObject.transform.childCount;i++){
                    Vector3 position = ParentObject.transform.GetChild(i).localPosition;
                    Quaternion rotation = ParentObject.transform.GetChild(i).localRotation;
                    UnitBase ThisUnitBase = ParentObject.transform.GetChild(i).gameObject.GetComponent<UnitBase>();
                    positionList.Add(position);
                    rotationList.Add(rotation);
                    UnitBaseList.Add(ThisUnitBase);
                    shapeTypeList.Add(ThisUnitBase.GetShapeType());
                }
                //コライダーを無効にしておかないとDestroyedHit2Dが有効のままで登録が繰り返される
                DUMS = ParentObject.GetComponent<DestroyedUnitManagementScript>();
                StartCoroutine(DUMS.ColliderAndSpriteProcess(1));
                PreviewPositioning(wheelInput,DUMS.gameObject,positionList,rotationList,shapeTypeList);
            }
        }
        //登録されている間は回転と移動を自由に行える
        if(isRegistered == true){
            DUMS.MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,10)));
            DUMS.Spin(wheelInput,PlayerUnit.transform.rotation);
            snapToGrid.OriginPosition = DUMS.transform.position;
            snapToGrid.OriginRotation = DUMS.transform.rotation.eulerAngles;
        }
        if(Input.GetMouseButtonDown(1)){
            Regenerate(wheelInput);
        }
        else if(Input.GetMouseButtonDown(2)){
            DestroyedUnitManagementScript DupliDUMS = DUMS;
            Unregister();
            if(DUMS != null){SCer.PlaySE(2);}
            else{SCer.PlaySE(3);}
            StartCoroutine(DupliDUMS?.ColliderAndSpriteProcess(2));
        }
    }
    /// <summary>
    /// PreviewUnitを生成する
    /// </summary>
    private void PreviewPositioning(float wheelInput,GameObject ParentObject,List<Vector3> positionList,List<Quaternion> rotationList,List<GSetting.ShapeType> shapeTypeList){
        //PreviewユニットをSimulaterの子オブジェクトとして生成する
        playerUnitSimulateScript.SetIsNotIsolatedFalse();
        for(int i = 0;i < positionList.Count;i++){
            //Instantiateのオーバーライドで引数を別のにすると挙動が変わってしまうので注意
            GameObject Preview = Instantiate(PreviewUnit,positionList[i],rotationList[i]);
            PreviewUnitManagerScript PUMS = Preview.GetComponent<PreviewUnitManagerScript>();
            PUMS.SetShapeType(shapeTypeList[i]);
            Preview.transform.SetParent(UnitSimulater.transform,false);
            previewObjectList.Add(Preview);
        }
    }
    /// <summary>
    /// 右クリックを使って鹵獲、PlayerUnitとして再生成する処理
    /// </summary>
    private void Regenerate(float wheelInput){
        WheelInput = 0;
        //Simulaterの複製を削除する
        //PlayerUnitに複製する
        Debug.Log("MI Plunderable" + playerUnitSimulateScript.Plunderable());
        if(playerUnitSimulateScript.Plunderable()){
            SCer.PlaySE(1);
            //元データのコライダーと透明度を戻す
            StartCoroutine(DUMS.ColliderAndSpriteProcess(0));
            GameObject SelectedUnit = DUMS.gameObject;
            Debug.Log("MI SelectedUnit" +SelectedUnit);
            for(int i = 0;i < SelectedUnit.transform.childCount;i++){
                Debug.Log(SelectedUnit.transform.GetChild(i).gameObject);
                Debug.Log("MI pr"+ previewObjectList[i].transform.position);
                Debug.Log("MI pr"+ previewObjectList[i].transform.rotation);
                RegenerateAsPlayerUnit(SelectedUnit.transform.GetChild(i).gameObject,previewObjectList[i].transform.position,previewObjectList[i].transform.rotation);
            }
            snapToGrid.OriginRotation = Vector3.zero;
            //DestroyUnitがPlayerUnitに隣接しているかを判定し、隣接しているUnitはAdjacentUnitListに登録
            List<UnitBase> AdjacentUnitList = playerUnitSimulateScript.GetAdjacentUnitList();
            //データの更新にAdjacentUnitListを用いる
            foreach(UnitBase unitBase in AdjacentUnitList){
                unitBase.ReRegistData();
                Debug.Log("FFB" + unitBase.name);
            }
            //PUDMSのデータ更新
            PUDMS.SetUnitData();
            GameObject DestroyedUnit = DUMS.gameObject;
            //選んでいたデブリの親オブジェクトの参照を消去
            Destroy(DestroyedUnit);
            Unregister();
        }else{SCer.PlaySE(3);}
    }
    /// <summary>
    /// 実際に再生成する処理
    /// </summary>
    private void RegenerateAsPlayerUnit(GameObject PlunderUnit,Vector3 Position, Quaternion Rotation){
        Position = new Vector3(Position.x, Position.y,0);
        Instantiate(PlunderUnit,Position,Rotation,PlayerUnit.transform).GetComponent<UnitBase>().UnitSetting(GSetting.ObjTagName.PlayerUnit.ToString(),(int)GSetting.ObjTagName.PlayerUnit);
    }
    /// <summary>
    /// 登録中のユニットを登録解除し、再び登録できるようにデータを空にしておく
    /// </summary>
    void Unregister(){
        //シミュレート用のプレビューオブジェクトを削除する
        for(int i = 0;i < UnitSimulater.transform.childCount;i++){
            Destroy(UnitSimulater.transform.GetChild(i).gameObject);
        }
        DUMS = null;
        positionList.Clear();
        rotationList.Clear();
        shapeTypeList.Clear();
        //プレビューのデータを消去
        previewObjectList.Clear();
        isRegistered = false;
        //元データを消去
        UnitBaseList.Clear();
    }
    public void SetPlayer(GameObject Unit){
        if(Unit == null){this.enabled = false; return;}
        if(Unit.GetComponent<PlayerUnitDestroyManagementScript>()){
            PlayerUnit = Unit;
            InitialSetting();
        }
    }
}

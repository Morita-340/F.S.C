using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using Unity.VisualScripting;

public class SettingMouseInput : MonoBehaviour
{
    //[SerializeField]
    [SerializeField,ReadOnly]
    GameObject UnitSimulater;
    [SerializeField]GameObject PreviewUnit;
    //[SerializeField]
    [SerializeField,ReadOnly]
    GameObject PlayerUnit;
    [SerializeField]ChargeCursolIconController ChargeIcon;
    [SerializeField,Range(0f,2f)]float firstAttackInterval = 0.5f;
    //[SerializeField,Range(0f,10f)]private float divideLimit = 6f;
    //[SerializeField]
    //private UnitBase PlayerUnitSCore;

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
    private Vector3 target;
    private float WheelInput = 0;
    private float chargeAttackTimer = 0;
    private float normalAttackTimer = 0;
    //private float leftClickTimer = 0;
    //private float rightClickTimer = 0;
    private bool isRegistered = false;
    //private bool clickCountStart = false;
    //private int clickCount;
    // Start is called before the first frame update
    void Start()
    {
        //シーン内のプレイヤーが単一であるため
        if(PlayerUnit == null){this.enabled = false;}
        else{InitialSetting();}
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
        ChargeIcon.ChangeCircleRange(chargeAttackTimer + firstAttackInterval,PUAMS,target);
        CursorControll(WheelInput);
    }
    /// <summary>
    /// カーソルを動かすことで入力をする処理まとめ
    /// </summary>
    private void CursorControll(float wheelInput){
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        NormalAttack();
        var Hit2DList = RockOn(ray);
        Targetting(Hit2DList.Item1);
        UnitCapture(Hit2DList.Item2,wheelInput);
        //PlayerDivided(Hit2DList.Item3);
    }
    private void NormalAttack(){
        if(Input.GetMouseButton(1)){
            PUAMS.NormalAttack(normalAttackTimer,target);
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
        if(EnemyHit2D){
            chargeAttackTimer += Time.deltaTime;
        }else{
            //カーソルを合わせてもすぐには発射しないようにしてクールタイムを無視した連射を防ぐ
            chargeAttackTimer = -firstAttackInterval;
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
        if(Input.GetMouseButtonDown(0)){
            Regenerate(wheelInput);
        }
        else if(Input.GetMouseButtonDown(2)){
            DestroyedUnitManagementScript DupliDUMS = DUMS;
            Unregister();
            StartCoroutine(DupliDUMS?.ColliderAndSpriteProcess(2));
        }
        ////左クリックで入力検知開始
        ////timeが閾値を超えるまで入力を検知する
        ////入力検知終了後クリック回数を基にダブルクリックとシングルクリックを判別する
        ////値の初期化と入力検知開始を行う
        ////Debug.Log("MI " + clickCountStart +" " + leftClickTimer+" "  + clickCount);
        //if(Input.GetMouseButtonDown(0) && clickCountStart == false){
        //    clickCountStart = true;
        //    clickCount =0;
        //    leftClickTimer =0;
        //}
        ////一定時間内のクリック回数を計測する
        //if(clickCountStart){
        //    if(leftClickTimer < 0.3f){
        //        leftClickTimer += Time.deltaTime;
        //        if(Input.GetMouseButtonDown(0)){
        //        clickCount ++;
        //        }
        //    }else{
        //        //計測後のクリック回数に基づき処理を行う
        //        LeftClickProcessing(clickCount,wheelInput);
        //        clickCountStart = false;
        //    }
        //}
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
    /// ダブルクリックとシングルクリックを検知できるメソッド
    /// </summary>
    /// <param name="clickCount"></param>
    /// <param name="wheelInput"></param>
    //private void LeftClickProcessing(int clickCount,float wheelInput){
    //    if(clickCount<1){Debug.LogAssertion("チートかバグが出てるぞ");}
    //    else if(clickCount == 1){
    //        Debug.Log("MI OneClick");
    //        Regenerate(wheelInput);
    //    }
    //    else if(clickCount > 1){
    //        Debug.Log("MI MultiClick");
    //        Unregister();
    //        isRegistered = false;
    //    }
    //}
    /// <summary>
    /// 右クリックを使って鹵獲、PlayerUnitとして再生成する処理
    /// </summary>
    private void Regenerate(float wheelInput){
        WheelInput = 0;
        //Simulaterの複製を削除する
        //PlayerUnitに複製する
        Debug.Log("MI Plunderable" + playerUnitSimulateScript.Plunderable());
        if(playerUnitSimulateScript.Plunderable()){
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
        }
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
    /// <summary>
    /// 選択したプレイヤーユニットのリンクを削除することでHP0になった時と同じように分離できるようにする
    /// 選択したユニットはdividableがtrue
    /// リストにぶちこまれているので、周囲のリンク（dividableがfalseのやつ）を削除
    /// HP0の時と同様にリンク上でも孤立するので、その状態で探索、分離操作を行えば安全に分離できる
    /// </summary>
    /// <param name="PlayerHit2D"></param>
    //private void PlayerDivided(RaycastHit2D PlayerHit2D){
    //    //かざされたら一度だけ分離対象リストに登録
    //    if(PlayerHit2D){
    //        Debug.Log("MI PD PlayerHit2D" + PlayerHit2D.collider.name);
    //        //分離できるのは武器ユニットだけなので予め調べておく
    //        WeaponUnitBase PlayerWeaponUnit = PlayerHit2D.collider.GetComponent<WeaponUnitBase>();
    //        //かざされたユニットが分離できない状態であれば（初期状態ではdividableはfalse）分離できるようにする
    //        if(PlayerWeaponUnit?.GetThisUnitData()?.dividable == false){
    //            DivideUnitList.Add(PlayerWeaponUnit);
    //            rightClickTimer = divideLimit;
    //            PlayerWeaponUnit.GetThisUnitData().dividable = true;
    //        }
    //    }
    //    if(rightClickTimer >0){
    //        rightClickTimer -= Time.deltaTime;
    //        if(Input.GetMouseButtonDown(1)){
    //            //GameObject ParentObject = Instantiate(DestroyParentPlunderUnitect,PlayerUnitSCore.transform.position,PlayerUnitSCore.transform.rotation);
    //            //dividableなユニットを分離させる
    //            //分離させたい対象ユニットをリストに格納→周囲とのリンクを消去し、AUDMSのChildrenListからも消去しておく→
    //            //dividableなやつらだけの状態で探索を行ってまとまりで分離させる。
    //            foreach(WeaponUnitBase unitBase in DivideUnitList){
    //                unitBase.GetThisUnitData().DeleteFourWayLinkThatIsNotDividable();
    //                unitBase.gameObject.tag = GSetting.ObjTagName.DestroyedUnit.ToString();
    //                unitBase.gameObject.layer = (int)GSetting.UniqueLayerName.DestroyedUnit;
    //                //unitBase.gameObject.transform.SetParent(ParentObject.transform,false);
    //            }
    //            //DeleteFourWayLinkThatIsNotDividable()の際にdividableの値が使われるので、対象のユニット全てに対してメソッドの実行が終了してからfalseに書き換えなおさないといけない
    //            foreach(WeaponUnitBase unitBase in DivideUnitList){
    //                Debug.Log("MI PD Dividable"+ unitBase.gameObject.name);
    //                unitBase.GetThisUnitData().dividable = false;
    //            }
    //            PUDMS.DestroyProcess();
    //            DivideUnitList.Clear();
    //        }
    //    }else{
    //        foreach(WeaponUnitBase unitBase in DivideUnitList){
    //            unitBase.GetThisUnitData().dividable = false;
    //        }
    //        DivideUnitList.Clear();
    //    }
    //}
    public void SetPlayer(GameObject Unit){
        if(Unit == null){this.enabled = false; return;}
        if(Unit.GetComponent<PlayerUnitDestroyManagementScript>()){
            this.enabled = true;
            PlayerUnit = Unit;
            InitialSetting();
        }
    }
}


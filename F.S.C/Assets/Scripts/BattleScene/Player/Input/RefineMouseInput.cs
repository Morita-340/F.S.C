using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class RefineMouseInput : MonoBehaviour
{
    [SerializeField] GameObject UnitSimulater;
    [SerializeField] GameObject PreviewUnit;
    [SerializeField, ReadOnly] GameObject PlayerUnit;
    [SerializeField, Range(0f, 2f)] float firstAttackInterval = 0.5f;
    [SerializeField, ReadOnly] protected SoundController SCer;
    [SerializeField]private RefinePreviewControll RePC;
    private RefinePlayerUnitAttackManagementScript RePUAMS;
    [SerializeField,ReadOnly]private RefinePlayerUnitDestroyManagementScript RePUDMS;
    private PlayerUnitSimulateScript playerUnitSimulateScript;
    private DestroyedUnitManagementScript DUMS;
    private SnapToGrid snapToGrid;
    private List<GameObject> previewObjectList = new List<GameObject>();
    private List<Vector3> positionList = new List<Vector3>();
    private List<Quaternion> rotationList = new List<Quaternion>();
    private List<GSetting.ShapeType> shapeTypeList = new List<GSetting.ShapeType>();
    private List<UnitBase> UnitBaseList = new List<UnitBase>();
    //private List<WeaponUnitBase> DivideUnitList = new List<WeaponUnitBase>();
    [SerializeField, ReadOnly] private Vector3 target;
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
    /// <summary>
    /// このクラスで使うコンポーネントのインスタンスの参照を諸々設定
    /// </summary>
    void InitialSetting()
    {
        //UnitSimulater = PlayerUnit.transform.Find("UnitSimulater").gameObject;
        //Cursor.visible = false;
        //snapToGrid = UnitSimulater.GetComponent<SnapToGrid>();
        //playerUnitSimulateScript = UnitSimulater.GetComponent<PlayerUnitSimulateScript>();
        RePUAMS = PlayerUnit.GetComponent<RefinePlayerUnitAttackManagementScript>();
        RePUDMS = PlayerUnit.GetComponent<RefinePlayerUnitDestroyManagementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //マウスホイールの入力を取得
        WheelInput += Input.GetAxis("Mouse ScrollWheel");
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        if (!RePUDMS.GetIsDead())//プレイヤーが死んでいないならば
        {
            //攻撃目標座標を更新し続ける
            RePUAMS?.SetTargetPosition(target);
            //カーソルを動かす諸々の処理を繰り返す
            CursorControll(WheelInput);
        }
    }
    /// <summary>
    /// カーソルを動かすことで入力をする処理まとめ
    /// </summary>
    private void CursorControll(float wheelInput)
    {
        //ChargeIcon.ChangeCircleRange(chargeAttackTimer + firstAttackInterval,RePUAMS,target);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        NormalAttack();
        var Hit2DList = RockOn(ray);
        //パーツの情報を閲覧可能
        PartsInfoDisplay(Hit2DList.Item1, Hit2DList.Item2, Hit2DList.Item1);
        //右クリックで鹵獲処理開始
        if (Input.GetMouseButtonDown(1))
        {
            PartsCapture(Hit2DList.Item2, wheelInput);
        }
    }
    private void NormalAttack()
    {
        if (Input.GetMouseButton(0))
        {
            RePUAMS?.NormalAttack(normalAttackTimer);
            normalAttackTimer += Time.deltaTime;
        }
        else
        {
            //カーソルを合わせてもすぐには発射しないようにしてクールタイムを無視した連射を防ぐ
            normalAttackTimer = -firstAttackInterval;
        }
    }
    /// <summary>
    /// カーソルをEnemyUnitとDestroyedUnitPlayerUnitとPlayerUnitにかざすと認識できるようにする
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    private (RaycastHit2D, RaycastHit2D, RaycastHit2D) RockOn(Ray ray)
    {
        //Update関数内で実行するためかざされている間は連続で登録されないようにするためのフラグ
        bool enemyHitFirst = true;
        bool destroyedHitFirst = true;
        bool playerHitFirst = true;
        //オブジェクトを登録するためのraycasthit2d
        RaycastHit2D DestroyedHit2D = new RaycastHit2D();
        RaycastHit2D EnemyHit2D = new RaycastHit2D();
        RaycastHit2D PlayerHit2D = new RaycastHit2D();
        foreach (RaycastHit2D hit2D in Physics2D.RaycastAll((Vector2)ray.origin, (Vector2)ray.direction))
        {
            if (hit2D)
            {
                switch ((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), hit2D.collider.tag, true))
                {
                    case GSetting.ObjTagName.EnemyUnit:
                        {
                            if (enemyHitFirst) { EnemyHit2D = hit2D; enemyHitFirst = false; }
                            break;
                        }
                    case GSetting.ObjTagName.EnemyWeapon1:
                        {
                            if (enemyHitFirst) { EnemyHit2D = hit2D; enemyHitFirst = false; }
                            break;
                        }
                    case GSetting.ObjTagName.DestroyedUnit:
                        {
                            if (destroyedHitFirst) { DestroyedHit2D = hit2D; destroyedHitFirst = false; }
                            break;
                        }
                    case GSetting.ObjTagName.PlayerUnit:
                        {
                            if (playerHitFirst) { PlayerHit2D = hit2D; playerHitFirst = false; }
                            break;
                        }
                    default: break;
                }
            }
            else
            {
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
        return (EnemyHit2D, DestroyedHit2D, PlayerHit2D);
    }
    /// <summary>
    /// パーツの情報を表示する。
    /// 自他関係なくカーソルがかざしているパーツの情報を取得し、詳細は表示専用UIオブジェクトに渡して表示
    /// </summary>
    /// <param name="EnemyHit2D">敵のパーツの強化具合を見て、鹵獲の目標を決める</param>
    /// <param name="DestroyedHit2D">鹵獲対象のパーツを見て、鹵獲するか判断</param>
    /// <param name="PlayerHit2D">自分のパーツの強化具合を見て、強さを認識</param>
    void PartsInfoDisplay(RaycastHit2D EnemyHit2D, RaycastHit2D DestroyedHit2D, RaycastHit2D PlayerHit2D)
    {
        //パーツをかざしていれば情報を取得する
        if (EnemyHit2D || DestroyedHit2D || PlayerHit2D)
        {
            //情報を取得する
            //情報をUIに渡す
        }
    }
    /// <summary>
    /// 破壊済みユニットを鹵獲する処理。
    /// かざすと選択可能→右クリックで一つだけキャプチャ→マウスホイールで場所を選んで右クリックで場所決定
    /// 以上の右クリックによる決定は、キャプチャ操作と場所決定だけホイールクリックでキャンセル可能
    /// </summary>
    private void PartsCapture(RaycastHit2D DestroyedHit2D, float wheelInput)
    {
        //鹵獲対象が無いなら処理終わり（バグ対策）
        if (!DestroyedHit2D)
        {
            return;
        }
        //鹵獲するパーツだけを取り出して、不要なパーツ周りの処理も済ませる
        DecideTargetParts(DestroyedHit2D,wheelInput);
    }
    /// <summary>
    /// どのユニットを鹵獲するか決める
    /// 決めたら、プレビュー表示メソッドに決めたユニットが何なのか渡す
    /// </summary>
    private void DecideTargetParts(RaycastHit2D DestroyedHit2D, float wheelInput)
    {
        //パーツを解析して接続状況を取得
        GameObject hitObj = DestroyedHit2D.collider.gameObject;
        RefineDestroyedUnitManagementScript ReDUMS = hitObj.transform.root.GetComponent<RefineDestroyedUnitManagementScript>();
        AbstractPartsController APC = hitObj.transform.parent.GetComponent<AbstractPartsController>();
        List<AbstractPartsController> CapturePartsGroup = new List<AbstractPartsController>();
        //接続状況に応じて場合分け
        switch ((GSetting.PartsConnectSituation)Enum.Parse(typeof(GSetting.PartsConnectSituation), ReDUMS.GetPartsConnectSituation().ToString(), true))
        {
            //大破パーツのみ
            case GSetting.PartsConnectSituation.OnlyWreckParts:
                {
                    GainEXP(ReDUMS);
                    //合体処理が不要なのでここで処理終わり
                    return;
                }
            //小破以下のまとまりが複数
            case GSetting.PartsConnectSituation.MultipleScrachPartsGroup:
                {
                    GainEXP(ReDUMS);
                    //残ったまとまりのうち、クリック時にカーソルがかざしていたまとまりを鹵獲対象とする
                    CapturePartsGroup = ReDUMS.GetSelectPartsGroup(APC);
                    if(CapturePartsGroup == null){return;}
                    //かざした方を分裂させる
                    //これにより、ReDUMS配下にあるパーツの塊は一つになった
                    ReDUMS.PartsExtract(CapturePartsGroup);
                    break;
                }
            //小破以下のまとまりが一つだけ
            case GSetting.PartsConnectSituation.A_ScrachPartsGroup:
                {
                    GainEXP(ReDUMS);
                    //残ったパーツのまとまりが鹵獲対象
                    CapturePartsGroup = ReDUMS.GetSelectPartsGroup(APC);
                    if(CapturePartsGroup == null){return;}
                    break;
                }
            //それ以外
            default:
                {
                    GSetting.RefineDebugAssertinLog(DestroyedHit2D.transform, "接続状況が設定されていない！" + ReDUMS.GetPartsConnectSituation().ToString());
                    //接続状況が設定されていないためこれ以上処理を進められない。鹵獲はしない
                    return;
                }
        }
        //合体対象を一時的に避難（有効のままスプライトと当たり判定をOFFにして画面から離す）
        StartCoroutine(ReDUMS.ColliderAndSpriteProcess(1));
        //どこにパーツを置くか決める
        //StartCoroutine(DecideWhereToPutParts(CapturePartsGroup,ReDUMS,wheelInput));
    }
    /// <summary>
    /// ユニットを置く位置を決める
    /// </summary>
    private IEnumerator DecideWhereToPutParts(List<AbstractPartsController>CapturePartsGroup,RefineDestroyedUnitManagementScript ReDUMS, float wheelInput)
    {
        //空きのある機体側のパッシブジョイントユニットの座標をリストにまとめる
        List<Vector3> passiveJointList = RePUDMS.GetAllEmptyPassiveJointSPositionList();
        //プレビューを表示するために選択中の座標や回転角を渡しておく
        //マウスホイールで選べるようにする
        Vector3 selectedPos = passiveJointList[(int)wheelInput % passiveJointList.Count];
        //合体させるactiveJointUnitがあるパーツの座標を取得
        AbstractPartsController CenterAPC = ReDUMS.GetEmptyJoint();
        RePC.SetPreviewInfo(CapturePartsGroup,CenterAPC,selectedPos,RePUDMS.transform.rotation.eulerAngles);
        //右クリックまたはホイールクリックをするまで先に進まない
        yield return new WaitUntil(() => Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2));
        //右クリックなら処理を進める
        if (Input.GetMouseButtonDown(1))
        {
            //設置場所が機体と被るか？
            if (RePC.CapturePartsCoveredPlayer())//被る
            {
                //エラー音を鳴らす
                //もう一度
                StartCoroutine(DecideWhereToPutParts(CapturePartsGroup, ReDUMS, wheelInput));
                yield break;
            }
            else//被らない
            {
                //座標決定、合体処理
                //合体させるactivejointUnitがあるパーツの座標を取得
                foreach (AbstractPartsController part in ReDUMS.GetChildPartsList())
                {
                    //塊を構成する各パーツの相対座標を取得
                    Vector3 position = part.transform.position - selectedPos;
                    //activeJointのパーツの座標を上記指定座標に設定、残りのパーツに相対座標を加算して配置
                    part.transform.position = position + selectedPos;
                    part.transform.rotation = RePUDMS.transform.rotation;
                    part.transform.parent = RePUDMS.gameObject.transform;
                    //多分タグ周りの処理が必要
                    part.tag = GSetting.ObjTagName.PlayerUnit.ToString();
                }

            }

        }//ホイールクリックならパーツをリリースして選びなおせるようにする
        else if (Input.GetMouseButtonDown(2))
        {
            //パーツをリリース
            PartsRelease(ReDUMS);
        }
    }
    /// <summary>
    /// 大破したパーツを強化ポイントに変換して消去し、機体の経験値に加算
    /// </summary>
    private void GainEXP(RefineDestroyedUnitManagementScript ReDUMS)
    {
        //大破パーツを全て取得
        //強化ポイントを計上して大破パーツを消す
        //子にパーツが無くなった場合はReDUMSを消す
        //ジョイントパーツのリンクを取り直し
        //機体の経験値に加算
    }
    /// <summary>
    /// 鹵獲したパーツの集合体（パーツ1個以上）を（カーソルの場所に）リリースする（もう一度放出する）
    /// </summary>
    /// <param name="ReDUMS"></param>
    private void PartsRelease(RefineDestroyedUnitManagementScript ReDUMS)
    {
        ReDUMS.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        StartCoroutine(ReDUMS.ColliderAndSpriteProcess(2));
    }
}

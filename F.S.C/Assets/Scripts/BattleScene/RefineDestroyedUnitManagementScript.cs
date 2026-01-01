using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSCGeneral;
using UnityEngine;

public class RefineDestroyedUnitManagementScript : MonoBehaviour
{
    //生成時に決めておく
    [SerializeField, ReadOnly]protected GSetting.PartsConnectSituation ConnectSituation = GSetting.PartsConnectSituation.NotSet;
    [SerializeField] private Rigidbody2D rb2D;
    /// <summary>
    /// 生成時の回転角と移動方向を格納するx,y=移動方向のベクトル、z=回転角
    /// </summary>
    [SerializeField] Vector3 InstMoveAndRotateVector = new Vector3(0, 0, 0);
    /// <summary>
    /// 合体処理に用いる空きがあるA_JointUnit
    /// </summary>
    [SerializeField, ReadOnly] ActiveJointUnit EmptyActiveJointUnit = null;
    private GameObject DestroyedUnitManager;
    MainCameraController MainCamera;
    /// <summary>
    /// 損傷状況に関係なく、今子オブジェクトとして存在しているパーツのリスト
    /// </summary>
    [SerializeField,ReadOnly]List<AbstractPartsController> ChildPartsList = new List<AbstractPartsController>();
    /// <summary>
    /// 小破及び無傷のパーツのグループをまとめたリスト
    /// </summary>
    List<List<AbstractPartsController>> PartsGroupList = new List<List<AbstractPartsController>>();
    [SerializeField,ReadOnly]
    private int partsAttackPower = 0;
    [SerializeField,ReadOnly]
    private int partsHP = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        DestroyedUnitManager = this.gameObject;
        int instModeNum;
        if (colliderAndSpriteONFlag) { instModeNum = 0; }
        else { instModeNum = 2; }
        StartCoroutine(ColliderAndSpriteProcess(instModeNum));
    }
    private void Start(){
        MainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraController>();
        //武器が制御ユニットとの接続を取れるように分離できているなら、表彰として何かしらUIを表示させたい
        foreach(Transform child in transform){
            if(child.GetComponent<WeaponUnitBase>()){
                if(child.GetComponent<WeaponUnitBase>().IsWCUBenabled()){
                    //表彰の処理
                }
            }
        }
    }

    /// <summary>
    /// 生成時に明滅するか、点灯するか
    /// </summary>
    private bool colliderAndSpriteONFlag = true;
    /// <summary>
    /// colliderAndSpriteONFlagを設定する
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>分離処理の際に生成のタイミングで呼び出さないといけないので、Instantiateの行でGameObjectを返さないといけない</returns>
    public GameObject SetColliderAndSpriteONFlag(bool flag)
    {
        colliderAndSpriteONFlag = flag;
        return gameObject;
    }
    /// <summary>
    /// ReDUMSの初期設定諸々。コルーチンとして処理しないと処理が正しく動かない
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitialSetting()
    {
        ChildPartsList.Clear();
        PartsGroupList.Clear();
        yield return new WaitForSeconds(0.2f);
        //子オブジェクトのパーツを取得
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AbstractPartsController>())
            {
                ChildPartsList.Add(transform.GetChild(i).GetComponent<AbstractPartsController>());
            }
        }
        foreach (AbstractPartsController part in ChildPartsList)
        {
            part.CaluculateCombatPower();
            partsAttackPower += part.GetAttackPower();
            partsHP += part.GetHP();
        }
        Debug.LogAssertion("AAAA");
        AnalysisConnectSituation();

    }
    public int GetAttackPower()
    {
        return partsAttackPower;
    }
    public int GetHP()
    {
        return partsHP;
    }
    private void FuncInitialSetting()
    {
        ChildPartsList.Clear();
        PartsGroupList.Clear();
        //子オブジェクトのパーツを取得
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AbstractPartsController>())
            {
                ChildPartsList.Add(transform.GetChild(i).GetComponent<AbstractPartsController>());
            }
        }
        AnalysisConnectSituation();
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのコライダーを有効にするか無効にするかを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    private void ChildrenSColliderEnabled(bool flag)
    {
        if (DestroyedUnitManager.transform.childCount != 0)
        {
            foreach (AbstractPartsController APC in ChildPartsList)
            {
                APC.ChildrenSColliderEnabled(flag);
            }
        }
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのスプライトを半透明にするか否かを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    private void ChildrenSpriteTranslucent(bool flag){
        if (DestroyedUnitManager.transform.childCount != 0)
        {
            foreach (AbstractPartsController APC in ChildPartsList)
            {
                APC.ChildrenSpriteTranslucent(flag);
            }
        } 
    }
    /// <summary>
    /// DestroyedUnit生成時のコライダーとスプライトの処理が3パターン存在するためここで管理
    /// </summary>
    /// <param name="processMode"></param>
    /// <returns></returns>
    public IEnumerator ColliderAndSpriteProcess(int processMode)
    {
        Debug.LogAssertion(processMode);
        switch (processMode)
        {
            case 0:
                {//初期生成はこれ
                    ChildrenSpriteTranslucent(false);
                    ChildrenSColliderEnabled(true);
                    break;
                }
            case 1:
                {//合体処理の鹵獲開始はこれ
                    ChildrenSpriteTranslucent(true);
                    ChildrenSColliderEnabled(false);
                    break;
                }
            case 2:
                {//合体処理の鹵獲破棄はこれ
                    ChildrenSpriteTranslucent(true);
                    ChildrenSColliderEnabled(false);
                    yield return new WaitForSeconds(1);
                    //元データのコライダーと透明度を戻す
                    ChildrenSpriteTranslucent(false);
                    ChildrenSColliderEnabled(true);
                    break;
                }
            case 3:
                {//合体直前の演出時はこれ
                    ChildrenSpriteTranslucent(false);
                    ChildrenSColliderEnabled(false);
                    break;
                }
            default:
                {
                    Debug.LogWarning("Invalid processMode");
                    break;
                }
        }
        yield break;
    }
    /// <summary>
    /// 入力情報をもとに生成時の回転角度と移動方向を格納する
    /// </summary>
    /// <param name="OriginX">移動するにあたって離れたい対象（分離処理の場合、コアから離れる方向に移動させたいならコアの座標を入力すればいい）</param>
    /// <param name="OriginY"></param>
    /// <param name="RotateVector"></param>
    public void SetMoveAndRotateVector(float OriginX, float OriginY)
    {
        float vectorX = this.transform.position.x - OriginX;
        float vectorY = this.transform.position.y - OriginY;
        if ((vectorX > 1 && vectorX < -1) || (vectorY > 1 && vectorY < -1))
        {
            vectorX = (vectorX <= vectorY) ? vectorX / vectorY : vectorX;
            vectorY = (vectorX <= vectorY) ? vectorY : vectorY / vectorX;
        }
        InstMoveAndRotateVector.x = vectorX;
        InstMoveAndRotateVector.y = vectorY;
        InstMoveAndRotateVector.z = UnityEngine.Random.Range(-0.2f, 0.2f);
    }
    /*以下接続状況の把握処理**************************************/
    private void AnalysisConnectSituation()
    {
        //子パーツを値コピー。参照ではないためコピー元に影響は無い
        List<AbstractPartsController> ChildPartsListCopy = new List<AbstractPartsController>(ChildPartsList);
        //まとまりを作る
        Debug.LogAssertion("AAAA");
        ConnectSituationBreathFirstSearch(ChildPartsListCopy);
        //まとまりに応じて場合分け
        RecordConnectSituation();
        foreach (AbstractPartsController childPart in ChildPartsList)
        {
            if (childPart.GetActiveJointLink() == null ? false:childPart.GetActiveJointLink().isJointLinkEmpty())
            {
                EmptyActiveJointUnit = childPart.GetActiveJointLink();
            }
        }
    }
    /// <summary>
    /// パーツレベルのリンクを取得して横型探索を行い、小破以下のまとまりを把握し、まとまりを記録
    /// </summary>
    private void ConnectSituationBreathFirstSearch(List<AbstractPartsController> ChildPartsListCopyRef)
    {
        Debug.LogAssertion("AAAB");
        while (ChildPartsListCopyRef.Count > 0)
        {
            AbstractPartsController APC = ChildPartsListCopyRef[0];
            GSetting.PartsDamageStatus? partsDamageStatus = APC.GetPartsDamageStatus();
            Debug.LogAssertion("AAAC");
            //小破・無傷以外（登録忘れ含む）なら探索対象から除外
            if (!(partsDamageStatus == GSetting.PartsDamageStatus.NoDamage || partsDamageStatus == GSetting.PartsDamageStatus.MinorDamage))
            {
                ChildPartsListCopyRef.Remove(APC);
                //大破以外＝未登録orNULLならログ表示
                if (!(partsDamageStatus == GSetting.PartsDamageStatus.HeavilyDamage))
                {
                    GSetting.RefineDebugAssertinLog(APC.transform, "損傷状況の登録情報がおかしい" + partsDamageStatus);
                }
                Debug.LogAssertion("AAAD");
                continue;
            }
            //小破及び無傷の場合＝まとまりに分ける
            //スタックを定義、種を格納
            Stack<AbstractPartsController> stack = new Stack<AbstractPartsController>();
            stack.Push(APC);
            //まとまり一つをリストとして格納を進めていくためにリストを定義
            List<AbstractPartsController> Parts = new List<AbstractPartsController>();
            //=今回のループで使うリストにスタックの種になるパーツを格納
            Debug.LogAssertion("AAAE"+APC.gameObject.name);
            Parts.Add(APC);
            APC.SetSearchFlag(true);
            //ローカルの方のコピーリストから探索済みパーツを削除する
            ChildPartsListCopyRef.Remove(APC);
            //while文。スタックが空になるまで続ける
            while (stack.Count > 0)
            {
                //パーツのリンクのリストを取得する
                AbstractPartsController popAPC = stack.Pop();
                List<AbstractPartsController> partsLink = new List<AbstractPartsController>();
                foreach (PassiveJointUnit joint in popAPC.GetPassiveJointLinkList())
                {
                    AbstractPartsController jointedAP = joint.GetJointedAnotherPart();
                    if (jointedAP != null) partsLink.Add(jointedAP);
                }
                Debug.Log("AAV" + popAPC.GetActiveJointLink());
                AbstractPartsController A_jointedAP = popAPC.GetActiveJointLink()?.GetJointedAnotherPart();
                if (A_jointedAP != null) partsLink.Add(A_jointedAP);
                //リストを走査
                foreach (AbstractPartsController part in partsLink)
                {
                    Debug.LogAssertion("AAAU"+part.gameObject.name);
                    //未探索　かつ　損傷が小破及び無傷　なら
                    if (!part.GetSearchFlag() && (part.GetPartsDamageStatus() == GSetting.PartsDamageStatus.NoDamage || part.GetPartsDamageStatus() == GSetting.PartsDamageStatus.MinorDamage))
                    {
                        Debug.LogAssertion("AAAV"+part.gameObject.name);
                        //スタックにリンクを格納する
                        stack.Push(part);
                        //ローカルの方のコピーリストから探索済みパーツを削除する
                        ChildPartsListCopyRef.Remove(part);
                        //PartsGroupList[groupCount][element]にパーツを格納
                        //=今回使っているまとまり一つのリストにパーツを格納
                        Parts.Add(part);
                        //探索フラグを有効にする
                        part.SetSearchFlag(true);
                    }
                }
            }
            Debug.LogAssertion("AAAW");
            //while文終了。
            //まとまりのリストに格納しておく
            PartsGroupList.Add(Parts);
        }
        //探索フラグをリセットしておく
        foreach (AbstractPartsController part in ChildPartsList)
        {
            part.SetSearchFlag(false);
        }
    }
    /// <summary>
    /// 引数のパーツが含まれるグループを返す
    /// </summary>
    /// <param name="APC"></param>
    /// <returns></returns>
    public List<AbstractPartsController> GetSelectPartsGroup(AbstractPartsController APC)
    {
        foreach (List<AbstractPartsController> Group in PartsGroupList)
        {
            foreach (AbstractPartsController Part in Group)
            {
                if (Part == APC)
                {
                    return Group;
                }
            }
        }
        GSetting.RefineDebugAssertinLog(APC.transform, "グループリストの中に鹵獲対象が無い");
        return null;
    }
    public void ReloadJointLink()
    {
        foreach (AbstractPartsController Part in ChildPartsList)
        {
            Part.ReloadJointUnitLink();
        }
    }
    /// <summary>
    /// まとまりの種類に応じて接続状況を分類
    /// </summary>
    private void RecordConnectSituation()
    {
        Debug.LogAssertion("AAAZ"+PartsGroupList.Count);
        switch (PartsGroupList.Count)
        {
            case 0:
                {
                    ConnectSituation = GSetting.PartsConnectSituation.OnlyWreckParts;
                    break;
                }
            case 1:
                {
                    ConnectSituation = GSetting.PartsConnectSituation.A_ScrachPartsGroup;
                    break;
                }
            case int s when (s >= 2):
                {//小破以下のまとまりが複数
                    ConnectSituation = GSetting.PartsConnectSituation.MultipleScrachPartsGroup;
                    break;
                }
            default:
                {
                    GSetting.RefineDebugAssertinLog(this.transform, "グループの数がおかしい" + PartsGroupList.Count);
                    break;
                }
        }
    }
    public GSetting.PartsConnectSituation GetPartsConnectSituation()
    {
        return ConnectSituation;
    }
    public void DeleteHeavilyDamagedParts()
    {
        List<AbstractPartsController> heavilyPartsList = new List<AbstractPartsController>();
        foreach (AbstractPartsController part in ChildPartsList)
        {
            if (part.GetPartsDamageStatus() == GSetting.PartsDamageStatus.HeavilyDamage) heavilyPartsList.Add(part);
        }
        ChildPartsList.RemoveAll(x => x.GetPartsDamageStatus() == GSetting.PartsDamageStatus.HeavilyDamage);
        //PartsGroupList
        foreach (AbstractPartsController part in heavilyPartsList)
        {
            Destroy(part.gameObject);
        }
    }
    /// <summary>
    /// 引数で指定したパーツのまとまりを分離させる
    /// </summary>
    /// <param name="APCList">分離対象のパーツのまとまり</param>
    /// <param name="ReDUMSObj">Prefabにある、子が無いReDUMS。元々Prefabにある自分自身を呼び出そうとすると、Scene上の自分自身を参照してしまうみたいなので、他オブジェクトにPrefabの参照を置かないといけない</param>
    /// <param name="callback">         返り値を使えないので代わりのコールバック    分離したパーツのまとまり（カーソルをかざした方）</param>
    /// <param name="callbackReDUMS">   返り値を使えないので代わりのコールバック    分離させて新たに生成したReDUMS（分離したパーツのまとまりをまとめる方のReDUMS）</param>
    /// <returns></returns>
    public IEnumerator PartsExtract(List<AbstractPartsController> APCList, GameObject ReDUMSObj, Action<List<AbstractPartsController>> callback, Action<RefineDestroyedUnitManagementScript> callbackReDUMS)
    {
        //分離時の初期設定
        //ReDUMSのオブジェクトを作って配下にパーツをまとめて追加すればOK
        GameObject RegeneDUMSObj = Instantiate(ReDUMSObj);
        RegeneDUMSObj.transform.position = APCList[0].transform.position;
        RegeneDUMSObj.transform.rotation = APCList[0].transform.rotation;
        List<AbstractPartsController> CapturePartsGroup = new List<AbstractPartsController>();
        Debug.LogAssertion(ReDUMSObj.transform.childCount);
        Debug.LogAssertion(APCList.Count);
        foreach (AbstractPartsController Part in APCList)
        {
            //Part.transform.parent = RegeneDUMSObj.transform;
            CapturePartsGroup.Add(Instantiate(Part.gameObject, Part.transform.position, APCList[0].transform.rotation, RegeneDUMSObj.transform).GetComponent<AbstractPartsController>());
            Destroy(Part.gameObject);
        }
        yield return StartCoroutine(RegeneDUMSObj.GetComponent<RefineDestroyedUnitManagementScript>().InitialSetting());
        //StartCoroutine(RegeneDUMSObj.GetComponent<RefineDestroyedUnitManagementScript>().ColliderAndSpriteProcess(1));
        callback?.Invoke(CapturePartsGroup);
        callbackReDUMS?.Invoke(RegeneDUMSObj.GetComponent<RefineDestroyedUnitManagementScript>());
        //選んでない方の諸々の処理
        //分離元から消去する
        yield return StartCoroutine(InitialSetting());
        //ReDUMSの原点とパーツの配置座標が一致しないので調整
        //ディレクトリの一番上にあるパーツのローカル座標とグローバル座標を取得
        Vector3 BaseGroPos = ChildPartsList[0].transform.position;
        Vector3 BaseLocPos = ChildPartsList[0].transform.localPosition;
        //ReDUMSの座標をグローバル座標に変更
        transform.position = BaseGroPos;
        //各パーツのローカル座標をディ一ローカル座標 - ローカル座標にして合わせる
        foreach (AbstractPartsController Part in ChildPartsList)
        {
            Part.transform.localPosition = Part.transform.localPosition - BaseLocPos;
            Part.ReloadJointUnitLink();
        }
    }
    public List<AbstractPartsController> GetChildPartsList()
    {
        return ChildPartsList;
    }
    /// <summary>
    /// 合体させるactiveJointUnitがあるパーツの座標を取得
    /// これを呼び出す時点で合体対象のまとまり以外を分離させている前提
    /// </summary>
    /// <returns></returns>
    public AbstractPartsController GetEmptyJoint()
    {
        foreach (AbstractPartsController Part in ChildPartsList)
        {
            ActiveJointUnit activeJoint = Part.GetActiveJointLink();
            if (activeJoint.isJointLinkEmpty())
            {
                return Part;
            }
        }
        GSetting.RefineDebugAssertinLog(transform, "ActiveJointUnitが見つからない");
        return null;
    }
    /// <summary>
    /// LineRendererの描画及び、ReMouseInputのRaycast用の処理、他のスクリプトで呼び出すべきではない
    /// </summary>
    /// <param name="SelectedP_JointPos">RayCastに用いる。常時動くためVector3ではなくコンポーネントごと渡す</param>
    /// <returns></returns>
    public ActiveJointUnit SetLineAtoPJoint(Vector3 SelectedP_JointPos)
    {
        LineRenderer line = EmptyActiveJointUnit.GetComponent<LineRenderer>();
        //ループ設定
        line.loop = false;

        //頂点の初期設定
        Vector3[] positions = new Vector3[]{EmptyActiveJointUnit.transform.position,SelectedP_JointPos};

        //頂点の数を設定
        line.positionCount = positions.Length;

        //頂点の位置設定
        line.SetPositions(positions);

        //幅の設定
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;

        //色の設定
        line.startColor = Color.red;
        line.endColor = Color.red;

        //曲がり角の丸み設定
        line.numCornerVertices = 10;

        //線終端の丸み設定
        line.numCapVertices = 0;
        return EmptyActiveJointUnit;
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0){Destroy(gameObject);}
        rb2D.velocity = new Vector3(InstMoveAndRotateVector.x,InstMoveAndRotateVector.y,0);
        transform.Rotate(0,0,InstMoveAndRotateVector.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSCGeneral;
using UnityEngine;

public class RefineDestroyedUnitManagementScript : MonoBehaviour
{
    [SerializeField] GameObject ReDUMSObj;
    //生成時に決めておく
    [SerializeField, ReadOnly]protected GSetting.PartsConnectSituation ConnectSituation = GSetting.PartsConnectSituation.NotSet;
    [SerializeField] private Rigidbody2D rb2D;
    /// <summary>
    /// 生成時の回転角と移動方向を格納するx,y=移動方向のベクトル、z=回転角
    /// </summary>
    [SerializeField] Vector3 InstMoveAndRotateVector = new Vector3(0, 0, 0);
    private GameObject DestroyedUnitManager;
    MainCameraController MainCamera;
    List<AbstractPartsController> ChildPartsList = new List<AbstractPartsController>();
    List<List<AbstractPartsController>> PartsGroupList = new List<List<AbstractPartsController>>();
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
    public IEnumerator InitialSetting()
    {
        yield return new WaitForSeconds(0.2f);
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
    private void ChildrenSColliderEnabled(bool flag){
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
    /// DestroyedUnit生成時のコライダーとスプライトの処理が2パターン存在するためここで管理
    /// </summary>
    /// <param name="time"></param>
    /// <param name="processMode"></param>
    /// <returns></returns>
    public IEnumerator ColliderAndSpriteProcess(int processMode)
    {
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
                    ChildrenSpriteTranslucent(false);
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
        InstMoveAndRotateVector.z = Random.Range(-0.2f, 0.2f);
    }
    /*以下接続状況の把握処理**************************************/
    private void AnalysisConnectSituation()
    {
        //子パーツを値コピー。参照ではないためコピー元に影響は無い
        List<AbstractPartsController> ChildPartsListCopy = new List<AbstractPartsController>(ChildPartsList);
        //まとまりを作る
        ConnectSituationBreathFirstSearch(ChildPartsListCopy);
        //まとまりに応じて場合分け
        RecordConnectSituation();
    }
    /// <summary>
    /// パーツレベルのリンクを取得して横型探索を行い、小破以下のまとまりを把握し、まとまりを記録
    /// </summary>
    private void ConnectSituationBreathFirstSearch(List<AbstractPartsController> ChildPartsListCopyRef)
    {
        while (ChildPartsListCopyRef.Count > 0)
        {
            Debug.LogAssertion("aaaa" + ChildPartsListCopyRef.Count);
            AbstractPartsController APC = ChildPartsListCopyRef[0];
            GSetting.PartsDamageStatus? partsDamageStatus = APC.GetPartsDamageStatus();
            //小破・無傷以外（登録忘れ含む）なら探索対象から除外
            if (!(partsDamageStatus == GSetting.PartsDamageStatus.NoDamage || partsDamageStatus == GSetting.PartsDamageStatus.MinorDamage))
            {
                ChildPartsListCopyRef.Remove(APC);
                //大破以外＝未登録orNULLならログ表示
                if (!(partsDamageStatus == GSetting.PartsDamageStatus.HeavilyDamage))
                {
                    GSetting.RefineDebugAssertinLog(APC.transform, "損傷状況の登録情報がおかしい" + partsDamageStatus);
                }
                continue;
            }
            //小破及び無傷の場合＝まとまりに分ける
            //スタックを定義、種を格納
            Stack<AbstractPartsController> stack = new Stack<AbstractPartsController>();
            stack.Push(APC);
            //まとまり一つをリストとして格納を進めていくためにリストを定義
            List<AbstractPartsController> Parts = new List<AbstractPartsController>();
            //=今回のループで使うリストにスタックの種になるパーツを格納
            Parts.Add(APC);
            //ローカルの方のコピーリストから探索済みパーツを削除する
            ChildPartsListCopyRef.Remove(APC);
            Debug.LogAssertion(APC);
            //while文。スタックが空になるまで続ける
            while (stack.Count > 0)
            {
                //パーツのリンクのリストを取得する
                AbstractPartsController popAPC = stack.Pop();
                List<AbstractPartsController> partsLink = new List<AbstractPartsController>();
                Debug.LogAssertion(popAPC.GetPassiveJointLinkList().Count);
                Debug.LogAssertion(popAPC.GetActiveJointLinkList().Count);
                foreach (JointUnit joint in popAPC.GetPassiveJointLinkList())
                {
                    AbstractPartsController jointedAP = joint.GetJointedAnotherPart();
                    if (jointedAP != null) partsLink.Add(jointedAP);
                }
                foreach (JointUnit joint in popAPC.GetActiveJointLinkList())
                {
                    AbstractPartsController jointedAP = joint.GetJointedAnotherPart();
                    if (jointedAP != null) partsLink.Add(jointedAP);
                }
                Debug.LogAssertion(partsLink.Count);
                //リストを走査
                foreach (AbstractPartsController part in partsLink)
                {
                    GSetting.RefineDebugAssertinLog(part.transform, "");
                    //未探索　かつ　損傷が小破及び無傷　なら
                    if (!part.GetSearchFlag() && (part.GetPartsDamageStatus() == GSetting.PartsDamageStatus.NoDamage || part.GetPartsDamageStatus() == GSetting.PartsDamageStatus.MinorDamage))
                    {
                        //スタックにリンクを格納する
                        stack.Push(part);
                        Debug.LogAssertion(part);
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
            //while文終了。
            Debug.LogAssertion("aaaa" + ChildPartsListCopyRef.Count);
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
    /// <summary>
    /// まとまりの種類に応じて接続状況を分類
    /// </summary>
    private void RecordConnectSituation()
    {
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
    /// <summary>
    /// 引数で指定したパーツのまとまりを分離させる
    /// </summary>
    /// <param name="APCList"></param>
    public void PartsExtract(List<AbstractPartsController> APCList)
    {
        //分離時の初期設定
        //ReDUMSのオブジェクトを作って配下にパーツをまとめて追加すればOK
        GameObject RegeneDUMSObj = Instantiate(ReDUMSObj);
        foreach (AbstractPartsController Part in APCList)
        {
            Part.transform.parent = RegeneDUMSObj.transform;
        }
        RegeneDUMSObj.GetComponent<RefineDestroyedUnitManagementScript>().InitialSetting();
        //分離元から消去する
        ChildPartsList.RemoveAll(x => APCList.Contains(x));
        PartsGroupList.Remove(APCList);
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
            List<JointUnit> activeJointList = Part.GetActiveJointLinkList();
            foreach (JointUnit jointUnit in activeJointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    return Part;
                }
            }
        }
        GSetting.RefineDebugAssertinLog(transform, "ActiveJointUnitが見つからない");
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0){Destroy(gameObject);}
        //rb2D.velocity = new Vector3(InstMoveAndRotateVector.x,InstMoveAndRotateVector.y,0);
        //transform.Rotate(0,0,InstMoveAndRotateVector.z);
    }
}

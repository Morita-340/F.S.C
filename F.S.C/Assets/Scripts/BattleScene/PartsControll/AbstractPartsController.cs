using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// 機能ごとにパーツを分けて管理する
/// パーツを構成するUnitはここで管理する
/// このクラス自体はReAUDMSやReAUAMSに管理してもらう
/// 攻撃や分離処理などの中間地点
/// </summary>
public class AbstractPartsController : MonoBehaviour
{
    [SerializeField,ReadOnly]
    protected int numOfInitialJointUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfInitialFunctionUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfInitialChildUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfJointUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfFunctionUnit = 1;
    [SerializeField,ReadOnly]
    protected int numOfChildUnit = 1;
    /// <summary>
    /// 初期生成時のジョイントユニットと機能ユニットのみを記録したいので（再生成時には記録したくない）、フラグを設定。
    /// </summary>
    protected bool initialFlag = true;
    /// <summary>
    /// パーツのまとまりを作る際の探索で使う
    /// </summary>
    protected bool searchFlag = false;
    [SerializeField, ReadOnly]
    protected GSetting.PartsDamageStatus DamageStatus = GSetting.PartsDamageStatus.NotSet;
    [SerializeField, ReadOnly]
    protected RefineAbstractUnitDestroyManagementScript ReAUDMS;
    protected DestroyedUnitManagementScript DUMS;
    protected GSetting.ObjTagName childObjTagName;
    protected bool caluculateFlag = false;
    /// <summary>
    /// Previewとして表示させる際に使う。Previewの見た目をなるべく本物に寄せたいので、見た目（どの向きなのか確認）と配置（Playerと被っていないか）だけを流用する
    /// </summary>
    protected bool previewFlag = false;
    /// <summary>
    /// 子オブジェクトのユニット全て
    /// </summary>
    protected List<UnitData> ChildUnitDataList = new List<UnitData>();
    protected List<UnitData> NotResearchUnitList = new List<UnitData>();
    protected List<UnitData> RegeneUnitList = new List<UnitData>();
    /// <summary>
    /// 受動ジョイントユニット側の
    /// </summary>
    protected List<JointUnit> PassiveJointLink = new List<JointUnit>();
    protected List<JointUnit> ActiveJointLink = new List<JointUnit>();
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ReAUDMS = gameObject.transform.parent.GetComponent<RefineAbstractUnitDestroyManagementScript>();
        DUMS = gameObject.transform.parent.GetComponent<DestroyedUnitManagementScript>();
        if (ReAUDMS != null)
        {
            childObjTagName = ReAUDMS.GetChildObjTagName();
        }
        if (DUMS != null)
        {
            childObjTagName = GSetting.ObjTagName.DestroyedUnit;
        }
        SetUnitData();
        AnalysisDamageStatus();
    }
    /// <summary>
    /// 再生成時の処理諸々をまとめた
    /// </summary>
    public void RegeneProcess()
    {
        initialFlag = false;
        SetUnitData();
        AnalysisDamageStatus();
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    protected virtual void SetUnitData()
    {
        ChildUnitDataList.Clear();
        //Debug.Log("HHH"+gameObject.transform.childCount);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            //Debug.Log("GGG" + childUnitObject.tag+childObjTagName.ToString());
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                ChildUnitDataList.Add(ChildUnit.GetThisUnitData());
            }
        }
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
    }
    /// <summary>
    /// Preview状態で表示（SpriteRendererを弄って薄緑にする）
    /// </summary>
    /// <returns></returns>
    public GameObject SetPreview()
    {
        //各UnitをPreview設定とする
        foreach (UnitData child in ChildUnitDataList)
        {
            if (CheckPartsCoveredPlayer())
            {
                //SpriteRendererを取得して被っていないなら薄緑に
                child.ReturnThisUnit().GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.3f);
            }
            else
            {
                //SpriteRendererを取得して被っているなら薄赤に
                child.ReturnThisUnit().GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
            }
        }
        //Previewなので諸々の処理を止める
        previewFlag = true;
        //攻撃しない（ReDUMSからは攻撃命令を出さない）
        //被弾しない（タグがDestroyedUnitならUnit側ですり抜けるよう処理する）
        //コライダーそのものをOFFにする
        ChildrenSColliderEnabled(false);
        return gameObject;
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのコライダーを有効にするか無効にするかを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    public void ChildrenSColliderEnabled(bool flag){
        if (ChildUnitDataList.Count != 0)
        {
            foreach (UnitData unitData in ChildUnitDataList)
            {
                GameObject childObj = unitData.ReturnThisUnit().gameObject;
                BoxCollider2D boxCollider2D = childObj.gameObject.GetComponent<BoxCollider2D>();
                boxCollider2D.enabled = flag;
            }
        }
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのスプライトを半透明にするか否かを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    public void ChildrenSpriteTranslucent(bool flag){
        if(ChildUnitDataList.Count != 0){
        foreach(UnitData unitData in ChildUnitDataList){
            GameObject childObj = unitData.ReturnThisUnit().gameObject;
            SpriteRenderer spriteRenderer = childObj?.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.green;
            if(flag){
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50/255f);
                if(childObj.GetComponent<WeaponUnitBase>()){
                    SpriteRenderer WeaponEfficiencyUISR = childObj.GetComponent<SpriteRenderer>();
                    WeaponEfficiencyUISR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50/255f);
                    }
                }
            else{
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                if(childObj.GetComponent<WeaponUnitBase>()){
                    SpriteRenderer WeaponEfficiencyUISR = childObj.GetComponent<SpriteRenderer>();
                    WeaponEfficiencyUISR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                    }
                }
            }
        }
    }
    /// <summary>
    /// パーツがPreview状態の時、各々のUnitがPlayerと被っていないかを判定
    /// </summary>
    /// <returns></returns>
    public bool CheckPartsCoveredPlayer()
    {
        //全てのUnitの座標へRayを飛ばして被っていないか判定
        foreach (UnitData child in ChildUnitDataList)
        {
            GameObject childObj = child.ReturnThisUnit().gameObject;
            RaycastHit2D hit2D = Physics2D.Raycast(childObj.transform.position, new Vector3(0, 0, 1));
            if (hit2D.collider.tag == GSetting.ObjTagName.PlayerUnit.ToString())
            {
                return true;
            }
        }
        return false;
    }
    /*以下損傷状況の把握処理**************************************/
    private void AnalysisDamageStatus()
    {
        CountNOJU();
        CountNOFU();
        CountNOCU();
        RecordDamageStatus();
    }
    /// <summary>
    /// numOfJointUnitを数える
    /// </summary>
    private void CountNOJU()
    {
        if (initialFlag)
        {
            numOfInitialJointUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                UnitBase childUnit = transform.GetChild(i).GetComponent<UnitBase>();
                if (childUnit is JointUnit)
                {
                    numOfInitialJointUnit++;
                    //Debug.LogAssertion("FFF"+childUnit);
                    if (childUnit is PassiveJointUnit)
                    {
                        PassiveJointLink.Add(childUnit as JointUnit);
                    }
                    if (childUnit is ActiveJointUnit)
                    {
                        ActiveJointLink.Add(childUnit as JointUnit);
                    }
                }
            }
            numOfJointUnit = numOfInitialJointUnit;
        }
        else
        {
            numOfJointUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                UnitBase childUnit = transform.GetChild(i).GetComponent<UnitBase>();
                if (childUnit is JointUnit)
                {
                    numOfJointUnit++;
                    if (childUnit is PassiveJointUnit)
                    {
                        PassiveJointLink.Add(childUnit as JointUnit);
                    }
                    if (childUnit is ActiveJointUnit)
                    {
                        ActiveJointLink.Add(childUnit as JointUnit);
                    }
                }
            }
        }
    }
    private void CountNOCU() {
        if (initialFlag)
        {
            numOfInitialChildUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<UnitBase>())
                {
                    numOfInitialChildUnit ++;
                }
            }
            numOfChildUnit = numOfInitialChildUnit;
        }
        else
        {
            numOfChildUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<UnitBase>())
                {
                    numOfChildUnit++;
                }
            }
        }
    }
    /// <summary>
    /// numOfFunctionUnitを数える。FunctionUnitにどのユニットが該当するかはパーツによって異なるため各自継承して記入が必要
    /// </summary>
    protected virtual void CountNOFU()
    {
    }
    public void SetSearchFlag(bool flag)
    {
        searchFlag = flag;
    }
    public bool GetSearchFlag()
    {
        return searchFlag;
    }
    /// <summary>
    /// 損傷状況を記録する
    /// </summary>
    private void RecordDamageStatus()
    {
        if (numOfFunctionUnit == 0 || numOfJointUnit == 0)
        {
            DamageStatus = GSetting.PartsDamageStatus.HeavilyDamage;
        }
        else if (numOfChildUnit == numOfInitialChildUnit)
        {
            DamageStatus = GSetting.PartsDamageStatus.NoDamage;
        }
        else
        {
            DamageStatus = GSetting.PartsDamageStatus.MinorDamage;
        }
    }
    public GSetting.PartsDamageStatus GetPartsDamageStatus()
    {
        return DamageStatus;
    }
    public List<JointUnit> GetPassiveJointLinkList()
    {
        return PassiveJointLink;
    }
    public List<JointUnit> GetActiveJointLinkList()
    {
        return ActiveJointLink;
    }
    /*以下分離処理**************************************/
    /// <summary>
    /// APCのDestroyProcess
    /// </summary>
    /// <param name="DeleteData"></param>
    /// <param name="inputIsDead"></param>
    public virtual void DestroyProcess(UnitData DeleteData, bool inputIsDead)
    {
        //Debug.Log("GGG" +childObjTagName.ToString());
        ChildUnitDataList.Remove(DeleteData);
        ReAUDMS.DestroyProcess(DeleteData, inputIsDead);
    }
    /// <summary>
    /// パーツに含まれるデータを削除する
    /// </summary>
    /// <param name="unitData"></param>
    public void DeleteChildUnitData(UnitData unitData)
    {
        GameObject deleteObj = unitData.ReturnThisUnit().gameObject;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (deleteObj == transform.GetChild(i).gameObject)
            {
                DestroyImmediate(unitData.ReturnThisUnit().gameObject);
                ChildUnitDataList.Remove(unitData);
                NotResearchUnitList.Remove(unitData);
                return;
            }
        }
        GSetting.RefineDebugAssertinLog(transform,"データがこのオブジェクトの中に無い");
    }
    public List<UnitData> GetNotResearchUnitList()
    {
        return NotResearchUnitList;
    }
    public List<UnitData> GetRegeneUnitList()
    {
        return RegeneUnitList;
    }
    /// <summary>
    /// 子オブジェクトのUnitについて、全て探索結果がflagであるかどうか判定
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="listNo">0:ChildUnitDataList
    ///                      1:NotResearchUnitList
    ///                      2:RegeneUnitList</param>
    /// <returns></returns>
    public bool AllUnitSearched(bool flag, int listNo)
    {
        List<UnitData> DataList = new List<UnitData>();
        switch (listNo)
        {
            case 0:DataList = ChildUnitDataList; break;
            case 1:DataList = NotResearchUnitList; break;
            case 2:DataList = RegeneUnitList; break;
            default:Debug.LogAssertion("無効な数値が入力されている："+listNo);break;
        }
        foreach (UnitData unitData in DataList)
        {
            if (unitData.AlreadySearch != flag) { return false; }
        }
        return true;
    }
    public void SetRegeneUnitList()
    {
        RegeneUnitList.Clear();
        foreach (UnitData unitData in NotResearchUnitList)
        {
            if (unitData.AlreadySearch == true)
            {
                //コアを再生成対象にしてはならない
                if (unitData.ReturnThisUnit() is CoreBase) { continue; }
                //GSetting.RefineDebugAssertinLog(unitData.ReturnThisUnit().gameObject.transform, "意図しないオブジェクトが紛れてるかも");
                RegeneUnitList.Add(unitData);
            }
        }
    }
    public void SetNotResearchUnitList()
    {
        NotResearchUnitList.Clear();
        foreach (UnitData unitData in ChildUnitDataList)
        {
            if (unitData.AlreadySearch == false)
            {
                NotResearchUnitList.Add(unitData);
            }
        }
    }
    public virtual int CaluculateCombatPower()
    {
        int combatPower = 0;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            AttackUnit AU = childUnitObject.GetComponent<AttackUnit>();
            WeaponControllUnitBase WCUB = childUnitObject.GetComponent<WeaponControllUnitBase>();
            ReactorBase reactorBase = childUnitObject.GetComponent<ReactorBase>();
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                if (AU != null)
                {
                    combatPower += AU.GetUnitStatus();
                }
                else if (WCUB != null)
                {
                    combatPower += WCUB.GetUnitStatus();
                }
                else if (reactorBase != null)
                {
                    combatPower += reactorBase.CaluculateReactorEffect() + reactorBase.GetUnitStatus();
                }
            }
        }
        return combatPower;
    }
    public virtual void DeleteAllRangeMesh()
    {
        foreach (UnitData childUnitData in ChildUnitDataList)
        {
            if (childUnitData == null)
            {
                Debug.LogWarning("AAA");
                continue;
            }
            if (childUnitData.ReturnThisUnit() == null)
            {
                Debug.LogWarning("AAA");
                continue;
            }
            if (childUnitData.ReturnThisUnit() is CoreBase coreBase)
            {
                coreBase.DestroyFRMesh();
            }
            if (childUnitData.ReturnThisUnit() is WeaponUnitBase weaponUnitBase)
            {
                weaponUnitBase.DestroyFRMesh();
            }
        }
    }
    /*以下攻撃処理**************************************/
    [SerializeField, ReadOnly] protected Vector3 TargetPosition;
    public void SetTargetPosition(Vector3 inputTargetPosion)
    {
        TargetPosition = inputTargetPosion;
        foreach (UnitData child in ChildUnitDataList)
        {
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (child.ReturnThisUnit() is AttackUnit AU)
            {
                AU.SetTargetPosition(TargetPosition);
            }
        }
    }
    public virtual void NormalAttack(Vector3 inputTargetPosion){
        if(childObjTagName == GSetting.ObjTagName.DestroyedUnit){ GSetting.RefineDebugAssertinLog(transform,"DestroyedUnitは攻撃しない"); return;}
        foreach (UnitData child in ChildUnitDataList)
        {
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (child.ReturnThisUnit() is AttackUnit AU)
            {
                StartCoroutine(AU.NormalAttack(TargetPosition));
            }
        }
    }
}

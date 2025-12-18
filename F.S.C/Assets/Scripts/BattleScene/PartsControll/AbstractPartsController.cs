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
    [SerializeField, ReadOnly]
    protected int numOfInitialJointUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfInitialFunctionUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfInitialChildUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfJointUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfFunctionUnit = 1;
    [SerializeField, ReadOnly]
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
    [SerializeField, ReadOnly] protected RefineDestroyedUnitManagementScript ReDUMS;
    [SerializeField, ReadOnly] protected GSetting.ObjTagName childObjTagName;
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
    protected List<PassiveJointUnit> PassiveJointLink = new List<PassiveJointUnit>();
    /// <summary>
    /// ActiveJointは如何なるパーツに対しても一つのみ（合体対象の合体向きの候補が増えてしまって合体までの操作数が増えてしまうのは、操作テンポを悪くしかねないため）
    /// </summary>
    protected ActiveJointUnit ActiveJointLink;
    protected int PartsAttackPower = 0;
    protected int PartsHP = 0;
    private void NotifyThisIsPreviewPart()
    {
        if (previewFlag) { Debug.LogAssertion("previewなのにこのメソッドを呼び出すな"); }
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (previewFlag) return;
        SetPartSetting();
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
    public void CaptureProcess()
    {
        SetPartSetting();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.GetComponent<UnitBase>().UnitSetting(tag, gameObject.layer);
        }
        SetUnitData();
    }
    private void SetPartSetting()
    {
        ReAUDMS = gameObject.transform.parent.GetComponent<RefineAbstractUnitDestroyManagementScript>();
        ReDUMS = gameObject.transform.parent.GetComponent<RefineDestroyedUnitManagementScript>();
        if (ReAUDMS != null)
        {
            childObjTagName = ReAUDMS.GetChildObjTagName();
            tag = childObjTagName.ToString();
            if (ReAUDMS is RefineEnemyUnitDestroyManagementScript)
            {
                gameObject.layer = (int)GSetting.UniqueLayerName.EnemyUnit;
            }
            if (ReAUDMS is RefinePlayerUnitDestroyManagementScript)
            {
                //Debug.LogAssertion(childObjTagName + transform.parent.name);
                gameObject.layer = (int)GSetting.UniqueLayerName.PlayerUnit;
            }
        }
        if (ReDUMS != null)
        {
            childObjTagName = GSetting.ObjTagName.DestroyedUnit;
            Debug.LogAssertion(childObjTagName);
            tag = childObjTagName.ToString();
            gameObject.layer = (int)GSetting.UniqueLayerName.DestroyedUnit;
        }
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    protected virtual void SetUnitData()
    {
        ChildUnitDataList.Clear();
        GSetting.RefineDebugAssertinLog(transform, gameObject.transform.childCount.ToString());
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            //Debug.Log("GGG" + childUnitObject.tag+childObjTagName.ToString());
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                ChildUnitDataList.Add(ChildUnit.GetThisUnitData());
                ChildUnit.ReRegistData();
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
        //Previewなので諸々の処理を止める
        previewFlag = true;
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
            //tagを変更（合体演出用）
            child.ReturnThisUnit().gameObject.tag = GSetting.ObjTagName.SimulateUnit.ToString();
            //Colliderをistriggerに（衝突判定は無効にしたいが、Rayに当たってほしい）
            child.ReturnThisUnit().gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        tag = GSetting.ObjTagName.SimulateUnit.ToString();
        return gameObject;
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのコライダーを有効にするか無効にするかを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    public void ChildrenSColliderEnabled(bool flag)
    {
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
    public void ChildrenSpriteTranslucent(bool flag)
    {
        NotifyThisIsPreviewPart();
        if (ChildUnitDataList.Count != 0)
        {
            foreach (UnitData unitData in ChildUnitDataList)
            {
                GameObject childObj = unitData.ReturnThisUnit().gameObject;
                SpriteRenderer spriteRenderer = childObj?.gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = Color.green;
                if (flag)
                {//半透明
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50 / 255f);
                    if (childObj.GetComponent<WeaponUnitBase>())
                    {
                        SpriteRenderer WeaponEfficiencyUISR = childObj.GetComponent<SpriteRenderer>();
                        WeaponEfficiencyUISR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50 / 255f);
                    }
                }
                else
                {//透けない
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                    if (childObj.GetComponent<WeaponUnitBase>())
                    {
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
                        PassiveJointLink.Add(childUnit as PassiveJointUnit);
                    }
                    if (childUnit is ActiveJointUnit)
                    {
                        ActiveJointLink = childUnit as ActiveJointUnit;
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
                        PassiveJointLink.Add(childUnit as PassiveJointUnit);
                    }
                    if (childUnit is ActiveJointUnit)
                    {
                        ActiveJointLink = childUnit as ActiveJointUnit;
                    }
                }
            }
        }
    }
    private void CountNOCU()
    {
        if (initialFlag)
        {
            numOfInitialChildUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<UnitBase>())
                {
                    numOfInitialChildUnit++;
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
        NotifyThisIsPreviewPart();
        searchFlag = flag;
    }
    public bool GetSearchFlag()
    {
        NotifyThisIsPreviewPart();
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
        NotifyThisIsPreviewPart();
        return DamageStatus;
    }
    public List<PassiveJointUnit> GetPassiveJointLinkList()
    {
        NotifyThisIsPreviewPart();
        return PassiveJointLink;
    }
    public ActiveJointUnit GetActiveJointLink()
    {
        return ActiveJointLink;
    }
    public void SetA_JointLineGuide()
    {

    }
    public void ReloadJointUnitLink()
    {
        NotifyThisIsPreviewPart();
        foreach (JointUnit p_jointUnit in PassiveJointLink)
        {
            p_jointUnit.ReloadLink();
        }
        ActiveJointLink.ReloadLink();
    }
    /*以下分離処理**************************************/
    /// <summary>
    /// APCのDestroyProcess
    /// </summary>
    /// <param name="DeleteData"></param>
    /// <param name="inputIsDead"></param>
    public virtual void DestroyProcess(UnitData DeleteData, bool inputIsDead)
    {
        NotifyThisIsPreviewPart();
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
        NotifyThisIsPreviewPart();
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
        GSetting.RefineDebugAssertinLog(transform, "データがこのオブジェクトの中に無い");
    }
    public List<UnitData> GetNotResearchUnitList()
    {
        NotifyThisIsPreviewPart();
        return NotResearchUnitList;
    }
    public List<UnitData> GetRegeneUnitList()
    {
        NotifyThisIsPreviewPart();
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
        NotifyThisIsPreviewPart();
        List<UnitData> DataList = new List<UnitData>();
        switch (listNo)
        {
            case 0: DataList = ChildUnitDataList; break;
            case 1: DataList = NotResearchUnitList; break;
            case 2: DataList = RegeneUnitList; break;
            default: Debug.LogAssertion("無効な数値が入力されている：" + listNo); break;
        }
        foreach (UnitData unitData in DataList)
        {
            if (unitData.AlreadySearch != flag) { return false; }
        }
        return true;
    }
    public void SetRegeneUnitList()
    {
        NotifyThisIsPreviewPart();
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
        NotifyThisIsPreviewPart();
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
        SetPartSetting();
        NotifyThisIsPreviewPart();
        int combatPower = 0;
        PartsAttackPower = 0;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            AttackUnit AU = childUnitObject.GetComponent<AttackUnit>();
            WeaponControllUnitBase WCUB = childUnitObject.GetComponent<WeaponControllUnitBase>();
            ReactorBase reactorBase = childUnitObject.GetComponent<ReactorBase>();
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                //Debug.LogAssertion(combatPower);
                if (AU != null)
                {
                    combatPower += AU.GetUnitStatus();
                    PartsAttackPower += AU.GetUnitAttackPower();
                    PartsHP += AU.GetHP();
                }
                else if (WCUB != null)
                {
                    combatPower += WCUB.GetUnitStatus();
                    PartsHP += WCUB.GetUnitStatus();
                }
                else if (reactorBase != null)
                {
                    combatPower += reactorBase.CaluculateReactorEffect() + reactorBase.GetUnitStatus();
                    PartsHP += reactorBase.GetUnitStatus();
                }
            }
        }
        return combatPower;
    }
    public virtual void DeleteAllRangeMesh()
    {
        NotifyThisIsPreviewPart();
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
    public bool SetTargetPosition(Vector3 inputTargetPosion, RaycastHit2D enemyHit2D)
    {
        NotifyThisIsPreviewPart();
        bool rockONFlag = false;
        TargetPosition = inputTargetPosion;
        foreach (UnitData child in ChildUnitDataList)
        {
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (child.ReturnThisUnit() is AttackUnit AU)
            {
                bool flag = AU.SetTargetPosition(TargetPosition, enemyHit2D);
                if (flag) { rockONFlag = true; }
            }
        }
        return rockONFlag;
    }
    public virtual void NormalAttack(Vector3 inputTargetPosion)
    {
        NotifyThisIsPreviewPart();
        if (childObjTagName == GSetting.ObjTagName.DestroyedUnit) { GSetting.RefineDebugAssertinLog(transform, "DestroyedUnitは攻撃しない"); }
        foreach (UnitData child in ChildUnitDataList)
        {
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (child.ReturnThisUnit() is AttackUnit AU)
            {
                StartCoroutine(AU.NormalAttack(TargetPosition));
            }
        }
    }
    public int GetAttackPower()
    {
        return PartsAttackPower;
    }
    public int GetHP()
    {
        return PartsHP;
    }
}

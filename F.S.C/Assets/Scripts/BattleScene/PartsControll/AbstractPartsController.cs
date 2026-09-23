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
    protected int numOfInitialFunctionUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfInitialChildUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfFunctionUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfChildUnit = 1;
    /// <summary>
    /// 初期生成時のジョイントユニットと機能ユニットのみを記録したいので（再生成時には記録したくない）、フラグを設定。
    /// </summary>
    protected bool initialFlag = true;
    [SerializeField, ReadOnly]
    protected RefineAbstractUnitDestroyManagementScript ReAUDMS;
    [SerializeField, ReadOnly] protected GSetting.ObjTagName childObjTagName;
    /// <summary>
    /// Previewとして表示させる際に使う。Previewの見た目をなるべく本物に寄せたいので、見た目（どの向きなのか確認）と配置（Playerと被っていないか）だけを流用する
    /// </summary>
    protected bool previewFlag = false;
    protected bool caluculateFlag = false;
    /// <summary>
    /// 子オブジェクトのユニット全て
    /// </summary>
    protected List<UnitData> ChildUnitDataList = new List<UnitData>();
    protected List<AttackUnit> ChildAttackUnitList = new List<AttackUnit>();
    [SerializeField, ReadOnly]
    protected int PartsAttackPower = 0;
    [SerializeField, ReadOnly]
    protected int PartsHP = 0;
    [SerializeField, ReadOnly]
    protected GSetting.UniqueLayerName LayerName;
    private MaterialPropertyBlock block;
    [SerializeField, ReadOnly] protected Color outlineColor = Color.white;
    /// <summary>
    /// 武装UIに渡す用
    /// </summary>
    [SerializeField] protected Sprite PartIconSprite;
    /// <summary>
    /// アウトラインの描画に用いる
    /// </summary>
    private static readonly int MaskColorId = Shader.PropertyToID("_MaskColor");
    protected void NotifyThisIsPreviewPart()
    {
        if (previewFlag) { Debug.LogAssertion("previewなのにこのメソッドを呼び出すな"); }
    }

    private void OnEnable()
    {
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (ChildUnitDataList == null) return;

        foreach (UnitData unitData in ChildUnitDataList)
        {
            SpriteRenderer SR = unitData.ReturnThisUnit().gameObject.GetComponent<SpriteRenderer>();
            if (SR == null) continue;
            SR.GetPropertyBlock(block);
            block.SetColor(MaskColorId, outlineColor);
            SR.SetPropertyBlock(block);
        }
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetPartSetting();
        SetUnitData();
        initialFlag = true;
        AnalysisPartStatus();
        block = new MaterialPropertyBlock();
        ApplyColor();
    }
    /// <summary>
    /// ReAUDMSが空なので、継承先で適宜代入しておく
    /// </summary>
    protected virtual void SetPartSetting()
    {
        if (ReAUDMS != null)
        {
            childObjTagName = ReAUDMS.GetChildObjTagName();
            tag = childObjTagName.ToString();
            if (ReAUDMS is RefineEnemyUnitDestroyManagementScript)
            {
                LayerName = GSetting.UniqueLayerName.EnemyUnit;
                outlineColor = Color.red;
                gameObject.layer = (int)LayerName;
            }
            if (ReAUDMS is RefinePlayerUnitDestroyManagementScript)
            {
                //Debug.LogAssertion(childObjTagName + transform.parent.name);
                LayerName = GSetting.UniqueLayerName.PlayerUnit;
                outlineColor = Color.green;
                gameObject.layer = (int)LayerName;
            }
        }
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納して子オブジェクト側の初期設定も行っておく関数
    /// </summary>
    protected virtual void SetUnitData()
    {
        ChildUnitDataList.Clear();
        ChildAttackUnitList.Clear();
        //GSetting.RefineDebugAssertinLog(transform, gameObject.transform.childCount.ToString());
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            //Debug.Log("GGG" + childUnitObject.tag+childObjTagName.ToString());
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                if (ChildUnit is AttackUnit)
                {
                    AttackUnit AU = ChildUnit as AttackUnit;
                    ChildAttackUnitList.Add(AU);
                    Debug.Log(ChildUnit.GetThisUnitData());
                }
                ChildUnitDataList.Add(ChildUnit.GetThisUnitData());
                ChildUnit.ReRegistData();
            }
        }
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
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
    /*以下損傷状況の把握処理**************************************/
    protected virtual void AnalysisPartStatus()
    {
        CountNOFU();
        CountNOCU();
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
    public void SetA_JointLineGuide()
    {

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
        ReAUDMS?.DestroyProcess(DeleteData, inputIsDead);
    }
    public virtual int CaluculateCombatPower()
    {
        SetPartSetting();
        NotifyThisIsPreviewPart();
        int combatPower = 0;
        PartsAttackPower = 0;
        bool primeUnitHPAdded = false;
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
                    if (AU.GetIsPrime() && primeUnitHPAdded) continue;
                    //GSetting.RefineDebugAssertinLog(transform, AU.GetAttackPower().ToString());
                    combatPower += AU.GetUnitStatus();
                    PartsAttackPower += AU.GetAttackPower();
                    PartsHP += AU.GetHP();
                    if (AU.GetIsPrime()) primeUnitHPAdded = true;
                }
                else if (WCUB != null)
                {
                    combatPower += WCUB.GetUnitStatus();
                    PartsHP += WCUB.GetUnitStatus();
                    if (WCUB.GetIsPrime()) primeUnitHPAdded = true;
                }
                else if (reactorBase != null)
                {
                    combatPower += reactorBase.CaluculateReactorEffect() + reactorBase.GetUnitStatus();
                    PartsHP += reactorBase.GetUnitStatus();
                    if (reactorBase.GetIsPrime()) primeUnitHPAdded = true;
                }
            }
        }
        return combatPower;
    }
    /// <summary>
    /// パーツに含まれるデータを削除する
    /// </summary>
    /// <param name="unitData"></param>
    public virtual void DeleteChildUnitData(UnitData unitData)
    {
        NotifyThisIsPreviewPart();
        GameObject deleteObj = unitData.ReturnThisUnit().gameObject;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (deleteObj == transform.GetChild(i).gameObject)
            {
                DestroyImmediate(unitData.ReturnThisUnit().gameObject);
                ChildUnitDataList.Remove(unitData);
                return;
            }
        }
        GSetting.RefineDebugAssertinLog(transform, "データがこのオブジェクトの中に無い");
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
        //Listの操作による影響を受けない
        foreach (Transform TF in transform)
        {
            UnitBase unitBase = TF.GetComponent<UnitBase>();
            if (unitBase is AttackUnit AU)
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
        if (childObjTagName == GSetting.ObjTagName.DestroyedUnit) { GSetting.RefineDebugAssertinLog(transform, "DestroyedUnitは攻撃しない"+childObjTagName.ToString()); }
        //Listの操作による影響を受けない
        foreach (Transform TF in transform)
        {
            UnitBase unitBase = TF.GetComponent<UnitBase>();
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (unitBase is AttackUnit AU)
            {
                Debug.Log("LLLL");
                StartCoroutine(AU.NormalAttack(TargetPosition));
            }
        }
    }
    public bool InRange()
    {
        foreach (Transform TF in transform)
        {
            UnitBase unitBase = TF.GetComponent<UnitBase>();
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (unitBase is AttackUnit AU)
            {
                if(AU.InRange()){return true;}
            }
        }
        return false;
    }
    public int GetAttackPower()
    {
        return PartsAttackPower;
    }
    public int GetHP()
    {
        return PartsHP;
    }
    public void SetInvincible(bool flag)
    {
        foreach (UnitData childUnitData in ChildUnitDataList)
        {
            childUnitData.ReturnThisUnit().SetInvincible(flag);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using System;
/// <summary>
/// ジョイントユニットがある合体と分離が可能なパーツ
/// </summary>
public class ReversibleConnectionPartsController : AbstractPartsController
{
    [SerializeField, ReadOnly]
    protected int numOfInitialJointUnit = 1;
    [SerializeField, ReadOnly]
    protected int numOfJointUnit = 1;
    /// <summary>
    /// パーツのまとまりを作る際の探索で使う
    /// </summary>
    protected bool searchFlag = false;
    [SerializeField, ReadOnly]
    protected GSetting.PartsDamageStatus DamageStatus = GSetting.PartsDamageStatus.NotSet;
    /// <summary>
    /// 受動ジョイントユニット側の
    /// </summary>
    [SerializeField, ReadOnly]
    protected List<PassiveJointUnit> PassiveJointLink = new List<PassiveJointUnit>();
    /// <summary>
    /// ActiveJointは如何なるパーツに対しても一つのみ（合体対象の合体向きの候補が増えてしまって合体までの操作数が増えてしまうのは、操作テンポを悪くしかねないため）
    /// </summary>
    [SerializeField, ReadOnly]
    protected ActiveJointUnit ActiveJointLink;
    protected List<UnitData> NotResearchUnitList = new List<UnitData>();
    protected List<UnitData> RegeneUnitList = new List<UnitData>();
    [SerializeField, ReadOnly] protected RefineDestroyedUnitManagementScript ReDUMS;
    // Start is called before the first frame update
    protected override void Start()
    {
        if (previewFlag) return;
        base.Start();
    }
    /// <summary>
    /// 再生成時の処理諸々をまとめた
    /// </summary>
    public void RegeneProcess()
    {
        initialFlag = false;
        SetUnitData();
        AnalysisPartStatus();
    }
    protected override void SetPartSetting()
    {
        ReAUDMS = gameObject.transform.parent.GetComponent<RefineAbstractUnitDestroyManagementScript>();
        base.SetPartSetting();
        ReDUMS = gameObject.transform.parent.GetComponent<RefineDestroyedUnitManagementScript>();
        if (ReDUMS != null)
        {
            childObjTagName = GSetting.ObjTagName.DestroyedUnit;
            Debug.LogAssertion(childObjTagName);
            tag = childObjTagName.ToString();
            LayerName = GSetting.UniqueLayerName.DestroyedUnit;
            outlineColor = new Color(0.2f, 0.5f, 0.2f);
            gameObject.layer = (int)LayerName;
        }
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
    /// <summary>
    /// numOfJointUnitを数える
    /// </summary>
    private void CountNOJU()
    {
        PassiveJointLink.Clear();
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
    protected override void AnalysisPartStatus()
    {
        base.AnalysisPartStatus();
        CountNOJU();
        RecordDamageStatus();
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
    /// <summary>
    /// パーツに含まれるデータを削除する
    /// </summary>
    /// <param name="unitData"></param>
    public override void DeleteChildUnitData(UnitData unitData)
    {
        base.DeleteChildUnitData(unitData);
        //GameObject deleteObj = unitData.ReturnThisUnit().gameObject;
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    if (deleteObj == transform.GetChild(i).gameObject)
        //    {
        NotResearchUnitList.Remove(unitData);
        return;
        //    }
        //}
        //GSetting.RefineDebugAssertinLog(transform, "データがこのオブジェクトの中に無い");
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
            //格納すべきユニットを格納できてない
            if (unitData.AlreadySearch == false)
            {
                //GSetting.RefineDebugAssertinLog(unitData.ReturnThisUnit().transform,"A");
                NotResearchUnitList.Add(unitData);
            }
        }
    }
    public virtual GameObject DuplicateParentOnly(Vector3 basePosition, Transform parentTransform)
    {
        GameObject newParent = new GameObject(gameObject.name);
        //面倒くさいが、https://chatgpt.com/c/6aaee639-c78c-83e8-904f-60ae4ecb4a4b
        //↑こっちで行く
        //if (gameObject.GetComponent<BodyPartsController>())
        //{
        //    BodyPartsController BPC = newParent.AddComponent<BodyPartsController>();
        //    BPC = gameObject.GetComponent<BodyPartsController>();
        //}
        //else
        //{
        //    AbstractPartsController APC = newParent.AddComponent<AbstractPartsController>();
        //    APC = gameObject.GetComponent<AbstractPartsController>();
        //}
        newParent.AddComponent(GetType());
        if (this is PartConnectToUI_IF PC2UI_before)
        {
            Debug.LogAssertion("AAABBB");
            PartConnectToUI_IF PC2UI_after = newParent.GetComponent<PartConnectToUI_IF>();
            PC2UI_after.SetPartIcon(PC2UI_before.GetThisPartIcon());
        }
        
        /*
        // 新しい空オブジェクトを生成
        GameObject newParent = new GameObject(originalParent.name);
        */

        newParent.transform.parent = parentTransform;
        //分離一個目のパーツからの相対位置を取得
        //Transformに反映
        newParent.transform.localPosition = gameObject.transform.localPosition - basePosition;
        // 元のTransform情報をコピー
        newParent.transform.rotation = gameObject.transform.rotation;
        newParent.transform.localScale = gameObject.transform.localScale;

        /*
        // 元の親に付いているコンポーネントをコピー
        foreach (var component in originalParent.GetComponents<Component>())
        {
            if (component is Transform) continue; // Transformは除外
            if (component == null) continue;
            newParent.AddComponent(component);
        }
        */

        return newParent;
    }
}

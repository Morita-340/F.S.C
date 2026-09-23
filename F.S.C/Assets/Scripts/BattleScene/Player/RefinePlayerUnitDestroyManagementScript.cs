using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
public class PartSlot
{
    private bool slotFlag;
    private bool integrated;
    private PassiveJointUnit passiveJointUnit;
    private AbstractPartsController APC;
    public PartSlot(bool slotFlag, bool integrated, PassiveJointUnit passiveJointUnit, AbstractPartsController APC)
    {
        this.slotFlag = slotFlag;
        this.integrated = integrated;
        this.passiveJointUnit = passiveJointUnit;
        this.APC = APC;
    }
    public bool SlotFlag()
    {
        return slotFlag;
    }
    public bool Integrated()
    {
        return integrated;
    }
    public PartConnectToUI_IF GetPart()
    {
        PartConnectToUI_IF part = null;
        if (slotFlag)
        {
            if (integrated)
            {
                part = APC as PartConnectToUI_IF;
            }
            else
            {
                part = passiveJointUnit.GetJointedAnotherPart() as PartConnectToUI_IF;
            }
        }
        else
        {
            part = null;
        }
        return part;
    }
    public void ReloadSlot()
    {
        if (slotFlag && !integrated)
        {
            APC = passiveJointUnit.GetJointedAnotherPart();
        }
    }
}
public class RefinePlayerUnitDestroyManagementScript : RefineAbstractUnitDestroyManagementScript
{
    /// <summary>
    /// インスペクター上で設定する必要があるのでクラス化せずに記述
    /// 
    /// </summary>
    [Header("Front")]
    [SerializeField] bool slotFlag_Front;
    [SerializeField] bool Integrated_Front;
    [SerializeField] PassiveJointUnit PassiveJoint_Front;
    [SerializeField] AbstractPartsController Part_Front;
    [Header("FrontRight")]
    [SerializeField] bool slotFlag_FrontRight;
    [SerializeField] bool Integrated_FrontRight;
    [SerializeField] PassiveJointUnit PassiveJoint_FrontRight;
    [SerializeField] AbstractPartsController Part_FrontRight;
    [Header("FrontLeft")]
    [SerializeField] bool slotFlag_FrontLeft;
    [SerializeField] bool Integrated_FrontLeft;
    [SerializeField] PassiveJointUnit PassiveJoint_FrontLeft;
    [SerializeField] AbstractPartsController Part_FrontLeft;
    [Header("Right")]
    [SerializeField] bool slotFlag_Right;
    [SerializeField] bool Integrated_Right;
    [SerializeField] PassiveJointUnit PassiveJoint_Right;
    [SerializeField] AbstractPartsController Part_Right;
    [Header("Left")]
    [SerializeField] bool slotFlag_Left;
    [SerializeField] bool Integrated_Left;
    [SerializeField] PassiveJointUnit PassiveJoint_Left;
    [SerializeField] AbstractPartsController Part_Left;
    [Header("Back")]
    [SerializeField] bool slotFlag_Back;
    [SerializeField] bool Integrated_Back;
    [SerializeField] PassiveJointUnit PassiveJoint_Back;
    [SerializeField] AbstractPartsController Part_Back;
    [Header("BackRight")]
    [SerializeField] bool slotFlag_BackRight;
    [SerializeField] bool Integrated_BackRight;
    [SerializeField] PassiveJointUnit PassiveJoint_BackRight;
    [SerializeField] AbstractPartsController Part_BackRight;
    [Header("BackLeft")]
    [SerializeField] bool slotFlag_BackLeft;
    [SerializeField] bool Integrated_BackLeft;
    [SerializeField] PassiveJointUnit PassiveJoint_BackLeft;
    [SerializeField] AbstractPartsController Part_BackLeft;
    /// <summary>
    /// プレイヤー機体のBodyに接続しているパーツはこちらに格納。小破以下のパーツ同士がくっついているものを合体させた場合はBodyと直接つながっているものだけ格納する
    /// ReAUDMSのPartsList（パーツ全てを格納。）と使い分ける
    /// こちらは主にUIに渡す際に用いる
    /// </summary>
    private PartSlot[] dir8partSlots = new PartSlot[8];
    //ダメージを受けていない状態であるかどうか
    private bool noDamageFlag = true;
    //ダメージを受け付けない（無敵）であるかどうか
    private bool invincible = false;
    private bool touchDamageInterval = false;
    [SerializeField]//挙動把握のためにシリアル化
    protected MachineStatusHUDController MSHUDC;
    public void SetMSHUDC(MachineStatusHUDController MSHUDC)
    {
        this.MSHUDC = MSHUDC;
    }
    protected override void Awake()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        //dir8partSlotsの初期化（格納順はHUDとしてリング状に表示することを考えて輪を描くような順序にしている）
        PartSlot frontSlot = new PartSlot(slotFlag_Front, Integrated_Front, PassiveJoint_Front, Part_Front);
        dir8partSlots[0] = frontSlot;
        PartSlot frontLeftSlot = new PartSlot(slotFlag_FrontLeft, Integrated_FrontLeft, PassiveJoint_FrontLeft, Part_FrontLeft);
        dir8partSlots[1] = frontLeftSlot;
        PartSlot leftSlot = new PartSlot(slotFlag_Left, Integrated_Left, PassiveJoint_Left, Part_Left);
        dir8partSlots[2] = leftSlot;
        PartSlot BackLeftSlot = new PartSlot(slotFlag_BackLeft, Integrated_BackLeft, PassiveJoint_BackLeft, Part_BackLeft);
        dir8partSlots[3] = BackLeftSlot;
        PartSlot backSlot = new PartSlot(slotFlag_Back, Integrated_Back, PassiveJoint_Back, Part_Back);
        dir8partSlots[4] = backSlot;
        PartSlot backRightSlot = new PartSlot(slotFlag_BackRight, Integrated_BackRight, PassiveJoint_BackRight, Part_BackRight);
        dir8partSlots[5] = backRightSlot;
        PartSlot rightSlot = new PartSlot(slotFlag_Right, Integrated_Right, PassiveJoint_Right, Part_Right);
        dir8partSlots[6] = rightSlot;
        PartSlot frontRightSlot = new PartSlot(slotFlag_FrontRight, Integrated_FrontRight, PassiveJoint_FrontRight, Part_FrontRight);
        dir8partSlots[7] = frontRightSlot;
        base.Awake();
    }
    public override void SetUnitData()
    {
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        base.SetUnitData();
        //
        foreach (PartSlot partSlot in dir8partSlots)
        {
            partSlot.ReloadSlot();
        }
        MSHUDC.SetPart(dir8partSlots);
    }
    public List<Vector3> GetAllEmptyPassiveJointSPositionList()
    {
        List<Vector3> jointUnitSPositionList = new List<Vector3>();
        //Debug.LogAssertion(PartsList.Count);
        foreach (AbstractPartsController APC in PartsList)
        {
            if (!(APC is ReversibleConnectionPartsController)) { continue; }
            ReversibleConnectionPartsController RCPC = APC as ReversibleConnectionPartsController;
            List<PassiveJointUnit> passivejointList = RCPC.GetPassiveJointLinkList();
            foreach (JointUnit jointUnit in passivejointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    Vector3 EmptyPassiveJointSPosition = jointUnit.GetEmptyLinkPosition();
                    jointUnitSPositionList.Add(EmptyPassiveJointSPosition);
                }
            }
        }
        //Debug.LogAssertion("jointUnitSPositionList.Count" + jointUnitSPositionList.Count);
        return jointUnitSPositionList;
    }
    public List<PassiveJointUnit> GetSelectedPassiveJointList()
    {
        List<PassiveJointUnit> EmptyPassiveJointList = new List<PassiveJointUnit>();
        //Debug.LogAssertion(PartsList.Count);
        foreach (AbstractPartsController APC in PartsList)
        {
            if (!(APC is ReversibleConnectionPartsController)) { continue; }
            ReversibleConnectionPartsController RCPC = APC as ReversibleConnectionPartsController;
            List<PassiveJointUnit> passivejointList = RCPC.GetPassiveJointLinkList();
            //Debug.LogAssertion(passivejointList.Count);
            foreach (PassiveJointUnit jointUnit in passivejointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    EmptyPassiveJointList.Add(jointUnit);
                }
            }
        }
        return EmptyPassiveJointList;
    }
    /// <summary>
    /// 無敵時間なら被弾しない
    /// </summary>
    /// <param name="decreaseValue">ダメージによるHP減少量</param>
    public override void DecreasePrimeUnitsHP(int decreaseValue, bool unitToUnitTouchDamageMode, float interval, bool ratio)
    {
        if (invincible)
        {

        }
        else
        {
            //衝突ダメージのみインターバルを設ける
            if (unitToUnitTouchDamageMode)
            {
                if (touchDamageInterval)
                {

                }
                else
                {
                    StartCoroutine(TemporaryTouchDamageInvincibility(decreaseValue, unitToUnitTouchDamageMode, interval, ratio));
                }
            }
            else
            {
                base.DecreasePrimeUnitsHP(decreaseValue, unitToUnitTouchDamageMode, interval, ratio);
                //一定時間無敵になる
                StartCoroutine(TemporaryInvincibility(0.1f));
            }
        }
    }
    private IEnumerator TemporaryTouchDamageInvincibility(int decreaseValue, bool unitToUnitTouchDamageMode, float interval, bool ratio)
    {
        touchDamageInterval = true;
        base.DecreasePrimeUnitsHP(decreaseValue, unitToUnitTouchDamageMode, interval, ratio);
        yield return new WaitForSeconds(interval);
        touchDamageInterval = false;
    }
    /// <summary>
    /// 一定時間無敵になる
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator TemporaryInvincibility(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
    public override int CaluculateCombatPower()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData, bool inputIsDead)
    {
        noDamageFlag = false;
        base.DestroyProcess(DeleteData, inputIsDead);
        SetUnitData();
    }
    /// <summary>
    /// HP回復　初期値以上には増えない
    /// </summary>
    /// <param name="RepairHP"></param>
    public void RepairMachine(int RepairHP)
    {
        int val = primeUnitsHP += RepairHP;
        if (val > initPrimeUnitHP)
        {
            primeUnitsHP = initPrimeUnitHP;
        }
        else
        {
            primeUnitsHP = val;
        }
    }
    public int GetAttackPower()
    {
        return attackPower;
    }
    public void noDamageFlagReset()
    {
        noDamageFlag = true;
    }
    public bool GetNoDamageFlag()
    {
        return noDamageFlag;
    }
    public void SetInvincible(bool flag)
    {
        invincible = flag;
        foreach (AbstractPartsController APC in PartsList)
        {
            APC.SetInvincible(flag);
        }
    }
    public PartSlot[] GetPartSlotList()
    {
        return dir8partSlots;
    }
}


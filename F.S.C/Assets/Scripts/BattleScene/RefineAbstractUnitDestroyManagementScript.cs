using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// AUDMSのリファイン版。
/// AUDMS→Unitで管理していたが、複数のUnitをまとめて一つの機能として動かせるようにしたくなった。そこでAPCという役割を追加し、ReAUDMS→APC→Unitという構造で動かす
/// ユニットを機能ごとにパーツという形でオブジェクトにまとめて管理するため、それに合うように作り直し
/// </summary>
public class RefineAbstractUnitDestroyManagementScript : MonoBehaviour
{
    //インスペクター上での操作があるためSerializeFieldで登録しておかなければならない
    [SerializeField]
    protected GameObject ThisGameObject;
    [SerializeField]
    private CoreBase ThisUnitSCore;
    //分離処理時の分離したユニットの親オブジェクト
    [SerializeField]
    GameObject DestroyParentUnitObject;
    [SerializeField] protected bool Setting = false;
    [SerializeField, ReadOnly]
    MainCameraController MCC;
    protected GSetting.ObjTagName childObjTagName;
    [SerializeField, Range(1, 100)]
    protected int primeUnitsHP = 10;
    [SerializeField, ReadOnly]
    protected int combatPower = 0;
    [SerializeField, ReadOnly]
    protected int attackPower = 0;
    protected bool isDead = false;
    /// <summary>
    /// 起動後最初のUpdateが呼ばれたタイミングでのみ処理を行えるようにフラグを用意した。combatpowerにリアクターの効果が初めて乗るのがメインスレッドのStart関数ではなくUpdate関数なので、UI表記をちゃんとするためにこれが必要
    /// </summary>
    protected bool initialUpdate = true;
    protected bool caluculateFlag = false;
    [SerializeField,ReadOnly]
    protected List<AbstractPartsController> PartsList = new List<AbstractPartsController>();
    //横型探索用のスタック用リスト
    private Stack<UnitData> StackForBreathFirstSearch = new Stack<UnitData>();
    //探索フラグ解除用のスタックデータのコピー（幅優先探索の終了条件はスタックを空にすることなので、探索終了後に対象ユニット探索フラグを解除するためにもう一度アクセスする必要がある）
    List<UnitData> StackCopy = new List<UnitData>();
    float time = 0;
    [SerializeField] protected SoundController SCer;
    protected virtual void Awake()
    {
        ThisGameObject = gameObject;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        MCC = GameObject.Find("Main Camera")?.GetComponent<MainCameraController>();
        SetUnitData();
    }
    protected virtual void Update()
    {
        if (initialUpdate)
        {
            initialUpdate = false;
            CaluculateCombatPower();
        }
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    public virtual void SetUnitData()
    {
        //if (this is RefineEnemyUnitDestroyManagementScript) GSetting.RefineDebugAssertinLog(transform, "AAAAAA");
        //オブジェクト構造が変わったため変更
        PartsList.Clear();
        //パーツリストを取得（各パーツがそれぞれの子オブジェクトであるユニットを取得しているのでこれでいい）
        for (int i = 0; i < ThisGameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            AbstractPartsController childParts = childUnitObject.GetComponent<AbstractPartsController>();
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                PartsList.Add(childParts);
                childParts.RegeneProcess();
            }
        }
        combatPower = CaluculateCombatPower();
    }
    /// <summary>
    /// 破壊時に呼び出されるDestroyProcess
    /// </summary>
    /// <param name="DeleteData"></param>
    /// <param name="inputIsDead">コアの破壊により機体が撃墜されたかどうかを識別</param>
    public virtual void DestroyProcess(UnitData DeleteData,bool inputIsDead)
    {
        //探索が届かなかったユニットAlreadySearch = falseのユニットは分離対象となる
        UnitBreathFirstSearch(ThisUnitSCore.GetThisUnitData());
        MCC.ExplosionShake(0.3f, 0.2f);
        Regenerate(inputIsDead);
        //破壊音を再生
        SCer.PlaySE(0);
        //DeleteAllRangeMesh();
        StackCopy.Clear();
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
    }
    //幅優先探索の処理(リンクの繋がっているユニットの洗い出し)
    /*ここのAlreadySearchがおかしい！*/
    private void UnitBreathFirstSearch(UnitData SerachStartUnit)
    {
        StackForBreathFirstSearch.Push(SerachStartUnit);
        SerachStartUnit.AlreadySearch = true;
        Debug.LogWarning("AAC" + SerachStartUnit.ReturnThisUnit().gameObject.name);
        StackCopy.Add(SerachStartUnit);
        while (StackForBreathFirstSearch.Count > 0)
        {
            UnitData PopData = StackForBreathFirstSearch.Pop();
            DataAddToDoubleList(PopData.ReturnFourWayLink());
        }
    }
    /*ここのAlreadySearchがおかしい！*/
    private void DataAddToDoubleList(List<UnitData> unitDataList)
    {
        foreach (UnitData unitData in unitDataList)
        {
            if (unitData == null || unitData.ReturnThisUnit() == null) { continue; }//ここにDebug.Logを挟まないこと。処理のスパイクが発生します
            if (unitData.AlreadySearch == true) { continue; }
            StackForBreathFirstSearch.Push(unitData);
            Debug.LogWarning("AAB" + unitData.ReturnThisUnit().gameObject.name);
            unitData.AlreadySearch = true;
            StackCopy.Add(unitData);
        }
    }
    protected virtual List<GameObject> Regenerate(bool inputIsDead)
    {
        //再生成するパーツ（破損無破損関係なし）を格納するリスト。未探索ユニットとその親のパーツ制御者だけで構成される
        List<AbstractPartsController> NotResearchPartsList = new List<AbstractPartsController>();
        //後でUI制御などに渡す用。再生成した全てのオブジェクトのリスト
        List<GameObject> ParentObjectList = new List<GameObject>();
        bool regeneFirstTime = true;
        Vector3 UnitDefferenceVector = new Vector3(0, 0, 0);
        Vector3 regenePosition = new Vector3(0, 0, 0);
        //各パーツ制御者に、分離する＝探索がfalseなユニットを親オブジェクトごと丸々渡してもらう
        foreach (AbstractPartsController Parts in PartsList)
        {
            Debug.LogWarning("FFF"+Parts.AllUnitSearched(true,0)+Parts.gameObject.name);
            //falseになったユニットが存在しない=全てtrueなパーツ=コアと繋がっている=分離対象ではないパーツはあとの処理を飛ばす
            if (Parts.AllUnitSearched(true,0)) { continue; }
            //分離対象が存在するパーツは分離対象のユニットを全てリストにまとめておく
            Parts.SetNotResearchUnitList();
            Debug.LogWarning(Parts.name);
            NotResearchPartsList.Add(Parts);
        }
        ///パーツを構成するユニットが再生成対象かどうか
        while (NotResearchPartsList.Count > 0)
        {
            List<AbstractPartsController> RegenePartsList = new List<AbstractPartsController>();
            //未探索データの中でもう一度探索をすることで、オブジェクトの接続関係でグループに分ける
            UnitData FirstSearchData = NotResearchPartsList[0].GetNotResearchUnitList()[0];
            UnitBreathFirstSearch(FirstSearchData);
            //元々どのパーツに属していたのか
            //パーツそれぞれに対して今回の探索でtrueになったUnitを親オブジェクトごとまとめて渡してもらう
            foreach (AbstractPartsController Parts in NotResearchPartsList)
            {
                //分離対象のうち、今回の探索でtrueになったユニットが存在しないパーツはあとの処理を飛ばす
                if (Parts.AllUnitSearched(false,1)) { continue; }
                Parts.SetRegeneUnitList();
                RegenePartsList.Add(Parts);
                //再生成対象が初めて検出された時のみこの処理を行う
                if (regeneFirstTime == true)
                {
                    //再生成時の回転角及び座標を計算する
                    GameObject unitObj = FirstSearchData.ReturnThisUnit().gameObject;
                    UnitDefferenceVector = new Vector3(unitObj.transform.localPosition.x, unitObj.transform.localPosition.y, 0);
                    //ベクトルだから引き算の計算を逆にしてはいけない
                    regenePosition = new Vector3(unitObj.transform.position.x, unitObj.transform.position.y, 5);
                    regeneFirstTime = false;
                }
            }
            GameObject ParentObject = Instantiate(DestroyParentUnitObject, regenePosition, ThisUnitSCore.transform.rotation).GetComponent<RefineDestroyedUnitManagementScript>().SetColliderAndSpriteONFlag(true);
            //ParentObject.transform.position = ThisUnitSCore.transform.position + regenePosition;//鹵獲時に元のコアまでの距離だけ離れてしまう不具合の修正
            //生成パーツの一つ目の座標を取得して、位置調整に使う
            Vector3 basePosition = RegenePartsList[0].transform.localPosition;
            //元のパーツの親オブジェクトごと再生成
            foreach (AbstractPartsController Parts in RegenePartsList)
            {
                //パーツ制御オブジェクトを生成
                GameObject PartsObj = DuplicateParentOnly(Parts.gameObject,basePosition,ParentObject.transform);
                //ユニットオブジェクトをパーツ制御オブジェクトの下に生成
                List<UnitData> RegeneUnitList = Parts.GetRegeneUnitList();
                PartsObj.transform.localPosition -= UnitDefferenceVector;
                foreach (UnitData unitData in RegeneUnitList)
                {
                    GameObject RegeneObj = Instantiate(unitData.ReturnThisUnit().gameObject,PartsObj.transform).GetComponent<UnitBase>().UnitSetting(GSetting.ObjTagName.DestroyedUnit.ToString(),(int)GSetting.ObjTagName.DestroyedUnit);;
                    //RegeneObj.transform.parent = PartsObj.transform;
                    //複製元の消去（オブジェクトとデータ）
                    unitData.ReturnThisUnit().GetAPC().DeleteChildUnitData(unitData);
                    FieldManager FM = FieldManager.GetInstance();
                    FM.UnitList.Remove(unitData);
                }
                //DuplicateParentOnly()実行時には子オブジェクトは空だったので、改めて初期設定
                AbstractPartsController APC = PartsObj.GetComponent<AbstractPartsController>();
                APC.RegeneProcess();
            }
            //パーツのうち、今回の探索ですべてのUnitがtrueになったものを除外する（APCのDeleteChildUnitData()で既に探索済みかつ再生成済みのオブジェクトはデータもオブジェクトも消去済み）
            foreach (AbstractPartsController Parts in NotResearchPartsList)
            {
                if (Parts.transform.childCount == 0 && !(Parts is BodyPartsController))
                {
                    Debug.LogWarning("AAZ"+Parts.gameObject.name);
                    PartsList.Remove(Parts);
                    Destroy(Parts.gameObject);
                }
            }
            NotResearchPartsList.RemoveAll(AbstractPartsController => AbstractPartsController.AllUnitSearched(true,0) == true);
            StartCoroutine(ParentObject.GetComponent<RefineDestroyedUnitManagementScript>().InitialSetting());
            ParentObject.GetComponent<RefineDestroyedUnitManagementScript>().SetMoveAndRotateVector(transform.position.x,transform.position.y);
            if (ParentObject.transform.childCount <= 0) { DestroyImmediate(ParentObject); }
            else { ParentObjectList.Add(ParentObject); }
        }
        //探索フラグのリセット。
        regeneFirstTime = true;
        foreach (UnitData unitData in StackCopy)
        {
            unitData.AlreadySearch = false;
        }
        return ParentObjectList;
    }
    public GSetting.ObjTagName GetChildObjTagName()
    {
        return childObjTagName;
    }
    /// <summary>
    /// 撃墜された際はこれを呼び出し撃墜判定をtrueにする。これによりゲームフローが進む
    /// </summary>
    public virtual void IsDead()
    {
        //DeleteAllRangeMesh();
        isDead = true;
    }
    /// <summary>
    /// isDeadを参照する際に呼び出す
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead(){
        return isDead;
    }
    public virtual void DeleteAllRangeMesh()
    {
        foreach (AbstractPartsController Parts in PartsList)
        {
            Parts.DeleteAllRangeMesh();
        }
    }
    public virtual List<AbstractPartsController> GetPartsList()
    {
        return PartsList;
    }
    public int GetPrimeUnitsHP()
    {
        return primeUnitsHP;
    }
    public void DecreasePrimeUnitsHP(int decreaseValue)
    {
        primeUnitsHP -= decreaseValue;
    }
    public int GetCombatPower(){
        return combatPower;
    }
    public virtual int CaluculateCombatPower(){
        combatPower = 0;
        attackPower = 0;
        //オブジェクト構造が変わったため変更
        PartsList.Clear();
        //パーツリストを取得（各パーツがそれぞれの子オブジェクトであるユニットを取得しているのでこれでいい）
        for (int i = 0; i < ThisGameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            AbstractPartsController childParts = childUnitObject.GetComponent<AbstractPartsController>();
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                PartsList.Add(childParts);
            }
        }
        //GSetting.RefineDebugAssertinLog(transform,PartsList.Count.ToString());
        //各パーツで計測した戦闘力をそのまま加算
        foreach (AbstractPartsController Parts in PartsList)
        {
            //GSetting.RefineDebugAssertinLog(transform,Parts.CaluculateCombatPower().ToString() +"+"+ Parts.GetAttackPower());
            combatPower += Parts.CaluculateCombatPower();
            attackPower += Parts.GetAttackPower();
            //Debug.Log(combatPower);
        }
        return combatPower;
    }
    /// <summary>
    /// 子オブジェクトは無視して、コンポーネントはコピーしたい
    /// </summary>
    /// <param name="originalParent"></param>
    /// <param name="basePosition"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject DuplicateParentOnly(GameObject originalParent, Vector3 basePosition, Transform parent)
    {
        GameObject newParent = new GameObject(originalParent.name);
        if (originalParent.GetComponent<BodyPartsController>())
        {
            BodyPartsController BPC = newParent.AddComponent<BodyPartsController>();
            BPC = originalParent.GetComponent<BodyPartsController>();
        }
        else
        {
            AbstractPartsController APC = newParent.AddComponent<AbstractPartsController>();
            APC = originalParent.GetComponent<AbstractPartsController>();
        }
        /*
        // 新しい空オブジェクトを生成
        GameObject newParent = new GameObject(originalParent.name);
        */

        newParent.transform.parent = parent;
        //分離一個目のパーツからの相対位置を取得
        //Transformに反映
        newParent.transform.localPosition = originalParent.transform.localPosition - basePosition;
        // 元のTransform情報をコピー
        newParent.transform.rotation = originalParent.transform.rotation;
        newParent.transform.localScale = originalParent.transform.localScale;

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

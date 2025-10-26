using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

/// <summary>
/// ユニット全体の破壊処理もとい子オブジェクト全体から抽出したデータはここで管理する。子オブジェクトのデータはここで取得できる
/// </summary>
public class AbstractUnitDestroyManagementScript : MonoBehaviour
{
    //インスペクター上での操作があるためSerializeFieldで登録しておかなければならない
    [SerializeField]
    protected GameObject ThisGameObject;
    [SerializeField]
    private CoreBase ThisUnitSCore;
    [SerializeField]
    GameObject DestroyParentUnitObject;
    [SerializeField]protected bool Setting = false;
    MainCameraController MCC;
    protected GSetting.ObjTagName childObjTagName;
    protected int maximumDistanseFromCore = 0;
    [SerializeField,Range(1, 100)]
    protected int primeUnitsHP = 10;
    [SerializeField,ReadOnly]
    protected int combatPower = 0;
    protected bool isDead = false;
    /// <summary>
    /// 起動後最初のUpdateが呼ばれたタイミングでのみ処理を行えるようにフラグを用意した。combatpowerにリアクターの効果が初めて乗るのがメインスレッドのStart関数ではなくUpdate関数なので、UI表記をちゃんとするためにこれが必要
    /// </summary>
    protected bool initialUpdate = true;
    protected bool caluculateFlag = false;
    protected List<UnitData> ChildUnitDataList = new List<UnitData>();
    //横型探索用のスタック用リスト
    private Stack<UnitData> StackForBreathFirstSearch = new Stack<UnitData>();
    //探索フラグ解除用のスタックデータのコピー（幅優先探索の終了条件はスタックを空にすることなので、探索終了後に対象ユニット探索フラグを解除するためにもう一度アクセスする必要がある）
    List<UnitData> StackCopy = new List<UnitData>();
    float time = 0;
    [SerializeField]protected SoundController SCer;
    protected virtual void Awake(){
        ThisGameObject = this.gameObject;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetUnitData();
        MCC = GameObject.Find("Main Camera")?.GetComponent<MainCameraController>();
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    public virtual void SetUnitData(){
        ChildUnitDataList.Clear();
        for(int i = 0; i < ThisGameObject.transform.childCount; i++){
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            if(childUnitObject.tag == childObjTagName.ToString()){
                ChildUnitDataList.Add(ChildUnit.GetThisUnitData());
                ReloadMaxDistanse(ChildUnit);
            }
        }
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
    }
    /// <summary>
    /// maximumDistanseFromCoreを更新
    /// </summary>
    /// <param name="ChildUnit"></param>
    private void ReloadMaxDistanse(UnitBase ChildUnit){
        int distanseFromCore = ChildUnit.GetDistanseFromCore();
        if(maximumDistanseFromCore < distanseFromCore){
            maximumDistanseFromCore = distanseFromCore;
        }
    }
    /// <summary>
    /// 破壊時に呼び出されるDestroyProcess
    /// </summary>
    /// <param name="DeleteData"></param>
    /// <param name="inputIsDead">コアの破壊により機体が撃墜されたかどうかを識別</param>
    public virtual void DestroyProcess(UnitData DeleteData,bool inputIsDead){
        ChildUnitDataList.Remove(DeleteData);
        UnitBreathFirstSearch(ThisUnitSCore.GetThisUnitData());
        MCC.ExplosionShake(0.3f,0.2f);
        Regenerate(inputIsDead);
        //破壊音を再生
        SCer.PlaySE(0);
        maximumDistanseFromCore = 0;
        for(int i = 0; i < ThisGameObject.transform.childCount; i++){
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            if(childUnitObject.tag == childObjTagName.ToString()){
                ReloadMaxDistanse(ChildUnit);
            }
        }
        //DeleteAllRangeMesh();
        StackCopy.Clear();
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
    }
    //幅優先探索の処理(リンクの繋がっているユニットの洗い出し)
    private void UnitBreathFirstSearch(UnitData SerachStartUnit){
        StackForBreathFirstSearch.Push(SerachStartUnit);
        SerachStartUnit.AlreadySearch = true;
        StackCopy.Add(SerachStartUnit);
        while(StackForBreathFirstSearch.Count > 0){
            UnitData PopData = StackForBreathFirstSearch.Pop();
            DataAddToDoubleList(PopData.ReturnFourWayLink());
        }
    }
    private void DataAddToDoubleList(List<UnitData> unitDataList){
        foreach(UnitData unitData in unitDataList){
            if(unitData == null||unitData.ReturnThisUnit() == null){continue;}//ここにDebug.Logを挟まないこと。処理のスパイクが発生します
            if(unitData.AlreadySearch != false){continue;}
            StackForBreathFirstSearch.Push(unitData);
            unitData.AlreadySearch = true;
            StackCopy.Add(unitData);
        }
    }
    //未探索群の除外と再生成処理
    //名前はあとで適切なものに書き換える
    protected virtual List<GameObject> Regenerate(bool inputIsDead){
        List<UnitData> NotResearchUnitList = new List<UnitData>();
        List<GameObject> ParentObjectList = new List<GameObject>();
        bool regeneFirstTime = true;
        Vector3 UnitDefferenceVector = new Vector3(0,0,0);
        Vector3 regenePosition = new Vector3(0,0,0);
        //foreach内で走査対象のリストを書き換えるとエラーが発生するので注意
        //未探索のユニットを別のリストに再格納
        //探索範囲を子オブジェクトのユニット全体から未探索だったユニットのみに絞っている
        foreach(UnitData unitData in ChildUnitDataList){
            if(unitData.AlreadySearch == false){
                NotResearchUnitList.Add(unitData);
            }
        }
        //未探索のユニットを集めたリスト内で再び探索を行って、リンクで繋がれたまとまり毎に分離させて再生成させる
        while(NotResearchUnitList.Count > 0){
            //一つ選ぶ
            //探索をする
            //除外対象の親オブジェクトを生成（生成座標は破壊されたオブジェクトに依存するようにする）
            GameObject ParentObject = Instantiate(DestroyParentUnitObject,ThisUnitSCore.transform.position,ThisUnitSCore.transform.rotation).GetComponent<DestroyedUnitManagementScript>().SetColliderAndSpriteONFlag(true);
            UnitBreathFirstSearch(NotResearchUnitList[0]);
            //探索済みのユニットを取り出して再生成
            foreach(UnitData unitData in NotResearchUnitList){
                if(unitData.AlreadySearch == true){
                    //再生成処理。走査のタイミングで一度親オブジェクト下で再生成を終えてから親オブジェクトの位置を変更している
                    GameObject unitObj = unitData.ReturnThisUnit().gameObject;
                    if(regeneFirstTime == true){//再生成対象が初めて検出された時のみこの処理を行う
                        UnitDefferenceVector = new Vector3(unitObj.transform.localPosition.x, unitObj.transform.localPosition.y,0);
                        //ベクトルだから引き算の計算を逆にしてはいけない
                        regenePosition = new Vector3(unitObj.transform.position.x - ThisUnitSCore.transform.position.x , unitObj.transform.position.y - ThisUnitSCore.transform.position.y,5);
                        regeneFirstTime = false;
                    }
                    if(unitObj.GetComponent<WeaponControllUnitBase>()){
                        WeaponControllUnitBase WCUB = unitObj.GetComponent<WeaponControllUnitBase>();
                        GameObject RegeneObj = Instantiate(unitObj,ParentObject.transform,false).GetComponent<WeaponControllUnitBase>().UnitSetting(GSetting.ObjTagName.DestroyedUnit.ToString(),(int)GSetting.ObjTagName.DestroyedUnit);
                        RegeneObj.transform.localPosition -= UnitDefferenceVector;
                    }else if(unitObj.GetComponent<WeaponUnitBase>()){
                        WeaponUnitBase weaponUnitBase = unitObj.GetComponent<WeaponUnitBase>();
                        GameObject RegeneObj = Instantiate(unitObj,ParentObject.transform,false).GetComponent<WeaponUnitBase>().UnitSetting(GSetting.ObjTagName.DestroyedUnit.ToString(),(int)GSetting.ObjTagName.DestroyedUnit);
                        RegeneObj.transform.localPosition -= UnitDefferenceVector;
                        RegeneObj.GetComponent<WeaponUnitBase>() .GetThisUnitData().dividable = false;
                    }else{
                        //上記二つ以外のユニットだった場合
                        GameObject RegeneObj = Instantiate(unitObj,ParentObject.transform,false).GetComponent<UnitBase>().UnitSetting(GSetting.ObjTagName.DestroyedUnit.ToString(),(int)GSetting.ObjTagName.DestroyedUnit);
                        RegeneObj.transform.localPosition -= UnitDefferenceVector;
                    }
                    //子オブジェクトが消去されるのでデータリンクも消去する
                    ChildUnitDataList.Remove(unitData);
                    //複製元の消去
                    FieldManager FM = FieldManager.GetInstance();
                    FM.UnitList.Remove(unitData);
                    DestroyImmediate(unitObj);
                }
            }
            NotResearchUnitList.RemoveAll(unitData => unitData.AlreadySearch == true);
            ParentObject.transform.position = ThisUnitSCore.transform.position + regenePosition;//鹵獲時に元のコアまでの距離だけ離れてしまう不具合の修正
            ParentObject.GetComponent<DestroyedUnitManagementScript>().SetMoveAndRotateVector(transform.position.x,transform.position.y);
            regeneFirstTime = true;
            if(ParentObject.transform.childCount <= 0){DestroyImmediate(ParentObject);}
            else{ParentObjectList.Add(ParentObject);}
        }
        
        //探索フラグのリセット。何かのミスで消去出来なかったデータが生まれた場合にここでリセットする
        ChildUnitDataList.RemoveAll(unitData => unitData.AlreadySearch == false);
        if(inputIsDead){IsDead();}
        foreach(UnitData unitData in StackCopy){
            unitData.AlreadySearch = false;
        }
        return ParentObjectList;
    }
    /// <summary>
    /// 撃墜された際はこれを呼び出し撃墜判定をtrueにする。これによりゲームフローが進む
    /// </summary>
    public virtual void IsDead(){
        //DeleteAllRangeMesh();
        isDead = true;
    }
    public virtual void DeleteAllRangeMesh(){
        foreach(UnitData childUnitData in ChildUnitDataList){
            if(childUnitData == null){
                Debug.LogWarning("AAA");
                continue;}
            if(childUnitData.ReturnThisUnit() == null){
                Debug.LogWarning("AAA");
                continue;}
            if(childUnitData.ReturnThisUnit() is CoreBase coreBase){
                coreBase.DestroyFRMesh();
            }
            if(childUnitData.ReturnThisUnit() is WeaponUnitBase weaponUnitBase){
                weaponUnitBase.DestroyFRMesh();
            }
        }
    }
    public void DeleteChildrenDataFromFM(){
        FieldManager FM = FieldManager.GetInstance();
        foreach(UnitData childrenData in ChildUnitDataList){
            FM.UnitList.Remove(childrenData);
        }
    }
    public List<UnitData> GetChildUnitDataList(){
        return ChildUnitDataList;
    }
    public int GetChildNum(){
        return ChildUnitDataList.Count;
    }
    public CoreBase GetUnitCore(){
        return ThisUnitSCore;
    }
    public int GetMaximumDistanseFromCore(){
        return maximumDistanseFromCore;
    }
    public int GetPrimeUnitsHP(){
        return primeUnitsHP;
    }
    public void DecreasePrimeUnitsHP(int decreaseValue){
        primeUnitsHP -= decreaseValue;
    }
    public int GetCombatPower(){
        return combatPower;
    }
    public virtual int CaluculateCombatPower(){
        int combatPower = 0;
        for(int i = 0; i < ThisGameObject.transform.childCount; i++){
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            AttackUnit AU = childUnitObject.GetComponent<AttackUnit>();
            WeaponControllUnitBase WCUB = childUnitObject.GetComponent<WeaponControllUnitBase>();
            ReactorBase reactorBase = childUnitObject.GetComponent<ReactorBase>();
            if(childUnitObject.tag == childObjTagName.ToString()){
                if(AU != null){
                    combatPower += AU.GetUnitStatus();
                    }
                else if(WCUB != null){
                    combatPower += WCUB.GetUnitStatus();
                    }
                else if(reactorBase != null){
                    combatPower += reactorBase.CaluculateReactorEffect() + reactorBase.GetUnitStatus();
                    }
            }
        }
        return combatPower;
    }
    /// <summary>
    /// isDeadを参照する際に呼び出す
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead(){
        return isDead;
    }
    protected void Update(){
        if(initialUpdate){initialUpdate = false;combatPower = CaluculateCombatPower();}
        if(time < 0){time = 0;}
        else{
            time -= Time.deltaTime;
            combatPower = CaluculateCombatPower();}
        if(caluculateFlag){caluculateFlag = false; time ++;}
        if(primeUnitsHP < 0){primeUnitsHP = 0;}
    }
    IEnumerator CaluculateFlagFalseGraceTime(){
        yield return new WaitForSeconds(1f);
        caluculateFlag = false;
    }
    /// <summary>
    /// ダメージを受けるとユニット全体（被弾したユニットをのぞく）にもダメージを受ける
    /// これにより「とりあえず攻撃を当てれば倒せる」ようになる
    /// 10/30追記
    /// 攻撃テンポの向上のために実装したが、機体強化に従って自ずとDPSは上昇する＋スリップダメージによって分離処理に不具合が発生するため廃止
    /// </summary>
    /// <param name="damagedUnit"></param>
    public void DamageStore(UnitData damagedUnit,int damagePoint){
        int storeDamagePoint = damagePoint/3;
        if(storeDamagePoint <= 0){storeDamagePoint = 1;}
        //被弾したユニットがハードユニットならダメージ蓄積はしない
        if(damagedUnit?.ReturnThisUnit() is HardUnit){return;}
        foreach(UnitData unit in ChildUnitDataList){
            //引数のデータが消えていたとしてもダメージ蓄積は行う
            if(damagedUnit != null){
                unit.ReturnThisUnit().TakeDamage(storeDamagePoint);
                continue;
            }
            if(unit != damagedUnit){
                unit.ReturnThisUnit().TakeDamage(storeDamagePoint);
            }
            
        }
    }
}

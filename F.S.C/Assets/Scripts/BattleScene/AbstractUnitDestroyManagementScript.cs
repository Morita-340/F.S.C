using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

public class AbstractUnitDestroyManagementScript : MonoBehaviour
{
    [SerializeField]
    protected GameObject ThisGameObject;
    [SerializeField]
    private UnitBase ThisUnitSCore;
    [SerializeField]
    GameObject DestroyParentUnitObject;
    protected GSetting.ObjTagName childObjTagName ;
    protected List<UnitData> ChildUnitDataList = new List<UnitData>();
    //横型探索用のスタック用リスト
    private Stack<UnitData> StackForBreathFirstSearch = new Stack<UnitData>();
    //探索フラグ解除用のスタックデータのコピー（幅優先探索の終了条件はスタックを空にすることなので、探索終了後に対象ユニット探索フラグを解除するためにもう一度アクセスする必要がある）
    List<UnitData> StackCopy = new List<UnitData>();
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetUnitData();
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    public virtual void SetUnitData(){
        ChildUnitDataList.Clear();
        for(int i = 0; i < ThisGameObject.transform.childCount; i++){
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            if(childUnitObject.tag == childObjTagName.ToString()){
            ChildUnitDataList.Add(childUnitObject.GetComponent<UnitBase>().GetThisUnitData());
            Debug.Log("PUDMS PUDL set" + childUnitObject.name);
            }
        }
        Debug.Log("PUDMS PUDL" + ChildUnitDataList.Count);
    }
    /// <summary>
    /// 破壊時に呼び出されるDestroyProcess
    /// </summary>
    /// <param name="DeleteData"></param>
    public void DestroyProcess(UnitData DeleteData){
        Debug.Log("PUDMS DP");
        ChildUnitDataList.Remove(DeleteData);
        UnitBreathFirstSearch(ThisUnitSCore.GetThisUnitData());
        Regenerate();
        StackCopy.Clear();
    }
    /// <summary>
    /// 分離時に呼び出されるDestroyProcess
    /// </summary>
    public void DestroyProcess(){
        Debug.Log("PUDMS DP");
        UnitBreathFirstSearch(ThisUnitSCore.GetThisUnitData());
        Regenerate();
        StackCopy.Clear();
    }
    //幅優先探索の処理(リンクの繋がっているユニットの洗い出し)
    private void UnitBreathFirstSearch(UnitData SerachStartUnit){
        StackForBreathFirstSearch.Push(SerachStartUnit);
        SerachStartUnit.AlreadySearch = true;
        StackCopy.Add(SerachStartUnit);
        Debug.Log("PUDMS UBFS Stack" + StackForBreathFirstSearch.Count);
        while(StackForBreathFirstSearch.Count > 0){
            UnitData PopData = StackForBreathFirstSearch.Pop();
            Debug.Log("PUDMS UBFS while" + PopData.ReturnThisUnit().name);
            DataAddToDoubleList(PopData.ReturnFourWayLink());
        }
    }
    private void DataAddToDoubleList(UnitData unitData){
        if(unitData == null)return;
        if(unitData.AlreadySearch != false)return;
        StackForBreathFirstSearch.Push(unitData);
        unitData.AlreadySearch = true;
        StackCopy.Add(unitData);
    }
    private void DataAddToDoubleList(List<UnitData> unitDataList){
        foreach(UnitData unitData in unitDataList){
            if(unitData == null){Debug.Log("PUDMS unitData null");continue;}
            if(unitData.AlreadySearch != false){Debug.Log("PUDMS AS true" + unitData.ReturnThisUnit().name);continue;}
            Debug.Log("PUDMS DataAdd2WList" + unitData.ReturnThisUnit().name);
            StackForBreathFirstSearch.Push(unitData);
            unitData.AlreadySearch = true;
            StackCopy.Add(unitData);
        }
    }
    //未探索群の除外と再生成処理
    //名前はあとで適切なものに書き換える
    private void Regenerate(){
        List<UnitData> NotResearchUnitList = new List<UnitData>();
        bool regeneFirstTime = true;
        Vector3 UnitDefferenceVector = new Vector3(0,0,0);
        Vector3 regenePosition = new Vector3(0,0,0);
        Debug.Log("PUDMS PUDL" + ChildUnitDataList.Count);
        //foreach内で走査対象のリストを書き換えるとエラーが発生するので注意
        //未探索のユニットを別のリストに再格納
        //探索範囲を子オブジェクトのユニット全体から未探索だったユニットのみに絞っている
        foreach(UnitData unitData in ChildUnitDataList){
            if(unitData.AlreadySearch == false){
                Debug.Log("PUDMS falseUnit" + unitData.ReturnThisUnit().name );
                NotResearchUnitList.Add(unitData);
            }
        }
        //未探索のユニットを集めたリスト内で再び探索を行って、リンクで繋がれたまとまり毎に分離させて再生成させる
        while(NotResearchUnitList.Count > 0){
            //一つ選ぶ
            //探索をする
            GameObject ParentObject = Instantiate(DestroyParentUnitObject,ThisUnitSCore.transform.position,ThisUnitSCore.transform.rotation);//除外対象の親オブジェクトを生成（生成座標は破壊されたオブジェクトに依存するようにする）
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
                        Debug.Log("PUDMS DeffVec" + UnitDefferenceVector.x + UnitDefferenceVector.y);
                        regeneFirstTime = false;
                    }
                    GameObject RegeneObj = Instantiate(unitObj,ParentObject.transform,false);
                    RegeneObj.tag = GSetting.ObjTagName.DestroyedUnit.ToString();
                    RegeneObj.layer = (int)GSetting.UniqueLayerName.DestroyedUnit;
                    RegeneObj.GetComponent<WeaponUnitBase>().GetThisUnitData().dividable = false;
                    RegeneObj.transform.localPosition -= UnitDefferenceVector;
                    //子オブジェクトが消去されるのでデータリンクも消去する
                    ChildUnitDataList.Remove(unitData);
                    //複製元の消去
                    FieldManager FM = FieldManager.GetInstance();
                    FM.UnitList.Remove(unitData);
                    Destroy(unitObj);
                }
            }

            NotResearchUnitList.RemoveAll(unitData => unitData.AlreadySearch == true);
            ParentObject.transform.position = ThisUnitSCore.transform.position + regenePosition;//鹵獲時に元のコアまでの距離だけ離れてしまう不具合の修正
            regeneFirstTime = true;
            if(ParentObject.transform.childCount <= 0){Destroy(ParentObject);}
            Debug.Log("PUDMS PUDL" + ChildUnitDataList.Count);
            StartCoroutine(WaitTimeForUnregist(1,ParentObject.GetComponent<DestroyedUnitManagementScript>()));
        }
        
        //探索フラグのリセット
        ChildUnitDataList.RemoveAll(unitData => unitData.AlreadySearch == false);
        Debug.Log("PUDMS RG StackCopy" + StackCopy.Count);
        foreach(UnitData unitData in StackCopy){
            unitData.AlreadySearch = false;
        }
    }
    IEnumerator WaitTimeForUnregist(int time,DestroyedUnitManagementScript DUMS){
        Debug.Log("AUDMS WTFU" + DUMS.gameObject.name);
        DUMS.ChildrenSpriteTranslucent(true);
        DUMS.ChildrenSColliderEnabled(false);
        yield return new WaitForSeconds(time);
        //元データのコライダーと透明度を戻す
        DUMS.ChildrenSpriteTranslucent(false);
        DUMS.ChildrenSColliderEnabled(true);
    }
    public void DeleteChildrenDataFromFM(){
        FieldManager FM = FieldManager.GetInstance();
        foreach(UnitData childrenData in ChildUnitDataList){
            Debug.Log("AUDMS DCDFFM");
            FM.UnitList.Remove(childrenData);
        }
    }
    public List<UnitData> GetChildUnitDataList(){
        return ChildUnitDataList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

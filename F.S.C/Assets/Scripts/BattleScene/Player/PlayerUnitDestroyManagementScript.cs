using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;
//直接書いているが、実際は同時に複数個所が破壊されるのでおそらく非同期処理が必要になる
public class PlayerUnitDestroyManagementScript : MonoBehaviour
{
    [SerializeField]
    private GameObject ThisGameObject;
    [SerializeField]
    private UnitBase ThisUnitSCore;
    [SerializeField]
    GameObject DestroyParentUnitObject;
    private List<UnitData> PlayerUnitDataList = new List<UnitData>();
    //横型探索用のスタック用リスト
    private Stack<UnitData> StackForBreathFirstSearch = new Stack<UnitData>();
    //探索フラグ解除用のスタックデータのコピー（幅優先探索の終了条件はスタックを空にすることなので、探索終了後に対象ユニット探索フラグを解除するためにもう一度アクセスする必要がある）
    List<UnitData> StackCopy = new List<UnitData>();
    // Start is called before the first frame update
    void Start()
    {
        SetUnitData();
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    public void SetUnitData(){
        PlayerUnitDataList.Clear();
        for(int i = 0; i < ThisGameObject.transform.childCount; i++){
            GameObject childUnitObject = ThisGameObject.transform.GetChild(i).gameObject;
            if(childUnitObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            PlayerUnitDataList.Add(childUnitObject.GetComponent<UnitBase>().GetThisUnitData());
            Debug.Log("PUDMS PUDL set" + childUnitObject.name);
            }
        }
        Debug.Log("PUDMS PUDL" + PlayerUnitDataList.Count);
    }
    public void DestroyProcess(UnitData DeleteData){
        Debug.Log("PUDMS DP");
        PlayerUnitDataList.Remove(DeleteData);
        UnitBreathFirstSearch();
        Regenerate();
        StackCopy.Clear();
    }
    //幅優先探索の処理(リンクの繋がっているユニットの洗い出し)
    private void UnitBreathFirstSearch(){
        UnitData ThisCoreData = ThisUnitSCore.GetThisUnitData();
        StackForBreathFirstSearch.Push(ThisCoreData);
        ThisCoreData.AlreadySearch = true;
        StackCopy.Add(ThisCoreData);
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
        GameObject ParentObject = Instantiate(DestroyParentUnitObject,ThisUnitSCore.transform.position,ThisUnitSCore.transform.rotation);//除外対象の親オブジェクトを生成（生成座標は破壊されたオブジェクトに依存するようにする）
        List<UnitData> NotResearchUnitList = new List<UnitData>();
        Debug.Log("PUDMS PUDL" + PlayerUnitDataList.Count);
        //foreach内で走査対象のリストを書き換えるとエラーが発生するので注意
        foreach(UnitData unitData in PlayerUnitDataList){
            if(unitData.AlreadySearch == false){
                Debug.Log("PUDMS falseUnit" + unitData.ReturnThisUnit().name );
                NotResearchUnitList.Add(unitData);
                //再生成処理
                GameObject unitObj = unitData.ReturnThisUnit().gameObject;
                //Instantiate(unitObj,unitObj.transform.position,unitObj.transform.rotation,ParentObject.transform).tag = GSetting.ObjTagName.DestroyedUnit.ToString();
                Instantiate(unitObj,ParentObject.transform,false).tag = GSetting.ObjTagName.DestroyedUnit.ToString();
                //複製元の消去
                FieldManager FM = FieldManager.GetInstance();
                FM.UnitList.Remove(unitData);
                Destroy(unitObj);
            }
        }
        if(ParentObject.transform.childCount <= 0){Destroy(ParentObject);}
        //探索フラグのリセット
        PlayerUnitDataList.RemoveAll(unitData => unitData.AlreadySearch == false);
        Debug.Log("PUDMS RG StackCopy" + StackCopy.Count);
        foreach(UnitData unitData in StackCopy){
            unitData.AlreadySearch = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

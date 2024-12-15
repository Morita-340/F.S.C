using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UnitSimulaterにアタッチされるコンポーネントで、子オブジェクト各々鹵獲可能であるかどうかの判定を調べさせてGetIsPlunderable()でInput側にSimulater全体としての判断を返している
/// </summary>
public class PlayerUnitSimulateScript : MonoBehaviour
{
    [SerializeField]
    GameObject ThisUnitSimulater;
    private bool IsPlunderable = false;
    private bool IsNotIsolated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ThisUnitSimulater.transform.childCount != 0){
            //これを消すと設置→分離→設置→分離と繰り返した後にIsNotIsolatedが必ずtrueになってしまい、PlayerUnitから離れた場所に設置できてしまうバグが発生してしまう
            IsNotIsolated = false;
            for(int i = 0; i < ThisUnitSimulater.transform.childCount;i++){
                GameObject PreviewObject = ThisUnitSimulater.transform.GetChild(i).gameObject;
                PreviewUnitManagerScript previewUnitManagerScript = PreviewObject.GetComponent<PreviewUnitManagerScript>();
                if(previewUnitManagerScript.GetIsCovered() == true){
                    Debug.Log("RRR");
                    IsPlunderable = false;
                    break;
                }else{IsPlunderable = true;}
                if(previewUnitManagerScript.GetIsNotIsolated()){IsNotIsolated = true;}
                /*
                if(previewUnitManagerScript.GetIsCovered() == false){
                    IsPlunderable = true;
                    break;
                }else{IsPlunderable = false;}
                */
            }
        }else{IsNotIsolated = false;}
        Debug.Log("PUSS" + IsPlunderable + IsNotIsolated);
        
    }
    public bool Plunderable(){
        return IsPlunderable && IsNotIsolated;
    }
    public bool GetIsPlunderable(){
        return IsPlunderable;
    }
    public void SetIsNotIsolatedFalse(){
        IsNotIsolated = false;
    }
    public List<UnitBase> GetAdjacentUnitList(){
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        if(ThisUnitSimulater.transform.childCount != 0){
            //UnitSimulaterの子オブジェクトそれぞれに対して
            for(int i = 0; i < ThisUnitSimulater.transform.childCount;i++){
                GameObject PreviewObject = ThisUnitSimulater.transform.GetChild(i).gameObject;
                PreviewUnitManagerScript PVUMS = PreviewObject.GetComponent<PreviewUnitManagerScript>();
                List<UnitBase> unitlist = PVUMS.GetAdjacentPlayerUnit();
                //隣接するユニットのリストを渡してもらいAdjacentUnitListに格納しなおす
                if(unitlist.Count != 0){
                    for(int j = 0; j < unitlist.Count; j++){
                        if(unitlist[j]==null){continue;}
                        AdjacentUnitList.Add(unitlist[j]);
                    }
                }
            }
        }
        return AdjacentUnitList;
    }
}

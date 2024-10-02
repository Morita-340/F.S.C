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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ThisUnitSimulater.transform.childCount != 0){
            for(int i = 0; i < ThisUnitSimulater.transform.childCount;i++){
                GameObject PreviewObject = ThisUnitSimulater.transform.GetChild(i).gameObject;
                PreviewUnitManagerScript previewUnitManagerScript = PreviewObject.GetComponent<PreviewUnitManagerScript>();
                if(previewUnitManagerScript.GetIsCovered() == true){
                    Debug.Log("RRR");
                    IsPlunderable = false;
                    break;
                }else{IsPlunderable = true;}
                /*
                if(previewUnitManagerScript.GetIsCovered() == false){
                    IsPlunderable = true;
                    break;
                }else{IsPlunderable = false;}
                */
            }
        }
        Debug.Log("IsPlunderable" + IsPlunderable);
        
    }
    public bool GetIsPlunderable(){
        return IsPlunderable;
    }
}

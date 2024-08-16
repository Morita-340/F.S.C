using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            }
        }
        Debug.Log("IsPlunderable" + IsPlunderable);
        
    }
    public bool GetIsPlunderable(){
        return IsPlunderable;
    }
}

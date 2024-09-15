using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using System.Linq;
using Unity.VisualScripting;
using System;

public class UnitLinkDebugger : MonoBehaviour
{
    [SerializeField]
    GameObject DebugLinkObjIcon;
    private FieldManager FM = FieldManager.GetInstance();
    List<GameObject> IconList = new List<GameObject>();
    Transform pastObject = null;
    Transform currentObject = null;
    GameObject UpperIcon;
    GameObject DownerIcon;
    GameObject RightIcon;
    GameObject LeftIcon;
    //カーソルを当てたオブジェクトのスクリプトを取得
    //そのスクリプトのthisUnitについてLINQを用いてFieldManagerから探す。
    //該当した要素の上下左右のUnitBaseを取得し、そこからオブジェクトを辿ってそのオブジェクトのSpriteRendererにアクセスし、色を変える
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        if(hit2D){
            //Rayを照射した先にあるオブジェクトのクラスを登録
            if(hit2D.collider.tag == "PlayerUnit"){
                currentObject = hit2D.collider.gameObject.transform;
                //カーソルを当てたユニットが格納されたUnitDataを検索する。
                UnitBase HitObj = hit2D.collider.gameObject.GetComponent<UnitBase>();
                //UnitData SameData = (UnitData)FM.UnitList.Where(unitdata => unitdata.ReturnThisUnit() == HitObj);
                UnitData SameData = new UnitData(null,null,null,null,null,0);
                if(HitObj == null){Debug.Log("LLH"); return; }
                SameData = FM.SearchUnit(HitObj);Debug.Log("LLJ"+SameData.ReturnThisUnit() + HitObj);
                Debug.Log("LLG"+(GSetting.ShapeType)SameData.ShapeTypeNum);
                if(SameData.ReturnThisUnit()==null){Debug.LogWarning("UnitData's ThisData is null");}
                if(pastObject != currentObject)DestroyIcon();
                foreach(UnitBase fourwayUnit in SameData.ReturnFourWayLink()){
                    Debug.Log("LLK");
                    if(fourwayUnit != null){
                        Debug.Log("LLN"+fourwayUnit);
                        Debug.Log("LLO");
                        IconList.Add(Instantiate(DebugLinkObjIcon,fourwayUnit.gameObject.transform));
                    }
                }
                pastObject = hit2D.collider.gameObject.transform;
            }else{DestroyIcon();}
        }else{DestroyIcon();}
    }
    void DestroyIcon(){
        foreach(GameObject Icon in IconList){
            Destroy(Icon);
        }
    }
}

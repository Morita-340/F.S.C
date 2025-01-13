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
    [SerializeField]
    GameObject UpperIcon;
    [SerializeField]
    GameObject DownerIcon;
    [SerializeField]
    GameObject RightIcon;
    [SerializeField]
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
        //foreach(RaycastHit2D hit2D in Physics2D.RaycastAll((Vector2)ray.origin, (Vector2)ray.direction)){
            if(hit2D){
                //Rayを照射した先にあるオブジェクトのクラスを登録
                if(hit2D.collider.tag == GSetting.ObjTagName.PlayerUnit.ToString() ||hit2D.collider.tag == GSetting.ObjTagName.EnemyUnit.ToString()){
                    currentObject = hit2D.collider.gameObject.transform;
                    //カーソルを当てたユニットが格納されたUnitDataを検索する。
                    UnitBase HitObj = hit2D.collider.gameObject.GetComponent<UnitBase>();
                    //UnitData SameData = (UnitData)FM.UnitList.Where(unitdata => unitdata.ReturnThisUnit() == HitObj);
                    UnitData SameData = new UnitData(null,0);
                    if(HitObj == null){Debug.Log("LLH"); return; }
                    SameData = FM.SearchUnit(HitObj);
                    Debug.Log("LLJ"+SameData.ReturnThisUnit() + HitObj);
                    Debug.Log("LLG"+(GSetting.ShapeType)SameData.ShapeTypeNum);
                    if(SameData.ReturnThisUnit()==null){Debug.LogWarning("UnitData's ThisData is null");}
                    if(pastObject != currentObject){DestroyIcon();}
                    List<UnitData> FourWayLink = SameData.ReturnFourWayLink();
                    for(int i=0; i < FourWayLink.Count; i++){
                        if(FourWayLink[i] != null){
                            switch(i){
                                case 0: UpperIcon.SetActive(true); UpperIcon.transform.position = FourWayLink[i].ReturnThisUnit().gameObject.transform.position;break;
                                case 1: DownerIcon.SetActive(true); DownerIcon.transform.position = FourWayLink[i].ReturnThisUnit().gameObject.transform.position;break;
                                case 2: RightIcon.SetActive(true); RightIcon.transform.position = FourWayLink[i].ReturnThisUnit().gameObject.transform.position;break;
                                case 3: LeftIcon.SetActive(true); LeftIcon.transform.position = FourWayLink[i].ReturnThisUnit().gameObject.transform.position;break;
                            }
                        }
                    }
                    pastObject = hit2D.collider.gameObject.transform;
                }else{DestroyIcon();}
            }else{DestroyIcon();}
        //}
    }   
    void DestroyIcon(){
        UpperIcon.SetActive(false);
        DownerIcon.SetActive(false);
        RightIcon.SetActive(false);
        LeftIcon.SetActive(false);
    }
}

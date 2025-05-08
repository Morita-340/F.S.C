using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// PlayerがMouseInputで入力するのに対して、EnemyはEnemySearchManagementScriptで入力を行う
/// </summary>
public class EnemySearchManagementScript : MonoBehaviour
{
    [SerializeField,ReadOnly] FanRange SearchRader;
    [SerializeField]
    EnemyUnitMoveManagementScript EUMMS;
    [SerializeField]
    bool isSetting = false;
    CircleCollider2D circleCollider2D;
    /// <summary>
    /// ソナーで発見した周囲のオブジェクトのリスト。重複が無いように格納することで処理を早める
    /// </summary>
    private List<GameObject> DiscoveredObjectList = new List<GameObject>();
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        SearchRader = GetComponent<FanRange>();
        circleCollider2D = this.gameObject.GetComponent<CircleCollider2D>();
        circleCollider2D.radius = SearchRader.GetRangeRadius();
        SearchRader.InitialSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay2D(Collider2D other){
        if(!isSetting){
            if(other.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            timer += Time.deltaTime;
            }
            //Debug.LogWarning(other.gameObject.name);
            if(!SearchRader.InRange(other.transform.position)){
                //Debug.LogWarning("AAAAAA");
                return;}
            foreach(GameObject obj in DiscoveredObjectList){
                //重複していないか確認 重複があるならスルー
                if(other.transform.root == obj.transform.root){
                    //Debug.LogWarning("BBBBBB");
                    return;}
            }
            //このオブジェクトそのものを検知した場合もスルー
            if(other.transform.root == transform.root.gameObject){
                //Debug.LogWarning("CCCCCCC");
                return;}
            if(other.tag == GSetting.ObjTagName.EnemyWeapon1.ToString()){
                //Debug.LogWarning("DDDDDDD");
                return;}
            //リストに格納
            DiscoveredObjectList.Add(other.transform.root.gameObject);
            //リストをステートマシンに渡す
            EUMMS.SetInputObjList(DiscoveredObjectList);
        }
    }
    public void OnTriggerExit2D(Collider2D other){
        if(!isSetting){
        //検知したオブジェクトがリストにあるなら消去
        DiscoveredObjectList.Remove(other.transform.root.gameObject);
        //リストをステートマシンに渡す
        EUMMS?.SetInputObjList(DiscoveredObjectList);}
    }
    public void DestroyRMM(){
        SearchRader.DestroyRMM();
    }
}

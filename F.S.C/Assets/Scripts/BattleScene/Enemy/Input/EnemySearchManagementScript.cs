using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// PlayerがMouseInputで入力するのに対して、EnemyはEnemySearchManagementScriptで入力を行う
/// </summary>
public class EnemySearchManagementScript : MonoBehaviour
{
    [SerializeField]
    FanRange SearchRader;
    [SerializeField]
    EnemyUnitAttackManagementScript EUAMS;
    [SerializeField]
    EnemyUnitMoveManagementScript EUMMS;
    CircleCollider2D circleCollider2D;
    /// <summary>
    /// ソナーで発見した周囲のオブジェクトのリスト。重複が無いように格納することで処理を早める
    /// </summary>
    private List<GameObject> DiscoveredObjectList = new List<GameObject>();
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider2D = this.gameObject.GetComponent<CircleCollider2D>();
        circleCollider2D.radius = SearchRader.GetRangeRadius();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            EUAMS.NormalAttack(timer,other.transform.position);
            timer += Time.deltaTime;
        }
    }
    public void OnTriggerEnter2D(Collider2D other){
        //重複していないか確認 重複があるなら処理を終える
        foreach(GameObject obj in DiscoveredObjectList){
            Debug.Log("VVV"+obj.name);
            if(obj == other.transform.root.gameObject){return;}
        }
        //リストに格納
        DiscoveredObjectList.Add(other.transform.root.gameObject);
        //リストをステートマシンに渡す
        EUMMS.SetInputObjList(DiscoveredObjectList);
    }
    public void OnTriggerExit2D(Collider2D other){
        //リストから消去
        DiscoveredObjectList.Remove(other.transform.root.gameObject);
        //リストをステートマシンに渡す
        EUMMS.SetInputObjList(DiscoveredObjectList);
    }
}

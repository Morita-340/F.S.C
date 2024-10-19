using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using FSCGeneral;

/// <summary>
/// マウスカーソルによる入力はここで管理する
/// </summary>
public class MouseCursorControllScript : MonoBehaviour
{
    [SerializeField]GameObject UnitSimulater;
    SnapToGrid snapToGrid;
    PlayerUnitSimulateScript playerUnitSimulateScript;
    PlayerUnitAttackManagementScript PUAMS;
    [SerializeField]GameObject PreviewUnit;
    [SerializeField]GameObject Unit1;
    [SerializeField]GameObject Unit2;
    [SerializeField]GameObject Unit3;
    [SerializeField]GameObject PlayerUnit;
    [SerializeField]ChargeCursolIconController ChargeIcon;
    GameObject ParentObject = null;
    private Vector3 target;
    private float WheelInput = 0;
    private List<Vector3> positionList = new List<Vector3>();
    private List<Quaternion> rotationList = new List<Quaternion>();
    private List<GameObject> previewObjectList = new List<GameObject>(); 
    private List<GameObject> plunderObjectList  = new List<GameObject>();
    private List<UnitBase> UnitBaseList = new List<UnitBase>();
    private bool lockOnEnemyUnit = false;
    private bool lockOnEnemyUnitLongTime = false;
    private float timer = 0;
        //プレビューオブジェクトを格納する。接合判定に利用する
    //private List<string> CopyObjectNameList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        snapToGrid = UnitSimulater.GetComponent<SnapToGrid>();
        playerUnitSimulateScript = UnitSimulater.GetComponent<PlayerUnitSimulateScript>();
        PUAMS = PlayerUnit.GetComponent<PlayerUnitAttackManagementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("MCCS" + timer);
        WheelInput += Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(Input.mousePosition);
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        Debug.Log("bbbZ" + target);
        ChargeIcon.ChangeCircleRange(timer,PUAMS,target);
        CursorDragClick(WheelInput);
    }
    /// <summary>
    /// Unitを右クリックしたら離すまでドラッグし続ける
    /// </summary>
    void CursorDragClick(float wheelInput){
        //Rayを照射する
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        //ロックオン機能
        if(hit2D && hit2D.collider.tag == GSetting.ObjTagName.EnemyUnit.ToString()){
            PUAMS.NormalAttack(timer,target);
            timer += Time.deltaTime;
        }else{
            timer =0;
        }
        //Debug.Log(hit2D.collider.gameObject.name);
        if(Input.GetMouseButtonDown(0)&&hit2D){
            //Rayを照射した先にあるオブジェクトを登録
            //オブジェクトが撃破後Unitであるなら
            if(hit2D.collider.tag == GSetting.ObjTagName.DestroyedUnit.ToString()/*撃破後のUnitであることを識別できる何かをフラグに持ってくる*/){
                ParentObject = hit2D.collider.gameObject.transform.root.gameObject;
                snapToGrid.Origin = ParentObject.transform;
                //子オブジェクトのlocalPositionとlocalRotationを取得してリストに格納
                for(int i = 0;i < ParentObject.transform.childCount;i++){
                        //Transform child = ParentObject.transform.GetChild(i);
                    Vector3 position = ParentObject.transform.GetChild(i).localPosition;
                    Quaternion rotation = ParentObject.transform.GetChild(i).localRotation;
                    string ObjectName = ParentObject.transform.GetChild(i).name;
                    UnitBase ThisUnitBase = ParentObject.transform.GetChild(i).gameObject.GetComponent<UnitBase>();
                    //position = snapToGrid.SnapPositon(position);
                    //rotation = snapToGrid.SnapRotation(rotation);
                    plunderObjectList.Add(ParentObject.transform.GetChild(i).gameObject);
                    //CopyObjectNameList.Add(ObjectName);
                    positionList.Add(position);
                    rotationList.Add(rotation);
                    UnitBaseList.Add(ThisUnitBase);
                }
                //Debug.Log("bbbA" + UnitSimulater.transform.position);
                //PreviewユニットをSimulaterの子オブジェクトとして生成する
                for(int i = 0;i < positionList.Count;i++){
                    //Instantiateのオーバーライドで引数を別のにすると挙動が変わってしまうので注意
                    GameObject Preview = Instantiate(PreviewUnit,positionList[i],rotationList[i]);
                    Preview.transform.SetParent(UnitSimulater.transform,false);
                    previewObjectList.Add(Preview);
                }
                //Debug.Log("bbbA"+ ParentObject.transform.position);
            }
        }
        if(Input.GetMouseButton(0)){
            Debug.Log("aaa");
            if(ParentObject == null){
                Debug.LogWarning("Clicked Parent Object is null!");
            }
            else{
                Debug.Log("bbbB"+ ParentObject.name + ParentObject.transform.position);
                EnemyUnitManagementScript enemyUnitManagementScript = ParentObject.GetComponent<EnemyUnitManagementScript>();
                if(enemyUnitManagementScript == null){
                    Debug.LogWarning("enemyUnitManagementScript is null" + ParentObject.name);
                }else{
                //親オブジェクトのスクリプトEnemyUnitManagementScriptにアクセス
                enemyUnitManagementScript.MovePosition(target);
                //マウスに追従するようにするMovePosition関数を呼び出す
                enemyUnitManagementScript.Spin(wheelInput,PlayerUnit.transform.rotation);
                //プレビューオブジェクトを走査して
                Debug.Log("ccc");
                }
            }
        }
        if(Input.GetMouseButtonUp(0)){
            WheelInput = 0;
            //Simulaterの複製を削除する
            bool isNotIsolated = false;
            List<UnitBase> AdjacentUnitList = new List<UnitBase>();
            //シミュレート用のプレビューオブジェクトを削除する
            for(int i = 0;i < UnitSimulater.transform.childCount;i++){
                Destroy(UnitSimulater.transform.GetChild(i).gameObject);
            }
            //鹵獲中のユニットについて一つ一つRayを飛ばすために初期値を登録して離れ小島にならないかIsNotIsolatedUnit()で判別している
            for(int i = 0;i < plunderObjectList.Count;i++){
                if(UnitBaseList[i] == null){Debug.Log("KZM"); break;}
                UnitBaseList[i].SetRayBasePosition(previewObjectList[i].transform.position);
                Debug.Log("KZM" + previewObjectList[i].transform.root.rotation.eulerAngles.z);
                UnitBaseList[i].SetPreviewObjRotate(previewObjectList[i].transform.root.rotation.eulerAngles.z);
                Debug.Log("KZM" + UnitBaseList[i] + previewObjectList[i].transform.position + previewObjectList[i].transform.rotation.eulerAngles.z);
                if(UnitBaseList[i].IsNotIsolatedUnit() == true){isNotIsolated = true; Debug.Log("KZM");}
            }
            //DestroyUnitがPlayerUnitに隣接しているかを判定し、隣接しているUnitはAdjacentUnitListに登録
            foreach(UnitBase unitBase in UnitBaseList){
                Debug.Log("GGGA" + unitBase);
                foreach(UnitBase adjacentUnit in unitBase.ReturnAdjacentUnitList()){
                    if(adjacentUnit!=null){AdjacentUnitList.Add(adjacentUnit);
                    Debug.Log("GGGB" + adjacentUnit);}
                    else break;
                }
                //if(AdjacentUnitList.Any(n => n != null)){isNotIsolated = true;}
                //else{isNotIsolated = false;}
            }
            //PlayerUnitに複製する
            Debug.Log("MCCS Plunderable" + playerUnitSimulateScript.GetIsPlunderable() + " isNotIsolated" + isNotIsolated);
            if(playerUnitSimulateScript.GetIsPlunderable() == true && isNotIsolated == true){
                ///for(int i = 0;i < CopyObjectNameList.Count;i++){Debug.Log("ddd");
                //Plunder(CopyObjectNameList[i],previewObjectList[i].transform.position,previewObjectList[i].transform.rotation);
                //}
                for(int i = 0;i < plunderObjectList.Count;i++){
                    Plunder(plunderObjectList[i],previewObjectList[i].transform.position,previewObjectList[i].transform.rotation);
                }
                Debug.Log("EEE");
                Destroy(ParentObject);
                //データの更新にAdjacentUnitListを用いる
                foreach(UnitBase unitBase in AdjacentUnitList){
                    unitBase.ReRegistData();
                    Debug.Log("FFB" + unitBase.name);
                }
                //PUDMSのデータ更新
                PlayerUnitDestroyManagementScript PUDMS = PlayerUnit.GetComponent<PlayerUnitDestroyManagementScript>();
                PUDMS.SetUnitData();
            }else{ParentObject = null;}
            positionList.Clear();
            rotationList.Clear();
            plunderObjectList.Clear();
            //CopyObjectNameList.Clear();
            previewObjectList.Clear();
            UnitBaseList.Clear();
        }
    }
    //void Plunder(string copyName,Vector3 Position, Quaternion Rotation){
    //    Debug.Log("Name" + copyName);
    //    Position = new Vector3(Position.x, Position.y,0);
    //    switch(copyName){
    //        case "Unit1": Instantiate(Unit1,Position,Rotation,PlayerUnit.transform);Debug.Log("YYY"); break;
    //        case "Unit2":Instantiate(Unit2,Position,Rotation,PlayerUnit.transform);Debug.Log("YYY"); break;
    //        case "Unit3":Instantiate(Unit3,Position,Rotation,PlayerUnit.transform);Debug.Log("YYY"); break;
    //    }
    //}
    void Plunder(GameObject PlunderUnit,Vector3 Position, Quaternion Rotation){
        Position = new Vector3(Position.x, Position.y,0);
        Instantiate(PlunderUnit,Position,Rotation,PlayerUnit.transform).tag = GSetting.ObjTagName.PlayerUnit.ToString();
    }
    /// <summary>
    /// マウスホイールでドラッグ中のオブジェクトを回転させる
    /// EnemyUnitManagementScript.Spinで対応させたので現在不要
    /// </summary>
    void CursorRotate(){

    }
}

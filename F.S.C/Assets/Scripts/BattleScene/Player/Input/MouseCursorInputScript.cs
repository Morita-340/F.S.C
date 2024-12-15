using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class MouseCursolInputScript : MonoBehaviour
{
    [SerializeField]GameObject UnitSimulater;
    [SerializeField]GameObject PreviewUnit;
    [SerializeField]GameObject PlayerUnit;
    [SerializeField]GameObject DestroyedUnitStorage;
    [SerializeField]ChargeCursolIconController ChargeIcon;
    [SerializeField,Range(0f,2f)]float firstAttackInterval = 0.5f;
    private PlayerUnitAttackManagementScript PUAMS;
    private PlayerUnitSimulateScript playerUnitSimulateScript;
    private SnapToGrid snapToGrid;
    private Vector3 target;
    private List<GameObject> previewObjectList = new List<GameObject>(); 
    //private List<GameObject> plunderObjectList  = new List<GameObject>();
    private List<UnitBase> UnitBaseList = new List<UnitBase>();
    private List<CaptureUnitData> captureDataList = new List<CaptureUnitData>();
    private float WheelInput = 0;
    private float timer = 0;
    private bool lockOnEnemyUnit = false;
    private bool lockOnEnemyUnitLongTime = false;
    private bool isRegistered = false;
    private int postKeyNum = -1;
    private CaptureUnitData SelectCaptureData = null;

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
        Debug.Log("MCIS" + timer);
        WheelInput += Input.GetAxis("Mouse ScrollWheel");
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        snapToGrid.OriginPosition = target;
        ChargeIcon.ChangeCircleRange(timer + firstAttackInterval,PUAMS,target);
        CursorControll(WheelInput);
    }
    /// <summary>
    /// カーソルを動かすことで入力をする処理まとめ
    /// </summary>
    private void CursorControll(float wheelInput){
        Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        var Hit2DList = RockOn(ray);
        UnitCapture(Hit2DList.Item1,wheelInput);
        Attack(Hit2DList.Item2);
    }
    /// <summary>
    /// カーソルをEnemyUnitとDestroyedUnitにかざすと認識できるようにする
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    private (RaycastHit2D,RaycastHit2D) RockOn(Ray ray){
        bool enemyHitFirst = true;
        bool destroyedHitFirst = true;
        RaycastHit2D DestroyedHit2D = new RaycastHit2D();
        RaycastHit2D EnemyHit2D = new RaycastHit2D();
        foreach(RaycastHit2D hit2D in Physics2D.RaycastAll((Vector2)ray.origin, (Vector2)ray.direction)){
            if(hit2D){
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), hit2D.collider.tag,true)){
                    case GSetting.ObjTagName.EnemyUnit:{
                        if(enemyHitFirst){EnemyHit2D = hit2D;enemyHitFirst = false;}
                        break;}
                    case GSetting.ObjTagName.EnemyWeapon1:{
                        if(enemyHitFirst){EnemyHit2D = hit2D;enemyHitFirst = false;}
                        break;}
                    case GSetting.ObjTagName.DestroyedUnit:{
                        if(destroyedHitFirst){DestroyedHit2D = hit2D;destroyedHitFirst = false;}
                        break;}
                    default:break;
                }
            }else{
                enemyHitFirst = true;
                destroyedHitFirst = true;
            }
            // 両方のヒットがすでに割り当てられたら、ループを終了
            if (DestroyedHit2D && EnemyHit2D)
            {
                Debug.Log("MCCS PlundeAndRockOn");
                break;
            }
        }
        return (DestroyedHit2D,EnemyHit2D);
    }
    /// <summary>
    /// 破壊済みユニットを鹵獲する処理
    /// </summary>
    private void UnitCapture(RaycastHit2D DestroyedHit2D,float wheelInput){
        Debug.Log("MCIS UC" + isRegistered);
        //Rayを照射した瞬間に一度だけDestroyedUnitを登録しPreviewを作成する
        if(DestroyedHit2D){
            if(isRegistered == false){
                List<Vector3> positionList = new List<Vector3>();
                List<Quaternion> rotationList = new List<Quaternion>();
                List<GSetting.ShapeType> shapeTypeList = new List<GSetting.ShapeType>();
                isRegistered = true;
                GameObject ParentObject = DestroyedHit2D.collider.gameObject.transform.parent.gameObject;
                //Previewを動かすために補正対象として登録する
                //snapToGrid.Origin = ParentObject.transform;
                //子オブジェクトのlocalPositionとlocalRotationを取得してリストに格納
                for(int i = 0;i < ParentObject.transform.childCount;i++){
                    Vector3 position = ParentObject.transform.GetChild(i).localPosition;
                    Quaternion rotation = ParentObject.transform.GetChild(i).localRotation;
                    UnitBase ThisUnitBase = ParentObject.transform.GetChild(i).gameObject.GetComponent<UnitBase>();
                    //plunderObjectList.Add(ParentObject.transform.GetChild(i).gameObject);
                    positionList.Add(position);
                    rotationList.Add(rotation);
                    UnitBaseList.Add(ThisUnitBase);
                    shapeTypeList.Add(ThisUnitBase.GetShapeType());
                }
                ParentObject.transform.position = Vector3.zero;
                ParentObject.transform.SetParent(DestroyedUnitStorage.transform);
                CaptureUnitData captureUnitData = new CaptureUnitData(target,ParentObject.transform.rotation.eulerAngles,ParentObject,positionList,rotationList,shapeTypeList);
                captureDataList.Add(captureUnitData);
                //PreviewPositioning(wheelInput, ParentObject,positionList,rotationList);
            }
        //RayがDestroyedUnitに当たっていなければPreviewを削除する
        }else{
            isRegistered = false;
            //DeletePreviewObj();
        }
        UnitSelect(wheelInput);
    }
    private void Attack(RaycastHit2D EnemyHit2D){
        if(EnemyHit2D){
            PUAMS.NormalAttack(timer,target);
            timer += Time.deltaTime;
        }else{
            //カーソルを合わせてもすぐには発射しないようにしてクールタイムを無視した連射を防ぐ
            timer = -firstAttackInterval;
        }
    }
    /// <summary>
    /// 再生成をするためにStorageからDestroyedUnit一つを選ぶ
    /// </summary>
    /// <returns></returns>
    private void UnitSelect(float wheelInput){
        CaptureUnitData EmptyData = new CaptureUnitData(new Vector3(0,0,0),new Vector3(0,0,0),DestroyedUnitStorage,null,null,null);
        SelectCaptureData = SelectCaptureData??EmptyData;
        //右クリック長押し中にマウスホイールを操作すると対象を選べる
        if(Input.GetMouseButton(1) && captureDataList.Count > 0){
            int storageKeyNum = (int)(wheelInput*10) % captureDataList.Count;
            if(storageKeyNum < 0){
                storageKeyNum += captureDataList.Count;
            }
            else if(storageKeyNum > captureDataList.Count -1){
                storageKeyNum -= captureDataList.Count;
            }
            Debug.Log("MCIS US" + storageKeyNum +" " + postKeyNum + " " + wheelInput + " " + SelectCaptureData);
            //プレビューの生成は対象が切り替わったタイミングで一度だけ行う
            if(postKeyNum != storageKeyNum){
                DeletePreviewObj();
                SelectCaptureData = captureDataList[storageKeyNum]??EmptyData;
                if(SelectCaptureData.GetPreviewParentObject() != DestroyedUnitStorage.transform?.GetChild(storageKeyNum)?.gameObject){
                    snapToGrid.OriginRotation = Vector3.zero;
                    captureDataList.Remove(SelectCaptureData);
                }
                PreviewPositioning(wheelInput,SelectCaptureData.GetPreviewParentObject(),SelectCaptureData.GetPreviewUnitPositionList(),SelectCaptureData.GetPreviewUnitRotationList(),SelectCaptureData.GetShapeTypeList());
                postKeyNum = storageKeyNum;
            }
        }
        if(SelectCaptureData != null){
            snapToGrid.OriginRotation = (SelectCaptureData??EmptyData).GetPreviewParentObject().transform.rotation.eulerAngles;
            SelectCaptureData.GetPreviewParentObject()?.GetComponent<DestroyedUnitManagementScript>()?.Spin(wheelInput,PlayerUnit.transform.rotation);
            if(Input.GetMouseButtonDown(0)){
            Regenerate(wheelInput);
            postKeyNum = -1;
            }
        }
    }
    private void DeletePreviewObj(){
        if(UnitSimulater.transform.childCount > 0){
            for(int i = 0;i < UnitSimulater.transform.childCount;i++){
                Destroy(UnitSimulater.transform.GetChild(i).gameObject);
            }
        }
        previewObjectList.Clear();
    }
    /// <summary>
    /// PreviewUnitを生成する
    /// </summary>
    private void PreviewPositioning(float wheelInput,GameObject ParentObject,List<Vector3> positionList,List<Quaternion> rotationList,List<GSetting.ShapeType> shapeTypeList){
        //PreviewユニットをSimulaterの子オブジェクトとして生成する
        playerUnitSimulateScript.SetIsNotIsolatedFalse();
        for(int i = 0;i < positionList.Count;i++){
            //Instantiateのオーバーライドで引数を別のにすると挙動が変わってしまうので注意
            GameObject Preview = Instantiate(PreviewUnit,positionList[i],rotationList[i]);
            PreviewUnitManagerScript PUMS = Preview.GetComponent<PreviewUnitManagerScript>();
            PUMS.SetShapeType(shapeTypeList[i]);
            Preview.transform.SetParent(UnitSimulater.transform,false);
            previewObjectList.Add(Preview);
        }
        /*
        if(ParentObject != null){
            //else{
            //Debug.Log("bbbB"+ ParentObject.name + ParentObject.transform.position);
            DestroyedUnitManagementScript DUMS = ParentObject.GetComponent<DestroyedUnitManagementScript>();
            if(DUMS == null){
                Debug.LogWarning("DestroyedUnitManagementScript is null" + ParentObject.name);
            }else{
            //親オブジェクトのスクリプトDestroyedUnitManagementScriptにアクセス
            DUMS.MovePosition(target);
            //マウスに追従するようにするMovePosition関数を呼び出す
            DUMS.Spin(wheelInput,PlayerUnit.transform.rotation);
            //プレビューオブジェクトを走査して
            Debug.Log("ccc");
            }
        }*/
    }
    /// <summary>
    /// 右クリックを使って鹵獲、PlayerUnitとして再生成する処理
    /// </summary>
    private void Regenerate(float wheelInput){
        Debug.Log("HHHH");
        WheelInput = 0;
        //Simulaterの複製を削除する
        bool isNotIsolated = false;
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        //シミュレート用のプレビューオブジェクトを削除する
        for(int i = 0;i < UnitSimulater.transform.childCount;i++){
            Destroy(UnitSimulater.transform.GetChild(i).gameObject);
        }
        ////鹵獲中のユニットについて一つ一つRayを飛ばすために初期値を登録して離れ小島にならないかIsNotIsolatedUnit()で判別している
        //for(int i = 0;i < plunderObjectList.Count;i++){
        //    if(UnitBaseList[i] == null){Debug.Log("KZM"); break;}
        //    UnitBaseList[i].SetRayBasePosition(previewObjectList[i].transform.position);
        //    Debug.Log("KZM" + previewObjectList[i].transform.root.rotation.eulerAngles.z);
        //    UnitBaseList[i].SetPreviewObjRotate(previewObjectList[i].transform.root.rotation.eulerAngles.z);
        //    Debug.Log("KZM" + UnitBaseList[i] + previewObjectList[i].transform.position + previewObjectList[i].transform.rotation.eulerAngles.z);
        //    if(UnitBaseList[i].IsNotIsolatedUnit() == true){isNotIsolated = true; Debug.Log("KZM");}
        //}
        //PlayerUnitに複製する
        Debug.Log("MCCS Plunderable" + playerUnitSimulateScript.Plunderable());
        if(playerUnitSimulateScript.Plunderable()){
            ///for(int i = 0;i < CopyObjectNameList.Count;i++){Debug.Log("ddd");
            //Plunder(CopyObjectNameList[i],previewObjectList[i].transform.position,previewObjectList[i].transform.rotation);
            //}
            GameObject SelectedUnit = SelectCaptureData.GetPreviewParentObject();
            for(int i = 0;i < SelectedUnit.transform.childCount;i++){
                Debug.Log(SelectedUnit.transform.GetChild(i).gameObject);
                Debug.Log(previewObjectList[i].transform.position);
                Debug.Log(previewObjectList[i].transform.rotation);
                RegenerateAsPlayerUnit(SelectedUnit.transform.GetChild(i).gameObject,previewObjectList[i].transform.position,previewObjectList[i].transform.rotation);
            }
            Debug.Log("EEE");
            snapToGrid.OriginRotation = Vector3.zero;
            //Destroy(SelectedUnit);
            //DestroyUnitがPlayerUnitに隣接しているかを判定し、隣接しているUnitはAdjacentUnitListに登録
            AdjacentUnitList = playerUnitSimulateScript.GetAdjacentUnitList();
            //データの更新にAdjacentUnitListを用いる
            foreach(UnitBase unitBase in AdjacentUnitList){
                unitBase.ReRegistData();
                Debug.Log("FFB" + unitBase.name);
            }
            //PUDMSのデータ更新
            PlayerUnitDestroyManagementScript PUDMS = PlayerUnit.GetComponent<PlayerUnitDestroyManagementScript>();
            PUDMS.SetUnitData();
            captureDataList.RemoveAt(postKeyNum);
            SelectCaptureData = null;
            Destroy(DestroyedUnitStorage.transform.GetChild(postKeyNum).gameObject);
            previewObjectList.Clear();
        }
        //CopyObjectNameList.Clear();
        UnitBaseList.Clear();
    }
    /// <summary>
    /// 実際に再生成する処理
    /// </summary>
    private void RegenerateAsPlayerUnit(GameObject PlunderUnit,Vector3 Position, Quaternion Rotation){
        Position = new Vector3(Position.x, Position.y,0);
        Instantiate(PlunderUnit,Position,Rotation,PlayerUnit.transform).tag = GSetting.ObjTagName.PlayerUnit.ToString();
    }
}

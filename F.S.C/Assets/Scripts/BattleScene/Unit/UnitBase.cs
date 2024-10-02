using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField]
    protected GSetting.ShapeType shapeType;
    [SerializeField]
    private int HitPoint = 5;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    private string UnitTagName = GSetting.ObjTagName.PlayerUnit.ToString();
    private GameObject thisGameObject;
    private FieldManager FM = FieldManager.GetInstance();
    private PlayerUnitDestroyManagementScript PUDMS;
    private bool isDestroyed = false;
    private UnitBase thisUnit;
    //以下4つのUnitBase型変数は参照渡しにだけ利用すること。このデータを使いたい場合はUnitDataからアクセスすること。
    UnitBase upperUnit = null;//分離処理実装後でいいからこちらに他のUnitBaseのデータを残さないように（Rayを飛ばして得られた情報は直接UnitDataに登録するように）リファクタリングしたい。こんなグローバル変数があると無暗に使用してスパゲティコードになる危険性がある。
    UnitBase downerUnit = null;//分離処理実装後でいいからこちらに他のUnitBaseのデータを残さないように（Rayを飛ばして得られた情報は直接UnitDataに登録するように）リファクタリングしたい。こんなグローバル変数があると無暗に使用してスパゲティコードになる危険性がある。
    UnitBase rightUnit = null;//分離処理実装後でいいからこちらに他のUnitBaseのデータを残さないように（Rayを飛ばして得られた情報は直接UnitDataに登録するように）リファクタリングしたい。こんなグローバル変数があると無暗に使用してスパゲティコードになる危険性がある。
    UnitBase leftUnit = null;//分離処理実装後でいいからこちらに他のUnitBaseのデータを残さないように（Rayを飛ばして得られた情報は直接UnitDataに登録するように）リファクタリングしたい。こんなグローバル変数があると無暗に使用してスパゲティコードになる危険性がある。
    private UnitData ThisUnitData;
    public UnitData GetThisUnitData(){
        return ThisUnitData;
    }
    private Vector3 RayBasePosition;//対応するPreviewObjectの座標を代入してある
    private float PreviewObjZRotate;//
    public void SetRayBasePosition(Vector3 position){
        RayBasePosition = position;
    }
    public void SetPreviewObjRotate(float ZRotate){
        PreviewObjZRotate = ZRotate;
    }
    // Start is called before the first frame update
    void Awake()
    {
        thisGameObject = this.gameObject;
        thisUnit = this;
        ThisUnitData= new UnitData(thisUnit,(int)shapeType);
        FM.UnitList.Add(ThisUnitData);
    }
    protected void Start(){
        GetAdjacentObjLink();//RegistData()に格納するとなぜか動かなくなるので注意
        RegistData(upperUnit,downerUnit,rightUnit,leftUnit);
        if(thisGameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString())PUDMS = thisGameObject.transform.root.GetComponent<PlayerUnitDestroyManagementScript>();
    }
    protected virtual void GetAdjacentObjLink()
    {
        //Debug.Log("LLA" + thisUnit.gameObject.name);
        if(shapeType == GSetting.ShapeType.Square){
            upperUnit = GetUpLink();
            downerUnit = GetDownLink();
            rightUnit = GetRightLink();
            leftUnit = GetLeftLink();
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            downerUnit = GetDownLink();
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            downerUnit = GetDownLink();
            rightUnit = GetRightLink();
        }else Debug.LogAssertion("ShapeType is null!");
    }
    void RegistData(UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
    }
    private UnitBase GetUpLink(){
        UnitBase upperUnit;
        GameObject UpperObj = GetUpperGameObject(UnitTagName);
        if(UpperObj == null){return null;}
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if(upperUnit == null)Debug.LogAssertion("UpperUnit is null;");
        return upperUnit;
    }
    private UnitBase GetDownLink(){
        UnitBase downerUnit;
        GameObject DownerObj = GetDownerGameObject(UnitTagName);
        if(DownerObj == null){return null;}
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if(downerUnit == null)Debug.LogAssertion("DownerUnit is null;");
        return downerUnit;
    }
    private UnitBase GetRightLink(){
        UnitBase rightUnit;
        GameObject RightObj = GetRightGameObject(UnitTagName);
        if(RightObj == null){return null;}
        rightUnit = RightObj.GetComponent<UnitBase>();
        if(rightUnit == null)Debug.LogAssertion("RightUnit is null;");
        return rightUnit;
    }
    private UnitBase GetLeftLink(){
        UnitBase leftUnit;
        GameObject LeftObj = GetLeftGameObject(UnitTagName);
        if(LeftObj == null){return null;}
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if(leftUnit == null)Debug.LogAssertion("LeftUnit is null;");
        return leftUnit;
    }
    private GameObject GetUpperGameObject(string ObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.up);
        if(!hit2D){return null;}
        GameObject UpperObj = hit2D.collider.gameObject;
        if(UpperObj.tag != ObjTag){return null;}
        else return UpperObj;
    }
    private GameObject GetDownerGameObject(string ObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.down);
        if(!hit2D){return null;}
        GameObject DownerObj = hit2D.collider.gameObject;
        if(DownerObj.tag != ObjTag){Debug.Log("AZKi"); return null;}
        else return DownerObj;
    }
    private GameObject GetRightGameObject(string ObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.right);
        if(!hit2D){return null;}
        GameObject RightObj = hit2D.collider.gameObject;
        if(RightObj.tag != ObjTag){return null;}
        else return RightObj;
    }
    private GameObject GetLeftGameObject(string ObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.left);
        if(!hit2D){return null;}
        GameObject LeftObj = hit2D.collider.gameObject;
        if(LeftObj.tag != ObjTag){return null;}
        else return LeftObj;
    }
    private RaycastHit2D RayCalculateAndCast(Vector3 RayOffsetDirection){
        Vector3 RayPosition = new Vector3(0,0,0);
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
            case GSetting.ObjTagName.PlayerUnit:
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
            case GSetting.ObjTagName.DestroyedUnit:
                RayPosition = (Quaternion.Euler(0,0,PreviewObjZRotate) * RayOffsetDirection) + RayBasePosition;
                break;
        }
        RaycastHit2D hit2D = Physics2D.Raycast(RayPosition,new Vector3(0,0,1));
        Debug.Log("AZKi" +this.gameObject + RayPosition);
        return hit2D;
    }
    // Update is called once per frame
    protected void Update()
    {
        spriteRenderer.color = new Color(25f*HitPoint/255f, 25f*HitPoint/255f, 25f*HitPoint/255f);
        if(HitPoint == 0){DestroyUnit();}
    }
    protected virtual void AttackAction(){}
    //HPが0になった時の破壊処理（分離の処理はPUDMSが行う）
    protected virtual void DestroyUnit(){
        ThisUnitData.DeleteFourWayLink();
        if(PUDMS != null)PUDMS.DestroyProcess(ThisUnitData);
        //分離エフェクトを実装する
        Debug.Log("PUDMS Destroy");
        Destroy(thisGameObject);
    }
    public bool IsNotIsolatedUnit(){
        if(shapeType == GSetting.ShapeType.Square){
            if(GetUpperGameObject(UnitTagName) != null)return true;
            else if(GetDownerGameObject(UnitTagName) != null)return true;
            else if(GetRightGameObject(UnitTagName) != null)return true;
            else if(GetLeftGameObject(UnitTagName) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            if(GetDownerGameObject(UnitTagName) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            if(GetDownerGameObject(UnitTagName) != null)return true;
            else if(GetRightGameObject(UnitTagName) != null)return true;
            else return false;
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null "); return false;
    }
    /// <summary>
    /// Unitの周りにPlayerUnitがあるかどうかを判別。
    /// PreviewObjectにアタッチする方がよろしいが、ShapeTypeによって処理が異なってしまうのでとりあえずこちらで実装。Intarfaceに直した方がいいかも
    /// </summary>
    /// <returns>隣接するPlayerUnitのUnitBaseのList</returns>
    public List<UnitBase> ReturnAdjacentUnitList(){
        GetAdjacentObjLink();
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        if(upperUnit != null)AdjacentUnitList.Add(upperUnit);
        if(downerUnit != null)AdjacentUnitList.Add(downerUnit);
        if(rightUnit != null)AdjacentUnitList.Add(rightUnit);
        if(leftUnit != null)AdjacentUnitList.Add(leftUnit);
        Debug.Log("JJJ"+thisUnit+upperUnit+downerUnit+rightUnit+leftUnit);
        return AdjacentUnitList;
    }
    public void ReRegistData(){
        GetAdjacentObjLink();
        //UnitData unitData = FM.SearchUnit(this);
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
        Debug.Log("JJJ"+thisUnit+upperUnit+downerUnit+rightUnit+leftUnit);
    }
    public void OnCollisionEnter2D(Collision2D collision2D){
        if(collision2D!.transform.tag == "PlayerWeapon"){
            if(HitPoint >0)HitPoint --;
            else isDestroyed = true;
        }
    }
    /// <summary>
    ///デバッグ用の被弾処理。それ以外では使わないこと
    /// </summary>
    public void DebugDamaged(){
        if(HitPoint >0)HitPoint --;
        else isDestroyed = true;
    }
}

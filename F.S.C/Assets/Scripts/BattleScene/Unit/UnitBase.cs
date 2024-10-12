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
    protected GameObject thisGameObject;
    private FieldManager FM = FieldManager.GetInstance();
    protected AbstractUnitDestroyManagementScript AUDMS;
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
    protected virtual void Start(){
        GetAdjacentObjLink(thisGameObject.tag);//RegistData()に格納するとなぜか動かなくなるので注意
        RegistData(upperUnit,downerUnit,rightUnit,leftUnit);
        AUDMS = thisGameObject.transform.root.GetComponent<AbstractUnitDestroyManagementScript>();
    }
    protected virtual void GetAdjacentObjLink(string SelectedObjTag)
    {
        //Debug.Log("LLA" + thisUnit.gameObject.name);
        if(shapeType == GSetting.ShapeType.Square){
            upperUnit = GetUpLink(SelectedObjTag);
            downerUnit = GetDownLink(SelectedObjTag);
            rightUnit = GetRightLink(SelectedObjTag);
            leftUnit = GetLeftLink(SelectedObjTag);
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            downerUnit = GetDownLink(SelectedObjTag);
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            downerUnit = GetDownLink(SelectedObjTag);
            rightUnit = GetRightLink(SelectedObjTag);
        }else Debug.LogAssertion("ShapeType is null!");
    }
    void RegistData(UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
    }
    private UnitBase GetUpLink(string SelectedObjTag){
        UnitBase upperUnit;
        GameObject UpperObj = GetUpperGameObject(SelectedObjTag);
        if(UpperObj == null){return null;}
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if(upperUnit == null)Debug.LogAssertion("UpperUnit is null;");
        return upperUnit;
    }
    private UnitBase GetDownLink(string SelectedObjTag){
        UnitBase downerUnit;
        GameObject DownerObj = GetDownerGameObject(SelectedObjTag);
        if(DownerObj == null){
            Debug.Log("UBase DownerObj null" + thisGameObject.name);
            return null;}
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if(downerUnit == null)Debug.LogAssertion("DownerUnit is null;");
        return downerUnit;
    }
    private UnitBase GetRightLink(string SelectedObjTag){
        UnitBase rightUnit;
        GameObject RightObj = GetRightGameObject(SelectedObjTag);
        if(RightObj == null){return null;}
        rightUnit = RightObj.GetComponent<UnitBase>();
        if(rightUnit == null)Debug.LogAssertion("RightUnit is null;");
        return rightUnit;
    }
    private UnitBase GetLeftLink(string SelectedObjTag){
        UnitBase leftUnit;
        GameObject LeftObj = GetLeftGameObject(SelectedObjTag);
        if(LeftObj == null){return null;}
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if(leftUnit == null)Debug.LogAssertion("LeftUnit is null;");
        return leftUnit;
    }
    private GameObject GetUpperGameObject(string SelectedObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.up);
        if(!hit2D){return null;}
        GameObject UpperObj = hit2D.collider.gameObject;
        if(UpperObj.tag != SelectedObjTag){return null;}
        else return UpperObj;
    }
    private GameObject GetDownerGameObject(string SelectedObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.down);
        if(!hit2D){Debug.Log("UBase RayCast not hit" + thisGameObject.name); return null;}
        GameObject DownerObj = hit2D.collider.gameObject;
        if(DownerObj.tag != SelectedObjTag){Debug.Log("UBase DownerObj Tag Wrong" + thisGameObject.name + thisGameObject.tag + thisGameObject.tag); return null;}
        else return DownerObj;
    }
    private GameObject GetRightGameObject(string SelectedObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.right);
        if(!hit2D){return null;}
        GameObject RightObj = hit2D.collider.gameObject;
        if(RightObj.tag != SelectedObjTag){return null;}
        else return RightObj;
    }
    private GameObject GetLeftGameObject(string SelectedObjTag){
        RaycastHit2D hit2D = RayCalculateAndCast(Vector3.left);
        if(!hit2D){return null;}
        GameObject LeftObj = hit2D.collider.gameObject;
        if(LeftObj.tag != SelectedObjTag){return null;}
        else return LeftObj;
    }
    /// <summary>
    /// オブジェクト種と調べたい隣のユニットの位置を入力にRayを照射する座標を計算している
    /// </summary>
    /// <param name="RayOffsetDirection">Rayを飛ばす位置をずらす際の初期値</param>
    /// <returns>座標を計算して照射したRayが当たったかどうかRaycastHit2Dで判定し、当たったなら情報を返している</returns>
    private RaycastHit2D RayCalculateAndCast(Vector3 RayOffsetDirection){
        Vector3 RayPosition = new Vector3(0,0,0);
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){//このオブジェクトのタグをenumでswitch文の分岐判別している
        //原点中心でオイラー角*ベクトルによる極座標を（直交座標系に変換して）Rayの本来の始点へ足し合わせて移動させている。この順番じゃないとちゃんと計算できないので注意
            case GSetting.ObjTagName.PlayerUnit://このオブジェクトの周辺のユニットを調べるためにRayを飛ばすので、このオブジェクトの座標と回転を考慮してオブジェクトから見た上下左右方向にRayを飛ばす
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
            case GSetting.ObjTagName.EnemyUnit://このオブジェクトの周辺のユニットを調べるためにRayを飛ばすので、このオブジェクトの座標と回転を考慮してオブジェクトから見た上下左右方向にRayを飛ばす
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
            case GSetting.ObjTagName.DestroyedUnit://鹵獲して接続する際に離れ小島になっていないかIsNotIsolatedUnit()で判別するときに使用される。値が違うだけで計算内容は上と一緒
                RayPosition = (Quaternion.Euler(0,0,PreviewObjZRotate) * RayOffsetDirection) + RayBasePosition;
                break;
        }
        RaycastHit2D hit2D = Physics2D.Raycast(RayPosition,new Vector3(0,0,1));
        Debug.Log("AZKi" +this.gameObject + RayPosition + hit2D.transform);
        return hit2D;
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        spriteRenderer.color = new Color(25f*HitPoint/255f, 25f*HitPoint/255f, 25f*HitPoint/255f);
        if(HitPoint <= 0){DestroyUnit();}
    }
    protected virtual void AttackAction(){}
    //HPが0になった時の破壊処理（分離の処理はAUDMSが行う）
    protected virtual void DestroyUnit(){
        ThisUnitData.DeleteFourWayLink();
        if(AUDMS != null)AUDMS.DestroyProcess(ThisUnitData);
        //分離エフェクトを実装する
        Debug.Log("AUDMS Destroy");
        Destroy(thisGameObject);
    }
    //離れ小島としてユニットを接続しないようにRayを照射して接しているか判別している
    public bool IsNotIsolatedUnit(){
        if(shapeType == GSetting.ShapeType.Square){
            if(GetUpperGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null "); return false;
    }
    /// <summary>
    /// Unitの周りにPlayerUnitがあるかどうかを判別。存在した場合、あとでリンクの更新を行う
    /// PreviewObjectにアタッチする方がよろしいが、ShapeTypeによって処理が異なってしまうのでとりあえずこちらで実装。Intarfaceに直した方がいいかも
    /// </summary>
    /// <returns>隣接するPlayerUnitのUnitBaseのList</returns>
    public List<UnitBase> ReturnAdjacentUnitList(){
        GetAdjacentObjLink(GSetting.ObjTagName.PlayerUnit.ToString());
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        if(upperUnit != null)AdjacentUnitList.Add(upperUnit);
        if(downerUnit != null)AdjacentUnitList.Add(downerUnit);
        if(rightUnit != null)AdjacentUnitList.Add(rightUnit);
        if(leftUnit != null)AdjacentUnitList.Add(leftUnit);
        Debug.Log("JJJ"+thisUnit+upperUnit+downerUnit+rightUnit+leftUnit);
        return AdjacentUnitList;
    }
    public void ReRegistData(){
        GetAdjacentObjLink(thisGameObject.tag);
        //UnitData unitData = FM.SearchUnit(this);
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
        Debug.Log("JJJ"+thisUnit+upperUnit+downerUnit+rightUnit+leftUnit);
    }
    public void OnCollisionEnter2D(Collision2D collision2D){
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
            case GSetting.ObjTagName.PlayerUnit: 
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    case GSetting.ObjTagName.EnemyWeapon1:{break;}
                    case GSetting.ObjTagName.EnemyWeapon2:{break;}
                }
                break;
            }
            case GSetting.ObjTagName.EnemyUnit:
            {
                    switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    case GSetting.ObjTagName.PlayerWeapon1:{
                        if(HitPoint >0)HitPoint --;
                        break;}
                    case GSetting.ObjTagName.PlayerWeapon2:{break;}
                }
                break;
            }
            default: break;
        }
        //if(collision2D!.transform.tag == GSetting.ObjTagName.PlayerWeapon1.ToString()){
        //    if(HitPoint >0)HitPoint --;
        //    else isDestroyed = true;
        //}
    }
    /// <summary>
    ///デバッグ用の被弾処理。それ以外では使わないこと
    /// </summary>
    public void DebugDamaged(){
        if(HitPoint >0)HitPoint --;
    }
}

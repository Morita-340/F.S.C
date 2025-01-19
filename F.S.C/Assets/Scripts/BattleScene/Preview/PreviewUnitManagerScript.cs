using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class PreviewUnitManagerScript : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool isCovered = false;
    [SerializeField]
    Sprite Square;
    [SerializeField]
    Sprite RegularTriangle;
    [SerializeField]
    Sprite IsoscelesRightTriangle;
    [SerializeField]
    Sprite Rectangle;
    [SerializeField]
    Sprite ObtusePentagon;
    private GSetting.ShapeType shapeType;
    public void SetShapeType(GSetting.ShapeType shapeType){
        this.shapeType = shapeType;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        switch(shapeType){
            case GSetting.ShapeType.Square:{
                spriteRenderer.sprite = Square;
                break;}
            case GSetting.ShapeType.RegularTriangle:{
                spriteRenderer.sprite = RegularTriangle;
                break;}
            case GSetting.ShapeType.IsoscelesRightTriangle:{
                spriteRenderer.sprite = IsoscelesRightTriangle;
                break;}
            case GSetting.ShapeType.Rectangle:{
                spriteRenderer.sprite = Rectangle;
                break;}
            case GSetting.ShapeType.ObtusePentagon:{
                spriteRenderer.sprite = ObtusePentagon;
                break;}
            default:break;
        }
        Debug.Log("TTT");
    }

    // Update is called once per frame
    void Update()
    {
        bool isNotIsolated = IsNotIsolatedUnit();
        if(isCovered &&!isNotIsolated){spriteRenderer.color = Color.red;}
        else if(!isCovered && !isNotIsolated){spriteRenderer.color = Color.magenta;}
        else if(isCovered && isNotIsolated){spriteRenderer.color = Color.yellow;}
        else{spriteRenderer.color = Color.green;}

    }
    public bool GetIsCovered(){
        return isCovered;
    }
    public bool GetIsNotIsolated(){
        return IsNotIsolatedUnit();
    }

    void OnTriggerStay2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //stayCovered  = true;
            isCovered = true;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTC");
        }
    }
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //enterCovered= true;
            isCovered = false;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTS");
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //exitCovered = true;
            isCovered = false;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTF");
        }
    }
    public List<UnitBase> GetAdjacentPlayerUnit(){
        List<UnitBase> UnitList = new List<UnitBase>();
        if(shapeType == GSetting.ShapeType.Square){
            UnitList.Add(GetUpperGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            UnitList.Add(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            UnitList.Add(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
        }else if(shapeType == GSetting.ShapeType.Rectangle){
            UnitList.Add(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
        }else if(shapeType == GSetting.ShapeType.ObtusePentagon){
            UnitList.Add(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
            UnitList.Add(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString())?.GetComponent<UnitBase>());
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null ");
        return UnitList;

    }
    //離れ小島としてユニットを接続しないようにRayを照射して接しているか判別している
    private bool IsNotIsolatedUnit(){
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
        }else if(shapeType == GSetting.ShapeType.Rectangle){
            if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.ObtusePentagon){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null "); return false;
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
        if(!hit2D){return null;}
        GameObject DownerObj = hit2D.collider.gameObject;
        if(DownerObj.tag != SelectedObjTag){ return null;}
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
    private RaycastHit2D RayCalculateAndCast(Vector3 RayOffsetDirection){
        Vector3 RayPosition = new Vector3(0,0,0);
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){//このオブジェクトのタグをenumでswitch文の分岐判別している
        //原点中心でオイラー角*ベクトルによる極座標を（直交座標系に変換して）Rayの本来の始点へ足し合わせて移動させている。この順番じゃないとちゃんと計算できないので注意
            case GSetting.ObjTagName.SimulateUnit://鹵獲して接続する際に離れ小島になっていないかIsNotIsolatedUnit()で判別するときに使用される。値が違うだけで計算内容は上と一緒
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.localRotation.eulerAngles.z + this.gameObject.transform.parent.rotation.eulerAngles.z) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
        }
        bool playerHitFirst = true;
        RaycastHit2D PlayerHit2D = new RaycastHit2D();
        foreach(RaycastHit2D hit2D in Physics2D.RaycastAll(RayPosition,new Vector3(0,0,1))){
            if(hit2D){
                if(hit2D.collider.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
                    if(playerHitFirst){
                        playerHitFirst = false;
                        PlayerHit2D = hit2D;
                    }
                    Debug.DrawRay(RayPosition,new(0.3f,0,0),Color.green);
                    Debug.Log("PUMS Ray" +this.gameObject + RayPosition + hit2D.transform);
                }else{Debug.DrawRay(RayPosition,new(0.3f,0,0),Color.red);}
            }else{playerHitFirst = true;}
            if(PlayerHit2D){
                Debug.Log("PUMS RaycastToPlayer");
                break;}
        }
        return PlayerHit2D;
    }
}

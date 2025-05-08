using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
/// <summary>
/// DestroyedUnitの時の移動制御を行う
/// </summary>
public class DestroyedUnitManagementScript : MonoBehaviour
{
    //このスクリプトがアタッチされたオブジェクト自身
    [SerializeField,Range(1,100)]
    //回転係数
    protected float gyrationFactor = 1;
    private GameObject DestroyedUnitManager;
    [SerializeField]private Rigidbody2D rb2D;
    [SerializeField]bool Setting = false;
    MainCameraController MainCamera;
    Vector2 PlayerVector;
    private bool plunderFlag = false;
    /// <summary>
    /// 生成時に明滅するか、点灯するか
    /// </summary>
    private bool colliderAndSpriteONFlag = true;
    /// <summary>
    /// 生成時の回転角と移動方向を格納するx,y=移動方向のベクトル、z=回転角
    /// </summary>
    [SerializeField]Vector3 InstMoveAndRotateVector = new Vector3(0,0,0);
    /// <summary>
    /// colliderAndSpriteONFlagを設定する
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>分離処理の際に生成のタイミングで呼び出さないといけないので、Instantiateの行でGameObjectを返さないといけない</returns>
    public GameObject SetColliderAndSpriteONFlag(bool flag){
        colliderAndSpriteONFlag = flag;
        return gameObject;
    }
    /// <summary>
    /// 入力情報をもとに生成時の回転角度と移動方向を格納する
    /// </summary>
    /// <param name="OriginX">移動するにあたって離れたい対象（分離処理の場合、コアから離れる方向に移動させたいならコアの座標を入力すればいい）</param>
    /// <param name="OriginY"></param>
    /// <param name="RotateVector"></param>
    public void SetMoveAndRotateVector(float OriginX, float OriginY){
        float vectorX = this.transform.position.x - OriginX;
        float vectorY = this.transform.position.y - OriginY;
        if((vectorX > 1 && vectorX < -1)||(vectorY > 1 && vectorY < -1)){
            vectorX = (vectorX <= vectorY)? vectorX/vectorY : vectorX;
            vectorY = (vectorX <= vectorY)? vectorY : vectorY/vectorX;
        }
        InstMoveAndRotateVector.x = vectorX;
        InstMoveAndRotateVector.y = vectorY;
        InstMoveAndRotateVector.z = Random.Range(-0.2f,0.2f); 
    }
    // Start is called before the first frame update
    private void Awake()
    {
        DestroyedUnitManager = this.gameObject;
        int instModeNum;
        if(colliderAndSpriteONFlag){instModeNum = 0;}
        else{instModeNum = 2;}
        StartCoroutine(ColliderAndSpriteProcess(instModeNum));
    }
    private void Start(){
        MainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraController>();
        //武器が制御ユニットとの接続を取れるように分離できているなら、表彰として何かしらUIを表示させたい
        foreach(Transform child in transform){
            if(child.GetComponent<WeaponUnitBase>()){
                if(child.GetComponent<WeaponUnitBase>().IsWCUBenabled()){
                    //表彰の処理
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0){Destroy(gameObject);}
        if(!plunderFlag){
            rb2D.velocity = new Vector3(InstMoveAndRotateVector.x,InstMoveAndRotateVector.y,0);
            transform.Rotate(0,0,InstMoveAndRotateVector.z);
        }else{
            rb2D.velocity = Vector3.zero;
        }
        if(!Setting){
        PlayerVector = transform.position -MainCamera.GetPlayer().transform.position;
            if(PlayerVector.x > 95 || PlayerVector.x < -95){MoveAroundPlayer(0);}
            if(PlayerVector.y >70 || PlayerVector.y < -70){MoveAroundPlayer(1);}
        }
    }
    /// <summary>
    /// カーソルに沿って動くRayCastで外部から呼び出される
    /// </summary>
    public void MovePosition(Vector3 position){
        plunderFlag = true;
        DestroyedUnitManager.transform.position = position;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="wheelInput"></param>
    /// <param name="rotation">
    /// 
    /// </param>
    public void Spin(float wheelInput,Quaternion rotation){
        float currentRotation = 0;
        currentRotation += wheelInput;
        //DestroyedUnitManager.transform.rotation = DestroyedUnitManager.transform.rotation * Quaternion.AngleAxis(wheelInput*gyrationFactor,Vector3.forward);
        DestroyedUnitManager.transform.rotation = Quaternion.Euler(0,0,rotation.z + currentRotation*gyrationFactor);
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのコライダーを有効にするか無効にするかを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    private void ChildrenSColliderEnabled(bool flag){
        foreach(Transform child in DestroyedUnitManager?.transform){
            BoxCollider2D boxCollider2D = child.gameObject.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = flag;
        }
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのスプライトを半透明にするか否かを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    private void ChildrenSpriteTranslucent(bool flag){
        if(DestroyedUnitManager.transform.childCount != 0){
        foreach(Transform child in DestroyedUnitManager.transform){
            SpriteRenderer spriteRenderer = child?.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.green;
            if(flag){
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50/255f);
                if(child.GetComponent<WeaponUnitBase>()){
                    SpriteRenderer WeaponEfficiencyUISR = child.GetComponent<SpriteRenderer>();
                    WeaponEfficiencyUISR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50/255f);
                    }
                }
            else{
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                if(child.GetComponent<WeaponUnitBase>()){
                    SpriteRenderer WeaponEfficiencyUISR = child.GetComponent<SpriteRenderer>();
                    WeaponEfficiencyUISR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                    }
                }
            }
        }
    }
    public bool WeaponControllConected(){
        bool flag = false;
        foreach(Transform child in this.transform){
            WeaponUnitBase weaponUnitBase = child.GetComponent<WeaponUnitBase>();
            if(weaponUnitBase != null){
                if(weaponUnitBase.IsWCUBenabled()){
                    flag = true;}
            }
        }
        return flag;
    }
    private void MoveAroundPlayer(int moveMode){
        if (MainCamera != null){
            Vector2 PlayerVector = this.transform.position - MainCamera.GetPlayer().transform.position;
            if(moveMode == 0){//x方向の移動
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3( - PlayerVector.x,PlayerVector.y,0)*0.95f;
            }else if(moveMode == 1){
                this.transform.position = MainCamera.GetPlayer().transform.position + new Vector3(PlayerVector.x, - PlayerVector.y,0)*0.95f;
            }else{Debug.LogWarning("Invalid moveMode");}
        }
    }
    /// <summary>
    /// DestroyedUnit生成時のコライダーとスプライトの処理が2パターン存在するためここで管理
    /// </summary>
    /// <param name="time"></param>
    /// <param name="processMode"></param>
    /// <returns></returns>
    public IEnumerator ColliderAndSpriteProcess(int processMode){
        switch(processMode){
            case 0:{//初期生成はこれ
                ChildrenSpriteTranslucent(false);
                ChildrenSColliderEnabled(true);
                break;}
            case 1:{//合体処理の鹵獲開始はこれ
                ChildrenSpriteTranslucent(true);
                ChildrenSColliderEnabled(false);
                break;}
            case 2:{//合体処理の鹵獲破棄はこれ
                ChildrenSpriteTranslucent(true);
                ChildrenSColliderEnabled(false);
                yield return new WaitForSeconds(1);
                //元データのコライダーと透明度を戻す
                ChildrenSpriteTranslucent(false);
                ChildrenSColliderEnabled(true);
                break;}
            default:{
                Debug.LogWarning("Invalid processMode");
                break;}
        }
        yield break;
    }
}
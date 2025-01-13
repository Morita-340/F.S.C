using System.Collections;
using System.Collections.Generic;
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
    private bool plunderFlag = false;
    /// <summary>
    /// 生成時の回転角と移動方向を格納するx,y=移動方向のベクトル、z=回転角
    /// </summary>
    [SerializeField]Vector3 InstMoveAndRotateVector = new Vector3(0,0,0);
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
        ChildrenSColliderEnabled(true);
        ChildrenSpriteTranslucent(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!plunderFlag){
            rb2D.velocity = new Vector3(InstMoveAndRotateVector.x,InstMoveAndRotateVector.y,0);
            transform.Rotate(0,0,InstMoveAndRotateVector.z);
        }else{
            rb2D.velocity = Vector3.zero;
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
    public void ChildrenSColliderEnabled(bool flag){
        foreach(Transform child in DestroyedUnitManager?.transform){
            BoxCollider2D boxCollider2D = child.gameObject.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = flag;
        }
    }
    /// <summary>
    /// 子オブジェクトの全ユニットのスプライトを半透明にするか否かを指定するメソッド
    /// </summary>
    /// <param name="flag"></param>
    public void ChildrenSpriteTranslucent(bool flag){
        Debug.Log("DUMS CST" + DestroyedUnitManager);
        Debug.Log("DUMS CST" + DestroyedUnitManager?.name);
        Debug.Log("DUMS CST" + DestroyedUnitManager?.transform);
        if(DestroyedUnitManager.transform.childCount != 0){
        foreach(Transform child in DestroyedUnitManager.transform){
            SpriteRenderer spriteRenderer = child?.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.green;
            if(flag){spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 50/255f);}
            else{spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);}
        }}
    }
}
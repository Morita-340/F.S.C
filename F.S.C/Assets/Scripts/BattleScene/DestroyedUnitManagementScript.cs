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
    private 
    // Start is called before the first frame update
    void Awake()
    {
        DestroyedUnitManager = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// カーソルに沿って動くRayCastで外部から呼び出される
    /// </summary>
    public void MovePosition(Vector3 position){
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
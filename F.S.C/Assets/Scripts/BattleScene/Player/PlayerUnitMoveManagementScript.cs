using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PlayerUnitの一番根のオブジェクトにアタッチされ、子オブジェクトの移動周りの挙動＝シーン上の物理的挙動をまとめて制御
/// </summary>
public class PlayerUnitMoveManagementScript : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUnitManager;
    [SerializeField]
    Rigidbody2D rb2d;
    [SerializeField,Range(10,100)]
    int turn_factor = 1;
    [SerializeField,Range(1,100)]
    int drive_speed = 1;
    [SerializeField,Range(0f,1f)]
    float drive_minimum_speed_factor = 0.5f;
    Transform thisTransform;
    // Start is called before the first frame update
    void Start()
    {
        thisTransform = PlayerUnitManager.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Manipulate();
        AutoDrive();
    }
    void FixedUpdate(){
    }
    /// <summary>
    /// このオブジェクトを自動的に前進させる命令
    /// </summary>
    void AutoDrive(){
        Vector2 forward = transform.right;
        //Debug.Log("VelX:"+ rb2d.velocity.x +"forX"+forward.x*drive_minimum_speed_factor + "\nVelY:"+ rb2d.velocity.y + "forY"+forward.y*drive_minimum_speed_factor);
        //transform.Translate(Vector2.right *drive_speed * Time.deltaTime);
        //rb2d.MovePosition(rb2d.position + forward*drive_speed*Time.deltaTime);
        if(Input.GetKey(KeyCode.A)){
            Decelerate(forward);
        }else{
            rb2d.velocity = forward * drive_speed;
            //Debug.Log("c");
        }
    }
    /// <summary>
    /// 正面方向への速度を減速する
    /// </summary>
    void Decelerate(Vector2 forward){
        //rb2d.velocity = new Vector2(0,0);
        if(Mathf.Abs(rb2d.velocity.x)>=Mathf.Abs(forward.x*drive_minimum_speed_factor)
        &&Mathf.Abs(rb2d.velocity.y)>=Mathf.Abs(forward.y*drive_minimum_speed_factor)
        ){
            //Debug.Log("aaaaa");
            rb2d.velocity -= forward * 0.1f;
            //Debug.Log("VelX:"+ rb2d.velocity.x +"forX"+forward.x*drive_minimum_speed_factor + "\nVelY:"+ rb2d.velocity.y + "forY"+forward.y*drive_minimum_speed_factor);
        }
        //Debug.Log("b");
    }
    /// <summary>
    /// このオブジェクトの加速減速旋回の入力を受け付けて処理する。InputSystemに対応させる
    /// </summary>
    void Manipulate(){
        if(Input.GetKey(KeyCode.W)){
            LeftTurn();
        }
        if(Input.GetKey(KeyCode.S)){
            RithtTurn();
        }
        if (Input.GetKeyDown(KeyCode.D)){
            Accelerate();
        }
    }
    /// <summary>
    /// 正面右方向に旋回する
    /// </summary>
    void RithtTurn(){
        thisTransform.Rotate(0,0,1*turn_factor* Time.deltaTime);
    }
    /// <summary>
    /// 正面左方向に旋回する
    /// </summary>
    void LeftTurn(){
        thisTransform.Rotate(0,0,-1*turn_factor* Time.deltaTime);
    }
    /// <summary>
    /// 正面方向へ加速する。但し僅かにクールタイムが必要である。
    /// </summary>
    void Accelerate(){

    }
}

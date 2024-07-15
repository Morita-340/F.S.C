using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitManagementScript : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUnitManager;
    [SerializeField]
    Rigidbody2D rigidbody2D;
    [SerializeField,Range(10,100)]
    int turn_factor = 1;
    [SerializeField,Range(10,100)]
    int drive_speed = 1;
    [SerializeField,Range(0f,1f)]
    float drive_minimum_speed_factor = 0.5f;
    Transform transform;
    // Start is called before the first frame update
    void Start()
    {
        transform = PlayerUnitManager.transform;
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
        //transform.Translate(Vector2.right *drive_speed * Time.deltaTime);
        //rigidbody2D.MovePosition(rigidbody2D.position + forward*drive_speed*Time.deltaTime);
        if(Input.GetKey(KeyCode.A)){
            Decelerate(forward);
        }else{
            rigidbody2D.velocity = forward * drive_speed;
        }
    }
    /// <summary>
    /// 正面方向への速度を減速する
    /// </summary>
    void Decelerate(Vector2 forward){
        //rigidbody2D.velocity = new Vector2(0,0);
        if((Mathf.Abs(rigidbody2D.velocity.x)>forward.x*drive_minimum_speed_factor
        &&Mathf.Abs(rigidbody2D.velocity.y)>forward.y*drive_minimum_speed_factor)
        ||rigidbody2D.velocity.x==0||rigidbody2D.velocity.y==0){
            rigidbody2D.velocity -= forward * 0.1f;
        }
    }
    /// <summary>
    /// このオブジェクトの加速減速旋回の入力を受け付けて処理する
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
        transform.Rotate(0,0,1*turn_factor* Time.deltaTime);
    }
    /// <summary>
    /// 正面左方向に旋回する
    /// </summary>
    void LeftTurn(){
        transform.Rotate(0,0,-1*turn_factor* Time.deltaTime);
    }
    /// <summary>
    /// 正面方向へ加速する。但し僅かにクールタイムが必要である。
    /// </summary>
    void Accelerate(){

    }
}

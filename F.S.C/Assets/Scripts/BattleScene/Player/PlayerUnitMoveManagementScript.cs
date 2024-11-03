using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PlayerUnitの一番根のオブジェクトにアタッチされ、子オブジェクトの移動周りの挙動＝シーン上の物理的挙動をまとめて制御
/// </summary>
public class PlayerUnitMoveManagementScript : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerUnit;
    [SerializeField]
    Rigidbody2D rb2d;
    [SerializeField,Range(0,5)]
    float turn_factor = 1;
    [SerializeField,Range(1,180)]
    int maximum_turn_factor = 1;
    private float angularVelocity = 1;
    private float maximum_rotation = 0;
    private float minimum_rotation = 0;
    private float rotationOffset = 0;

    [SerializeField,Range(0,100)]
    int drive_speed = 1;
    /// <summary>
    /// プレイヤーが減速するときの速度下限値
    /// </summary>
    [SerializeField,Range(0f,30f)]
    float drive_minimum_speed_factor = 1.5f;
    /// <summary>
    /// プレイヤーが加速するときの速度上限値
    /// </summary>
    [SerializeField,Range(0f,30f)]
    float drive_miximum_speed_factor = 1.5f;
    float thisRotationZ;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Manipulate();
        Drive();
        //Debug.Log("PUMMS" + thisRotationZ +" "+ maximum_rotation +" "+ minimum_rotation);
    }
    void FixedUpdate(){
    }
    /// <summary>
    /// このオブジェクトを前後に動かす命令
    /// 加速時はハンドルが利くが、無操作の時は慣性に従う
    /// 加速減速同時押しで停止する
    /// </summary>
    void Drive(){
        Vector2 forward = this.transform.up;
        if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W)){
            rb2d.velocity = Vector2.zero;
        }
        else if(Input.GetKey(KeyCode.S)){
            Decelerate(-forward);
        }else if (Input.GetKey(KeyCode.W)){
            Accelerate(forward);
        }
        //else{
        //    rb2d.velocity = forward * drive_speed;
        //    //Debug.Log("c");
        //}
    }
    /// <summary>
    /// 正面方向へ加速する
    /// </summary>
    void Accelerate(Vector2 forward){
        //rb2d.velocity = new Vector2(0,0);
        if(rb2d.velocity.x < drive_miximum_speed_factor
        && rb2d.velocity.x > -drive_minimum_speed_factor
        && rb2d.velocity.y < drive_miximum_speed_factor
        && rb2d.velocity.y > -drive_minimum_speed_factor)
        {
            rb2d.velocity += forward * 0.1f;
        }else{
            rb2d.velocity = new Vector2(forward.x*drive_miximum_speed_factor-0.3f,forward.y*drive_miximum_speed_factor-0.3f);
        }
    }
    /// <summary>
    /// 正面方向への速度を減速する
    /// </summary>
    void Decelerate(Vector2 behind){
        //rb2d.velocity = new Vector2(0,0);
        if(rb2d.velocity.x < drive_miximum_speed_factor
        && rb2d.velocity.x > -drive_minimum_speed_factor
        && rb2d.velocity.y < drive_miximum_speed_factor
        && rb2d.velocity.y > -drive_minimum_speed_factor)
        {
            rb2d.velocity += behind * 0.1f;
        }else{
            rb2d.velocity = new Vector2(behind.x*drive_minimum_speed_factor+0.3f,behind.y*drive_minimum_speed_factor+0.3f);
        }
    }
    /// <summary>
    /// このオブジェクトの加速減速旋回の入力を受け付けて処理する。InputSystemに対応させる
    /// </summary>
    void Manipulate(){
        thisRotationZ = rb2d.rotation -rotationOffset;
        if(thisRotationZ < -180){
            thisRotationZ += 360;
        }else if(thisRotationZ > 180){
            thisRotationZ -= 360;
        }
        if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D)){
            rotationOffset = rb2d.rotation;
            angularVelocity =1;
            maximum_rotation = 181;
            minimum_rotation = -181;
        }
        if(Input.GetKey(KeyCode.A)){
            LeftTurn();
        }
        if(Input.GetKey(KeyCode.D)){
            RightTurn();
        }
    }
    /// <summary>
    /// 正面右方向に旋回する
    /// </summary>
    void LeftTurn(){
        float left_miximum_rotation = maximum_rotation -182;
        if(left_miximum_rotation <= thisRotationZ && thisRotationZ <= maximum_rotation){
            angularVelocity += turn_factor * turn_factor;
            PlayerUnit.transform.Rotate(0,0,1*angularVelocity* Time.deltaTime);
        }
        Debug.Log("PUMMS" + left_miximum_rotation +" "+ thisRotationZ +" "+ maximum_rotation +"g" +PlayerUnit.transform.rotation.eulerAngles.z);
    }
    /// <summary>
    /// 正面左方向に旋回する
    /// </summary>
    void RightTurn(){
        float right_miximum_rotation = minimum_rotation + 182;
        if(minimum_rotation <= thisRotationZ && thisRotationZ <= right_miximum_rotation){
            angularVelocity += turn_factor * turn_factor;
            PlayerUnit.transform.Rotate(0,0,-1*angularVelocity* Time.deltaTime);
        }
       Debug.Log("PUMMS" + minimum_rotation +" "+ thisRotationZ +" "+ right_miximum_rotation +"g" +PlayerUnit.transform.rotation.eulerAngles.z);
    }
}

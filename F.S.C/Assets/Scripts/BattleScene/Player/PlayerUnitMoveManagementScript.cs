using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
/// <summary>
/// PlayerUnitの一番根のオブジェクトにアタッチされ、子オブジェクトの移動周りの挙動＝シーン上の物理的挙動をまとめて制御
/// </summary>
public class PlayerUnitMoveManagementScript : AbstractUnitMoveManagementScript
{
    [SerializeField]
    GameObject PlayerUnit;
    [SerializeField, Range(0, 3)]
    float turn_factor = 2;
    [SerializeField, Range(1, 180)]
    int maximum_turn_factor = 1;
    private float angularVelocity = 1;
    private float maximum_rotation = 0;
    private float minimum_rotation = 0;
    private float rotationOffset = 0;

    [SerializeField, Range(0, 100)]
    int drive_speed = 1;
    /// <summary>
    /// プレイヤーが減速するときの速度下限値
    /// </summary>
    [SerializeField, Range(0f, 11.4f)]
    float drive_minimum_speed_factor = 1.5f;
    /// <summary>
    /// プレイヤーが加速するときの速度上限値
    /// </summary>
    [SerializeField, Range(0f, 20f)]
    float drive_maximum_speed_factor = 1.5f;
    float thisRotationZ;
    [SerializeField] bool Setting = false;
    [SerializeField] protected SoundController Scer;
    [SerializeField, ReadOnly] protected bool isAuto = false;
    [SerializeField,ReadOnly]protected bool isIgnited = false;
    [SerializeField,ReadOnly]protected bool notDrive = false;
    /// <summary>
    /// 出撃演出時にはプレイヤーの操作を受け付けてほしくないので
    /// </summary>
    public void SetNotDrive()
    {
        notDrive = true;
    }
    /// <summary>
    /// これがtrueの間は旋回速度と加速度と最高速が上昇。それぞれの値は鹵獲したスラスターとその向きで決まる。もちろん最大値は存在する
    /// </summary>
    [SerializeField, ReadOnly] private bool isBoost = false;
    // Start is called before the first frame update
    void Awake()
    {
        Scer = GetComponent<SoundController>();
        AEC = GetComponent<AugmentorEffectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!notDrive){Manipulate();}
        else if(isIgnited){AEC.IgniteBoost();}
        else if (isAuto) { AEC.MoveForward(); }
        if (Input.GetKey(KeyCode.LeftShift)) { isBoost = true; }
        else { isBoost = false; }
    }
    void FixedUpdate()
    {
        if (!Setting && !isAuto && !notDrive) Drive();
    }
    /// <summary>
    /// このオブジェクトを前後に動かす命令
    /// 加速時はハンドルが利くが、無操作の時は慣性に従う
    /// 加速減速同時押しで停止する
    /// </summary>
    void Drive()
    {
        Vector2 forward = this.transform.up;
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
        {
            rb2d.velocity = Vector2.zero;
            AEC.PositionFix();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Decelerate(-forward);
            if (Input.GetKeyDown(KeyCode.S))
            {
                //ブースト音
                Scer.PlaySE(0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                AEC.MoveBackRight();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                AEC.MoveBackLeft();
            }
            else
            {
                AEC.MoveBack();
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Accelerate(forward);
            if (Input.GetKeyDown(KeyCode.W))
            {
                //ブースト音
                Scer.PlaySE(0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                AEC.MoveForwardLeft();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                AEC.MoveForwardRight();
            }
            else
            {
                AEC.MoveForward();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                AEC.LeftTurn();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                AEC.RightTurn();
            }
            else { AEC.Idle(); }
            Scer.FadeSE();
        }
    }
    /// <summary>
    /// 正面方向へ加速する
    /// </summary>
    void Accelerate(Vector2 forward)
    {
        if (isBoost)
        {
            //rb2d.velocity += forward * 0.3f * 正方向のスラスター個数による加速倍率;
        }
        else
        {
            rb2d.velocity += forward * 0.3f;
        }
        //閾値の範囲を超えれば
        if (rb2d.velocity.x > drive_maximum_speed_factor || rb2d.velocity.x < -drive_minimum_speed_factor || rb2d.velocity.y > drive_maximum_speed_factor || rb2d.velocity.y < -drive_minimum_speed_factor)
        {
            rb2d.velocity = new Vector2(forward.x * drive_maximum_speed_factor - 0.3f, forward.y * drive_maximum_speed_factor - 0.3f);
            //rb2d.velocity -= forward * 0.4f;
        }

    }
    /// <summary>
    /// 正面方向への速度を減速する
    /// </summary>
    void Decelerate(Vector2 behind)
    {
        if (isBoost)
        {
            //rb2d.velocity += behind * 0.3f * 逆方向のスラスター個数による加速倍率;
        }
        else
        {
            rb2d.velocity += behind * 0.3f;
        }
        //閾値の範囲を超えれば
        if (rb2d.velocity.x > drive_maximum_speed_factor || rb2d.velocity.x < -drive_minimum_speed_factor || rb2d.velocity.y > drive_maximum_speed_factor || rb2d.velocity.y < -drive_minimum_speed_factor)
        {
            rb2d.velocity = new Vector2(behind.x * drive_minimum_speed_factor + 0.3f, behind.y * drive_minimum_speed_factor + 0.3f);
            //rb2d.velocity -= behind * 0.4f;
        }
    }
    /// <summary>
    /// このオブジェクトの加速減速旋回の入力を受け付けて処理する。InputSystemに対応させる
    /// </summary>
    void Manipulate()
    {
        thisRotationZ = rb2d.rotation - rotationOffset;
        if (thisRotationZ < -180)
        {
            thisRotationZ += 360;
        }
        else if (thisRotationZ > 180)
        {
            thisRotationZ -= 360;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            rotationOffset = rb2d.rotation;
            angularVelocity = 1;
            maximum_rotation = 181;
            minimum_rotation = -181;
        }
        if (Input.GetKey(KeyCode.A))
        {
            LeftTurn();
        }
        if (Input.GetKey(KeyCode.D))
        {
            RightTurn();
        }
    }
    /// <summary>
    /// 正面右方向に旋回する
    /// </summary>
    void LeftTurn()
    {
        float left_miximum_rotation = maximum_rotation - 182;
        if (left_miximum_rotation <= thisRotationZ && thisRotationZ <= maximum_rotation)
        {
            if (isBoost)
            {
                //angularVelocity += turn_factor * turn_factor * 左向きに取り付けたスラスター個数による旋回倍率;
            }
            else
            {
                angularVelocity += turn_factor * turn_factor;
            }
            PlayerUnit.transform.Rotate(0, 0, 1 * angularVelocity * Time.deltaTime);
        }
        //Debug.Log("PUMMS" + left_miximum_rotation + " " + thisRotationZ + " " + maximum_rotation + "g" + PlayerUnit.transform.rotation.eulerAngles.z);
    }
    /// <summary>
    /// 正面左方向に旋回する
    /// </summary>
    void RightTurn()
    {
        float right_miximum_rotation = minimum_rotation + 182;
        if (minimum_rotation <= thisRotationZ && thisRotationZ <= right_miximum_rotation)
        {
            if (isBoost)
            {
                //angularVelocity += turn_factor * turn_factor * 右向きに取り付けたスラスター個数による旋回倍率;
            }
            else
            {
                angularVelocity += turn_factor * turn_factor;
            }
            PlayerUnit.transform.Rotate(0, 0, -1 * angularVelocity * Time.deltaTime);
        }
        Debug.Log("PUMMS" + minimum_rotation + " " + thisRotationZ + " " + right_miximum_rotation + "g" + PlayerUnit.transform.rotation.eulerAngles.z);
    }
    /// <summary>
    /// ステージ入場時の演出用
    /// </summary>
    public void AutoPilot(int PilotMode)
    {
        switch (PilotMode)
        {
            //前方に加速
            case 0:
                {
                    isAuto = true;
                    Scer.PlaySE(1,true);
                    float igniteVelocity = 0;
                    while (rb2d.velocity.y < drive_maximum_speed_factor + 10)
                    {
                        igniteVelocity += 0.3f;
                        rb2d.velocity = new Vector2(0, igniteVelocity);
                        AEC.MoveForward();
                    }
                    break;
                }
            //逆噴射によるブレーキ
            case 1:
                {
                    isAuto = false;
                    Scer.PlaySE(1,true);
                    Decelerate(-transform.up);
                    AEC.MoveBack();
                    break;
                }
            //単純なオートモード解除
            case 2:
                {
                    isAuto = false;
                    break;
                }
            default: { break; }
        }
    }
    public void Ignition()
    {
        //アフターバーナーちょび点火
        AEC.MoveForward(0.3f);
    }
    public IEnumerator TakeOFF()
    {
        float Yvelocity = 0.001f;
        //一瞬だけアフターバーナーを細長く伸ばして最大サイズに戻す
        //指数関数的に加速。演出優先なので最高速上限は無視
        while (rb2d.velocity.y < 100)
        {
            Yvelocity += Yvelocity;
            if (Yvelocity * Yvelocity < 0.1)
            {
                isIgnited = true;
                isAuto = true;
                Scer.PlaySE(1, true);
            }
            else
            {
                isIgnited = false;
                Scer.ChangeSEPitch(1.3f);
            }
            rb2d.velocity = new Vector2(rb2d.velocity.x, Yvelocity * Yvelocity);
            yield return new WaitForSeconds(0.2f);
        }
        //1秒経ったら終了
        yield return new WaitForSeconds(1f);
        Scer.FadeSE();
    }
    public float GetMaximumSpeed()
    {
        return drive_maximum_speed_factor;
    }
}

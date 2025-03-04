using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField,ReadOnly]
    protected Rigidbody2D thisRb2D;
    /// <summary>
    /// 武器の攻撃力
    /// </summary>
    [SerializeField,Range(0,100)]protected int attackPower = 0;
    /// <summary>
    /// 攻撃倍率
    /// </summary>
    protected int attackEfficiency = 1;
    /// <summary>
    /// 速度倍率
    /// </summary>
    [SerializeField,Range(0.1f,10f)]protected float velocityEfficiency = 1;
    [SerializeField,ReadOnly]protected SoundController SCer;
    public int GetAttackPower(){
        return attackPower;
    }
    public void SetAttackEfficiency(int efficiency){
        attackEfficiency = efficiency;
        if(attackEfficiency < 1){attackEfficiency = 1;}
    }
    public float GetVelocityEfficiency(){
        return velocityEfficiency;
    }
    //被弾時のダメージ総量を返す
    public int GetTotalDamage(){
        return attackPower*attackEfficiency;
    }
    public void SetVelocity(Vector2 velocity){
        thisRb2D.velocity = velocity;
        SCer.PlaySE(1);
    }
    protected virtual void Awake(){
        thisRb2D = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SCer = GetComponent<SoundController>();
        SCer.PlaySE(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    /// <summary>
    /// 武器の攻撃力
    /// </summary>
    [SerializeField,Range(0,100)]protected int attackPower = 0;
    /// <summary>
    /// 攻撃倍率
    /// </summary>
    protected int attackEfficiency = 1;
    public int GetAttackPower(){
        return attackPower;
    }
    public void SetAttackEfficiency(int efficiency){
        attackEfficiency = efficiency;
        if(attackEfficiency < 1){attackEfficiency = 1;}
    }
    //被弾時のダメージ総量を返す
    public int GetTotalDamage(){
        return attackPower*attackEfficiency;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

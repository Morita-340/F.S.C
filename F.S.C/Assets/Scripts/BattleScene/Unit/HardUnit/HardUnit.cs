using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardUnit : UnitBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    /// <summary>
    /// HardUnitは如何なる状況でも被弾によるダメージは1である
    /// </summary>
    /// <param name="damagePoint"></param>
    public override void TakeDamage(int damagePoint)
    {
        HitPoint --;
    }
}

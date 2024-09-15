using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBase : UnitBase
{
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            AttackAction();
        }
    }
}

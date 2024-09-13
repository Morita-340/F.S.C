using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBase : UnitBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            AttackAction();
        }
    }
}

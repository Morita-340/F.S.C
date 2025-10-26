using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class NormalCore : CoreBase
{
    protected override void Start()
    {
        Debug.Log("AAAAA"+gameObject.name);
        base.Start();
    }
    protected override void GetAdjacentObjLink(string SelectedObjTag)
    {
        base.GetAdjacentObjLink(SelectedObjTag);
    }
}

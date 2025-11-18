using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsController : AbstractPartsController
{
    protected override void CountNOFU()
    {
        if(initialFlag)
        {
            numOfInitialFunctionUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<CoreBase>())
                {
                    numOfInitialFunctionUnit++;
                }
            }
            numOfFunctionUnit = numOfInitialFunctionUnit;
        }
        else
        {
            numOfFunctionUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<CoreBase>())
                {
                    numOfFunctionUnit++;
                }
            }
        }
    }  
}

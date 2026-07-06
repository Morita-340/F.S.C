using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStartButton : SliderButton
{
    [SerializeField]
    TitleWindowManager TWM;
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
    protected override void IconPushedAccepted()
    {
        base.IconPushedAccepted();
        StartCoroutine(TWM.TitleToMenuOpen());
        SCer.PlaySE(1);
    }
}

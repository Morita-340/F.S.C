using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToStageSelectButton : PushableIcon
{
    [SerializeField]
    StagePlayerSelectMenuManager SPSMM;
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
        SPSMM.ChangeToStageSelectMenu();
        SCer.PlaySE(1);
    }
    protected override void IconPushedNotAccepted()
    {
        base.IconPushedNotAccepted();
        SCer.PlaySE(2);
    }
}

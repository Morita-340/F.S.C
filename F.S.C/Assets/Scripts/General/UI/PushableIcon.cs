using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableIcon : GeneralUIIconController
{
    /// <summary>
    /// 状況に応じて押して反応するか反応しないか、反応はするが処理が変わるのかなどを切り替えるためのフラグ
    /// </summary>
    bool pushAccepted = true;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0) && CursolSelected)
        {
            IconPushed();
            if (pushAccepted)
            {
                IconPushedAccepted();
            }
            else
            {
                IconPushedNotAccepted();
            }
        }
    }
    /// <summary>
    /// アイコン押し下げで必ず実行
    /// </summary>
    protected virtual void IconPushed()
    {

    }
    /// <summary>
    /// アイコン押し下げ且つ承認
    /// </summary>
    protected virtual void IconPushedAccepted()
    {

    }
    /// <summary>
    /// アイコン押し下げ且つ非承認
    /// </summary>
    protected virtual void IconPushedNotAccepted()
    {

    }
    public void SetPushAccepted(bool flag)
    {
        pushAccepted = flag;
    }
}

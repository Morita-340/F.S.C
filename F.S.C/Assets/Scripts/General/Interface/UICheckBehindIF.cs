using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UICheckBehindIF
{
    /// <summary>
    /// UIの後ろにオブジェクトがあるかを判定する
    /// </summary>
    bool CheckBehind(Vector2 position);
    bool CheckBehind();
}

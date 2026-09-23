using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PartConnectToUI_IF
{
    public abstract Sprite GetThisPartIcon();
    public abstract void SetPartIcon(Sprite sprite);
    /// <summary>
    /// 武装UIにパラメータを渡す用のメソッド
    /// </summary>
    public abstract (bool,float) GetActionProperty();
    /// <summary>
    /// カーソル座標が武装のうちいづれかのユニットで射程内になっているかどうか
    /// </summary>
    /// <returns></returns>
    public abstract bool GetInRange();
    /// <summary>
    /// 現在のリキャスト時間
    /// いつでもアクション可能な武装なら最大リキャスト時間をそのまま返す
    /// </summary>
    /// <returns></returns>
    public abstract float GetNowChargeTime();
    /// <summary>
    /// リキャストに掛かる時間
    /// いつでもアクション可能なら1を返す
    /// NowChargeTime / MaxChargeTimeが1になればアクション可能
    /// </summary>
    /// <returns></returns>
    public abstract float GetMaxChargeTime();
}
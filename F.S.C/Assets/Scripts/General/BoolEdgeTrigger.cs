/// <summary>
/// 一つのクラス内でグローバルに書き換えられるbool値に対して、変更があったタイミングをUpdate内で検知するクラス
/// </summary>
public class BoolEdgeTrigger
{
    readonly System.Func<bool> getter;
    bool prev;
    bool curr;
    bool initialized;

    public BoolEdgeTrigger(System.Func<bool> getter)
    {
        this.getter = getter;
    }
    /// <summary>
    /// フレームの最初に1回だけ呼ぶ。内部状態を更新する。
    /// </summary>
    public void Update()
    {
        bool newCurr = getter();
        if (!initialized)
        {
            prev = newCurr;
            initialized = true;
        }
        else
        {
            prev = curr;
        }
        curr = newCurr;
    }

    public bool Rising() => !prev && curr;
    public bool Falling() => prev && !curr;
}

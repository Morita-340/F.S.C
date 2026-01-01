/// <summary>
/// 一つのクラス内でグローバルに書き換えられるbool値に対して、変更があったタイミングをUpdate内で検知するクラス
/// </summary>
public class BoolEdgeTrigger
{
    readonly System.Func<bool> getter;
    bool prev;
    bool initialized;

    public BoolEdgeTrigger(System.Func<bool> getter)
    {
        this.getter = getter;
    }
    /// <summary>
    /// false → trueを検知
    /// </summary>
    /// <returns></returns>
    public bool Rising()
    {
        bool curr = getter();

        if (!initialized)
        {
            prev = curr;
            initialized = true;
            return false;
        }

        bool result = !prev && curr;
        prev = curr;
        return result;
    }
    /// <summary>
    /// true → falseを検知
    /// </summary>
    /// <returns></returns>
    public bool Falling()
    {
        bool curr = getter();

        if (!initialized)
        {
            prev = curr;
            initialized = true;
            return false;
        }

        bool result = prev && !curr;
        prev = curr;
        return result;
    }
}

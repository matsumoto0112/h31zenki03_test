//自動生成スクリプト
using System.Collections.Generic;
using System.Linq;

public enum TagName
{
    Untagged,
    Respawn,
    Finish,
    EditorOnly,
    MainCamera,
    Player,
    GameController,
}
public static class TagNameManager
{
    public static Dictionary<TagName, string> tagnames = new Dictionary<TagName, string> 
    {
        {TagName.Untagged,"Untagged"},
        {TagName.Respawn,"Respawn"},
        {TagName.Finish,"Finish"},
        {TagName.EditorOnly,"EditorOnly"},
        {TagName.MainCamera,"MainCamera"},
        {TagName.Player,"Player"},
        {TagName.GameController,"GameController"},
    };
    /// <summary>
    /// 文字列で取得する
    /// </summary>
    public static string GetString(this TagName tagname)
    {
        return tagnames[tagname];
    }
    /// <summary>
    /// TagNameで取得する
    /// </summary>
    public static TagName GetTagName(string name)
    {
        return tagnames.FirstOrDefault(pair => pair.Value == name).Key;
    }
}

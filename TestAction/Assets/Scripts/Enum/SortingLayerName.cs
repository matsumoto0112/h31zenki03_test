//自動生成スクリプト
using System.Collections.Generic;
using System.Linq;

public enum SortingLayerName
{
    Default,
}
public static class SortingLayerNameManager
{
    public static Dictionary<SortingLayerName, string> sortinglayernames = new Dictionary<SortingLayerName, string> 
    {
        {SortingLayerName.Default,"Default"},
    };
    /// <summary>
    /// 文字列で取得する
    /// </summary>
    public static string GetString(this SortingLayerName sortinglayername)
    {
        return sortinglayernames[sortinglayername];
    }
    /// <summary>
    /// SortingLayerNameで取得する
    /// </summary>
    public static SortingLayerName GetSortingLayerName(string name)
    {
        return sortinglayernames.FirstOrDefault(pair => pair.Value == name).Key;
    }
}

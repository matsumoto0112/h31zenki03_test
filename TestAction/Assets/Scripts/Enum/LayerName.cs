//自動生成スクリプト
using System.Collections.Generic;
using System.Linq;

public enum LayerName
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Water,
    UI,
    Ground,
}
public static class LayerNameManager
{
    public static Dictionary<LayerName, string> layernames = new Dictionary<LayerName, string> 
    {
        {LayerName.Default,"Default"},
        {LayerName.TransparentFX,"TransparentFX"},
        {LayerName.IgnoreRaycast,"IgnoreRaycast"},
        {LayerName.Water,"Water"},
        {LayerName.UI,"UI"},
        {LayerName.Ground,"Ground"},
    };
    /// <summary>
    /// 文字列で取得する
    /// </summary>
    public static string GetString(this LayerName layername)
    {
        return layernames[layername];
    }
    /// <summary>
    /// LayerNameで取得する
    /// </summary>
    public static LayerName GetLayerName(string name)
    {
        return layernames.FirstOrDefault(pair => pair.Value == name).Key;
    }
}

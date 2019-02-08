//自動生成スクリプト
using System.Collections.Generic;
using System.Linq;

public enum ButtonName
{
    Horizontal,
    Vertical,
    Fire1,
    Fire2,
    Fire3,
    Jump,
    MouseX,
    MouseY,
    MouseScrollWheel,
    Submit,
    Cancel,
}
public static class ButtonNameManager
{
    public static Dictionary<ButtonName, string> buttonnames = new Dictionary<ButtonName, string> 
    {
        {ButtonName.Horizontal,"Horizontal"},
        {ButtonName.Vertical,"Vertical"},
        {ButtonName.Fire1,"Fire1"},
        {ButtonName.Fire2,"Fire2"},
        {ButtonName.Fire3,"Fire3"},
        {ButtonName.Jump,"Jump"},
        {ButtonName.MouseX,"MouseX"},
        {ButtonName.MouseY,"MouseY"},
        {ButtonName.MouseScrollWheel,"MouseScrollWheel"},
        {ButtonName.Submit,"Submit"},
        {ButtonName.Cancel,"Cancel"},
    };
    /// <summary>
    /// 文字列で取得する
    /// </summary>
    public static string GetString(this ButtonName buttonname)
    {
        return buttonnames[buttonname];
    }
    /// <summary>
    /// ButtonNameで取得する
    /// </summary>
    public static ButtonName GetButtonName(string name)
    {
        return buttonnames.FirstOrDefault(pair => pair.Value == name).Key;
    }
}

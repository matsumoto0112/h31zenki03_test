using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumCreateDataScriptableObject : ScriptableObject
{
    [HideInInspector]
    public string savePath = "";
    [HideInInspector, SerializeField]
    private string tagLastsavedPath = "";
    [HideInInspector, SerializeField]
    private string layerLastSavedPath = "";
    [HideInInspector, SerializeField]
    private string sortingLayerLastSavedPath = "";
    [HideInInspector, SerializeField]
    private string buttonLastSavedPath = "";

    public string GetLastSavedPath(CreateEnumType type)
    {
        switch (type)
        {
            case CreateEnumType.Tag:
                return tagLastsavedPath;
            case CreateEnumType.Layer:
                return layerLastSavedPath;
            case CreateEnumType.SortingLayer:
                return sortingLayerLastSavedPath;
            case CreateEnumType.Button:
                return buttonLastSavedPath;
            default:
                Debug.LogError("未定義のtypeからパスを取得しようとしました。" + type.ToString());
                return "";
        }
    }

    public void SetLastSavedPath(CreateEnumType type, string fullpath)
    {
        switch (type)
        {
            case CreateEnumType.Tag:
                tagLastsavedPath = fullpath;
                break;
            case CreateEnumType.Layer:
                layerLastSavedPath = fullpath;
                break;
            case CreateEnumType.SortingLayer:
                sortingLayerLastSavedPath = fullpath;
                break;
            case CreateEnumType.Button:
                buttonLastSavedPath = fullpath;
                break;
            default:
                Debug.LogError("未定義のtypeにパスを設定しようとしました。" + type.ToString());
                break;
        }
    }
}

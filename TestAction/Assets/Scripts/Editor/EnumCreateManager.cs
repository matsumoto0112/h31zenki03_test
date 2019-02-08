using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 作成できるenumの種類
/// </summary>
public enum CreateEnumType
{
    Tag,
    Layer,
    SortingLayer,
    Button,
}

public delegate void WindowButtonCallBack();

/// <summary>
/// OK,NOボタンがあるウィンドウ
/// </summary>
class OKNOWindow : EditorWindow
{
    private bool pushedButton = false;
    /// <summary>
    /// 表示するメッセージ
    /// </summary>
    public string message { get; private set; }
    /// <summary>
    /// OKボタンが押されたときのコールバック
    /// </summary>
    public WindowButtonCallBack ok { get; private set; }
    /// <summary>
    /// NOボタンが押されたときのコールバック
    /// </summary>
    public WindowButtonCallBack no { get; private set; }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="message"></param>
    /// <param name="okCallBack"></param>
    /// <param name="noCallBack"></param>
    public void SetUp(string message, WindowButtonCallBack okCallBack, WindowButtonCallBack noCallBack)
    {
        this.message = message;
        this.ok = okCallBack;
        this.no = noCallBack;
        pushedButton = false;
        this.position = new Rect(150, 150, 450, 50);
    }

    private void OnGUI()
    {
        GUILayout.Label(message);
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("OK", GUILayout.Width(50)))
            {
                pushedButton = true;
                ok();
                Close();
            }
            if (GUILayout.Button("NO", GUILayout.Width(50)))
            {
                pushedButton = true;
                no();
                Close();
            }
        }
    }

    private void OnDestroy()
    {
        if (!pushedButton)
        {
            no();
        }
    }
}

public class OneButtonWindow : EditorWindow
{  
    private bool pushedButton;
    private string message;
    private WindowButtonCallBack callBack;

    public void SetUp(string message, WindowButtonCallBack callBack)
    {
        this.message = message;
        this.callBack = callBack;
        pushedButton = false;
        this.position = new Rect(150, 150, 150, 50);
    }
    private void OnGUI()
    {
        GUILayout.Label(message);
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("OK", GUILayout.Width(50)))
            {
                pushedButton = true;
                callBack();
                Close();
            }
        }
    }

    private void OnDestroy()
    {
        if (!pushedButton)
        {
            callBack();
        }
    }

}

public class EnumCreateManager : EditorWindow
{
    /// <summary>
    /// ファイルの存在の仕方
    /// </summary>
    private enum FileExistType
    {
        Nothing, //ない
        ExistSameFolder, //同じフォルダにある
        ExistDifferentFolder, //違うフォルダにある
    }

    // 無効な文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<",
    };
    private struct EnumCreateInfo
    {
        public delegate string[] GetNames();
        /// <summary>
        /// クラス名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Enum名を取得するメソッド
        /// </summary>
        public GetNames getNamesMethod { get; set; }
        public EnumCreateInfo(string name, GetNames getNamesMethod)
        {
            this.name = name;
            this.getNamesMethod = getNamesMethod;
        }
    }

    private EnumCreateDataScriptableObject lastSavedPathData;
    private string localFolderPath
    {
        get { return _localFolderPath; }
        set
        {
            if (!value.EndsWith("/"))
            {
                value += "/";
            }
            _localFolderPath = value;
        }
    }
    private string _localFolderPath;
    private bool initialized = false;
    private bool isDisable = false;

    /// <summary>
    /// 作成用データ
    /// </summary>
    private static Dictionary<CreateEnumType, EnumCreateInfo> infos = new Dictionary<CreateEnumType, EnumCreateInfo>
    {
        {CreateEnumType.Tag,  new EnumCreateInfo("TagName",()=> {return UnityEditorInternal.InternalEditorUtility.tags; }) },
        {CreateEnumType.Layer,new EnumCreateInfo("LayerName",()=> { return UnityEditorInternal.InternalEditorUtility.layers; }) },
        {CreateEnumType.SortingLayer,  new EnumCreateInfo("SortingLayerName",()=> {return SortingLayer.layers.Select(s => s.name).ToArray(); }) },
        {CreateEnumType.Button,new EnumCreateInfo("ButtonName",GetButtonString) },
    };
    /// <summary>
    /// 作成用データをforeachで回すためのキーリスト
    /// </summary>
    private static List<CreateEnumType> patternsKeyList = new List<CreateEnumType>(infos.Keys);

    [MenuItem("Editor/EnumCreate")]
    private static void Create()
    {
        if (!CanCreate()) return;
        GetWindow<EnumCreateManager>("EnumCreater");
    }

    [MenuItem("Editor/EnumCreate", true)]
    private static bool CanSelectMenu()
    {
        return CanCreate();
    }


    /// <summary>
    /// 作成できるか
    /// コンパイル中などは作成できない
    /// </summary>
    /// <returns></returns>
    private static bool CanCreate()
    {
        //デバッグ実行中ならfalse
        if (EditorApplication.isPlaying) return false;
        //アプリケーションが実行中ならfalse
        if (Application.isPlaying) return false;
        //コンパイル中ならfalse
        if (EditorApplication.isCompiling) return false;
        return true;
    }

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(isDisable);

        if (!initialized)
        {
            initialized = true;
            //まずこのファイルの保存されているパスを取得
            MonoScript mono = MonoScript.FromScriptableObject(this);
            string lastSavedDataExistPath = AssetDatabase.GetAssetPath(mono);
            //最後に保存されたパスの情報が入っているデータを読み込む
            string filename = System.IO.Path.GetFileName(lastSavedDataExistPath);
            lastSavedDataExistPath = lastSavedDataExistPath.Replace(filename, "");
            Debug.Log("ファイルの存在する場所:" + lastSavedDataExistPath);
            Debug.Log("ファイルパス:" + lastSavedDataExistPath + typeof(EnumCreateDataScriptableObject).Name + ".asset");
            lastSavedPathData = AssetDatabase.LoadAssetAtPath<EnumCreateDataScriptableObject>(lastSavedDataExistPath + typeof(EnumCreateDataScriptableObject).Name + ".asset");
            UnityEngine.Assertions.Assert.IsNotNull(lastSavedPathData, "error");
            localFolderPath = lastSavedPathData.savePath;
            CheckEnumFiles();
        }
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("保存するパス", GUILayout.Width(100));
            localFolderPath = GUILayout.TextField(localFolderPath, GUILayout.Width(200));
        }
        foreach (var key in patternsKeyList)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(key.ToString(), GUILayout.Width(100));

                //クラス名の変更機能
                EnumCreateInfo info = infos[key];
                info.name = (GUILayout.TextField(infos[key].name, GUILayout.Width(100)));
                infos[key] = info;

                //作成ボタンが押されたとき
                if (GUILayout.Button("作成", GUILayout.Width(100)))
                {
                    FileExistType existType = IsExistSameScript(key);
                    //ファイルが存在しなければそのまま作成
                    if (existType == FileExistType.Nothing)
                    {
                        CreateEnumByBaseFoamat(key, infos[key].getNamesMethod());
                        SaveLastCreatedPath(key);
                        continue;
                    }
                    //そうでなければ確認ウィンドウを出す
                    OKNOWindow okWindow = GetWindow<OKNOWindow>("確認");
                    //確認ウィンドウが出現中は操作できないようにする
                    isDisable = true;

                    string message = "";
                    //OKボタンが押されたときのコールバック
                    WindowButtonCallBack okCallBack;
                    //NOボタンが押されたら何もせずにウィンドウを閉じる
                    WindowButtonCallBack noCallBack = () =>
                    { Debug.Log("キャンセルされました。"); isDisable = false; };
                    //上書きしようとしている
                    if (existType == FileExistType.ExistSameFolder)
                    {
                        message = "上書きしてもよろしいですか？";
                        okCallBack = () =>
                        {
                            Debug.Log("上書きしました。");
                            CreateEnumByBaseFoamat(key, infos[key].getNamesMethod());
                            SaveLastCreatedPath(key);
                            isDisable = false;
                        };
                    }
                    //別フォルダに同じクラスが存在するのに保存しようとしている
                    else
                    {
                        message = "別フォルダに同じクラスが存在するため作成できません。\n" +
                        lastSavedPathData.GetLastSavedPath(key) + "を削除してもよろしいですか？";
                        okCallBack = () =>
                         {
                             Debug.Log(lastSavedPathData.GetLastSavedPath(key) + "を削除し、新しいファイルを作成しました。");
                             DestroyScript(key);
                             CreateEnumByBaseFoamat(key, infos[key].getNamesMethod());
                             SaveLastCreatedPath(key);
                             isDisable = false;
                         };
                    }
                    okWindow.SetUp(message, okCallBack, noCallBack);
                }

                //削除ボタンが押された
                if (GUILayout.Button("削除", GUILayout.Width(100)))
                {
                    PushedDeleteButton(key);
                }
            }
        }
    }

    /// <summary>
    /// 削除ボタンが押されたときの挙動
    /// </summary>
    /// <param name="type"></param>
    private void PushedDeleteButton(CreateEnumType type)
    {
        if (!ExistEnumFile(type))
        {
            ShowWindowNothingDeleteFile();
            return;
        }
        ShowWindowDeleteConfirmation(type);
    }

    /// <summary>
    /// 存在しないファイルを削除しようとしたときのエラーウィンドウ表示
    /// </summary>
    private void ShowWindowNothingDeleteFile()
    {
        OneButtonWindow window = GetWindow<OneButtonWindow>("エラー");
        isDisable = true;
        WindowButtonCallBack callback = () =>
        {
            isDisable = false;
        };
        string message = "ファイルが存在しません。";
        window.SetUp(message, callback);
    }

    /// <summary>
    /// ファイルを削除するときの確認ウィンドウ表示
    /// </summary>
    /// <param name="type"></param>
    private void ShowWindowDeleteConfirmation(CreateEnumType type)
    {
        OKNOWindow window = GetWindow<OKNOWindow>("確認");
        isDisable = true;
        WindowButtonCallBack ok = () =>
        {
            DestroyScript(type);
            isDisable = false;
        };
        WindowButtonCallBack no = () =>
        {
            isDisable = false;
        };
        string message = type.ToString() + "を" + lastSavedPathData.GetLastSavedPath(type) + "から削除してもよろしいですか？";
        window.SetUp(message, ok, no);
    }

    /// <summary>
    /// Enumのファイルが存在しなければパスを初期化する
    /// </summary>
    private void CheckEnumFiles()
    {
        foreach (var key in patternsKeyList)
        {
            if (!ExistEnumFile(key))
            {
                lastSavedPathData.SetLastSavedPath(key, "");
            }
        }
    }

    /// <summary>
    /// Enumファイルが存在するか
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool ExistEnumFile(CreateEnumType type)
    {
        string path = lastSavedPathData.GetLastSavedPath(type);
        if (path == null) return false;
        return System.IO.File.Exists(path);
    }

    private void OnDestroy()
    {
        //保存先のパスを更新する
        lastSavedPathData.savePath = localFolderPath;
        //変更があった事を記録する
        EditorUtility.SetDirty(lastSavedPathData);
        //保存する
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// InputManagerに登録されたボタンの名前を取得
    /// </summary>
    /// <returns>重複を削除されたリストを返す</returns>
    private static string[] GetButtonString()
    {
        List<string> buttonStringList = new List<string>();
        //ProjectSettingにあるInputManagerをシリアライズオブジェクトとして開く
        SerializedObject buttonObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        //ボタンの定義部分のプロパティを取得
        SerializedProperty buttonProperty = buttonObject.FindProperty("m_Axes");
        for (int i = 0; i < buttonProperty.arraySize; i++)
        {
            //一つずづ名前を取得
            SerializedProperty prop = buttonProperty.GetArrayElementAtIndex(i);
            buttonStringList.Add(GetChildProperty(prop, "m_Name").stringValue);
        }
        //重複を削除し返す
        return buttonStringList.Distinct().ToArray();
    }


    /// <summary>
    /// 子プロパティを取得
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        } while (child.Next(false));
        return null;
    }

    /// <summary>
    /// 無効な文字の削除
    /// </summary>
    /// <param name="str">削除する文字列</param>
    /// <returns>無効文字を削除された文字列</returns>
    protected static string RemoveInvalidChars(string str)
    {
        //無効文字が含まれていたら削除する
        System.Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }

    /// <summary>
    /// ローカルのパスからアセットパスを取得
    /// </summary>
    /// <param name="localPath"></param>
    /// <returns></returns>
    private string GetAssetPathFromLocalPath(string localPath)
    {
        return "Assets/" + localPath;
    }

    /// <summary>
    /// 最後に作成されたファイルへのパスを保存
    /// </summary>
    /// <param name="type"></param>
    private void SaveLastCreatedPath(CreateEnumType type)
    {
        string path = GetSaveFullPath(type);
        lastSavedPathData.SetLastSavedPath(type, path);
        EditorUtility.SetDirty(lastSavedPathData);
        AssetDatabase.SaveAssets();
        Debug.Log("パスを保存しました。" + type + lastSavedPathData.GetLastSavedPath(type));
    }

    /// <summary>
    /// スクリプトの削除
    /// </summary>
    /// <param name="type"></param>
    private void DestroyScript(CreateEnumType type)
    {
        Debug.Log(type.ToString() + "のスクリプトの削除");
        string path = lastSavedPathData.GetLastSavedPath(type);
        Debug.Log(path + "を削除する。");
        if (path == null || path == "")
        {
            Debug.LogError("存在しないtypeが削除されようとしました。" + type.ToString());
            return;
        }
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("ファイルが存在しません。" + path);
            return;
        }
        System.IO.File.Delete(path);
        Debug.Log(path + "にあった" + type + "を削除します。");
        //前に保存したパスの削除
        lastSavedPathData.SetLastSavedPath(type, "");
    }

    /// <summary>
    /// ファイルが存在しているか
    /// </summary>
    /// <param name="type"></param>
    /// <returns>存在しているか、その存在状態の種類を返す</returns>
    private FileExistType IsExistSameScript(CreateEnumType type)
    {
        //最後に保存されたパスがなければ存在しない
        string lastSavedPath = lastSavedPathData.GetLastSavedPath(type);
        if (lastSavedPath == "")
        {
            Debug.Log("未保存または削除済み");
            return FileExistType.Nothing;
        }
        //新しく保存するパスが前と同じ場所か
        string newSavePath = GetSaveFullPath(type);
        if (lastSavedPath == newSavePath)
        {
            Debug.Log("同じ場所に保存");
            return FileExistType.ExistSameFolder;
        }
        else
        {
            Debug.Log("違う場所に保存");
            return FileExistType.ExistDifferentFolder;
        }
    }

    /// <summary>
    /// クラスファイル名の取得
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    private string GetClassFileName(string className)
    {
        return className + ".cs";
    }

    /// <summary>
    /// 保存先のフルパスを取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetSaveFullPath(CreateEnumType type)
    {
        string fullPath = Application.dataPath + "/" + localFolderPath + GetClassFileName(infos[type].name);
        Debug.Log("保存されたフルパスは" + fullPath);
        return fullPath;
    }

    /// <summary>
    /// 保存先のアセットパスを取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetSaveAssetPath(CreateEnumType type)
    {
        return GetAssetPathFromLocalPath(localFolderPath) + GetClassFileName(infos[type].name);
    }

    /// <summary>
    /// 基本フォーマットでenum作成
    /// </summary>
    /// <param name="className"></param>
    /// <param name="enumNames"></param>
    private void CreateEnumByBaseFoamat(CreateEnumType type, string[] enumNames)
    {
        string className = infos[type].name;
        string filepath = GetSaveFullPath(type);
        Debug.Log(filepath + "に" + className + "を作成する");
        //0個だったら終了
        if (enumNames.Length == 0)
        {
            EditorUtility.DisplayDialog(filepath, className + "の中身が一つも存在しません", "OK");
            return;
        }
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("//自動生成スクリプト");
        stringBuilder.AppendLine("using System.Collections.Generic;");
        stringBuilder.AppendLine("using System.Linq;");
        stringBuilder.AppendLine();
        //クラス名をつける
        stringBuilder.AppendFormat("public enum {0}", className).AppendLine();
        stringBuilder.AppendLine("{");

        //各要素をカンマ区切りで格納
        for (int i = 0; i < enumNames.Length; i++)
        {
            enumNames[i] = RemoveInvalidChars(enumNames[i]);
            stringBuilder.AppendLine("    " + enumNames[i] + ",");
        }
        //enumの終わり
        stringBuilder.AppendLine("}");

        //classのManagerを作成
        stringBuilder.AppendFormat("public static class {0}Manager", className).AppendLine();
        stringBuilder.AppendLine("{");
        string classNameLower = className.ToLower();
        string dictionaryName = className.ToLower() + "s";
        //enumとstringのペアのディクショナリを作成
        stringBuilder.AppendFormat("    public static Dictionary<{0}, string> {1}s = new Dictionary<{0}, string> ", className, classNameLower).AppendLine();
        stringBuilder.AppendLine("    {");

        foreach (var tag in enumNames)
        {
            stringBuilder.AppendLine("        {" + className + "." + tag + "," + "\"" + tag + "\"},");
        }

        stringBuilder.AppendLine("    };");

        stringBuilder.AppendLine("    /// <summary>");
        stringBuilder.AppendLine("    /// 文字列で取得する");
        stringBuilder.AppendLine("    /// </summary>");
        stringBuilder.AppendFormat("    public static string GetString(this {0} {1})", className, classNameLower).AppendLine();
        stringBuilder.AppendLine("    {");
        stringBuilder.AppendFormat("        return {0}[{1}];", dictionaryName, classNameLower).AppendLine();
        stringBuilder.AppendLine("    }");

        stringBuilder.AppendLine("    /// <summary>");
        stringBuilder.AppendFormat("    /// {0}で取得する", className).AppendLine();
        stringBuilder.AppendLine("    /// </summary>");
        stringBuilder.AppendFormat("    public static {0} Get{1}(string name)", className, className).AppendLine();
        stringBuilder.AppendLine("    {");
        stringBuilder.AppendFormat("        return {0}.FirstOrDefault(pair => pair.Value == name).Key;", dictionaryName).AppendLine();
        stringBuilder.AppendLine("    }");

        stringBuilder.AppendLine("}");

        //ディレクトリのパスを取得し、そこにファイルがなければ新しく作成
        string directoryName = System.IO.Path.GetDirectoryName(filepath);
        if (!System.IO.Directory.Exists(directoryName))
        {
            System.IO.Directory.CreateDirectory(directoryName);
        }
        //書き込み
        System.IO.File.WriteAllText(filepath, stringBuilder.ToString(), System.Text.Encoding.UTF8);
        Debug.Log(className + "を作成しました。\n" + filepath);
    }
}

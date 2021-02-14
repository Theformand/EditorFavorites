using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using Assets.Scripts.Editor;

public class FavoritesWindow : EditorWindow
{
    private Favorites favorites;
    private const string PREFS_ID = "favorites_assetpaths";
    private static int minWidth = 235;
    private FavoritesWindSettings settings;

    private Vector2 scrollPos;
    private Rect scrollviewRect;
    private bool autoOpen;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Favorites")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FavoritesWindow window = GetWindow<FavoritesWindow>("Favorites", true);
        window.minSize = new Vector2(minWidth, 100);
        window.Show();
    }

    public void OnEnable()
    {
        LoadSettings();
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }

    private void Save()
    {
        EditorPrefs.SetString(PREFS_ID, JsonUtility.ToJson(favorites));
    }

    private void Load()
    {
        favorites = JsonUtility.FromJson<Favorites>(EditorPrefs.GetString(PREFS_ID));
        if (favorites == null)
            favorites = new Favorites();
    }

    private void OnGUI()
    {
        if (favorites == null)
            Load();
        if (settings == null)
            LoadSettings();

        if (Event.current.type == EventType.DragUpdated)
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        else if (Event.current.type == EventType.DragPerform)
            Add(DragAndDrop.paths);

        Vector2 entryPos = new Vector2(minWidth * 0.05f, 0f);
        Vector2 entrySize = new Vector2(settings.EntryWidth, settings.EntryHeight);
        Rect entryRect = new Rect(entryPos, entrySize);

        Rect delBtnRect = new Rect(entryRect.position, entryRect.size);
        var pos = delBtnRect.position;
        var size = delBtnRect.size;
        pos.x += entrySize.x * 0.5f + size.x * 0.5f;
        size.x *= settings.RemoveBtnWidthPercent;
        delBtnRect.size = size;
        delBtnRect.position = pos;

        GUIStyle entryStyle = new GUIStyle(GUI.skin.button);
        entryStyle.alignment = TextAnchor.MiddleLeft;
        int scrollRectHeight = favorites.Paths.Count * settings.EntryHeight;

        scrollviewRect = new Rect(0, 0, minWidth + 5, scrollRectHeight);

        scrollPos = GUI.BeginScrollView(scrollviewRect, scrollPos, new Rect(0, 0, 200, scrollRectHeight * 1.2f));
        {
            for (int i = favorites.Paths.Count - 1; i >= 0; i--)
            {
                string path = favorites.Paths[i];
                var icon = favorites.Icons[i];
                string name = favorites.Names[i];
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                float posY = settings.topPadding + i * (settings.EntryHeight + settings.EntrySpacing);
                entryRect.y = posY;
                delBtnRect.y = posY;


                if (GUI.Button(entryRect, new GUIContent(name, icon), entryStyle))
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                    if (autoOpen)
                        EditorUtility.OpenWithDefaultApp("");
                }
                var col = GUI.backgroundColor;
                GUI.backgroundColor = settings.RemoveButtonColor;

                if (GUI.Button(delBtnRect, "X"))
                    Remove(i);

                GUI.backgroundColor = col;
            }
        }
        GUI.EndScrollView();
    }

    private void Add(string[] paths)
    {
        favorites.Paths.AddRange(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            favorites.Icons.Add(GetIcon(paths[i]));
            favorites.Names.Add(AssetPathToAssetName(paths[i]));
        }
        Save();
    }

    private void Remove(int idx)
    {
        favorites.Paths.RemoveAt(idx);
        favorites.Icons.RemoveAt(idx);
        favorites.Names.RemoveAt(idx);
        Save();
    }
    private void LoadSettings()
    {
        var guids = AssetDatabase.FindAssets("FavoritesWindowSettings");
        if (guids.Length > 0)
            settings = AssetDatabase.LoadAssetAtPath<FavoritesWindSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
        else
        {
            settings = CreateInstance<FavoritesWindSettings>();
            AssetDatabase.CreateAsset(settings, "FavoritesWindowSettings.asset");
        }
    }

    public static string AssetPathToAssetName(string assetPath)
    {
        var splits = assetPath.Split('/');
        var last = splits[splits.Length - 1];
        var idxDot = last.IndexOf(".");
        if (idxDot >= 0)
            return last.Substring(0, idxDot);
        else
            return last;

    }

    private Texture GetIcon(string path)
    {
        return AssetDatabase.GetCachedIcon(path);
    }
}

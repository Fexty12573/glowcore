using Unity.CodeEditor;
using UnityEditor;

public static class CiProjectFiles
{
    public static void Generate()
    {
        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
        AssetDatabase.Refresh();
    }
}


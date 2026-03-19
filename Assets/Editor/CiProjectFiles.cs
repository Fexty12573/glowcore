using Unity.CodeEditor;
using UnityEditor;

public static class CiProjectFilescs
{
    public static void Generate()
    {
        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
        AssetDatabase.Refresh();
    }
}


using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Quét lỗi Missing Script trong Scene này"))
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
            int count = 0;
            foreach (GameObject go in allObjects)
            {
                Component[] components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.LogError("Phát hiện lỗi Missing Script tại: " + go.name, go);
                        count++;
                    }
                }
            }
            Debug.Log($"--- Quét xong! Tìm thấy {count} lỗi. ---");
        }
    }
}
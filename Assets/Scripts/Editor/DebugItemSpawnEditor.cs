using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

#if UNITY_EDITOR
[CustomEditor(typeof(DebugItemSpawn))]
public class DebugItemSpawnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugItemSpawn debugItemSpawn = target.GetComponent<DebugItemSpawn>();

        if (GUILayout.Button("Spawn"))
        {
            debugItemSpawn.Spawn();
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSystem
{
    private static string path = Application.persistentDataPath;
    private const string worldPath = "/Worlds/"; 

    public static void SaveData(Data data)
    {
        string path = GetPathFor(data.GetType());

        data.path = path;
        data.uid = data.name.ToSafeString();

        string json = JsonUtility.ToJson(data);

        System.IO.File.WriteAllText(path + $"{data.uid}.json", json);

        Debug.Log("Data saved at " + path);
    }

    public static List<Data> LoadAll<T>() where T : Data
    {
        if (typeof(T) == typeof(WorldData))
        {
            string path = GetPathFor(typeof(WorldData));

            string[] files = System.IO.Directory.GetFiles(path)
                .Where(f => !f.EndsWith(".DS_Store"))
                .ToArray();

            List<Data> datas = new List<Data>();

            foreach (string file in files)
            {
                string json = System.IO.File.ReadAllText(file);
                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning($"Empty file found: {file}");
                    continue;
                }

                try
                {
                    WorldData data = JsonUtility.FromJson<WorldData>(json);
                    datas.Add(data);
                    Debug.Log("Data loaded: " + data.name);
                }
                catch (ArgumentException e)
                {
                    Debug.LogError($"Failed to parse file {file}: {e.Message}");
                }
            }

            return datas;
        }

        return null;
    }

    private static string GetPathFor(Type type)
    {
        if (type ==  typeof(WorldData))
        {
            if (!System.IO.Directory.Exists(path + worldPath))
            {
                System.IO.Directory.CreateDirectory(path + worldPath);
            }

            return path + worldPath;
        }

        Debug.LogError("Data type not recognized");

        return null;
    }
}
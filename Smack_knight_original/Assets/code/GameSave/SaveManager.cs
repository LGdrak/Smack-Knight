using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

public static class SaveManager
{
    public static void SavePlayer(bool _load, int _round, int _currentHealth, int _souls, GameObject _equipped)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.dontTouch";
            
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveFile file = new SaveFile(_load, _round, _currentHealth, _souls, _equipped);

        formatter.Serialize(stream, file);
        stream.Close();
    }

    public static SaveFile LoadPlayer()
    {
        string path = Application.persistentDataPath + "/Player.dontTouch";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();            
            FileStream stream = new FileStream(path, FileMode.Open);

            if (stream.Length != 0)
            {
                SaveFile file = formatter.Deserialize(stream) as SaveFile;
                stream.Close();
                return file;
            }

            stream.Close();
            return null;

        }else
        {
            UnityEngine.Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

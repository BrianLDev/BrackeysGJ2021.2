using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveScore
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/score.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerScore data = new PlayerScore(GameManager.Instance.highScore);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerScore Load()
    {
        string path = Application.persistentDataPath + "/score.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerScore data = formatter.Deserialize(stream) as PlayerScore;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

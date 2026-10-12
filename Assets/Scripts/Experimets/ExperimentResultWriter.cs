using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ExperimentResultWriter
{
    public static void Save(string fileName, List<string> lines)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllLines(path, lines);
            Debug.Log($"Results saved to {path}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Results could not be saved: {exception.Message}");
        }
    }
}
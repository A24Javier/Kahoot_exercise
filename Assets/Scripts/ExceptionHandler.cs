using System;
using System.IO;
using UnityEngine;

public class ExceptionHandler : MonoBehaviour
{
    public static ExceptionHandler instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateErrorLog(Exception e)
    {
        DateTime actualMoment = DateTime.Now;
        string actualMomentText = actualMoment.ToString("dd-MM-yyyy_HH-mm-ss");

        string directoryPath = Application.persistentDataPath + "/logError/" + actualMomentText + ".txt";
        string directory = Application.persistentDataPath + "/logError/";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Añadir al archivo toda la exception
        string exceptionText = e.ToString();
        File.WriteAllText(directoryPath, exceptionText);
    }
}

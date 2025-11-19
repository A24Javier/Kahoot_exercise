using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Xml;
using UnityEngine.Pool;

public class LeaderboardManager : MonoBehaviour
{
    private string xmlPath;

    // Datos en la UI
    [SerializeField] private TMP_Text[] usernameTexts;
    [SerializeField] private TMP_Text[] pointsTexts;
    [SerializeField] private TMP_Text[] timeTexts;

    // Datos recogidos del XML
    private List<string> usernames = new List<string>();
    private List<int> points = new List<int>();
    private List<float> times = new List<float>();

    void Start()
    {
        string kahootLoad = PlayerPrefs.GetString("KahootName", "Test mates");
        xmlPath = Application.persistentDataPath + "/leaderboards/" + kahootLoad + ".xml";

        LoadXML();
    }

    private void LoadXML()
    {
        try
        {
            if (File.Exists(xmlPath))
            {
                // Carga el documento
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);

                // Guardamos en una lista (pero no es una List<>) todo lo que tenga el tag userData
                XmlNodeList userData = xmlDoc.GetElementsByTagName("userData");

                // Por cada nodo dentro de 'userData'...
                foreach (XmlNode user in userData)
                {
                    // Creamos una lista de nodos y metemos todos los hijos de uno de los 'user'
                    XmlNodeList userContent = user.ChildNodes;
                    string username = "user";
                    int pointsObt = 0;
                    float time = 0f;

                    // Por cada contenido dentro de userData...
                    foreach (XmlNode content in userContent)
                    {
                        if (content.Name == "username")
                        {
                            username = content.InnerText;
                        }
                        else if(content.Name == "pointsObtained")
                        {
                            pointsObt = int.Parse(content.InnerText);
                        }
                        else if(content.Name == "timeElapsed")
                        {
                            time = float.Parse(content.InnerText);
                        }
                    }

                    usernames.Add(username);
                    points.Add(pointsObt);
                    times.Add(time);
                }
            }

            OrderLists();
        }
        catch(Exception e)
        {
            ExceptionHandler.instance.CreateErrorLog(e);
        }
    }

    private void OrderLists()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < points.Count - i - 1; j++)
            {
                bool shouldSwap = points[j + 1] > points[j] || (points[j + 1] == points[j] && times[j + 1] < times[j]);

                if (shouldSwap)
                {
                    int tmpPoints = points[j];
                    points[j] = points[j + 1];
                    points[j + 1] = tmpPoints;

                    string tmpUsername = usernames[j];
                    usernames[j] = usernames[j + 1];
                    usernames[j + 1] = tmpUsername;

                    float tmpTime = times[j];
                    times[j] = times[j + 1];
                    times[j + 1] = tmpTime;
                }
            }
        }

        ConfigureLeaderboard();
    }

    private void ConfigureLeaderboard()
    {
        for(int i = 0; i < usernames.Count; i++)
        {
            if (usernames[i] != null)
            {
                usernameTexts[i].text = usernames[i];
                pointsTexts[i].text = points[i].ToString("0 points");
                timeTexts[i].text = times[i].ToString("0.00 seconds");
            }
        }
    }
}

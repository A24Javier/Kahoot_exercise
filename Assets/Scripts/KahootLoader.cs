using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KahootLoader : MonoBehaviour
{
    //private string testPath;
    private string testLoaded;

    // Cosas para el test
    [HideInInspector] public List<string> questions = new List<string>();
    [HideInInspector] public List<PossibleAnswers> answers = new List<PossibleAnswers>();
    [HideInInspector] public List<int> maxPoints = new List<int>();
    [HideInInspector] public List<float> maxTimes = new List<float>();
    public List<Sprite> questionImages = new List<Sprite>();

    public static KahootLoader instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        testLoaded = PlayerPrefs.GetString("TestCargar");
        //testLoaded = Application.dataPath + "/Archivos/Tests/" + "testTrol.json";
        /*testPath = Application.dataPath + "/Archivos/Tests/" + testLoaded;
        Debug.Log(testPath);*/

        LoadTest();
    }

    private void LoadTest()
    {
        try
        {
            string testJson = File.ReadAllText(testLoaded);
            TestInfo testInfo = JsonUtility.FromJson<TestInfo>(testJson);
            PlayerPrefs.SetString("KahootName", testInfo.testName);

            foreach (Question i in testInfo.preguntas)
            {
                questions.Add(i.pregunta);
                maxPoints.Add(i.maximoPuntos);
                maxTimes.Add(i.tiempo);
                Sprite sprite = Resources.Load<Sprite>(i.imagen);

                if (sprite != null)
                {
                    questionImages.Add(sprite);
                }
                else
                {
                    questionImages.Add(null);
                }

                foreach (PossibleAnswers j in i.posiblesRespuestas)
                {
                    answers.Add(new PossibleAnswers
                    {
                        respuesta = j.respuesta,
                        esCorrecto = j.esCorrecto,
                    });
                }
            }

            KahootController.instance.StartNewQuestion();
        }
        catch(Exception e)
        {
            ExceptionHandler.instance.CreateErrorLog(e);
        }
    }
}

[Serializable]
public class TestInfo
{
    public string testName;
    public List<Question> preguntas;
}

[Serializable]
public class Question
{
    public string pregunta;
    public List<PossibleAnswers> posiblesRespuestas = new List<PossibleAnswers>();
    public int maximoPuntos;
    public float tiempo;
    public string imagen;
}

[Serializable]
public class PossibleAnswers
{
    public string respuesta;
    public bool esCorrecto;
}



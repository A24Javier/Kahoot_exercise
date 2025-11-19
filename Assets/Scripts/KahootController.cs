using System.Collections;
using System.IO;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KahootController : MonoBehaviour
{
    [Header("Botones respuesta")]
    [SerializeField] private Button[] answerButtons;

    [Header("UI pregunta")]
    [SerializeField] private CanvasGroup canvasQuestion;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image questionImage;
    [SerializeField] private Button buttonNextQuestion;

    [Header("Loading screen")]
    [SerializeField] private CanvasGroup canvasLoading;
    [SerializeField] private TMP_Text loadingTimeText;

    private int questionIndex;
    private int correctIndex;
    private int obtainedPoints;

    private float timeLeft;
    private float totalTime;
    private bool timeRunning = false;

    public static KahootController instance;

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

    void Update()
    {
        if (timeRunning)
        {
            UpdateTime();
        }
        
    }

    public void StartNewQuestion()
    {
        if(questionIndex < KahootLoader.instance.questions.Count)
        {
            buttonNextQuestion.gameObject.SetActive(false);
            SetCanvasGroup(canvasQuestion, false);
            SetCanvasGroup(canvasLoading, true);
            StartCoroutine(WaitNewQuestion());
        }
        else
        {
            SaveKahootData();
        }
        
    }

    private IEnumerator WaitNewQuestion()
    {
        float waitTime = 3f;

        while(waitTime > 0)
        {
            loadingTimeText.text = waitTime.ToString("0");
            waitTime -= Time.deltaTime;
            yield return null;
        }

        SetCanvasGroup(canvasQuestion, true);
        SetCanvasGroup(canvasLoading, false);

        SetNextQuestion();
    }

    private void SetNextQuestion()
    {
        questionText.text = KahootLoader.instance.questions[questionIndex];

        for(int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].interactable = true;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = KahootLoader.instance.answers[i+(questionIndex*4)].respuesta;
            if(KahootLoader.instance.answers[i + (questionIndex * 4)].esCorrecto)
            {
                answerButtons[i].onClick.AddListener(CorrectAnswer);
                correctIndex = i;
            }
            else
            {
                answerButtons[i].onClick.AddListener(IncorrectAnswer);
            }
        }

        questionImage.sprite = KahootLoader.instance.questionImages[questionIndex];
        timeLeft = KahootLoader.instance.maxTimes[questionIndex];
        timeText.text = timeLeft.ToString();
        timeRunning = true;
    }

    private void UpdateTime()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            totalTime += Time.deltaTime;
            timeText.text = timeLeft.ToString("0");
        }
        else if(timeLeft <= 0)
        {
            IncorrectAnswer();
        }
    }

    private void CorrectAnswer()
    {
        int maxPoints = KahootLoader.instance.maxPoints[questionIndex];
        int maxTimeOfQuestion = Mathf.RoundToInt(KahootLoader.instance.maxTimes[questionIndex]);
        float questionPoints = ((1 - (((maxTimeOfQuestion - timeLeft) / maxTimeOfQuestion) / 2)) * maxPoints);
        int questionsPointsInt = Mathf.RoundToInt(questionPoints);
        obtainedPoints += questionsPointsInt;
        questionText.text = "¡Correcto! ¡Has obtenido " + questionsPointsInt + " puntos!";
        ShowCorrectAnswer();
    }

    private void IncorrectAnswer()
    {
        questionText.text = "¡Incorrecto! El resultado era: " + KahootLoader.instance.answers[correctIndex + (questionIndex * 4)].respuesta; ;
        ShowCorrectAnswer();
    }

    private void ShowCorrectAnswer()
    {
        timeRunning = false;
        for(int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
            if(correctIndex != i)
            {
                Color buttonColor = answerButtons[i].GetComponent<Image>().color;
                answerButtons[i].GetComponent<Image>().color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 128);
            }
        }

        questionIndex++;
        buttonNextQuestion.gameObject.SetActive(true);
    }

    private void SetCanvasGroup(CanvasGroup canvas, bool isActive)
    {
        canvas.alpha = isActive ? 1 : 0;
        canvas.blocksRaycasts = isActive;
        canvas.interactable = isActive;
    }

    private void SaveKahootData()
    {
        string username = PlayerPrefs.GetString("Username", "testUser");
        string kahootName = PlayerPrefs.GetString("KahootName", "kahootTest");
        string folderPath = Application.persistentDataPath + "/leaderboards";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string path = folderPath + "/" + kahootName + ".xml";

        XmlDocument xmlDoc = new XmlDocument();
        XmlElement body;

        // Si el archivo ya existe, lo cargamos
        if (File.Exists(path))
        {
            xmlDoc.Load(path);
            body = xmlDoc.DocumentElement;
        }
        else
        {
            // Crear nuevo documento XML
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            // Crear el elemento body
            body = xmlDoc.CreateElement("body");
            xmlDoc.AppendChild(body);
        }

        // Creamos el elemento userData y lo hacemos hijo del body
        XmlElement userData = xmlDoc.CreateElement(string.Empty, "userData", string.Empty);
        body.AppendChild(userData);

        // Guardar nombre de usuario
        XmlElement usernameXML = xmlDoc.CreateElement(string.Empty, "username", string.Empty);
        userData.AppendChild(usernameXML);

        XmlText usernameTextXML = xmlDoc.CreateTextNode(username);
        usernameXML.AppendChild(usernameTextXML);

        // Guardar tiempo tardado
        XmlElement timeElapsedXML = xmlDoc.CreateElement(string.Empty, "timeElapsed", string.Empty);
        userData.AppendChild(timeElapsedXML);

        XmlText timeElapsedTextXML = xmlDoc.CreateTextNode(totalTime.ToString("0.00"));
        timeElapsedXML.AppendChild(timeElapsedTextXML);

        // Guardar puntuación
        XmlElement pointsObtainedXML = xmlDoc.CreateElement(string.Empty, "pointsObtained", string.Empty);
        userData.AppendChild(pointsObtainedXML);

        XmlText pointsObtainedTextXML = xmlDoc.CreateTextNode(obtainedPoints.ToString());
        pointsObtainedXML.AppendChild(pointsObtainedTextXML);

        // Generar archivo XML
        xmlDoc.Save(path);

        // Ir a leaderboards para mostrar los resultados
        SceneController.instance.ChangeScene("KahootLeaderboard");
    }
}

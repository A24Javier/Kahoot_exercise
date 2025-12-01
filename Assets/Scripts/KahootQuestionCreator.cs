using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.Multiplayer.Center.Common;
using System.IO;

public class KahootQuestionCreator : MonoBehaviour
{
    [Header("Canvas groups")]
    [SerializeField] private CanvasGroup grpNameAndSize;
    [SerializeField] private CanvasGroup grpCreateQuestion;
    [SerializeField] private CanvasGroup grpKahootCreated;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField inputKahootName;
    [SerializeField] private TMP_InputField inputTotalQuestions;
    [SerializeField] private TMP_InputField inputQuestion;
    [SerializeField] private TMP_InputField[] inputAnswers;
    [SerializeField] private TMP_InputField inputMaxTime;
    [SerializeField] private TMP_InputField inputMaxPoints;

    [Header("Toggles")]
    [SerializeField] private Toggle[] correctAnsToggles;

    [Header("Texts")]
    [SerializeField] private TMP_Text actualQuestionText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Values")]
    private TestInfo test;
    private int maxQuestions = 1;
    private int actualQuestion = 1;


    void Start()
    {
        test = new TestInfo();
        GRPNameSizeController(true);
        GRPCreateQuestionController(false);
        GRPKahootCreatedController(false);
        AddListeners();
        CheckInitialConf();

        actualQuestionText.text = actualQuestion.ToString("Pregunta #0:");
    }

    private void AddListeners()
    {
        continueButton.onClick.AddListener(delegate { ContinueToQuestions(); });
        inputKahootName.onValueChanged.AddListener(delegate { CheckInitialConf(); });
        inputTotalQuestions.onValueChanged.AddListener(delegate { CheckInitialConf(); });

        inputQuestion.onValueChanged.AddListener(delegate { CheckQuestionConf(); });

        for(int i = 0; i < inputAnswers.Length; i++)
        {
            inputAnswers[i].onValueChanged.AddListener(delegate { CheckQuestionConf(); });
        }

        for(int i = 0; i < correctAnsToggles.Length; i++)
        {
            correctAnsToggles[i].onValueChanged.AddListener(delegate { CheckQuestionConf(); });
        }

        inputMaxTime.onValueChanged.AddListener(delegate { CheckQuestionConf(); });
        inputMaxPoints.onValueChanged.AddListener(delegate { CheckQuestionConf(); });
    }

    private void CheckInitialConf()
    {
        bool interactable = true;

        if(inputKahootName.text.Length <= 0)
        {
            interactable = false;
        }

        if(inputTotalQuestions.text.Length <= 0)
        {
            interactable = false;
        }

        continueButton.interactable = interactable;
    }

    private void CheckQuestionConf()
    {
        bool interactable = true;

        if (inputQuestion.text.Length <= 0)
        {
            interactable = false;
        }

        bool thereAreCorrectAns = false;

        for(int i = 0; i < inputAnswers.Length; i++)
        {
            if (inputAnswers[i].text.Length <= 0)
            {
                interactable = false;
            }

            if (correctAnsToggles[i].isOn)
            {
                thereAreCorrectAns = true;
            }
        }

        if(!thereAreCorrectAns)
        {
            interactable = false;
        }

        if (inputMaxTime.text.Length <= 0)
        {
            interactable = false;
        }
        
        if (inputMaxPoints.text.Length <= 0)
        {
            interactable = false;
        }

        continueButton.interactable = interactable;
    }

    private void ContinueToQuestions()
    {
        test.testName = inputKahootName.text;
        test.preguntas = new List<Question>();
        maxQuestions = int.Parse(inputTotalQuestions.text);

        GRPNameSizeController(false);
        GRPCreateQuestionController(true);
        continueButton.onClick.AddListener(delegate { CreateQuestion(); });
        CheckQuestionConf();
        actualQuestionText.text = actualQuestion.ToString("Pregunta #0:");
    }

    private void CreateQuestion()
    {
        Question question = new Question();
        question.pregunta = inputQuestion.text;

        question.posiblesRespuestas = new List<PossibleAnswers>();

        for (int i = 0; i < inputAnswers.Length; i++)
        {
            PossibleAnswers answer = new PossibleAnswers();
            answer.respuesta = inputAnswers[i].text;
            answer.esCorrecto = correctAnsToggles[i].isOn;

            question.posiblesRespuestas.Add(answer);
        }

        question.maximoPuntos = int.Parse(inputMaxPoints.text);
        question.tiempo = float.Parse(inputMaxTime.text);

        test.preguntas.Add(question);

        actualQuestion++;

        if(actualQuestion >= maxQuestions)
        {
            GRPCreateQuestionController(false);
            continueButton.onClick.AddListener(delegate { ContinueToQuestions(); });
            CreateTest();
        }
        else
        {
            actualQuestionText.text = actualQuestion.ToString("Pregunta #0:");
        }

        EmptyInputs();
    }

    private void CreateTest()
    {
        string json = JsonUtility.ToJson(test, true);

        string path = Application.persistentDataPath + "/tests/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(path + test.testName + ".json", json);

        StartCoroutine(KahootCreated());
    }

    private void EmptyInputs()
    {
        inputQuestion.text = null;

        for(int i = 0; i < inputAnswers.Length; i++)
        {
            inputAnswers[i].text = null;
            correctAnsToggles[i].isOn = false;
        }

        inputMaxPoints.text = null;
        inputMaxTime.text = null;
    }

    private void EmptyInitialConf()
    {
        inputKahootName.text = null;
        inputTotalQuestions.text = null;
    }

    private void GRPNameSizeController(bool show)
    {
        grpNameAndSize.alpha = show ? 1 : 0;
        grpNameAndSize.interactable = show;
        grpNameAndSize.blocksRaycasts = show;
    }
    
    private void GRPCreateQuestionController(bool show)
    {
        grpCreateQuestion.alpha = show ? 1 : 0;
        grpCreateQuestion.interactable = show;
        grpCreateQuestion.blocksRaycasts = show;
    }

    private void GRPKahootCreatedController(bool show)
    {
        grpKahootCreated.alpha = show ? 1 : 0;
        grpKahootCreated.interactable = show;
        grpKahootCreated.blocksRaycasts = show;
    }

    private IEnumerator KahootCreated()
    {
        actualQuestion = 1;
        GRPKahootCreatedController(true);
        yield return new WaitForSeconds(5f);
        test = new TestInfo();
        EmptyInitialConf();
        CheckInitialConf();
        GRPKahootCreatedController(false);
        GRPNameSizeController(true);
    }
}


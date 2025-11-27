using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KahootQuestionCreator : MonoBehaviour
{
    [Header("Canvas groups")]
    [SerializeField] private CanvasGroup grpNameAndSize;
    [SerializeField] private CanvasGroup grpCreateQuestion;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField inputKahootName;
    [SerializeField] private TMP_InputField inputTotalQuestions;
    [SerializeField] private TMP_InputField inputQuestion;
    [SerializeField] private TMP_InputField[] inputAnswers;
    [SerializeField] private TMP_InputField inputMaxTime;
    [SerializeField] private TMP_InputField inputMaxPoints;

    [Header("Texts")]
    [SerializeField] private TMP_Text actualQuestionText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        GRPNameSizeController(true);
        GRPCreateQuestionController(false);
        continueButton.onClick.AddListener(delegate { ContinueToQuestions(); });
    }

    void Update()
    {
        
    }

    private void ContinueToQuestions()
    {
        continueButton.onClick.AddListener(delegate { ContinueToCreateKahoot(); });
    }

    private void ContinueToCreateKahoot()
    {

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
}

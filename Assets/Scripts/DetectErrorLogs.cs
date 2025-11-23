using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectErrorLogs : MonoBehaviour
{
    private string testPersistentPath;

    [Header("UI Elements")]
    [SerializeField] private GameObject prefabButton;
    [SerializeField] private GameObject buttonsList;
    private List<Button> buttonList = new List<Button>();
    [SerializeField] private TMP_Text errorMessageText;

    void Start()
    {
        testPersistentPath = Application.persistentDataPath + "/logError/";

        AddLogButtons();
    }

    private void AddLogButtons()
    {
        try
        {
            string[] logFiles = Directory.GetFiles(testPersistentPath, "*.txt");

            foreach (string filePath in logFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Debug.Log(fileName);
                GameObject newButton = Instantiate(prefabButton, Vector3.zero, Quaternion.identity, buttonsList.transform);
                TMP_Text textButton = newButton.GetComponentInChildren<TMP_Text>();
                textButton.text = fileName;

                newButton.GetComponent<Button>().onClick.AddListener(delegate { OnLogSelected(newButton); });
                newButton.GetComponent<KahootButtonData>().SetFileName(fileName);

                buttonList.Add(newButton.GetComponent<Button>());
            }
        }
        catch (Exception e)
        {
            ExceptionHandler.instance.CreateErrorLog(e);
        }
    }

    private void OnLogSelected(GameObject button)
    {
        string filePath = testPersistentPath + button.GetComponent<KahootButtonData>().GetFileName() + ".txt";
        Debug.Log(filePath);
        errorMessageText.text = File.ReadAllText(filePath);
    }
}

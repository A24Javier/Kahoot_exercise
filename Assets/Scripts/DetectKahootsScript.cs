using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectKahootsScript : MonoBehaviour
{
    private string playerUsername = "defaultUser";

    private string testAssetsPath;
    private string testPersistentPath;

    [Header("UI Elements")]
    [SerializeField] private GameObject prefabButton;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private GameObject buttonsList;
    private List<Button> buttonList = new List<Button>();

    void Start()
    {
        testAssetsPath = Application.dataPath + "/Archivos/Tests/";
        testPersistentPath = Application.persistentDataPath + "/tests/";

        inputUsername.text = playerUsername;
        inputUsername.onValueChanged.AddListener(delegate { OnInputChange(inputUsername.text); });

        AddTestButtons();
    }

    private void AddTestButtons()
    {
        try
        {
            string[] jsonFiles = Directory.GetFiles(testAssetsPath, "*.json");

            foreach (string filePath in jsonFiles)
            {
                string jsonText = File.ReadAllText(filePath);
                TestName data = JsonUtility.FromJson<TestName>(jsonText);

                GameObject newButton = Instantiate(prefabButton, Vector3.zero, Quaternion.identity, buttonsList.transform);
                TMP_Text textButton = newButton.GetComponentInChildren<TMP_Text>();
                textButton.text = data.testName;
                newButton.GetComponent<Button>().onClick.AddListener(delegate { OnKahootSelected(newButton); });
                newButton.GetComponent<KahootButtonData>().SetFileName(filePath);
                buttonList.Add(newButton.GetComponent<Button>());
            }

            if (Directory.Exists(testPersistentPath))
            {
                string[] jsonFiles2 = Directory.GetFiles(testPersistentPath, "*.json");

                foreach (string filePath in jsonFiles2)
                {
                    string jsonText = File.ReadAllText(filePath);
                    TestName data = JsonUtility.FromJson<TestName>(jsonText);

                    GameObject newButton = Instantiate(prefabButton, Vector3.zero, Quaternion.identity, buttonsList.transform);
                    TMP_Text textButton = newButton.GetComponentInChildren<TMP_Text>();
                    textButton.text = data.testName;
                    newButton.GetComponent<Button>().onClick.AddListener(delegate { OnKahootSelected(newButton); });
                    newButton.GetComponent<KahootButtonData>().SetFileName(filePath);
                    buttonList.Add(newButton.GetComponent<Button>());
                }
            }
        }
        catch (Exception e)
        {
            ExceptionHandler.instance.CreateErrorLog(e);
        }
    }

    private void OnInputChange(string newUsername)
    {
        playerUsername = newUsername;

        Debug.Log("New username: " + playerUsername);

        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].interactable = playerUsername == "" ? false : true;
        }
    }

    private void OnKahootSelected(GameObject button)
    {
        PlayerPrefs.SetString("TestCargar", button.GetComponent<KahootButtonData>().GetFileName());
        PlayerPrefs.SetString("Username", playerUsername);
        SceneController.instance.ChangeScene("KahootTest");
    }

}


[Serializable]
public class TestName
{
    public string testName;
}

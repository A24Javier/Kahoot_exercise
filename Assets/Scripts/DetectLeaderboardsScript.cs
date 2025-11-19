using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectLeaderboardsScript : MonoBehaviour
{
    private string testPersistentPath;

    [Header("UI Elements")]
    [SerializeField] private GameObject prefabButton;
    [SerializeField] private GameObject buttonsList;
    private List<Button> buttonList = new List<Button>();

    void Start()
    {
        //string kahootLoad = PlayerPrefs.GetString("KahootName", "testMates");
        testPersistentPath = Application.persistentDataPath + "/leaderboards/";

        AddTestButtons();
    }

    private void AddTestButtons()
    {
        try
        {
            string[] xmlFiles = Directory.GetFiles(testPersistentPath, "*.xml");

            foreach (string filePath in xmlFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                GameObject newButton = Instantiate(prefabButton, Vector3.zero, Quaternion.identity, buttonsList.transform);
                TMP_Text textButton = newButton.GetComponentInChildren<TMP_Text>();
                textButton.text = fileName;

                newButton.GetComponent<Button>().onClick.AddListener(delegate { OnKahootSelected(newButton); });
                newButton.GetComponent<KahootButtonData>().SetFileName(fileName);

                buttonList.Add(newButton.GetComponent<Button>());
            }
        }
        catch (Exception e)
        {
            ExceptionHandler.instance.CreateErrorLog(e);
        }
    }

    private void OnKahootSelected(GameObject button)
    {
        PlayerPrefs.SetString("KahootName", button.GetComponent<KahootButtonData>().GetFileName());
        SceneController.instance.ChangeScene("KahootLeaderboard");
    }
}
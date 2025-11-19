using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

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

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    private void OnLevelWasLoaded(int level)
    {
        switch (level)
        {
            case 0:
                GameObject.Find("Button_seleccionKahoot").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("KahootSelection"); });
                GameObject.Find("Button_Puntuaciones").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("KahootLeaderboardSelection"); });
                GameObject.Find("Button_Creditos").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("About"); });
                break;
            case 1:
                GameObject.Find("Return_button").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("Menu"); });
                break;
            case 3:
                FindFirstObjectByType<Button>().onClick.AddListener(delegate { ChangeScene("Menu"); });
                break;
            case 4:
                GameObject.Find("Return_button").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("Menu"); });
                break;
            case 5:
                GameObject.Find("Itchio_button").GetComponent<Button>().onClick.AddListener(delegate { OpenURL("https://javiersc.itch.io/"); });
                GameObject.Find("Return_button").GetComponent<Button>().onClick.AddListener(delegate { ChangeScene("Menu"); });
                break;
        }
    }
}

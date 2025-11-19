using UnityEngine;

public class KahootButtonData : MonoBehaviour
{
    private string fileName; // Fichero kahoot asociado

    public void SetFileName(string newFileName)
    {
        fileName = newFileName;
    }

    public string GetFileName()
    {
        return fileName;
    }
}

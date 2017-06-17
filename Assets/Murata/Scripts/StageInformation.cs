using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageInformation : MonoBehaviour
{
    private static string nextSceneName;

    void Start()
    {

    }

    public void StageName(string SteageName)
    {
        
        string[] SteageNumber = SteageName.Split();
        string[] SteageLabel = SteageName.Split('-');
        string[] SteageClass = SteageName.Split();

        Debug.Log(string.Format(""));

        // SceneManager.LoadScene(nextSceneName);

    }
    void Update()
    {

    }
}


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
        
        string[] SteageLabel = SteageName.Split('-');  

        Debug.Log(string.Format("新たなるステージ、{0}",SteageName));
        GameManager.SetStageData(int.Parse(SteageLabel[0]),int.Parse(SteageLabel[1]));
        SceneManager.LoadScene(nextSceneName);

    }
    void Update()
    {

    }
}


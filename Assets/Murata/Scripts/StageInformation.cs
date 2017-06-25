using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageInformation : MonoBehaviour
{
    //private static string nextSceneName;
	bool isMovingScene = false;

	void Start()
    {
		//BGM再生
		AudioManager.FadeIn(2, BGMType.Select, 1, true);
	}

    public void StageName(string SteageName)
    {
		if (isMovingScene) return;
		isMovingScene = true;

        string[] SteageLabel = SteageName.Split('-');  

        Debug.Log(string.Format("新たなるステージ、{0}",SteageName));
        GameManager.SetStageData(int.Parse(SteageLabel[0]),int.Parse(SteageLabel[1]));

		//BGMフェードアウト
		AudioManager.FadeOut(2);

        SumCanvasAnimation.MoveScene("GameScene");

    }
    void Update()
    {

    }
}


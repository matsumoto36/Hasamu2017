using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

	bool isMovingScene = false;
	bool isFreeze = true;

	// Use this for initialization
	void Start () {

		//BGM再生
		AudioManager.FadeIn(2, BGMType.Title, 1, true);

		//アニメーション再生
		StartCoroutine(TitleAnim());
	}
	
	// Update is called once per frame
	void Update () {
        if (!isFreeze && Input.GetMouseButtonDown(0))
        {
			MoveScene();
		}
    }

	/// <summary>
	/// シーン遷移
	/// </summary>
	void MoveScene() {

		if (isMovingScene) return;
		isMovingScene = true;

		//BGMフェードアウト
		AudioManager.FadeOut(2);

		//シーン遷移
		SumCanvasAnimation.MoveScene("StageSelectScene");
	}

	IEnumerator TitleAnim() {

		yield return new WaitForSeconds(2.0f);


		//フリーズ解除
		isFreeze = false;

	}
}

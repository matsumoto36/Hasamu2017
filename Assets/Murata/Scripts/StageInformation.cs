using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageInformation : MonoBehaviour
{
	//private static string nextSceneName;
	bool isMovingScene = false;

	public GameObject[] floorPanels;		//フロアの部屋を表示するパネル
	public Button[] floorButtons;			//パネルを表示するためのボタン

	void Start() {
		//BGM再生
		AudioManager.FadeIn(2, BGMType.Select, 1, true);

		//一階を表示
		FloorView(1);
	}

	/// <summary>
	/// 指定のフロアの部屋を表示
	/// </summary>
	/// <param name="floorNum">フロアの番号(1 ~)</param>
	public void FloorView(int floorNum) {
		for (int i = 0; i < floorButtons.Length; i++) {
			floorButtons[i].interactable = true;
			floorPanels[i].SetActive(false);
		}

		floorButtons[floorNum - 1].interactable = false;
		floorPanels[floorNum - 1].SetActive(true);
	}

	public void StageName(string SteageName) {
		if(isMovingScene) return;
		isMovingScene = true;

		string[] SteageLabel = SteageName.Split('-');

		Debug.Log(string.Format("新たなるステージ、{0}", SteageName));
		GameManager.SetStageData(int.Parse(SteageLabel[0]), int.Parse(SteageLabel[1]));

		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("GameScene");

	}

	/// <summary>
	/// タイトル画面に移動
	/// </summary>
	public void GotoTitle() {

		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("TitleScene");
	}
}
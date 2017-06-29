using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageInformation : MonoBehaviour
{
	static int lastSelectedFloor = 1;		//最後に選択されたフロア
	public GameObject[] floorPanels;		//フロアの部屋を表示するパネル
	public Button[] floorButtons;			//パネルを表示するためのボタン

	void Start() {

		FloorView(lastSelectedFloor);

		//BGM再生
		AudioManager.FadeIn(2, BGMType.Select, 1, true);

		//環境エフェクト表示
		var ps = ParticleManager.Play(ParticleType.AmbientEffect, Vector2.zero, Quaternion.identity);
		var s = ps.shape;
		s.box = new Vector2(1.5f, 1) * 10;
	}

	/// <summary>
	/// 指定のフロアの部屋を表示
	/// </summary>
	/// <param name="floorNum">フロアの番号(1 ~)</param>
	void FloorView(int floorNum) {

		//いったんすべてリセット
		for (int i = 0; i < floorButtons.Length; i++) {
			floorButtons[i].interactable = true;
			floorPanels[i].SetActive(false);
		}

		//選ばれたものを変更
		floorButtons[floorNum - 1].interactable = false;
		floorPanels[floorNum - 1].SetActive(true);

		//次回ロード用に保存
		lastSelectedFloor = floorNum;
	}

	/// <summary>
	/// 指定のフロアの部屋を表示するボタン
	/// </summary>
	/// <param name="floorNum">フロアの番号(1 ~)</param>
	public void FloorViewButton(int floorNum) {
		//本体を実行
		FloorView(floorNum);
		//音の再生
		AudioManager.Play(SEType.Button, 1);
	}

	public void StageName(string SteageName) {

		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;

		string[] SteageLabel = SteageName.Split('-');

		Debug.Log(string.Format("新たなるステージ、{0}", SteageName));
		GameManager.SetStageData(int.Parse(SteageLabel[0]), int.Parse(SteageLabel[1]));

		//音の再生
		AudioManager.Play(SEType.Button, 1);
		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("GameScene");

	}

	/// <summary>
	/// タイトル画面に移動
	/// </summary>
	public void GotoTitle() {

		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;

		//音の再生
		AudioManager.Play(SEType.Button, 1);
		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("TitleScene");
	}
}
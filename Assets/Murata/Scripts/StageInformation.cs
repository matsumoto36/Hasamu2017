using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageInformation : MonoBehaviour
{
	static int lastSelectedFloor = 1;		//最後に選択されたフロア
	public GameObject[] floorPanels;		//フロアの部屋を表示するパネル
	public Button[] floorButtons;           //パネルを表示するためのボタン

	static int[] stageCount = new int[] { 6, 6, 6, 6 };

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

	/// <summary>
	/// 次のステージを取得する
	/// </summary>
	/// <param name="stageLevel">現在のステージのレベル</param>
	/// <param name="stageNum">現在のステージ</param>
	/// <param name="stageStr">次のステージの文字列</param>
	/// <returns>取得できたか</returns>
	public static bool GetNextFloor(int stageLevel, int stageNum, out string stageStr) {

		//値の初期化
		stageStr = "";

		//次のステージ
		stageNum++;

		//そのフロアのステージ数よりも超えたら、次のステージのレベルに
		int count = stageCount[stageLevel - 1];
		if (count < stageNum) {
			if (stageLevel >= stageCount.Length) return false;

			stageLevel++;
			stageNum = 1;
			
		}

		//作成して終了
		stageStr = stageLevel + "-" + stageNum;
		return true;
	}

	/// <summary>
	/// ボタン用
	/// </summary>
	/// <param name="stageName"></param>
	public void StageName(string stageName) {

		GoGameScene(stageName);

	}

	/// <summary>
	/// ゲームシーンに行く
	/// </summary>
	/// <param name="stageName"></param>
	public static void GoGameScene(string stageName) {
		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;

		string[] SteageLabel = stageName.Split('-');

		Debug.Log(string.Format("新たなるステージ、{0}", stageName));
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
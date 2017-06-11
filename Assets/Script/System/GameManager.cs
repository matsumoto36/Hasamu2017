using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	static int stageLevel, stageNum;

	public Vector2 clearPosition = new Vector2(3, 4);

	void Awake() {
		GameInitiarize();
	}

	/// <summary>
	/// ゲームの初期設定
	/// </summary>
	void GameInitiarize() {

		//音設定読み込み
		FindObjectOfType<AudioManager>().Initiarize();

		//マップチップのロード
		FindObjectOfType<ResourceLoader>().LoadChip();

		//後にCSVから読み込む
		int[,] map = new int[,] {
			{1, 1, 1, 1, 1, 1, 2, 2, 2},
			{1, 0, 0, 0, 2, 0, 0, 0, 1},
			{1, 0, 0, 0, 4, 5, 0, 0, 1},
			{1, 0, 0, 0, 3, 0, 0, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		//ステージの生成
		StageGenerator.GenerateMap(map);

		//クリア場所の作成
		Hole.CreateHole(clearPosition);
	}

	/// <summary>
	/// ゲームを開始する
	/// </summary>
	public static void GameStart() {

		//カウントダウン開始
		Timebar.StartTimer();
	}

	/// <summary>
	/// ゲームオーバー
	/// </summary>
	public static void GameOver() {
		Debug.Log("GameOver");
	}

	/// <summary>
	/// ゲームクリア
	/// </summary>
	public static void GameClear() {
		Debug.Log("GameClear");

	}
}

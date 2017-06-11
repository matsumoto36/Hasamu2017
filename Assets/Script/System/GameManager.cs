using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	void Awake() {
		GameInitiarize();
	}

	/// <summary>
	/// ゲームの初期設定
	/// </summary>
	static void GameInitiarize() {

		//音設定読み込み
		FindObjectOfType<AudioManager>().Initiarize();

		//マップチップのロード
		FindObjectOfType<ResourceLoader>().LoadChip();

		//後にCSVから読み込む
		int[,] map = new int[,] {
			{1, 1, 1, 1, 1, 1, 2, 2, 2},
			{1, 0, 0, 0, 3, 0, 0, 0, 1},
			{1, 0, 0, 0, 4, 5, 0, 0, 1},
			{1, 0, 0, 0, 3, 4, 5, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		//ステージの生成
		StageGenerator.GenerateMap(map);
	}

	/// <summary>
	/// ゲームを開始する
	/// </summary>
	public void GameStart() {

	}

	/// <summary>
	/// ゲームオーバー
	/// </summary>
	public void GameOver() {

	}

	/// <summary>
	/// ゲームクリア
	/// </summary>
	public void GameClear() {

	}
}

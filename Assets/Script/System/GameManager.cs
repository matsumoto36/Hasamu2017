using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	int stageLevel, stageNum;	//生成するステージ
	float limitTime = 100;			//制限時間

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
		//int[,] map = new int[,] {
		//	{1, 1, 1, 1, 1, 1, 2, 1, 2},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 0, 0, -1, 5, 0, 0, 0, 1},
		//	{1, 0, 0, 0, 0, 3, 0, 0, 1},
		//	{1, 0, 0, 0, 0, 0, 6, 0, 1},
		//	{1, 1, 1, 1, 1, 1, 1, 1, 1},
		//};

		//int[,] map = new int[,] {
		//	{1, 1, 1, 1, 1, 1, 2, 1, 2},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 0, -1, 4, 0, 3, 0, 0, 1},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 1, 1, 1, 1, 1, 1, 1, 1},
		//};

		//int[,] map = new int[,] {
		//	{1, 1, 1, 1, 1, 1, 2, 1, 1},
		//	{1, 0, 0, 0, 2, 0, 0, 0, 1},
		//	{1, 0, 0, -1, 4, 0, 3, 0, 1},
		//	{1, 0, 0, 0, 2, 4, 0, 0, 1},
		//	{1, 0, 0, 0, 2, 0, 0, 0, 1},
		//	{1, 1, 1, 1, 1, 1, 1, 1, 1},
		//};

		int[,] map = new int[,] {
			{1, 2, 2, 2, 1, 1, 1, 1, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 0, 0, -1, 0, 0, 0, 0, 1},
			{1, 0, 4, 0, 0, 0, 0, 0, 1},
			{1, 0, 0, 3, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		//ステージの生成
		StageGenerator.GenerateMap(map);

		//制限時間の設定
		Timebar.time = limitTime;
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

		//カウントダウンストップ


	}

	public void ShowMenu() {
		
	}
}

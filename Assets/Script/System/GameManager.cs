using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	static int stageLevel, stageNum;	//生成するステージ
	static float limitTime = 100;           //制限時間

	public Text stageText;

	void Awake() {

		//後にほかのところから割り当てられる
		stageLevel = stageNum = 1;

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

        //int[,] map = new int[,] {
        //	{1, 1, 1, 1, 1, 1, 1, 1, 1},
        //	{1, 0, 0, 0, 0, 0, 0, 0, 1},
        //	{1, 0, 0, -1, 0, 0, 0, 1, 1},
        //	{1, 0, 4, 0, 0, 0, 0, 0, 1},
        //	{1, 0, 0, 0, 0, 3, 0, 0, 1},
        //	{1, 1, 1, 1, 1, 1, 1, 1, 1},
        //};

        Timebar.time = CsvLoader.StageLoad(stageLevel, stageNum).time;
        int[,] map = CsvLoader.StageLoad(stageLevel, stageNum).mapData;

		//ステージの生成
		//StageData stageData = CsvLoader.StageLoad(stageLevel, stageNum);
		//StageGenerator.GenerateMap(stageData.mapData);
		StageGenerator.GenerateMap(map);

		//制限時間の設定
		//Timebar.time = stageData.time;
		Timebar.time = limitTime;

		//テキストの設定
		stageText.text = string.Format("{0} - {1}", stageLevel, stageNum);
	}

	/// <summary>
	/// 生成するステージの情報をセットする
	/// </summary>
	/// <param name="data">ステージのデータ</param>
	public static void SetStageData(int stageLevel, int stageNum) {
		GameManager.stageLevel = stageLevel;
		GameManager.stageNum = stageNum;
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
}

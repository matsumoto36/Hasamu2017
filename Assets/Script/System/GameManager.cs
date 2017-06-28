using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	static GameManager myManager;
	static int stageLevel, stageNum;	//生成するステージ
	static float limitTime = 9999;           //制限時間

	public Text stageText;

	bool isBGMSwitch = false;

	void Start() {

		//後にほかのところから割り当てられる
		stageLevel = stageNum = 1;

		myManager = this;

		GameInitiarize();
	}

	/// <summary>
	/// ゲームの初期設定
	/// </summary>
	void GameInitiarize() {

		//マップチップのロード
		FindObjectOfType<ResourceLoader>().LoadAll();

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

		//int[,] map = new int[,] {
		//	{1, 1, 1, 1, 1, 1, 1, 1, 1},
		//	{1, 0, 0, 3, 0, 0, 0, 0, 1},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 0, 4, 5, 6, 0, -1, 0, 1},
		//	{1, 0, 0, 0, 0, 0, 0, 0, 1},
		//	{1, 1, 1, 1, 1, 1, 1, 1, 1},
		//};

		//14:9
		int[,] map = new int[,] {
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 0, 0, 3, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1},
			{1, 0, 4, 5, 6, 0, -1, 0, 1, 1, 0, 0, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		//Timebar.time = CsvLoader.StageLoad(stageLevel, stageNum).time;
        //int[,] map = CsvLoader.StageLoad(stageLevel, stageNum).mapData;

		//ステージの生成
		StageData stageData = CsvLoader.StageLoad(stageLevel, stageNum);
		StageGenerator.GenerateMap(stageData.mapData);
		//StageGenerator.GenerateMap(map);

		//制限時間の設定
		Timebar.StopTimer();
		Timebar.Decpersec = 1;
		Timebar.time = stageData.time;

		//テキストの設定
		stageText.text = string.Format("{0} - {1}", stageLevel, stageNum);

		//音楽を再生
		AudioManager.FadeIn(2.0f, BGMType.Game, 1, true);

		//入力の許可
		InputManager.isFreeze = false;
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


	void Update() {

		if (Input.GetKeyDown(KeyCode.B)) {
			DebugPause();
		}

		//のこり10秒以下になったらBGM変更
		if (!isBGMSwitch && Timebar.time <= 10) {
			isBGMSwitch = true;

			AudioManager.FadeOut(1);
			AudioManager.FadeIn(1, BGMType.Game2, 1, true);
		}

	}

	/// <summary>
	/// ゲームオーバー
	/// </summary>
	public static void GameOver() {
		Debug.Log("GameOver");

		myManager.StartCoroutine(myManager.GameOverAnim());
	}
	IEnumerator GameOverAnim() {

		//入力禁止
		InputManager.isFreeze = true;

		//BGMフェード
		AudioManager.FadeOut(2.0f);

		//爆発
		AudioManager.Play(SEType.BombExplosion);
		ParticleManager.PlayOneShot(ParticleType.BombBlast, FindObjectOfType<PieceBomb>().transform.position, Quaternion.identity, 5);
		Player.DestroyCurrentContainer();
		PieceBomb pb = FindObjectOfType<PieceBomb>();
		StageGenerator.RemovePiece(pb);
		Destroy(pb.gameObject);

		yield return new WaitForSeconds(2.0f);

		//BGM再生
		AudioManager.Play(BGMType.Over, 1, true);

		//画面表示
		Gameview.GameOverView();

	}


	/// <summary>
	/// ゲームクリア
	/// </summary>
	public static void GameClear() {
		Debug.Log("GameClear");

		myManager.StartCoroutine(myManager.GameClearAnim());
	}
	IEnumerator GameClearAnim() {

		//入力禁止
		InputManager.isFreeze = true;

		//落ちるアニメーション
		yield return new WaitForSeconds(1.0f);

		//BGMフェード
		AudioManager.FadeOut(2.0f);

		yield return new WaitForSeconds(2.0f);

		//BGM再生
		AudioManager.Play(BGMType.Clear, 1, true);

		//画面表示
		Gameview.GameClearView();

	}
	public static void DebugPause() {
		Debug.Break();
	}
}

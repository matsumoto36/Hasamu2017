﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	static GameManager myManager;


	public static int stageLevel = 1, stageNum = 1;		//生成するステージ

	public string debug_loadStage = "";					//デバッグ用でロードするステージ
	static float limitTime = 9999;						//制限時間


	public Text stageText;
	public Text editModeText;


	bool isBGMSwitch = false;

	//エディットモード系
	public static bool IsEditMode;
	EditModeMain editModeMain;


	void Start() {

		StageInformation.lastSelectedFloor = stageLevel;

		myManager = this;
		editModeText.gameObject.SetActive(false);


		//エディットモードお知らせ
		if(IsEditMode) {
			editModeMain = FindObjectOfType<EditModeMain>();
			editModeText.gameObject.SetActive(true);
		}

		GameInitiarize();
	}

	/// <summary>
	/// ゲームの初期設定
	/// </summary>
	void GameInitiarize() {

		//マップチップのロード
		FindObjectOfType<ResourceLoader>().LoadAll();

		//デバッグに何か入ってたら優先する
		if(debug_loadStage != "") {
			string[] bff = debug_loadStage.Split('-');
			stageLevel = int.Parse(bff[0]);
			stageNum = int.Parse(bff[1]);
		}

		//ステージの生成
		StageData stageData;
		if(IsEditMode) {
			stageData = editModeMain.GenerateStageData();
			stageText.text = "Preview edit map";
		}
		else {
			stageData = CsvLoader.StageLoad(stageLevel, stageNum);
			stageText.text = string.Format("{0} F  -  R o o m  {1}", stageLevel, stageNum);
		}
		StageGenerator.GenerateMap(stageData.mapData);

		//制限時間の設定
		Timebar.StopTimer();
		Timebar.Decpersec = 1;
		Timebar.time = stageData.time;

		//音楽を再生
		AudioManager.FadeIn(2.0f, BGMType.Game, 1, true);

		//入力の許可
		InputManager.isFreeze = false;
		//メニューを開けるようにする
		UImanager.isFreeze = false;

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

		#region Debug
		if (Input.GetKeyDown(KeyCode.B)) {
			DebugPause();
		}
		if(Input.GetKeyDown(KeyCode.N)) {
			Timebar.time = 5;
		}
		if(Input.GetKeyDown(KeyCode.M)) {
			Timebar.time += 10;
		}
		#endregion

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
		//メニューを開けなくする
		UImanager.isFreeze = true;

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

		if(IsEditMode) {
			//戻る
			editModeMain.StartCoroutine(editModeMain.UnloadPreview());
		}
		else {
			//画面表示
			Gameview.GameOverView();
		}
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
		//メニューを開けなくする
		UImanager.isFreeze = true;

		//落ちるアニメーション
		yield return new WaitForSeconds(1.0f);

		//BGMフェード
		AudioManager.FadeOut(2.0f);
		yield return new WaitForSeconds(2.0f);
		//BGM再生
		AudioManager.Play(BGMType.Clear, 1, true);

		if(IsEditMode) {
			//戻る
			editModeMain.StartCoroutine(editModeMain.UnloadPreview());
		}
		else {
			//画面表示
			Gameview.GameClearView();
		}
	}
	public static void DebugPause() {
		Debug.Break();
	}
}

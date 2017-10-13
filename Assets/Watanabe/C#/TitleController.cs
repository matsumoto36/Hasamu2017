using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

	bool isFreeze = true;

	// Use this for initialization
	void Start () {

		//BGM再生
		AudioManager.FadeIn(2, BGMType.Title, 1, true);
		//アニメーション再生
		StartCoroutine(TitleAnim());

		//環境エフェクト表示
		var ps = ParticleManager.Play(ParticleType.AmbientEffect, Vector2.zero, Quaternion.identity);
		var s = ps.shape;
		s.scale = new Vector2(1.5f, 1) * 10;
	}

	/// <summary>
	/// シーン遷移
	/// </summary>
	public void MoveScene() {  //明日やること　タイトルのスタートボタン、終了ボタンの作成

		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;
		//操作禁止中は実行しない
		if (isFreeze) return;

		//音の再生
		AudioManager.Play(SEType.Button, 1);
		//BGMフェードアウト
		AudioManager.FadeOut(2);

		//シーン遷移
		SumCanvasAnimation.MoveScene("StageSelectScene");
	}

	public void Exit()
	{
			Application.Quit();
	}

	/// <summary>
	/// 一定時間待つ
	/// </summary>
	/// <returns></returns>
	IEnumerator TitleAnim() {

		yield return new WaitForSeconds(2.0f);

		//フリーズ解除
		isFreeze = false;

	}
}

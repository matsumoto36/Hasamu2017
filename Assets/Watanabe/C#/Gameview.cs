using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Gameview : MonoBehaviour {

    /// <summary>
    /// ゲームをクリアした時に実行される
    /// </summary>
    public static void GameClearView()
    {
        Debug.Log("ゲームクリア");

        GameObject viewPrefab = Resources.Load<GameObject>("Prefabs/[GameClearView]");
        Transform target = GameObject.Find("Canvas").transform;

        GameObject viewObject = Instantiate(viewPrefab, target);

        Text t = GameObject.Find("Remaining").GetComponent<Text>();
        t.text = string.Format("LimitTime : {0:0.0}",Timebar.time);

		Button nextStage = GameObject.Find("NextRoom").GetComponent<Button>();

		string s;
		//次のステージを取得
		if(!StageInformation.GetNextFloor(GameManager.stageLevel, GameManager.stageNum, out s)) {
			//投げればボタンを表示しない
			nextStage.gameObject.SetActive(false);
		} else {

			//次のステージボタンを押したとき
			nextStage.onClick.AddListener(() => {

				string next;
				//次のステージを取得
				StageInformation.GetNextFloor(GameManager.stageLevel, GameManager.stageNum, out next);

				string[] bff = next.Split('-');
				//情報を格納
				GameManager.SetStageData(int.Parse(bff[0]), int.Parse(bff[1]));
	
				//移動
				AudioManager.FadeOut(2);
				SumCanvasAnimation.MoveScene("GameScene");
			});
		}

		//ステージセレクトボタンを押したとき
		Button select = GameObject.Find("Stage select").GetComponent<Button>();
		select.onClick.AddListener(() => {
			AudioManager.FadeOut(2);
			SumCanvasAnimation.MoveScene("StageSelectScene");
		});
	}

    /// <summary>
    /// ゲームオーバーした時に実行される
    /// </summary>
    public static void GameOverView()
    {
        Debug.Log("ゲームオーバー");

        GameObject viewPrefab = Resources.Load<GameObject>("Prefabs/[GameOverView]");
        Transform target = GameObject.Find("Canvas").transform;

        GameObject viewObject = Instantiate(viewPrefab, target);

		//リトライボタンを押したとき
		Button retry = GameObject.Find("Retry").GetComponent<Button>();
		retry.onClick.AddListener(() => {
			AudioManager.FadeOut(2);
			SumCanvasAnimation.MoveScene("GameScene");
		});

		//ステージセレクトボタンを押したとき
		Button select = GameObject.Find("Stage select").GetComponent<Button>();
		select.onClick.AddListener(() => {
			AudioManager.FadeOut(2);
			SumCanvasAnimation.MoveScene("StageSelectScene");
		});

		//gv_textの内容を変更（サンプル）
		//Text t = GameObject.Find("gv_text").GetComponent<Text>();
		//t.text = "aaaaa";

		//タイムをもってくる
		//Timebar.time.ToString()
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Gameview : MonoBehaviour {
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            //ゲームクリア
            GameClearView();

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //ゲームオーバー
            GameOverView();
        }
    }

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
        t.text = Timebar.time.ToString();
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

        //gv_textの内容を変更（サンプル）
        //Text t = GameObject.Find("gv_text").GetComponent<Text>();
        //t.text = "aaaaa";

        //タイムをもってくる
        //Timebar.time.ToString()
    }

}

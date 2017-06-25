using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Timebar : MonoBehaviour
{
	static Timebar myTimebar;

    //private bool m_isVisibleTimer = false;//フレームカウント
    static bool isStarted = false;

    public static float time = 60;//初期値を60
    public static float Decpersec = 1;

	int counter = 0;
	float waitTime = 0;
	float[,] timerIntervalArray = new float[4, 2] { //タイマーを鳴らす間隔
		{ 2.0f, 30.0f },
		{ 1.0f, 10.0f },
		{ 0.5f, 5.0f },
		{ 0.2f, 0.0f },
	};

	void Start()
    {
		myTimebar = this;

		//float型からint型へCastし、String型に変換して表示
		//GetComponent<Text>().text = ((int)time).ToString();

		//Invoke("DelayMethod", 3.0f);//フレームカウント

	}
    void Update()
    {
        //isStartedがfalseならタイムを減らさない
        if (isStarted == false) return;
    //if (m_isVisibleTimer)//フレームカウント後
    //{
        //１秒に１ずつ減らしていく
        time -= Time.deltaTime * Decpersec;

        //マイナスは表示しない
        if (time < 0)
        {
            time = 0;
            isStarted = false;
            TimeUp();
        }
		//GetComponent<Text>().text = ((int)time).ToString();

		//音関係
		waitTime += Time.deltaTime;
		if(waitTime > timerIntervalArray[counter, 0]) {
			waitTime -= timerIntervalArray[counter, 0];

			//再生
			AudioManager.Play(SEType.BombTimer, 0.7f);
			//再生間隔を減らすか
			if (timerIntervalArray[counter, 1] > time) counter++;
		}
	}

    public static void StartTimer()
    {
        isStarted = true;

	}

    public static void StopTimer()
    {
        isStarted = false;

	}
    void TimeUp()
    {
        Debug.Log("Timeup!!");

		//ゲームオーバーを呼ぶ
		GameManager.GameOver();
	}

	//}
	//public void DelayMethod()//フレームカウント
	//{
	//    m_isVisibleTimer = true;
	//}
}

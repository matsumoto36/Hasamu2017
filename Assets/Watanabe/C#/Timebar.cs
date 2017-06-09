using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Timebar : MonoBehaviour
{
    //private bool m_isVisibleTimer = false;//フレームカウント
    static bool isStarted = false;

    public static float time = 10;//初期値を60
    public static float Decpersec = 2;

    void Start()
    {
        //float型からint型へCastし、String型に変換して表示
        //GetComponent<Text>().text = ((int)time).ToString();

        //Invoke("DelayMethod", 3.0f);//フレームカウント
    }
    void Update()
    {

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
    }

    public static void StartTimer()
    {
        isStarted = true;
    }

    void TimeUp()
    {
        Debug.Log("Timeup!!");
    }
    //}
    //public void DelayMethod()//フレームカウント
    //{
    //    m_isVisibleTimer = true;
    //}
}

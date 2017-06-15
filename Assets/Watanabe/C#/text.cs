using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour {

    public Text gameOverText;
    public Text timerText;
    void Start()
    {
        gameOverText.gameObject.SetActive(false);//Game Over非表示
    }
    void Update()
    {
        timerText.text = Timebar.time.ToString();
    }

    public void TestStartTimer()
    {
        Timebar.StartTimer();//タイム起動
    }
    public void TestStopTimer()
    {
        Timebar.StopTimer();//タイムストップ
    }
        void TestGameOver()
    {
        //gameOverText.gameObject.SetActive(true);
    }
}

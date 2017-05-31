﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timebar : MonoBehaviour
{
    public string TimeOve;//ゲームオーバーシーン
    private float time = 60;//初期値を60
    void Start()
    {
        //float型からint型へCastし、String型に変換して表示
        GetComponent<Text>().text = ((int)time).ToString();
    }
    void Update()
    {
        //１秒に１ずつ減らしていく
        time -= Time.deltaTime;
        //マイナスは表示しない
        if (time < 0) time = 0;
        GetComponent<Text>().text = ((int)time).ToString();
            if(time==0)
            SceneManager.LoadScene(TimeOve);
    }
}


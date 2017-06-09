using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CanvasDoor : MonoBehaviour
{
    Animator animator;//アニメーター
    AnimatorStateInfo stateInfo;
    RectTransform rectTransform;//トランスフォーム
    private string nextScene;

    void Start()
    {
        animator = GetComponent<Animator>();//アニメーション
        rectTransform = GetComponent<RectTransform>();//トランスフォーム
    }

    /// ドア開く（投げます）
    public void DoorOpen(string nextScene)
    {
        animator = GetComponent<Animator>();//アニメーション

        animator.SetFloat("State", 0);//ドアが開きます
        animator.SetBool("IsOpen", true);//アニメーション

        StartCoroutine("LoadScene", nextScene);//LoadSceneを読み込み
    }

    /// ドア閉まる（投げます）
    public void DoorClose(string nextScene)
    {

        animator = GetComponent<Animator>();//アニメーション

        animator.SetFloat("State", 1);//ドアが閉まります。
        animator.SetTrigger("IsClose");//アニメーション

        StartCoroutine("LoadScene", nextScene);//LoadSceneを読み込み
    }
    
    void Open()
    {
        animator.SetBool("IsOpen", true);//アニメーション
        Destroy(this.gameObject,2f);//消すよ
    }

    IEnumerator LoadScene(string nextScene)
    {
        yield return new WaitForSeconds(2.0f);
        animator.SetFloat("State", 0);

        SceneManager.LoadScene(nextScene);//シーンへ
        Open();//Open読み取り
        //Destroy(gameObject,0.01f);
    }
    void CanvasTo()
    {
        LoadScene(nextScene);
    }

    void Update()
    {
        if (!animator) return;

        //animatorの状態を取得
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log(stateInfo.IsName("Stop"));

        if (Debug.logger.logEnabled == true)
        {
            CanvasTo();
            
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {
    public GameObject door;
    Animator animator;

    void Start()
    {
        animator = door.GetComponent<Animator>();
    }

    //シーン遷移
    public string nextScene;
    public void OnClick()
    {//Gameシーンに行きす。
        animator.SetTrigger("IsOpen");

        Debug.Log("移動してきます。");//移動確認
        Invoke("LoadScene", 2.0f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextScene);//文字を変えれば行きたいシーン飛べますよ
    }

    //終了ボタン
    public void OnCallExit()
    {
        Debug.Log("ゲーム終了します。");//処理確認
        Application.Quit();//ゲーム終了
    }
}

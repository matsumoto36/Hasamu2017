using UnityEngine;
using UnityEngine.SceneManagement;

public class DOORS : MonoBehaviour {
    public Animator DOORs;//2Dドア
    public GameObject DOOR;//飾り
    Animator animator;//アニメーション

    void Start()
    {
    }

    //シーン遷移
    public string nextScene;//本家
    public string SceneDM;//文家
    public void OnClick()
    {//Gameシーンに行きす。
        Invoke("DoorOpen",0.0f);
        Debug.Log("移動してきます。(開きました)");//移動確認
        Invoke("LoadScene", 5.0f);//呼び出し
    }

    public void OnCross()
    {//閉じる
        Invoke("DoorCross",0.0f);
        Debug.Log("移動しました。（閉じました。）");
        Invoke("LoadScene", 5.0f);//呼び出し
    }

    public void OnMenu()
    {//閉じる
        Invoke("DoorCross", 0.0f);
        Debug.Log("移動しました。（閉じました。）");
        Invoke("LoadDM", 5.0f);//呼び出し
    }
    void DoorOpen()
    {//ドア開く
        GameObject door = (GameObject)Instantiate(DOORs).gameObject;//生成
        door.transform.position = Vector2.zero;//初期位置
        Destroy(DOOR);//削除
        animator = door.GetComponent<Animator>();//アニメーション
        animator.SetFloat("Button", 0);//開く
        animator.SetBool("ISOpen", true);//アニメーション実行
    }
    void DoorCross()
    {//ドア閉じる
        GameObject door = (GameObject)Instantiate(DOORs).gameObject;//生成
        door.transform.position = Vector2.zero;//初期位置
        animator = door.GetComponent<Animator>();//アニメーション
        animator.SetFloat("Button", 1);//閉じる
        animator.SetBool("IsClose", true);//アニメーション実行
    }

    void LoadScene()
    {//シーンへ飛ぶ
        SceneManager.LoadScene(nextScene);//文字を変えれば行きたいシーン飛べますよ
    }

    void LoadDM()
    {//シーンへ飛ぶ
        SceneManager.LoadScene(SceneDM);//保険もう一個
    }

    //終了ボタン
    public void OnCallExit()
    {
        Debug.Log("ゲーム終了します。");//処理確認
        Application.Quit();//ゲーム終了
    }
}

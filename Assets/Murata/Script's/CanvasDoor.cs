using UnityEngine;
using UnityEngine.SceneManagement;
public class CanvasDoor : MonoBehaviour
{
    public Animator DoorCanvas;//Canvasの扉
   // public GameObject Wall;//始めに扉のオブジェを配置用
    Animator animator;//Animator
    public string NexScene;//シーン遷移
    public string NextToScene;//シーン遷移複数用
    /// <summary>
    /// ボタンクリクでドアを開けるアニメーション
    /// </summary>
    public void OnOpenDoor()//ボタンクリック（開く）
    {
        Invoke("DoorOpen",0.0f);//物を持ってこい
        Debug.Log("移動します。（扉は開かれます。）");//確認
        Invoke("LoadScene", 2.0f);//呼び出し
    }
    /// <summary>
    /// ボタンクリックでドアを閉めるアニメーション
    /// </summary>
    public void OnCrossDoor()//ボタンクリック（閉じる）
    {
        Invoke("DoorCross", 0.0f);//物を持ってこい
        Debug.Log("移動します。（扉は閉まります。）");//確認
        Invoke("LoadScene", 2.0f);//呼び出し
    }

    /// <summary>
    /// ボタンクリックでドアを閉めるアニメーション
    /// </summary>
    public void OnCrossDoorToScene()
    {
        Invoke("DoorCross", 0.0f);//物を持ってこい
        Debug.Log("移動しますね。（扉を閉まりますよ。）");//確認
        Invoke("LoadToScene", 2.0f);//呼び出し
    }
    /// <summary>
    /// ドア開く（投げます）
    /// </summary>
    void DoorOpen()
    {
        //GameObject door = (GameObject)Instantiate(DoorCanvas).gameObject;//Canvasを生成
        //door.transform.position = Vector2.zero;//初期位置座標
        animator = DoorCanvas.GetComponent<Animator>();//アニメーション
        animator.SetFloat("State", 0);//ドアが開きます
        animator.SetBool("IsOpen",true);//アニメーション
     }

    /// <summary>
    /// ドア閉まる（投げます）
    /// </summary>

    void DoorCross()
    {
        GameObject door = (GameObject)Instantiate(DoorCanvas).gameObject;//Canvasを生成
        door.transform.position = Vector2.zero;//初期位置座標
        animator = door.GetComponent<Animator>();//アニメーション
        animator.SetFloat("State", 1);//ドアが閉まります。
        animator.SetBool("IsClose", true);//アニメーション
    }
    /// <summary>
    /// 設定したシーンに行きます
    /// </summary>
    void LoadScene()
    {//シーンへ飛ぶ
        SceneManager.LoadScene(NexScene);//シーン遷移
    }
    /// <summary>
    /// 設定されたシーンに行きます。
    /// </summary>
    void LoadToScene()
    {//シーンへ飛ぶ
        SceneManager.LoadScene(NextToScene);//シーン遷移
    }
    /// <summary>
    /// ゲームを終了させます
    /// </summary>
    public void OnCallExit()
    {
        Debug.Log("ゲーム終了します。");//処理確認
        Application.Quit();//ゲーム終了
    }


    void Start () {
		
	}
	
	
	void Update () {
		
	}
}

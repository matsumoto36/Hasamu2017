using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SumCanvasAnimation : MonoBehaviour
{
    static Animator animator;//アニメーター(CanvasDoor)
    static RectTransform rectTransform;//トランスフォーム(CanvasDoor)
    static private string nextSceneName;    //移動したいシーンの名前

    static bool isMovingScene = false;


    void Awake ()
    {   //CanvasDoor
        animator = GetComponent<Animator>();//アニメーション
        rectTransform = GetComponent<RectTransform>();//トランスフォーム
        //ステージ遷移で削除されないようにする
         DontDestroyOnLoad(gameObject);
        //移動開始
        StartCoroutine(MoveSceneAnimation());
    }
	

    /// <summary>
    /// シーン移動するアニメーション本体
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveSceneAnimation()
    {
        //閉じるアニメーション
        DoorClose(nextSceneName);
        yield return new WaitForSeconds(2.0f);
        Debug.Log("CloseAnimComplete");

        //ステージ移動
        SceneManager.LoadScene(nextSceneName);
        Debug.Log("SceneMoved : " + nextSceneName);

        //開くアニメーション
        DoorOpen(nextSceneName);
        yield return new WaitForSeconds(2.2f);
        Debug.Log("OpenAnimComplete");

		//移動許可
		isMovingScene = false;

		Destroy(gameObject);
        Debug.Log("消えろ");
    }


    /// <summary>
    /// シーンを移動する
    /// </summary>
    /// <param name="sceneName">移動したいシーンの名前</param>
    public static void MoveScene(string sceneName)
    {
		//シーン移動中は移動禁止
		if (isMovingScene) return;
		isMovingScene = true;

		nextSceneName = sceneName;
        //自分を生成してアニメーションする
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Door_b");
        SumCanvasAnimation anim = Instantiate(prefab).GetComponent<SumCanvasAnimation>();
        //CanvasDoor door = anim.transform.GetChild(0).GetComponent<CanvasDoor>();
    }

    // ドア開く
    public static void DoorOpen(string nextScene)
    {
        animator.SetFloat("State", 0);//ドアが開きます
        animator.SetTrigger("IsOpen");//アニメーション
    }

    /// ドア閉まる
    public static void DoorClose(string nextScene)
    {
        animator.SetFloat("State", 1);//ドアが閉まります。
        animator.SetTrigger("IsClose");//アニメーション
    }

}

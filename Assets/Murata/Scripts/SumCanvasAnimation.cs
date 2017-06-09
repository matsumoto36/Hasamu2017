using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SumCanvasAnimation : MonoBehaviour
{
    static Animator animator;//アニメーター(CanvasDoor)
    static AnimatorStateInfo stateInfo;//トランスフォーム(CanvasDoor)
    static RectTransform rectTransform;//トランスフォーム(CanvasDoor)
    static private string nextSceneName;    //移動したいシーンの名前
    static GameObject doorPre;


	void Awake ()
    {
        //CanvasDoor
        animator = GetComponent<Animator>();//アニメーション
        rectTransform = GetComponent<RectTransform>();//トランスフォーム

        //ステージ遷移で削除されないようにする
        DontDestroyOnLoad(gameObject);

        //移動開始
        StartCoroutine(MoveSceneAnimation());

    }
	
	//void Update ()
 //   {   //TitleClick
 //       //ボタンクリックされたらDoorManagerからCreateDoorOpenを持ってきて指定したシーンに行きます
 //       if (Input.GetMouseButtonDown(0))
 //          // DoorManager.CreateDoorOpen(nextSceneName);//TitleClick
 //       CreateDoorOpen(nextSceneName);

 //       //CanvasDoor
 //       if (!animator) return;

 //       //animatorの状態を取得
 //       stateInfo = animator.GetCurrentAnimatorStateInfo(0);

 //       Debug.Log(stateInfo.IsName("Stop"));

 //       if (Debug.logger.logEnabled == true)
 //       {
 //           CanvasTo();

 //       }
 //   }

    /// <summary>
    /// シーン移動するアニメーション本体
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveSceneAnimation()
    {

        //閉じるアニメーション
        yield return new WaitForSeconds(2.0f);
        Debug.Log("CloseAnimComplete");

        //ステージ移動
        SceneManager.LoadScene(nextSceneName);
        Debug.Log("SceneMoved : " + nextSceneName);



        //開くアニメーション
        yield return new WaitForSeconds(2.0f);
        Debug.Log("OpenAnimComplete");

    }


    /// <summary>
    /// シーンを移動する
    /// </summary>
    /// <param name="sceneName">移動したいシーンの名前</param>
    public static void MoveScene(string sceneName)
    {
        nextSceneName = sceneName;

        //自分を生成してアニメーションする
        GameObject prefab = Resources.Load<GameObject>("StageAnimation");
        SumCanvasAnimation anim = Instantiate(prefab).GetComponent<SumCanvasAnimation>();
    }





    //SceneClick
    public void MenuButton(string nextSceneName)
    {
        //DoorManagerからCreateDoorCloseを持ってきて指定したシーンに行けます
        //DoorManager.CreateDoorClose(nextSceneName);//SceneClick
        CreateDoorClose(nextSceneName);
    }
    public void TitleButton(string nextSceneName)
    {
        //DoorManagerからTitleDoorCloseを持ってきて指定したシーンに行けます
       //DoorManager.TitleDoorClose(nextSceneName);//SceneClick
        TitleDoorClose(nextSceneName);
    }

    //DoorManager
    //開く
    public static void CreateDoorOpen(string nextScene)
    {
        //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
        doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

        //Scene上にDoor_Canvasを生成して、取得
        GameObject doorObj = Instantiate(doorPre);

        //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
        // CanvasDoor door = doorObj.transform.GetChild(0).GetComponent<CanvasDoor>();
       // GameObject door = doorObj.transform.GetChild(0).GetComponent<GameObject>();
        DoorOpen(nextScene);//シーンへ
      
    }
    //閉じる
    public static void CreateDoorClose(string nextScene)
    {
        //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
        doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

        //Scene上にDoor_Canvasを生成して、取得
        GameObject doorObj = Instantiate(doorPre);

        //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
        CanvasDoor door = doorObj.transform.GetChild(0).GetComponent<CanvasDoor>();

        //シーンに移動しても破棄されずにアニメーションが可能
        DontDestroyOnLoad(doorObj);

       DoorClose(nextScene);//シーンへ
        
    }

    //閉じるタイトル専用
    public static void TitleDoorClose(string nextScene)
    {
        //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
        doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

        //Scene上にDoor_Canvasを生成して、取得
        GameObject doorObj = Instantiate(doorPre);

        //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
        GameObject door = doorObj.transform.GetChild(0).GetComponent<GameObject>();

        DoorClose(nextScene);//シーンへ
        
    }

    //CanvasDoor
    /// ドア開く（投げます）
    public static void DoorOpen(string nextScene)
    {
        //animator =GetComponent<Animator>();//アニメーション

        animator.SetFloat("State", 0);//ドアが開きます
        animator.SetTrigger("IsOpen");//アニメーション

        LoadScene(nextScene);//LoadSceneを読み込み
    }

    /// ドア閉まる（投げます）
    public static void DoorClose(string nextScene)
    {

        //animator = GetComponent<Animator>();//アニメーション

        animator.SetFloat("State", 1);//ドアが閉まります。
        animator.SetTrigger("IsClose");//アニメーション

        LoadScene(nextScene);//LoadSceneを読み込み
    }

    static void Open()
    {
        animator.SetTrigger("IsOpen");//アニメーション
        Destroy(doorPre, 2.3f);//消すよ
    }

    static IEnumerator LoadScene(string nextScene)
    {
        yield return new WaitForSeconds(2.0f);
        animator.SetFloat("State", 0);
        SceneManager.LoadScene(nextScene);//シーンへ
        Open();//Open読み取り
    }
    void CanvasTo()
    {
        LoadScene(nextScene);
    }

}

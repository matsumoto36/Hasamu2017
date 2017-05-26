using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGoto : MonoBehaviour
{ //シーン遷移
    public string nextScene;
    public void OnClick()
    {//Gameシーンに行きす。
        Debug.Log("移動してきます。");//移動確認
        SceneManager.LoadScene(nextScene);//文字を変えれば行きたいシーン飛べますよ
    }
}
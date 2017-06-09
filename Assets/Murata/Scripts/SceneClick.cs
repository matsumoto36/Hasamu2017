using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneClick : MonoBehaviour {

    public string nextSceneName;//シーン名

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) DoorManager.CreateDoorClose(nextSceneName);
    }

    public void MenuButton(string nextSceneName)
    {//DoorManagerからCreateDoorCloseを持ってきて指定したシーンに行けます
        DoorManager.CreateDoorClose(nextSceneName);
    }

    public void TitleButton(string nextSceneName)
    {//DoorManagerからTitleDoorCloseを持ってきて指定したシーンに行けます
        DoorManager.TitleDoorClose(nextSceneName);
    }
}

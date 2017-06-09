using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoorManager
{
    
    ////開く
    //public static void CreateDoorOpen(string nextScene)
    //{
    //    //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
    //    GameObject doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

    //    //Scene上にDoor_Canvasを生成して、取得
    //    GameObject doorObj = (GameObject)MonoBehaviour.Instantiate(doorPre);

    //    //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
    //    CanvasDoor door = doorObj.transform.GetChild(0).GetComponent<CanvasDoor>();

    //    door.DoorOpen(nextScene);//シーンへ
    //}

    ////閉じる
    //public static void CreateDoorClose(string nextScene)
    //{
    //    //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
    //    GameObject doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

    //    //Scene上にDoor_Canvasを生成して、取得
    //    GameObject doorObj = (GameObject)MonoBehaviour.Instantiate(doorPre);

    //    //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
    //    CanvasDoor door = doorObj.transform.GetChild(0).GetComponent<CanvasDoor>();

    //    //シーンに移動しても破棄されずにアニメーションが可能
    //    MonoBehaviour.DontDestroyOnLoad(doorObj);

    //    door.DoorClose(nextScene);//シーンへ
    //}
    ////閉じるタイトル専用
    //public static void TitleDoorClose(string nextScene)
    //{
    //    //Resourcesフォルダ内のPrefabsフォルダ内のDoor_Canvasを取得
    //    GameObject doorPre = (GameObject)Resources.Load("Prefabs/Door_Canvas");

    //    //Scene上にDoor_Canvasを生成して、取得
    //    GameObject doorObj = (GameObject)MonoBehaviour.Instantiate(doorPre);

    //    //Door_Canvasの子オブジェクトのDoorのscript、CanvasDoorを取得
    //    CanvasDoor door = doorObj.transform.GetChild(0).GetComponent<CanvasDoor>();

    //    door.DoorClose(nextScene);//シーンへ
    //}
}


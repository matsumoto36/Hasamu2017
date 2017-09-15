using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EditFileIO : MonoBehaviour {

	static readonly string FILEPATH_EDITOR = Application.dataPath + "/Resources/CSV/";
	static readonly string FILEPATH_APP    = "/Tentacroom";

	public static StageData[] GetAllFile() {

		return Resources.LoadAll<TextAsset>("CSV")
			.Select((item) => CsvLoader.ConvertStageData(item)).ToArray();
	}

	public static void SaveFile(string fileName, StageData data) {
		StreamWriter sw;

		string path = "";

		if(Application.isEditor) {
			path = FILEPATH_EDITOR + fileName + ".csv";
		}
		else {

#if UNITY_ANDROID
			using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
			using(AndroidJavaObject exDir = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")) {

				path = exDir.Call<string>("toString") + FILEPATH_APP;

				//ファイルがなければ作成
				if(!Directory.Exists(path)){
					Directory.CreateDirectory(path);
				}

				path += "/" + fileName + ".csv";
			}
#else
			path = FILEPATH_APP + fileName + ".csv";
#endif


		}

		FileInfo fi = new FileInfo(path);
		sw = fi.CreateText();
		sw.Write(CsvLoader.ConvertCSV(data));
		sw.Flush();
		sw.Close();
	}
}

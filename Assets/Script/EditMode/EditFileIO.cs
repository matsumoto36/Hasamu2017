using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EditFileIO : MonoBehaviour {

	static readonly string FILEPATH_EDITOR = Application.dataPath + "/Resources/CSV/";
	static readonly string FILEPATH_APP    = Application.persistentDataPath;

	public static StageData[] GetAllFile() {

		return Resources.LoadAll<TextAsset>("CSV")
			.Select((item) => CsvLoader.ConvertStageData(item)).ToArray();
	}

	public static void SaveFile(string fileName, StageData data) {
		StreamWriter sw;

		string path;

		if(Application.isEditor) {
			path = FILEPATH_EDITOR + fileName + ".csv";
		}
		else {

#if UNITY_ANDROID
			using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
			using(AndroidJavaObject exDir = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")) {

				path = exDir.Call<string>("toString") + FILEPATH_APP + fileName + ".csv";
			}
#endif

			path = FILEPATH_APP + fileName + ".csv";

		}

		FileInfo fi = new FileInfo(path);
		sw = fi.AppendText();
		sw.Write(CsvLoader.ConvertCSV(data));
		sw.Flush();
		sw.Close();
	}
}

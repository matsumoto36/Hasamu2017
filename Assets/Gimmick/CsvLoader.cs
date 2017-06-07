using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvLoader : MonoBehaviour
{
    private static TextAsset csv;
    private static List<int[]> csvDate = new List<int[]>();
    private static int height = 0;


	// Use this for initialization
	void Start ()
    {

    }

    /// <summary>
    /// ステージの番号を受け取って該当するcsvを読み込む
    /// </summary>
    public static int[,] StageLoad(string stageLevel, string stageNumber)
    {
        csv = Resources.Load("CSV/" + stageLevel + "-" + stageNumber) as TextAsset;
        StringReader reader = new StringReader(csv.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();

            //一時的に保持
            string[] bff = line.Split(',');
            int[] list = new int[bff.Length];

            //int型にキャスト
            for (int i = 0; i < bff.Length; i++)
            {
                list[i] = int.Parse(bff[i]);
            }

            //マップデータに追加
            csvDate.Add(list);
            height++;
        }
        return null;
    }
}

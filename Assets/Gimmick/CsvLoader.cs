﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvLoader : MonoBehaviour
{
    /// <summary>
    /// ステージの番号を受け取って該当するcsvを読み込む
    /// </summary>
    public static int[,] StageLoad(string stageLevel, string stageNumber)
    {
        TextAsset csv;
        List<string[]> csvDataS = new List<string[]>(); // CSVの中身を入れるリスト
        int height = 0; // CSVの行数

        //Resources内のCSVフォルダからcsvファイルを読み込んでList<string>に代入
        csv = Resources.Load("CSV/" + stageLevel + "-" + stageNumber) as TextAsset;
        if (!csv) Debug.Log("ない！");
        Debug.Log(stageLevel + "-" + stageNumber);
        StringReader reader = new StringReader(csv.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDataS.Add(line.Split(',')); // リストに入れる
            height++; // 行数加算
        }
 
        int[,] csvData = new int[csvDataS.Count, csvDataS[0].Length];

        //List<string>をint二次元配列に変換
        for (int i = 0; i < csvData.GetLength(0); i++)
        {
            for (int j = 0; j < csvData.GetLength(1); j++)
            {
                csvData[i, j] = int.Parse(csvDataS[i][j]);
            }
        }
        return csvData;
    }
}
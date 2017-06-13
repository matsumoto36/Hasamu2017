using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StageData
{
    public int time;
    public int[,] mapData;

    public StageData(int time,int[,] mapData)
    {
        this.time = time;
        this.mapData = mapData;
    }
}
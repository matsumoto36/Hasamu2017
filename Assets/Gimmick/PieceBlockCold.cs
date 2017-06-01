﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBlockCold : Piece, IExecutable
{
    Piece[] p = new Piece[4];

    // Update is called once per frame
    void Update ()
    {
        /*Vector2[] checkPositioin = new Vector2[]
        { new Vector2(position.x + 1, position.y),
            new Vector2(position.x-1,position.y),
            new Vector2(position.x,position.y+1),
            new Vector2(position.x,position.y-1)};*/
        for (int i = 0; i > checkPos.Length; i++)
        {
            p[i] = StageGenerator.GetPiece(checkPos[i]);
        }

        //p[i]がnullであれば判定しない
        if (!(p[0] == null || p[2] == null))  
        {
            if (p[0].id == p[2].id)
            {
                Sandwiched(p[0].id);
            }
        }

        if (!(p[1] == null || p[3] == null)) 
        {
            if (p[1].id == p[3].id)
            {
                Sandwiched(p[1].id);
            }
        }
    }


	public void Action()
	{

	}

    /// <summary>
    /// 挟まれなう
    /// </summary>
    void Sandwiched(int id)
    {
        switch (id)
        {
            case 5:
                id = 4;
                break;
            default:
                break;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆弾
/// </summary>
public class PieceBomb : Piece, IExecutable
{

    public float coldtime = 0;
    float decpersec;

	Timebar tmber;

    Piece[] p = new Piece[4];

	void Start() {
		tmber = FindObjectOfType<Timebar>();
	}

    public void Update()
    {
        for (int i = 0; i < checkPos.Length; i++) 
        {
            p[i] = StageGenerator.GetPiece(checkPos[i]);
			if(!p[i]) continue;
            switch(p[i].id)
            {
                case 5:
                    hot();
                    break;
                case 6:
                    cold();
                    break;
            }
        }
    }

    /// <summary>
	/// 触手に掴まれているときに毎フレーム実行される
	/// </summary>
	public void Action()
    {

	}
    
    ///<summary>
    ///あついブロックに隣接すると呼び出される
    ///</summary>
    public void hot()
    {
		if(!tmber) return;

        decpersec = tmber.Decpersec;
        coldtime += Time.deltaTime;
        if (coldtime <= 3.0f)
            tmber.Decpersec = 2;
        else
            tmber.Decpersec = decpersec;
    }

    /// <summary>
    /// さむいブロックに隣接すると呼び出される
    /// </summary>
    public void cold()
    {
		if(!tmber) return;

		decpersec = tmber.Decpersec;
        coldtime += Time.deltaTime;
        if (coldtime <= 3.0f)
            tmber.Decpersec = 0.5f;
        else
            tmber.Decpersec = decpersec;
    }
}
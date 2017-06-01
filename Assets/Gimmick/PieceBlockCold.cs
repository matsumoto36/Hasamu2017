using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBlockCold : Piece, IExecutable
{
    Piece[] p = new Piece[4];

    // Update is called once per frame
    void Update ()
    {
        for (int i = 0; i > checkPos.Length; i++)
        {
            p[i] = StageGenerator.GetPiece(checkPos[i]);
            if (p == null) return;

            switch (p[i].id)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    PieceBomb bomb = FindObjectOfType<PieceBomb>();
                    bomb.cold();
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    Action();
                    break;
            }
        }
    }


	public void Action()
	{

	}
}

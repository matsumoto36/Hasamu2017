using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBlockFire : Piece, IExecutable
{

    Piece[] p = new Piece[4];

    // Update is called once per frame
    void Update ()
    {
        /*Vector2 [] checkPositioin=new Vector2[]
            { new Vector2(position.x + 1, position.y),
            new Vector2(position.x-1,position.y),
            new Vector2(position.x,position.y+1),
            new Vector2(position.x,position.y-1)};*///隣接するピースの情報を得る
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
                    bomb.Boom();
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;                 
            }
        } 
    }

	public void Action() 
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆弾
/// </summary>
public class PieceBomb : Piece, IExecutable
{

    Piece[] p = new Piece[4];

    public void Update()
    {
		//取得用位置を格納
		Vector2[] checkPos = new Vector2[] {
			new Vector2(position.x, position.y + 1),
			new Vector2(position.x - 1, position.y),
			new Vector2(position.x, position.y - 1),
			new Vector2(position.x + 1, position.y),
		};

		//挟まれ検知
        for (int i = 0; i < checkPos.Length; i++)
        {
            p[i] = StageGenerator.GetPiece(checkPos[i]);

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
	}

    /// <summary>
	/// 触手に掴まれているときに毎フレーム実行される
	/// </summary>
	public void Action()
    {

	}

    ///<summary>
    ///挟まれると呼び出される
    ///</summary>
    public void Sandwiched(int id)
    {
        switch (id)
        {
            case 4:
                Timebar.Decpersec = 1;
                break;
            case 5:
                Timebar.Decpersec = 2;
                break;
            case 6:
                Timebar.Decpersec = 0.5f;
                break;
        }
    }
}

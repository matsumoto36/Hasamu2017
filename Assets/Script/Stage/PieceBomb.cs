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

		//温度を計測
		int checkThrmo = 0;
		for(int i = 0; i < checkPos.Length; i++) 
        {
            p[i] = StageGenerator.GetPiece(checkPos[i]);
			if(!p[i]) continue;
            switch(p[i].id)
            {
                case 5:
					//熱くなる
					checkThrmo++;
					break;
                case 6:
					//冷たくなる
					checkThrmo--;
                    break;
            }
        }

		//結果で判断
		if(checkThrmo < 0) cold();
		else if(checkThrmo > 0) hot();
		else Normal();
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
		Timebar.Decpersec = 2;
	}

    /// <summary>
    /// さむいブロックに隣接すると呼び出される
    /// </summary>
    public void cold()
    {
		Timebar.Decpersec = 0.5f;
	}

	public void Normal() {
		Timebar.Decpersec = 1;
	}
}

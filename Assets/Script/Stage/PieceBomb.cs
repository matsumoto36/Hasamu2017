using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆弾
/// </summary>
public class PieceBomb : Piece, IExecutable {

	/// <summary>
	/// 触手に掴まれているときに毎フレーム実行される
	/// </summary>
	public void Action()
    {

	}

    ///<summary>
    ///あついブロックに隣接すると呼び出される
    ///</summary>
    public void Boom()
    {

    }

    /// <summary>
    /// さむいブロックに隣接すると呼び出される
    /// </summary>
    public void cold()
    {

    }

}

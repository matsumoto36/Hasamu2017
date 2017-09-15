using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBlockFire : Piece, IExecutable
{

    Piece[] p = new Piece[4];
	ParticleSystem ps;

	void Start() {
		ps = ParticleManager.Play(ParticleType.HotBlock, transform.position, Quaternion.identity);
		ps.transform.SetParent(transform);
	}

	// Update is called once per frame
	void Update ()
    {
		//取得用位置を格納
		Vector2[] checkPos = new Vector2[] {
			new Vector2(position.x, position.y + 1),
			new Vector2(position.x - 1, position.y),
			new Vector2(position.x, position.y - 1),
			new Vector2(position.x + 1, position.y),
		};

		for(int i = 0; i < checkPos.Length; i++) 
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
    /// 挟まれた場合にidを通常ブロックと同じ4にする
    /// </summary>
    void Sandwiched(int id)
    {
        switch (id)
        {
            case 6:
				//遅延して実行
				StageGenerator.lateExecution += () => {
					//id = 4;
					//パーティクル削除
					ps.transform.SetParent(null);
					ParticleManager.ParticleDestroy(ps);

					//音再生
					AudioManager.Play(SEType.Cold, 1);

					//エフェクト発生
					ParticleManager.PlayOneShot(
						ParticleType.ChangeBlockTemp,
						transform.position,
						Quaternion.identity,
						2.0f);

					//判定がない = コンテナの中にいる
					if (noCollision) {
						Player.currentPieceContainer.ReplacementPiece(this, 4);
					} else {
						StageGenerator.EditPieceID(this, 4);
					}
				};
				break;
            default:
                break;
        }
    }
}

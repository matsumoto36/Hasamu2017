using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceTentacle : Piece {

	ParticleSystem[] ps = new ParticleSystem[4];

	// Update is called once per frame
	void Update () {
		
	}

	public override void SpriteLoad() {
		//subIDを決定
		SetSubIDAndRotation();

		if(subId == 0) {
			_renderer.sprite = ResourceLoader.GetChips(R_MapChipType.MainChip)[id];
		}
		else {
			_renderer.sprite = ResourceLoader.GetChips(R_MapChipType.Sub1_Tentacle)[subId - 1];
		}

		//パーティクル生成
		SetParticle();
		
		//更新
		UpdateParticle();
	}

	/// <summary>
	/// パーティクルを生成
	/// </summary>
	void SetParticle() {

		Vector2[] spawnPos = new Vector2[] {
			new Vector2(position.x, position.y + 0.5f),
			new Vector2(position.x - 0.5f, position.y),
			new Vector2(position.x, position.y - 0.5f),
			new Vector2(position.x + 0.5f, position.y),
		};

		for (int i = 0; i < 4; i++) {

			//生成
			ps[i] = ParticleManager.Play(ParticleType.TentacleSpawnEdge,
				spawnPos[i],
				Quaternion.Euler(0, 0, i * 90));

		}
	}

	/// <summary>
	/// パーティクルを制御
	/// </summary>
	public void UpdateParticle() {

		Vector2[] checkPos = new Vector2[] {
			new Vector2(position.x, position.y + 1),
			new Vector2(position.x - 1, position.y),
			new Vector2(position.x, position.y - 1),
			new Vector2(position.x + 1, position.y),
		};

		for (int i = 0; i < 4; i++) {

			if (StageGenerator.CheckStageBound(checkPos[i]) ||
				StageGenerator.GetPiece(checkPos[i])) {

				ps[i].Stop();
			}else{
				ps[i].Play();
			}

		}

	}
}

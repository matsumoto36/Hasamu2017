using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ中の触手ブロック
/// </summary>
public class PieceTentacle : Piece {

	ParticleSystem[] ps = new ParticleSystem[4];    //4方向に作成するパーティクル

	SkinnedMeshRenderer skinRenderer;


	// Update is called once per frame
	void Update () {
		
	}

	public override void SpriteLoad() {
		//subIDを決定
		SetSubIDAndRotation();

		var meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		meshRenderer.material = ResourceLoader.GetMaterial(R_MaterialType.TentaclePiece);
		meshRenderer.sortingOrder = GetOrderInLayer(id);
		//meshRenderer.transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);

		Sprite spr = null;

		if(subId == 0) {

			spr = ResourceLoader.GetChips(R_MapChipType.MainChip)[id];
		}
		else {
			spr = ResourceLoader.GetChips(R_MapChipType.Sub1_Tentacle)[subId - 1];
		}

		Texture2D tex = new Texture2D((int)spr.rect.width, (int)spr.rect.height);

		var pixels = spr.texture.GetPixels(
			(int)spr.textureRect.x,
			(int)spr.textureRect.y,
			(int)spr.textureRect.width,
			(int)spr.textureRect.height);

		var distPixels = new Color[pixels.Length];
		int width = (int)spr.textureRect.width;
		int height = (int)spr.textureRect.height;

		//ピクセルを回転
		int rotCount = (int)(rotAngle / 90);
		Debug.Log(rotCount);

		for(int i = 0;i < rotCount;i++) {

			for(int y = 0;y < height;y++) {
				for(int x = 0;x < width;x++) {

					distPixels[(width - 1 - y) + x * width] =
						pixels[x + y * width];
				}
			}
			//回転の適用
			distPixels.CopyTo(pixels, 0);
		}

		//裏から見てるので反転
		for(int y = 0;y < height;y++) {
			for(int x = 0;x < width;x++) {

				distPixels[(width - 1 - x) + y * width] =
					pixels[x + y * width];
			}
		}
		//反転の適用
		distPixels.CopyTo(pixels, 0);

		/*
		 0  1  2  3  4
		 5  6  7  8  9
		10 11 12 13 14
		15 16 17 18 19
		20 21 22 23 24

		  j
		i 0 1 2 3 4
		  1 3 4 3 2
		  2 1 1 1 1
		  3 2 4 6 8
		  4 2 3 4 5

		  4 3 2 1 0
		          1
				  2
				  3
				  4

		[i, j]
		[0, 0] => [0, 4]
		[0, 1] => [1, 4]
		[1, 0] => [0, 3]
		
		[i, j] => [j, width - 1 - i]
		[x + y*width] -> [(width - 1 - y) + x * width]

		//時計回り
		[x + y*width] -> [(width - 1 - y) + x * width]
		//逆
		[x + y*width] -> [y + ((width - 1) - x) * width]

		*/

		tex.SetPixels(pixels);
		tex.Apply();

		meshRenderer.material.SetTexture("_MainTex", tex);


		//パーティクル生成
		SetParticle();
		
		//更新
		UpdateParticle();
	}

	/// <summary>
	/// パーティクルを生成
	/// </summary>
	void SetParticle() {

		//パーティクルを4方向に作成
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

			//触手が生成できる壁のみエフェクト再生
			if (StageGenerator.CheckStageBound(checkPos[i]) ||
				StageGenerator.GetPiece(checkPos[i])) {

				ps[i].Stop();
			}else{
				ps[i].Play();
			}

		}

	}
}

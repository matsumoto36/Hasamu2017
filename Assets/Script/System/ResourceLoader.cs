using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum R_MapChipType {
	MainChip,
	Sub1_Tentacle,
	Hole,
}

public enum R_OtherSpriteType {
	Tentacle,
	Mask,
}

public enum R_MaterialType {
	MaskingSprite,
	MaskableSprite,
	AdditiveSprite,
}

public enum R_PrefabType {
	TentacleBody,
}

public class ResourceLoader : MonoBehaviour {

	const int MAPCHIPWIDTH = 4;

	static List<Sprite[]> mapChipList;
	static Sprite[] otherSpriteArray;
	static Material[] materialArray;
	static GameObject[] prefabArray;

	public Sprite[] mapChipSrc;
	public Sprite[] otherSpriteSrc;
	public Material[] materialSrc;
	public GameObject[] prefabSrc;

	/// <summary>
	/// 素材をロードする
	/// </summary>
	public void LoadAll() {

		//マップチップ読み込み
		mapChipList = new List<Sprite[]>();
		for(int i = 0;i < mapChipSrc.Length;i++) {
			mapChipList.Add(Split(mapChipSrc[i], MAPCHIPWIDTH, System.Enum.GetName(typeof(R_MapChipType), i)));
		}

		Debug.Log("All MapChip Loaded.");

		//そのほかの画像読み込み
		otherSpriteArray = otherSpriteSrc;

		//マテリアル読み込み
		materialArray = materialSrc;

		//プレハブ読み込み
		prefabArray = prefabSrc;
	}

	#region GetResource

	/// <summary>
	/// マップチップを取得する
	/// </summary>
	/// <param name="type">マップチップのタイプ</param>
	/// <returns>マップチップ</returns>
	public static Sprite[] GetChips(R_MapChipType type) {
		return mapChipList[(int)type];
	}

	/// <summary>
	/// マップチップ以外のスプライトを取得する
	/// </summary>
	/// <param name="type">スプライトのタイプ</param>
	/// <returns>スプライト</returns>
	public static Sprite GetOtherSprite(R_OtherSpriteType type) {
		return otherSpriteArray[(int)type];
	}

	/// <summary>
	/// マテリアルを取得する
	/// </summary>
	/// <param name="type">マテリアルのタイプ</param>
	/// <returns>マテリアル</returns>
	public static Material GetMaterial(R_MaterialType type) {
		return materialArray[(int)type];
	}

	/// <summary>
	/// プレハブを取得する
	/// </summary>
	/// <param name="type">プレハブのタイプ</param>
	/// <returns>マテリアル</returns>
	public static GameObject GetPrefab(R_PrefabType type) {
		return prefabArray[(int)type];
	}

	#endregion

	/// <summary>
	/// スプライトを分割する
	/// </summary>
	/// <param name="src">元のスプライト</param>
	/// <param name="sqCount">一辺の分割数</param>
	/// <param name="name">おなまえ</param>
	/// <returns>分割されたスプライト</returns>
	Sprite[] Split(Sprite src, int sqCount, string name) {

		Sprite[] sprRet = new Sprite[sqCount * sqCount];

		Texture2D tex = src.texture;
		Vector2 pixelSize = new Vector2(tex.width / sqCount, tex.height / sqCount);

		for(int i = 0;i < sqCount;i++) {
			for(int j = 0;j < sqCount;j++) {

				Rect r = new Rect(new Vector2(j * pixelSize.x, tex.height - (i + 1) * pixelSize.y), pixelSize);

				sprRet[i * 4 + j] = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), pixelSize.x);
				sprRet[i * 4 + j].name = name + " " + (i * 4 + j);
			}
		}

		Debug.Log("Loaded " + name);

		return sprRet;
	}
}

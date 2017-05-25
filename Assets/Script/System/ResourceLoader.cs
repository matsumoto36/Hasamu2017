﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapChipType {
	MainChip,
	Sub1_Tentacle,
}

public class ResourceLoader : MonoBehaviour {

	const int MAPCHIPWIDTH = 4;
	const float PIXELPERUNIT = 100;

	static List<Sprite[]> mapChipList;
	public Sprite[] mapChipSrc;

	void Awake() {
		LoadChip();
	}

	/// <summary>
	/// 指定したマップチップの一辺のサイズを取得
	/// </summary>
	/// <param name="type">マップチップのタイプ</param>
	/// <returns>サイズ</returns>
	public static float GetChipSize(MapChipType type) {
		return mapChipList[(int)type][0].bounds.size.x;
	}

	/// <summary>
	/// マップチップをロードする
	/// </summary>
	void LoadChip() {

		mapChipList = new List<Sprite[]>();
		for(int i = 0;i < mapChipSrc.Length;i++) {
			mapChipList.Add(Split(mapChipSrc[i], MAPCHIPWIDTH, PIXELPERUNIT, System.Enum.GetName(typeof(MapChipType), i)));
		}

		Debug.Log("All MapChip Loaded.");
	}

	/// <summary>
	/// マップチップを取得する
	/// </summary>
	/// <param name="type">マップチップのタイプ</param>
	/// <returns>マップチップ</returns>
	public static Sprite[] GetChips(MapChipType type) {
		return mapChipList[(int)type];
	}

	/// <summary>
	/// スプライトを分割する
	/// </summary>
	/// <param name="src">元のスプライト</param>
	/// <param name="sqCount">一辺の分割数</param>
	/// <param name="pixPUnit">PixelPerUnit</param>
	/// <param name="name">おなまえ</param>
	/// <returns></returns>
	Sprite[] Split(Sprite src, int sqCount, float pixPUnit, string name) {

		Sprite[] sprRet = new Sprite[sqCount * sqCount];

		Texture2D tex = src.texture;
		Vector2 pixelSize = new Vector2(tex.width / sqCount, tex.height / sqCount);

		for(int i = 0;i < sqCount;i++) {
			for(int j = 0;j < sqCount;j++) {

				Rect r = new Rect(new Vector2(j * pixelSize.x, tex.height - (i + 1) * pixelSize.y), pixelSize);

				sprRet[i * 4 + j] = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), pixPUnit);
				sprRet[i * 4 + j].name = name + " " + (i * 4 + j);
			}
		}

		Debug.Log("Loaded " + name);

		return sprRet;
	}
}
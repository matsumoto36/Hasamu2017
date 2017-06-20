using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceTentacle : Piece {

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
		
	}

}

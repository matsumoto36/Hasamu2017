using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バーチャルなプレイヤー
/// </summary>
public class Player : MonoBehaviour {


	public bool isAction = false;
	Piece[] currentPiece = new Piece[2];

	// Update is called once per frame
	void Update () {

		Vector2[] pos = new Vector2[2];

		if(InputManager.GetInputDouble(out pos)) {

			pos[0] = Camera.main.ScreenToWorldPoint(pos[0]);
			pos[1] = Camera.main.ScreenToWorldPoint(pos[1]);

			if(!isAction) {

				RaycastHit2D[] hits = new RaycastHit2D[2];
				hits[0] = Physics2D.Raycast(pos[0], Vector2.zero);
				hits[1] = Physics2D.Raycast(pos[1], Vector2.zero);
				if(!hits[0] || !hits[1]) return;

				currentPiece[0] = hits[0].collider.GetComponent<Piece>();
				currentPiece[1] = hits[1].collider.GetComponent<Piece>();

				if(currentPiece[0].id != currentPiece[1].id) return;
				isAction = true;
			}

			currentPiece[0].transform.position = pos[0];
			currentPiece[1].transform.position = pos[1];

		}
		else if(InputManager.GetInput(out pos[0])) {

		}



	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミックのインターフェース
/// </summary>
interface IExecutable {

	/// <summary>
	/// はさまれているときに毎フレーム呼ばれる
	/// </summary>
	void Action();

}

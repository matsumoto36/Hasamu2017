using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleClick : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(MoveScene());
    }

    IEnumerator MoveScene()
    {
        yield return new WaitForSeconds(1.0f);

        SumCanvasAnimation.MoveScene("Menu");
    }
}

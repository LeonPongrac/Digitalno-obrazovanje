using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireHandler : MonoBehaviour
{
    public int id;
    public bool solution = false;

    private void OnMouseDown()
    {
        PuzzleConfig p = this.GetComponentInParent<PuzzleConfig>();
        p.result(solution);
    }
}

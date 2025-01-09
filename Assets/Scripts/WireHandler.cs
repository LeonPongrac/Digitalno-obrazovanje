using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireHandler : MonoBehaviour
{
    public int id;
    public bool solution = false;
    public bool canBePressd = false;

    private void OnMouseDown()
    {
        if (canBePressd)
        {
            PuzzleConfig p = this.GetComponentInParent<PuzzleConfig>();
            p.result(solution);
        }        
    }

}

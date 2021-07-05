using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChessFigure 
{
    public List<Vector3> GetMovePositions();

    public void MoveTo(Vector3 position);
}

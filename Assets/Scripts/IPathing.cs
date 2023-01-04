using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPathing
{

    void RequestPath(Vector3 target);
    void OnPathCompleted(Path path);
    void DestroyPathing();
}

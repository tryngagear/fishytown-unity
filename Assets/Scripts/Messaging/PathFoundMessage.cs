using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PathFoundMessage : NonIntrusiveMessage
{
    protected Path _path;
    public Path Path => _path;

    public PathFoundMessage(Path p) : base(MessageType.Pathfinding)
    {
        _path = p;
    }

}

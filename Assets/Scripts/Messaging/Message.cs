using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Message
{
    public enum MessageType
    {
        Behaviour,
        Need,
        Pathfinding
    }

    protected MessageType _type;

    public MessageType Type => _type;

    public Message(MessageType type)
    {
        _type = type;
    }
}

public abstract class NonIntrusiveMessage : Message
{
    public NonIntrusiveMessage(MessageType type) : base(type) { }
}

public abstract class IntrusiveMessage : Message
{
    protected Type _target;
    public Type Target => _target;

    public IntrusiveMessage(MessageType type, Type target) : base(type)
    {
        _target = target;
    }
}




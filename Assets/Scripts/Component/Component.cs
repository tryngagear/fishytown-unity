using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Component : ITickable, IMessaging
{
    Actor _owner;

    public Component(Actor actor)
    {
        _owner = actor;
    }

    public virtual void Tick(int tick) { }
    public virtual void LongTick(int tick) { }

    public virtual void OnNonIntrusiveMessage(NonIntrusiveMessage message) { }

    public virtual object OnIntrusiveMessage(IntrusiveMessage message)
    {
        return null;
    }
}

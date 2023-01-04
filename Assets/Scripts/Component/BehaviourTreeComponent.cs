using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BehaviourTreeComponent : Component
{
    public BehaviourTreeComponent(Actor actor) : base(actor)
    {

    }

    public override void Tick(int tick)
    {

    }

    public override void OnNonIntrusiveMessage(NonIntrusiveMessage message)
    {
        throw new NotImplementedException();
    }

    public override object OnIntrusiveMessage(IntrusiveMessage message)
    {
        throw new NotImplementedException();
    }
}

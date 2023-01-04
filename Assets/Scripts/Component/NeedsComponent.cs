using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NeedsComponent : Component
{
    List<Need> _needs;
    public List<Need> Needs => _needs;

    public NeedsComponent(Actor actor) : base(actor)
    {
        _needs = new List<Need>();
    }

    public override void Tick(int tick)
    {
        for (int i = 0; i < _needs.Count; i++)
            _needs[i].Update();
    }

    public void AddNeed(NeedDef def)
    {
        _needs.Add(new Need(def));
    }

    public Need GetNeedByName(string name)
    {
        return _needs.FirstOrDefault(x => x.Name == name);
    }
}

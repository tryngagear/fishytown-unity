using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Villager : Actor
{
    public List<NeedDef> needs;

    NeedsComponent _needsTracker;

    public NeedsComponent NeedsTracker => _needsTracker;

    protected override void Start()
    {
        base.Start();

        foreach (var need in needs)
            _needsTracker.AddNeed(need);
    }

    public override void Tick(int tick)
    {
        base.Tick(tick);
    }


}

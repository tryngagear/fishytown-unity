using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Entity : MonoBehaviour, ITickable
{
    protected virtual void Start()
    {
        //Subscribe to Tick event
        TickManager.Instance.OnTick += (object sender, TickManager.OnTickEventArgs e) => Tick(e.tick);

        //Subscribe to LongTick event
        TickManager.Instance.OnLongTick += (object sender, TickManager.OnTickEventArgs e) => LongTick(e.tick);
    }

    protected virtual void Update()
    {

    }

    public virtual void Tick(int tick)
    {

    }

    public virtual void LongTick(int tick)
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ComponentController : Component
{
    List<Component> _components;

    public ComponentController(Actor actor) : base(actor)
    {

    }

    public override void Tick(int tick)
    {
        foreach (var component in _components)
            component.Tick(tick);
    }

    public override void LongTick(int tick)
    {
        foreach (var component in _components)
            component.LongTick(tick);
    }

    public override void OnNonIntrusiveMessage(NonIntrusiveMessage message)
    {
        foreach (var component in _components)
            component.OnNonIntrusiveMessage(message);
    }

    public override object OnIntrusiveMessage(IntrusiveMessage message)
    {
        throw new NotImplementedException();
    }

    public Component GetComponentOfType<T>() where T : Component
    {
        return _components.FirstOrDefault(x => x is T);
    }

    public Component GetComponentOfType(Type type)
    {
        return _components.FirstOrDefault(x => x.GetType() == type);
    }

    public void AddComponent(Component component, bool forceOverride = false)
    {
        Component oldComp = GetComponentOfType(component.GetType());
        if(oldComp == null)
        {
            _components.Add(component);
        }
        else
        {
            if(!forceOverride)
            {
                Debug.LogError("Trying to add already existing component");
                return;
            }

            int index = _components.IndexOf(oldComp);
            _components[index] = component;
        }
    }
}

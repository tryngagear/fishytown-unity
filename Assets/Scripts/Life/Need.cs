using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need
{
    NeedDef def;

    float _currentValue;

    //Getters
    public string Name => def.needName;
    public string Description => def.needDescription;
    public int MaxValue => def.maxValue;
    public float DecayRate => def.decayRate;
    public float CurrentValue => _currentValue;

    public Need() { }

    public Need(NeedDef def)
    {
        this.def = def;
        _currentValue = def.maxValue;
    }

    public void Update()
    {
        if (_currentValue == 0)
            return;

        _currentValue -= DecayRate;

        if (_currentValue < 0)
            _currentValue = 0;
    }

}

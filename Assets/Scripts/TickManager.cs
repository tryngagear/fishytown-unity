using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : Singleton<TickManager>
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    public int ticksPerSecond = 60;
    public int longTickInterval = 5;

    public event EventHandler<OnTickEventArgs> OnTick;
    public event EventHandler<OnTickEventArgs> OnLongTick;

    int _currentTick;
    float _tickTimer;

    //Properties
    public float TickInterval => 1 / ticksPerSecond;

    // Start is called before the first frame update
    void Start()
    {
        _currentTick = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer < TickInterval)
            return;

        _tickTimer -= TickInterval;
        ++_currentTick;

        OnTick(this, new OnTickEventArgs { tick = _currentTick });

        if(_currentTick % longTickInterval == 0)
            OnLongTick(this, new OnTickEventArgs { tick = _currentTick });
    }
}

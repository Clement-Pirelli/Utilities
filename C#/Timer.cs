using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float _timer = .0f;
    float _passedTime = .0f;
    public float passedTime { get { return _passedTime; } }
    public float timer { get { return _timer; } }
    public bool isDone { get { return _passedTime > _timer; } }

    public Timer(float timer) { this._timer = timer; }

    public void tick()
    {
        _passedTime += Time.deltaTime;
    }

    public void tick(float deltaTime) 
    {
        _passedTime += deltaTime;
    }

    public void reset(float newTimer)
    {
        _timer = newTimer;
        _passedTime = .0f;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Utils
{

    public static List<int> FlagsToList(int flags)
    {

        var results = new List<int>();
        if (flags == 0)
        {
            return results;
        }
        int maxValue = Mathf.FloorToInt(Mathf.Log(flags, 2));
        for (int i = 0; i <= maxValue; i++)
        {
            bool hasI = (flags & (int)Mathf.Pow(2,i)) != 0;
            if (hasI)
            {
                results.Add(i);
            }

        }

        return results;
    }

    public static int AddFlags(int flags, params int[] toAdd)
    {
        foreach (var flag in toAdd)
        {
            flags |= 1 << flag;
        }
        return flags;

    }

    public static int RemoveFlags(int flags, params int[] toRemove)
    {
        foreach (var flag in toRemove)
        {
            flags &= ~ (flag+1);
        }
        return flags;

    }


}

public class CountdownTimer
{
    private float _timeLeft;
    private float _time;
    private bool _timerPaused;
    private bool _restartOnExpiration;

    public float TimeLeft => _timeLeft;
    public bool TimerPaused => _timerPaused;

    public delegate void TimerExpiredSignature();
    private event TimerExpiredSignature OnTimerExpired;

    List<TimerExpiredSignature> _RegisteredEvents;

    public CountdownTimer(float timerSeconds, bool shouldStartPaused = false, bool shouldLoop = false, TimerExpiredSignature onExpiredEvent = null)
    {
        _timeLeft = timerSeconds;
        _time = timerSeconds;
        _timerPaused = shouldStartPaused;
        _restartOnExpiration = shouldLoop;
        _RegisteredEvents = new List<TimerExpiredSignature>();
        if (onExpiredEvent != null)
        {
            RegisterEvent(onExpiredEvent);
        }
        if (ControllerCountdown.Instance)
        {

            ControllerCountdown.Instance.RegisterTimer(this);
        }
        else
        {
            Debug.LogWarning("No controller countown to auto register for updates");
        }
    }

    public void RegisterEvent(TimerExpiredSignature _event)
    {
        _RegisteredEvents.Add(_event);
        OnTimerExpired += _event;
    }

    public void UnregisterEvent(TimerExpiredSignature _event)
    {
        if (_RegisteredEvents.Contains(_event)){
            _RegisteredEvents.Remove(_event);
            OnTimerExpired -= _event;
        }
    }

    public void Dispose()
    {
        foreach (var _event in _RegisteredEvents)
        {
            if (_event != null)
            {
                OnTimerExpired -= _event;
            }
        }
        _RegisteredEvents.Clear();
        if (ControllerCountdown.Instance)
        {
            ControllerCountdown.Instance.UnregisterTimer(this);
        }
    }

    public void Update(float deltaTime)
    {
        if (!_timerPaused)
        {
            _timeLeft -= deltaTime;

        }
        else
        {
            return;
        }

        if (_timeLeft <= 0)
        {
            if (!_restartOnExpiration)
                Pause();

            if (OnTimerExpired != null)
                OnTimerExpired.Invoke();

            Reset();

        }
    }
    public void Pause() { _timerPaused = true; }
    public void Resume() { _timerPaused = false; }
    public void Reset() { _timeLeft = _time; }
    public void SetShouldLoop(bool loop) { _restartOnExpiration = loop; }
    public void SetNewTime(float time)
    {
        _time = time;
        _timeLeft = time;
    }
    public void StepTime(float step)
    {
        float newTime = _timeLeft + step;

        if (newTime > _time)
        {
            Reset();
            return;
        }
        else if (newTime <= 0)
        {
            Update(0);
            return;
        }
        else
            _timeLeft = newTime;
    }

  
}
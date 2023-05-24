using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

public class Utils
{
    static public T AddArrayElement<T>(ref T[] array, T elToAdd)
    {
        if (array == null)
        {
            array = new T[1];
            array[0] = elToAdd;
            return elToAdd;
        }

        var newArray = new T[array.Length + 1];
        array.CopyTo(newArray, 0);
        newArray[array.Length] = elToAdd;
        array = newArray;
        return elToAdd;
    }

    static public void DeleteArrayElement<T>(ref T[] array, int index)
    {
        if (index >= array.Length || index < 0)
        {
            Debug.LogWarning("invalid index in DeleteArrayElement: " + index);
            return;
        }
        var newArray = new T[array.Length - 1];
        int i;
        for (i = 0; i < index; i++)
        {
            newArray[i] = array[i];
        }
        for (i = index + 1; i < array.Length; i++)
        {
            newArray[i - 1] = array[i];
        }
        array = newArray;
    }

}
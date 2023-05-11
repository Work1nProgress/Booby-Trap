using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCountdown : GenericSingleton<ControllerCountdown>
{


    public List<CountdownTimer> countdownTimers = new List<CountdownTimer>();


    public void RegisterTimer(CountdownTimer countdownTimer)
    {

        if (!countdownTimers.Contains(countdownTimer)){
            countdownTimers.Add(countdownTimer);
        }
    }

    public void UnregisterTimer(CountdownTimer countdownTimer)
    {
        if (countdownTimers.Contains(countdownTimer)){
            countdownTimers.Remove(countdownTimer);  
        }
        countdownTimer = null;
    }

    private void Update()
    {
        for(int i = countdownTimers.Count-1; i >= 0; i--)
        {
            if (countdownTimers[i] != null)
            {
                countdownTimers[i].Update(Time.deltaTime);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCountdown : GenericSingleton<ControllerCountdown>
{


    public List<CountdownTimer> countdownTimers = new List<CountdownTimer>();

    bool updating;
    public void RegisterTimer(CountdownTimer countdownTimer)
    {

        if (!countdownTimers.Contains(countdownTimer)){
            countdownTimers.Add(countdownTimer);
        }
    }

    public void UnregisterTimer(CountdownTimer countdownTimer)
    {
        if (updating)
        {
            StartCoroutine(WaitBeforeRemove(countdownTimer));
        }
        else
        {
            Remove(countdownTimer);
        }
    }

    void Remove(CountdownTimer ct)
    {
        if (countdownTimers.Contains(ct))
        {

            countdownTimers.Remove(ct);

        }
        ct = null;
    }

    IEnumerator WaitBeforeRemove(CountdownTimer countdownTimer)
    {
        yield return new WaitUntil(() => !updating);
        Remove(countdownTimer);
    }

    private void Update()
    {
        updating = true;
        for (int i = countdownTimers.Count-1; i >= 0; i--)
        {
            if (countdownTimers[i] != null)
            {
                countdownTimers[i].Update(Time.deltaTime);
            }
        }
        updating = false;
    }
}

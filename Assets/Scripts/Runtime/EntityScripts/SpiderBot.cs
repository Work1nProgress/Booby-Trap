using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBot : EnemyBase
{

    protected override void UpdateSteps()
    {
        if (string.IsNullOrEmpty(Sound.StepsPassive))
        {
            return;
        }
        _stepsTimer -= Time.deltaTime * Mathf.Abs(Stats.MovementSpeed) * Sound.stepVelocityFactor;
        if (_stepsTimer < 0)
        {
            SoundManager.Instance.Play(Sound.StepsPassive, transform);
            _stepsTimer = Sound.stepDelay;
        }
    }
}

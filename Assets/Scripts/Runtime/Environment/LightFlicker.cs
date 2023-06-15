using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField]
    float defaultPeriod;

    [SerializeField]
    float randomPeriod;

    [SerializeField]
    float intensityAmplitude;

    [SerializeField]
    float radiusAmplitude;

    Light2D[] Lights;

    List<float> timers = new List<float>();
    List<float> periods = new List<float>();
    List<float> startIntensities = new List<float>();

    List<float> startInner = new List<float>();

    List<float> startOuter = new List<float>();


    private void Start()
    {
        Lights = GetComponentsInChildren<Light2D>();
        foreach (var light in Lights)
        {
            var period = defaultPeriod + Random.Range(-randomPeriod, randomPeriod);
            timers.Add(Random.value* period);
            periods.Add(period);
            startIntensities.Add(light.intensity);
            startInner.Add(light.pointLightInnerRadius);
            startOuter.Add(light.pointLightOuterRadius);
        }
    }

    void Update()
    {
        for (int i = 0; i < Lights.Length; i++)
        {
            timers[i] += Time.deltaTime;
            if (timers[i] >= periods[i])
            {



                Lights[i].intensity = startIntensities[i] + (Random.value - 0.5f) * intensityAmplitude;

                Lights[i].pointLightInnerRadius = startInner[i] + (Random.value - 0.5f) * radiusAmplitude;
                Lights[i].pointLightOuterRadius = startOuter[i] + (Random.value - 0.5f) * radiusAmplitude;
                timers[i] = 0;

            }

        }


    }
}

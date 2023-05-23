using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : PoolObjectTimed
{


    [SerializeField]
    SpriteRenderer spriteRenderer;

    float Timer = 0;

    private void Awake()
    {

        spriteRenderer.material = new Material(spriteRenderer.material);
    }

    public override void Reuse()
    {
        spriteRenderer.material.SetFloat("_Cutoff", 0.5f);
        Timer = duration;
        base.Reuse();
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        spriteRenderer.material.SetFloat("_Cutoff", Mathf.Min(0.99f, 1f - (Timer / duration) * 0.5f));
        Debug.Log($"{Mathf.Min(0.99f, 1f - (Timer / duration) * 0.5f)}");
    }
}

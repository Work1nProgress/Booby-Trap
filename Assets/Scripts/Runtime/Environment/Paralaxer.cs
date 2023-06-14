using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Paralaxer : MonoBehaviour
{
    [SerializeField]
    Transform follow;

    [SerializeField]
    MeshRenderer meshRenderer;


    [SerializeField]
    float DistanceX;

    [SerializeField]
    float DistanceY;

    [SerializeField]
    [Range(0,1f)]
    float lerp;

    private void Start()
    {
        meshRenderer.material = new Material(meshRenderer.material);
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(follow.position.x, follow.position.y, -5), lerp);

        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(transform.position.x/DistanceX, transform.position.y/DistanceY));
    }

}

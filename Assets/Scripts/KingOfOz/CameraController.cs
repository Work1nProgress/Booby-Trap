using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool usePlayerPoistionAsOffset;
    public Vector2 offsetFromPlayer;

    private new Transform camera;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;
        player = GameObject.FindWithTag("Player").transform;
        
        if (usePlayerPoistionAsOffset)
        {
            offsetFromPlayer.x = -player.position.x;
            offsetFromPlayer.y = -player.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // make camera follow player if still on level
        if (camera.position.y >= GameManager.Instance.minCameraY)
        {
            camera.position = new Vector3(player.position.x + offsetFromPlayer.x,
                                          player.position.y + offsetFromPlayer.y, -10);
        }
        else
        {
            //camera.position = new Vector3(player.position.x + offsetFromPlayer.x,
                                          //GameManager.Instance.minCameraY, -10);
        }
    }
}

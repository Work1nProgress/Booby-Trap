using Pathfinding;
using UnityEngine;

public class FlippingYouTheBird : MonoBehaviour
{
    private AIPath _aiPath;

    private void Awake()
    {
        _aiPath = GetComponentInParent<AIPath>();
    }
    
    void Update()
    {
        
    }
}

using UnityEngine;
using System.Collections;
using Pathfinding;

public class AI_Movement : MonoBehaviour
{
    public IAstarAI aI;
    public Transform target;

    private Seeker sekr;

    void Start()
    {
        aI = GetComponent<IAstarAI>();
        sekr.GetComponent<Seeker>();

        if(aI != null) aI.onSearchPath += UpdatePath;
        sekr.pathCallback += OnPathComplete;
    }

    public void UpdatePath()
    {
        sekr.StartPath(transform.position, target.position);
    }

    public void OnPathComplete(Path p)
    {

    }

    public void OnDisable()
    {
        sekr.pathCallback -= OnPathComplete;
    }
}
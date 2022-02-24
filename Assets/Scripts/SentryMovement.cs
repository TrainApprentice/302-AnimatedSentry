using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMovement : MonoBehaviour
{
    CharacterController pawn;
    NavMeshAgent agent;

    Transform navTarget;
    PlayerTargeting player;

    private float destinationTimer = 3f;
    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerTargeting>();

        if(player)
        {
            FindNewDestination();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (destinationTimer > 0) destinationTimer -= Time.deltaTime;
        else
        {
            FindNewDestination();
            destinationTimer = 3f;
        }
        //pawn.SimpleMove(Vector3.zero);
    }

    void FindNewDestination()
    {
        navTarget = player.transform;
        float randAngle = Random.Range(0, 360);
        float randDistance = Random.Range(10, 30);

        Vector3 offset = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * randDistance;

        navTarget.position = player.transform.position + offset;
        agent.destination = navTarget.position;
    }
}

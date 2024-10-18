using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Very lethargic enemy. Hides in the darkness so it is not very visible. Only flashes visibility if under light, but is not constantly visible under the light.
/// Moves very slowly around the map, and if it sees you, starts moving towards you slightly faster. If it loses sight, goes to the point, looks around, continues roaming slowly if it doesn't find you.
/// Kills player upon catching them.
/// </summary>
public class InvisibleEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private int hp;
    [SerializeField] private float moveRadius;

    private Vector3 movePos;

    private float waitUntilNewPos = 0.25f;
    private bool isGettingNewPos = false;

    private float movingToCurrentDestinationTimer = 0f;
    [SerializeField] private float maxTimeToGetToDestination;

    [Header("Animations")]
    [SerializeField] private Animator anim;
    [SerializeField] private float deathAnimationDuration;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        health = hp;
        speed = moveSpeed;
        dieAnimDuration = deathAnimationDuration;

        agent.speed = speed;
        agent.acceleration = speed / 2f;

        GetNewPosition();
    }

    private void Update()
    {
        movingToCurrentDestinationTimer += Time.deltaTime;
        if (movingToCurrentDestinationTimer > maxTimeToGetToDestination || (!isGettingNewPos && Vector3.Distance(transform.position - Vector3.up, movePos) < 0.2f))
        {
            isGettingNewPos = true;
            Invoke(nameof(GetNewPosition), waitUntilNewPos);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetNewPosition()
    {
        movingToCurrentDestinationTimer = 0f;
        isGettingNewPos = false;
        movePos = Random.insideUnitSphere * moveRadius;
        movePos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(movePos, out hit, moveRadius, 1);
        movePos = hit.position;
    }

    public override void Attack()
    {
        if (isStunned) return;

    }

    public override void Move()
    {
        if (isStunned) return;

        agent.SetDestination(movePos);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}

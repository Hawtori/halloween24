using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Not very lethargic, but also not very energetic. Gets tired fairly quickly. Does not like staying close to player so roams around the map avoiding the player.
/// Occasionally gets close to player but not always intent on attacking player. Sometimes, it does have intent to attack player so it will sprint towards player.
/// Sprinting and attacking only last while it has stamina, if not then it will revert to just roaming and lurking. Player can pass by close if it is out of stamina, 
/// but will kill player otherwise if they get close regardless of intent to attack.
/// </summary>
public class LurkerEnemy : EnemyBase
{
    [Header("Player reference")]
    public Transform player; // need to avoid while lurking, and where to chase to

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float maxDistanceFromPlayer = 15f; // furthest it will go from player
    public float minDistanceFromPlayer = 5f; // closest it will get to player

    [Header("Sprinting")]
    public float sprintSpeedMultiplier = 1.75f;
    public float sprintTimerMax = 9f; // max time until it starts sprinting at player
    public float sprintTimerMin = 5f; // the earliest it can sprint at player
    public float maxStamina = 10f;
    public float staminaRecoveryRate = 0.5f; // like half a tick per second recovery
    public float staminaUseRate = 1.5f; // 1 and a half ticks per second sprinting

    [Header("State changing")]
    public float maxTimeToSpendOnCurrentPath = 10f; // how often to update state or path
    private float timeSpentOnCurrentPath = 0f;
    
    [Header("Combat")]
    public float reach = 2f; // how close we have to be to kill player
    public float chanceToSprint = 0.10f; // percent chance it goes into an offensive

    [Header("Internal")]
    [SerializeField]
    private int numberOfPointsInPath = 6;
    [SerializeField]
    private List<Vector3> pathPoints = new List<Vector3>();
    private float currentStamina;
    private bool isSprinting = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindWithTag("Player").transform;

        speed = moveSpeed;
        dieAnimDuration = 1.5f;
        health = 10;

        pathPoints = new List<Vector3>(numberOfPointsInPath);

        currentStamina = maxStamina;

        CalculatePath();
        agent.SetDestination(pathPoints[0]);
        //InvokeRepeating(nameof(Move), 0, updateStateInterval);
    }

    private void Update()
    {
        if(!isSprinting && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRecoveryRate * Time.deltaTime, maxStamina);
        }

        if (Vector3.Distance(player.position, transform.position) < reach && currentStamina > maxStamina / 2f) Attack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void StartSprinting()
    {
        Debug.Log("Sprinting at player");

        isSprinting = true;
        agent.speed = speed * sprintSpeedMultiplier;

        MoveTo(player.position);

        currentStamina -= staminaUseRate * Time.deltaTime;

        if (currentStamina <= 0) StopSprint();
    }

    private void StopSprint()
    {
        isSprinting = false;
        agent.speed = speed;
        currentStamina = 0;
    }

    // TODO improve
    private void CalculatePath()
    {
        pathPoints.Clear();

        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        for(int i = 0; i < numberOfPointsInPath; i++)
        {
            float dist = Mathf.Lerp(maxDistanceFromPlayer, minDistanceFromPlayer, (float)i / (numberOfPointsInPath - 1));
            Vector3 point = transform.position - dirToPlayer * dist;
            point.y = transform.position.y;
            
            pathPoints.Add(point);
            dirToPlayer = (player.position - point).normalized;
        }
    }

    private void FollowPath()
    {
        //if (isSprinting) return;

        if(timeSpentOnCurrentPath >= maxTimeToSpendOnCurrentPath)
        {
            CalculatePath();
            MoveTo(pathPoints[0]);
            timeSpentOnCurrentPath = 0;
        }

        timeSpentOnCurrentPath += Time.deltaTime;

        if(pathPoints.Count > 0 && agent.remainingDistance <= agent.stoppingDistance)
        {
            pathPoints.RemoveAt(0);
            if (pathPoints.Count == 0) CalculatePath();
            else MoveTo(pathPoints[0]);
        }
    }

    public override void Attack()
    {
        if (isStunned) return;

        Debug.Log("Killed player, distance to player: " + Vector3.Distance(transform.position, player.position) + ", stamina: " + currentStamina);

    }

    public override void Move()
    {
        if (isStunned) return;

        if (currentStamina >= maxStamina / 1.05f)
        {
            if (Random.Range(0, 1.01f) < chanceToSprint) 
            {
                StartSprinting();
                return;
            }
        }

        FollowPath();


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header ("Enemy stats")]
    protected int health;
    protected float speed;
    protected float dieAnimDuration;

    protected NavMeshAgent agent;

    protected bool isStunned = false; // if player hits enemy, stun for a short duration

    public abstract void Attack();

    public virtual void TakeDamage(int damage)
    {
        isStunned = true;
        Invoke(nameof(ResetStun), 0.15f);

        health -= damage;
        if (health < 0)
            Die();
    }

    private void ResetStun()
    {
        isStunned = false;
    }

    public virtual void Die()
    {
        Destroy(gameObject, dieAnimDuration);
    }

    public abstract void Move();

    protected void MoveTo(Vector3 targetPosition)
    {
        if(agent) agent.SetDestination(targetPosition);
    }

    protected void StopMoving()
    {
        if (agent) agent.ResetPath();
    }
}

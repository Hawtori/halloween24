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
    protected Vector3 knockBackDir;
    protected float knockBackForce;


    protected bool isStunned = false; // if player hits enemy, stun for a short duration

    public abstract void Attack();

    public virtual void TakeDamage(int damage, Vector3 fromWhere)
    {
        isStunned = true;
        Invoke(nameof(ResetStun), 0.25f);

        knockBackDir = (transform.position - fromWhere).normalized;
        knockBackForce = damage * 3f;

        GetComponent<Rigidbody>().AddForce(knockBackForce * knockBackDir + knockBackForce * Vector3.up, ForceMode.VelocityChange);


        health -= damage;
        if (health < 0)
            Die();
    }

    private void ResetStun()
    {
        isStunned = false;
        if (agent)
        {
            agent.isStopped = false;
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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

    private void OnCollisionEnter(Collision collision)
    {
        if(agent)
        agent.isStopped = true;
        Invoke(nameof(ResetStun), 0.25f);
    }
}

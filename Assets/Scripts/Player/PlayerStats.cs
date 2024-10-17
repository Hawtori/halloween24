using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int health = 10;
    private float stamina = 10;
    private float speed = 1f;

    private float staminaRecoverySpeed = 2f; // 2 ticks per second
    private float staminaUseageSpeed = 4f;
    private static float staminaRecoveryWaitTime = 2f;
    private float staminaRecoveryWaitTimer = 0f;

    private bool canSprint = true;
    private bool isSprinting = false;

    public void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            Time.timeScale = 0f;
            Debug.Log("Died");
        }
    }

    public void Heal(int healAmount)
    {
        health = Mathf.Max(health + healAmount, 10);
    }

    // TODO need to implement better
    public void Sprint()
    {
        if (stamina <= 0)
        {
            Debug.Log("No stamina");
            isSprinting = false;
            speed = 0.5f;
            return;
        }

        Debug.Log("Sprinting");
        isSprinting = true;
        speed = 1.5f;
        stamina -= staminaUseageSpeed * Time.deltaTime;
    }

    public void StopSprint()
    {
        isSprinting = false;
        Debug.Log("Not sprinting");

        if (stamina <= 0)
        {
            speed = 0.5f;
        }
        else if (stamina > 2f) 
            speed = 1f;

        if (staminaRecoveryWaitTimer > staminaRecoveryWaitTime)
        {
            stamina = Mathf.Max(stamina + staminaRecoverySpeed * Time.deltaTime, 10);
        }
        else
        {
            staminaRecoveryWaitTimer += Time.deltaTime;
        }
    }

    public float GetSpeedModifier() => speed;
}

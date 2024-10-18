using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int health = 10;
    private float stamina = 10;
    private float speed = 1f;

    private const float sprintSpeed = 1.5f;
    private const float walkSpeed = 1f;
    private const float exhaustSpeed = 0.5f;

    private const float maxStamina = 10f;
    private const float minStamina = 4f; // how much stamina we need to be back to walk speed

    private float staminaRecoverySpeed = 1.5f; // 2 ticks per second
    private float staminaUseageSpeed = 3.5f;
    private static float staminaRecoveryWaitTime = 2f;
    private float staminaRecoveryWaitTimer = 0f;

    private bool isSprinting = false;

    public void Update()
    {
        if(isSprinting)
        {
            Sprint();
            return;
        }

        if(stamina <= 0 || (stamina < minStamina && !isSprinting))
        {
            speed = exhaustSpeed;
        }
        else if (stamina > minStamina)
        {
            speed = walkSpeed;
        }

        if (staminaRecoveryWaitTimer > staminaRecoveryWaitTime) // start recovering stamina
        {
            stamina = Mathf.Min(stamina + staminaRecoverySpeed * Time.deltaTime, maxStamina);
        }
        else staminaRecoveryWaitTimer += Time.deltaTime;
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
        health = Mathf.Min(health + healAmount, 10);
    }

    public void Sprint()
    {
        if (stamina <= 0)
        {
            isSprinting = false;
            return;
        }
        isSprinting = true;
        speed = sprintSpeed;
        stamina = Mathf.Max(stamina - staminaUseageSpeed * Time.deltaTime, 0);
        staminaRecoveryWaitTimer = 0f;
    }

    public void StopSprint()
    {
        isSprinting = false;
    }

    public float GetSpeedModifier() => speed;
    public float GetStamina() => stamina;
    public int GetState()
    {
        // states: exhausted, walking, sprinting
        // indexes     1    ,    2   ,     3

        if (speed == exhaustSpeed || stamina < minStamina) return 1;
        if (speed == walkSpeed) return 2;
        return 3;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UnitHealth : MonoBehaviour
{
    #region Variables
    [SerializeField] private int health = 100;
    private int maxHealth;
    private bool canBeHealed;
    #endregion

    #region Events
    public event Action OnDead;
    public event Action OnDamaged;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        maxHealth = health;
        canBeHealed = false;
    }
    #endregion


    #region Logic
    public void Damage(int damage)
    {
        health -= damage;
        canBeHealed = true;

        if (health < 0)
            health = 0;

        OnDamaged?.Invoke();

        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke();
    }

    public float getHealthPrecent()
    {
        return  (float)health/ maxHealth;
    }

    public int getCurrentHealth()
    {
        return health;
    }

    public bool getCanBeHealed()
    {
        return canBeHealed;
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth) { 
            health = maxHealth;
            canBeHealed = false;
        }
        else
            canBeHealed = true;

        OnDamaged?.Invoke();
    }
    #endregion
}

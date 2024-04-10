using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SC_UnitAnimator : MonoBehaviour
{
    #region Variables
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform medkitPrefab;
    [SerializeField] private Transform gunPrefab;

    SC_MoveAction moveAction;
    SC_ShootAction shootAction;
    SC_HealAction healAction;

    #endregion




    #region MonoBehaviour
    private void Awake()
    {
        moveAction = GetComponent<SC_MoveAction>();
        shootAction = GetComponent<SC_ShootAction>();
        healAction = GetComponent<SC_HealAction>();
        if (moveAction != null)
        {
            moveAction.OnStartMoving += OnStartMoving;
            moveAction.OnStopMoving += OnStopMoving;
        }
        if (shootAction != null)
        {
            shootAction.OnShoot += OnShoot;
        }
        if (healAction != null)
        {
            healAction.OnStartHealing += OnStartHealing;
            healAction.OnFinishHealing += OnFinishHealing;
        }

    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.OnStartMoving -= OnStartMoving;
            moveAction.OnStopMoving -= OnStopMoving;
        }
        if (shootAction != null)
        {
            shootAction.OnShoot -= OnShoot;
        }
        if (healAction != null)
        {
            healAction.OnStartHealing -= OnStartHealing;
            healAction.OnFinishHealing -= OnFinishHealing;
        }
    }

    #endregion

    #region EventFunctions
    private void OnShoot(SC_Unit targetUnit)
    {
        animator.SetTrigger("Shoot");
        
        Transform bullet = Instantiate(bulletProjectilePrefab,shootPoint.position,Quaternion.identity);

        Vector3 targetUnitShootAtPosition = targetUnit.getWorldPosition();
        targetUnitShootAtPosition.y = shootPoint.position.y;

        bullet.GetComponent<SC_BulletProjectile>().setup(targetUnitShootAtPosition);
    
    }
    
    private void OnFinishHealing()
    {
        medkitPrefab.gameObject.SetActive(false);
        gunPrefab.gameObject.SetActive(true);
    }

    private void OnStartHealing()
    {
        gunPrefab.gameObject.SetActive(false);
        medkitPrefab.gameObject.SetActive(true);
        animator.SetTrigger("Healing");
    }

    private void OnStopMoving()
    {
        animator.SetBool("IsWalking", false);
    }

    private void OnStartMoving()
    {
        animator.SetBool("IsWalking", true);
    }

    #endregion
}

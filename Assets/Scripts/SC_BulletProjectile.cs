using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BulletProjectile : MonoBehaviour
{
    #region Variables

    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;
    #endregion

    #region Logic
    public void setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    #endregion

    #region MonoBehaviour
    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distanceBeforeShooting = Vector3.Distance(transform.position, targetPosition);

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterShooting = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeShooting < distanceAfterShooting)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate (bulletHitVfxPrefab, targetPosition, Quaternion.identity) ;
        }
    }
    #endregion


}

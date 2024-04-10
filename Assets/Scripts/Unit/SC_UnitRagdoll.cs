using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UnitRagdoll : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform ragdollPrefab;
    public static List<Transform> ragdollList;
    private SC_UnitHealth unitHealth;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        ragdollList = new List<Transform>();
        unitHealth = GetComponent<SC_UnitHealth>();
        unitHealth.OnDead += OnDead;
    }
    #endregion


    #region EventFunctions
    private void OnDead()
    {
        ragdollList.Add(Instantiate(ragdollPrefab, transform.position, transform.rotation));
    }

    #endregion


}

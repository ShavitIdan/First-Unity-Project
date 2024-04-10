using FischlWorks_FogWar;
using System.Collections.Generic;
using UnityEngine;
using static FischlWorks_FogWar.csFogWar;

public class SC_FogManager : MonoBehaviour
{
    #region Singleton

    public static SC_FogManager Instance { get; private set; }


    #endregion

    #region Variables
    [SerializeField] private csFogWar csFogWarReference;
    [SerializeField] private int RevealerSightRange = 17;
    private Dictionary<SC_Unit,int> unitIndexMap;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        if(Instance == null)
                Instance = this;
        
        unitIndexMap = new Dictionary<SC_Unit,int>();

    }
    #endregion

    #region Logic

    public void addRevealer(SC_Unit unit)
    {
        if (unit != null)
        {
            FogRevealer newRevealer = new FogRevealer(unit.transform, RevealerSightRange, false);
            unitIndexMap.Add(unit, csFogWarReference.AddFogRevealer(newRevealer));
        }
    }

    public void removeRevealer(SC_Unit unit)
    {
        if (unit == null || !unitIndexMap.ContainsKey(unit))
            return;

        int removedIndex = unitIndexMap[unit];
        csFogWarReference.RemoveFogRevealer(removedIndex);
        unitIndexMap.Remove(unit);

        List<SC_Unit> keysToUpdate = GetKeysToUpdate(removedIndex);

        UpdateIndices(keysToUpdate);

    }

    private List<SC_Unit> GetKeysToUpdate(int removedIndex)
    {
        List<SC_Unit> keysToUpdate = new List<SC_Unit>();
        foreach (SC_Unit key in unitIndexMap.Keys)
        {
            if (unitIndexMap[key] > removedIndex)
            {
                keysToUpdate.Add(key);
            }
        }
        return keysToUpdate;
    }

    private void UpdateIndices(List<SC_Unit> keysToUpdate)
    {
        foreach (var key in keysToUpdate)
        {
            unitIndexMap[key]--;
        }
    }

    #endregion

}

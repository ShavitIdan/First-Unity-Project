using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SC_GridDebug : MonoBehaviour
{

    [SerializeField] private TextMeshPro textMeshPro;
    private object gridNode;
    public virtual void SetGridNode (object g)
    {
        gridNode = g;
    }
    protected virtual void Update()
    {
        if(gridNode != null)
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = gridNode.ToString();

            }
        }
        
    }
}

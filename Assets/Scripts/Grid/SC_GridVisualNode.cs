using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GridVisualNode : MonoBehaviour
{
    #region Variables
    [SerializeField] private MeshRenderer meshRenderer;
    #endregion

    #region Logic
    public void Show( Material material)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = material;
    }

    public void Hide() { 
        meshRenderer.enabled = false; 
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GridVisual : MonoBehaviour
{
    #region Singleton
    public static SC_GridVisual Instance { private set; get; }
    #endregion

    #region Variables
    [SerializeField] private Transform GridVisualNode;
    [SerializeField] private List<GridVisualTypeMaterial> MaterialsList;
    private SC_GridVisualNode[,] gridVisualNodesArray;

    
    [Serializable] public struct GridVisualTypeMaterial
    {
        public gridVisualType visualType;
        public Material material;
    }

    public enum gridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Green
    }

    #endregion
    #region MonoBehaviour
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }


    private void Start()
    {
        gridVisualNodesArray = new SC_GridVisualNode[SC_LevelGrid.Instance.getWidth(), SC_LevelGrid.Instance.getHeight()];


        for (int x = 0; x <SC_LevelGrid.Instance.getWidth(); x++)
        {
            for (int z = 0; z < SC_LevelGrid.Instance.getHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform node = Instantiate(GridVisualNode, SC_LevelGrid.Instance.getWorldPosition(gridPosition),Quaternion.identity);

                gridVisualNodesArray[x,z] = node.GetComponent<SC_GridVisualNode>();
            }
        }
        //updateGridVisual();

        SC_LevelGrid.Instance.OnAnyUnitMoved += OnAnyUnitMoved;
        SC_UnitAction.Instance.OnChangeSelectedAction += OnChangeSelectedAction;



    }



    private void OnDisable()
    {
        SC_UnitAction.Instance.OnChangeSelectedAction -= OnChangeSelectedAction;
        SC_LevelGrid.Instance.OnAnyUnitMoved -= OnAnyUnitMoved;

    }

    #endregion

    #region Logic

    public void HideAllNodes()
    {
        for (int x = 0; x < SC_LevelGrid.Instance.getWidth(); x++)
        {
            for (int z = 0; z < SC_LevelGrid.Instance.getHeight(); z++)
            {

                gridVisualNodesArray[x, z].Hide();
            }
        }
    }

    public void ShowNodeList(List<GridPosition> gridPositionList, gridVisualType visualType) { 
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridVisualNodesArray[gridPosition.x, gridPosition.z].Show(getGridVisualTypeMaterial(visualType));
        }   
    }

    private void ShowRange(GridPosition gridPosition,int range, gridVisualType visualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!SC_LevelGrid.Instance.isValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                    continue;

                gridPositions.Add(testGridPosition);
            }

            ShowNodeList(gridPositions, visualType);
        }
    }

    private void updateGridVisual()
    {
        HideAllNodes();

        SC_BaseAction selectedAction = SC_UnitAction.Instance.getSelecetedAction();
        SC_Unit unit = SC_UnitAction.Instance.GetSelectedUnit();
        gridVisualType visualType;

        switch (selectedAction)
        {
            case SC_MoveAction moveAction:
                visualType = gridVisualType.White;
                break;
            
            case SC_ShootAction shootAction:
                visualType = gridVisualType.Red;

                ShowRange(unit.GetGridPosition(), shootAction.getMaxShootDistance(), gridVisualType.RedSoft);
                break;

            case SC_HealAction healAction:
                visualType = gridVisualType.Green;
                break;

            default:
                visualType = gridVisualType.White;
                break;

        }

        ShowNodeList(selectedAction.getValidPositions(), visualType);

    }
     private Material getGridVisualTypeMaterial(gridVisualType visualType) {
            foreach(GridVisualTypeMaterial gvtm in MaterialsList)
            {
                if(gvtm.visualType == visualType)
                {
                    return gvtm.material;
                }
            }
            Debug.LogError("Could not find Material for this visual type: " + visualType);
            return null;
        }
    #endregion

    #region Event Functions
    private void OnChangeSelectedAction(object sender)
    {
        updateGridVisual();
    }

    private void OnAnyUnitMoved()
    {
        updateGridVisual();

    }

    #endregion 




}

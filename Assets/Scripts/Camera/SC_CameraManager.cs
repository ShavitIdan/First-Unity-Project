using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CameraManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject actionCamera;
    [SerializeField] LayerMask normalCameraCullingMask;
    [SerializeField] LayerMask actionCameraCullingMask;
    #endregion

    #region MonoBehaviour

    private void Start()
    {
        hideActionCamera();

    }

    private void OnEnable()
    {
        SC_BaseAction.OnActionStarted += OnActionStarted;
        SC_BaseAction.OnActionCompleted += OnActionCompleted;

    }

    private void OnDisable()
    {
        SC_BaseAction.OnActionStarted -= OnActionStarted;
        SC_BaseAction.OnActionCompleted += OnActionCompleted;


    }
    #endregion

    #region Logic

    private void showActionCamera()
    {
        Camera.main.cullingMask = actionCameraCullingMask;
        actionCamera.SetActive(true);
    }

    private void hideActionCamera()
    {
        if(actionCamera != null)
        {
            Camera.main.cullingMask = normalCameraCullingMask;
            actionCamera.SetActive(false);
        }
        
    }
    #endregion

    #region Event functions

    private void OnActionStarted(object sender)
    {
        switch (sender)
        {
            case SC_ShootAction shootAction:
                SC_Unit shooter = shootAction.getUnit();
                SC_Unit target = shootAction.getTargetUnit();
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDirection = (target.getWorldPosition() - shooter.getWorldPosition()).normalized;

                float offsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * offsetAmount;

                Vector3 actionCameraPos = shooter.getWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDirection * -1);

                actionCamera.transform.position = actionCameraPos;
                actionCamera.transform.LookAt(target.getWorldPosition() + cameraCharacterHeight);

                showActionCamera();
                break;
        }
    }

    private void OnActionCompleted(object sender) { 
        switch (sender) 
        {  
            case SC_ShootAction shootAction:
                hideActionCamera();
                break;
        
        } 
    }
    #endregion


}

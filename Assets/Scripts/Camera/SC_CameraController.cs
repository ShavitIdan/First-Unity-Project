using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SC_CameraController : MonoBehaviour
{
    #region Variables
    [SerializeField] private CinemachineVirtualCamera cinemachineCam;

    [SerializeField] private float cameraMoveSpeed = 10f;
    [SerializeField] private float cameraRotationSpeed = 100f;
    [SerializeField] private float cameraZoomAmount = 1f;
    [SerializeField] private float cameraZoomSpeed = 5f;
    private const float MinFollowYOffset = 2f;
    private const float MaxFollowYOffset = 12f;
    private Vector3 followOffset;
    private CinemachineTransposer cinemachineTransposer;

    #endregion

    #region MnoBehaviour
    private void Awake()
    {
        cinemachineTransposer = cinemachineCam.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = cinemachineTransposer.m_FollowOffset;
    }


    private void Update()
    {
        handleMovement();
        handleRotation();
        handleZoom();


    }

    private void Start()
    {
        if (SC_GameManager.Instance.getIsMultiplayer())
        {
            if (SC_GameManager.Instance.getNextTurnPlayer() == SC_GlobalVariables.Instance.getUserId())
            {
                transform.position = new Vector3(2, 0, 2);
            }
            else
            {
                transform.position = new Vector3(36, 0, 56);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else
            transform.position = new Vector3(2, 0, 2);

    }
    #endregion


    #region Logic

    private void handleMovement() {
        bool isMoved = false;
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z = +1f;
            isMoved = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z = -1f;
            isMoved = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
            isMoved = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = +1f;
            isMoved = true;
        }

        if (isMoved) { 
            Vector3 moveVec = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
            transform.position += moveVec * cameraMoveSpeed * Time.deltaTime;
        }
    }

    private void handleRotation() {
        bool isRotated = false;
        Vector3 rotationVec = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVec.y = +1f;
            isRotated = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationVec.y = -1f;
            isRotated = true;
        }

        if(isRotated)
            transform.eulerAngles += rotationVec * cameraRotationSpeed * Time.deltaTime;
    }

    private void handleZoom() {
        bool isZoomed = false;

        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= cameraZoomAmount;
            isZoomed = true;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += cameraZoomAmount;
            isZoomed = true;
        }

        if (isZoomed) { 
            followOffset.y = Mathf.Clamp(followOffset.y, MinFollowYOffset, MaxFollowYOffset);
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, followOffset, Time.deltaTime * cameraZoomSpeed);
        }
    }
    #endregion


}

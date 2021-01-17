using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minSize = 10.0f;
    [SerializeField] private float maxSize = 150.0f;
    [SerializeField] private float sensitivity = 50.0f;

    [SerializeField] private float dragSpeed = 2.0f;
    private Vector3 dragOrigin;

    private Quaternion originalRotation;

    private bool isIso = false;
    private bool isAngleSet = true;
    private float[] rotationAngleSet = { -45.0f, 0.0f, 45.0f};
    private int setCounter = 0;

    private void Awake()
    {
        originalRotation = Camera.main.transform.rotation;
    }

    private void Update()
    {
        CameraZoom();

        if (Input.GetKeyDown(KeyCode.Space))
            SetIsoCam();

        if (Input.GetKeyDown(KeyCode.Q))
            SetAngle();

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(position.x * dragSpeed, 0.0f, position.y * dragSpeed);

        transform.Translate(move, Space.World);
    }

    private void ResetCameraRotation()
    {
        Camera.main.transform.rotation = originalRotation;
        setCounter = 0;
        isIso = false;
    }

    private void CameraZoom()
    {
        float cameraSize = Camera.main.orthographicSize;
        cameraSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        cameraSize = Mathf.Clamp(cameraSize, minSize, maxSize);
        Camera.main.orthographicSize = cameraSize;
    }

    private void SetIsoCam()
    {
        Debug.Log("Set iso cam");
        if (!isIso)
        {
            Camera.main.transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
            isIso = true;
        }
        else
            ResetCameraRotation();
    }

    private void SetAngle()
    {
        if (isIso)
        {
            Vector3 rotationAngle = new Vector3(45.0f, rotationAngleSet[setCounter], 0.0f);
            Camera.main.transform.rotation = Quaternion.Euler(rotationAngle);
            setCounter++;

            if (setCounter >= rotationAngleSet.Length)
                setCounter = 0;
        }
    }

}

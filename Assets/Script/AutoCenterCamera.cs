using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AutoCenterCamera : MonoBehaviour
{
    public GridManager gridManager;
    public float cameraHeight = 10f;
    public float cameraDistance = 15f;

    void Start()
    {
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        if (gridManager != null)
        {
            PositionCameraToGridCenter();
        }
    }

    private void PositionCameraToGridCenter()
    {
        // Tính toán tâm của grid
        float centerX = (gridManager.width - 1) / 2f;
        float centerZ = (gridManager.height - 1) / 2f;
        Vector3 gridCenter = new Vector3(centerX, 0, centerZ);

        // Position camera thẳng trên tâm grid
        transform.position = new Vector3(centerX, cameraHeight, centerZ);

        // FIX: Rotation (90, 0, 0) để nhìn thẳng từ trên xuống (top-down)
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);  // X=90 độ: nhìn xuống thẳng

        // Bỏ LookAt vì rotation trực tiếp hiệu quả hơn (tránh lệch nhỏ)
        // Nếu cần look at cụ thể: transform.LookAt(gridCenter); nhưng rotation 90 tốt hơn

        // FIX: Set Camera thành Orthographic để view phẳng từ trên (trong code hoặc Inspector)
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;  // Orthographic mode (2D-like từ trên)
            cam.orthographicSize = cameraHeight / 2f;  // Size để fit grid (điều chỉnh nếu cần)
        }
        else
        {
            Debug.LogWarning("Camera component không tồn tại trên GO này!");
        }

        Debug.Log($"✅ Camera chiếu thẳng từ trên tâm grid ({centerX:F1}, {cameraHeight}, {centerZ:F1}). Orthographic size: {cam.orthographicSize}.");
    }

    void Update()
    {

    }
}
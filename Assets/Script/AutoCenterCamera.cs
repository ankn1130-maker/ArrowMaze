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
    // Start is called before the first frame update
    void Start()
    {
        if(gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        if(gridManager != null)
        {
            PositionCameraToGridCenter();
        }
    }

    private void PositionCameraToGridCenter()
    {
        // tính toán tâm của grid
        float CenterX = (gridManager.width - 1) / 2f;
        float CenterZ = (gridManager.height - 1) / 2f;

        Vector3 GridCenter = new Vector3(CenterX , 0 , CenterZ);

        transform.position = new Vector3(CenterX, cameraHeight, CenterZ );
        
        transform.LookAt(GridCenter + Vector3.up * cameraHeight * 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

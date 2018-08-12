using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class GameInput : MonoBehaviour
{
    private TerrainManager _terrainManager;

    private void Start()
    {
        _terrainManager = TerrainManager.Instance;
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var dir = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(dir, out hit))
            {
                var voxelPosition = _terrainManager.GetVoxelCoordinatesByHitPoint(hit.point);
                if (_terrainManager.RemoveVoxel(voxelPosition))
                {
                    Debug.Log("Voxel deleted");
                }

                var updatedChunksCount = _terrainManager.UpdateChunks();
                if (updatedChunksCount > 0)
                {
                    Debug.Log(updatedChunksCount + " chunks updated");
                }
            }
        }
    }
}

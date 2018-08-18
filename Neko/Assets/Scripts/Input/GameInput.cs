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
            var addMode = Input.GetKey(KeyCode.LeftControl);

            RaycastHit hit;
            if (Physics.Raycast(dir, out hit))
            {
                var voxelPosition = addMode ? _terrainManager.GetOutsideVoxelCoordinatesByHitPoint(hit.point) : _terrainManager.GetInsideVoxelCoordinatesByHitPoint(hit.point);
                var result = addMode ? _terrainManager.AddVoxel(voxelPosition) : _terrainManager.RemoveVoxel(voxelPosition);

                if (result)
                {
                    Debug.Log(addMode ? "Voxel added" : "Voxel removed");
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

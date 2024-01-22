using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDebugger : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap tilemap;
    public GameObject highlightPrefab; 

    private GameObject currentHighlight;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject == tilemap.gameObject)
        {
            Vector3 hitPosition = Vector3Int.FloorToInt(hit.point);
            HighlightTile(hitPosition);

            if (Input.GetMouseButtonDown(0)) 
            {
                Debug.Log("Tile position: " + hitPosition);
                PlayerMovement.Instance.PosX = hitPosition.x;
                PlayerMovement.Instance.PosY = hitPosition.y;
            }
        }
        else
        {
            if (currentHighlight != null)
            {
                Destroy(currentHighlight);
            }
        }
    }

    void HighlightTile(Vector3 position)
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }

        currentHighlight = Instantiate(highlightPrefab, position, Quaternion.identity);
    }
}


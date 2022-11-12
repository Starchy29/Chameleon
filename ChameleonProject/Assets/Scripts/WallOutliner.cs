using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// when starting a scene, this outlines all walls in the level in black
public class WallOutliner : MonoBehaviour
{
    public GameObject OutlinePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap map = gameObject.GetComponent<Tilemap>();
        Vector3Int dims = map.size;
        for(int x = -dims.x / 2; x < dims.x / 2; x++) {
            for(int y = -dims.y / 2; y < dims.y / 2; y++) {
                Tile tile = map.GetTile<Tile>(new Vector3Int(x, y, 0));
                if(tile != null) {
                    Vector3 position = map.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0); // shift to middle of tile
                    if(map.GetTile<Tile>(new Vector3Int(x + 1, y, 0)) == null) { // check right
                        AddOutline(position, new Vector3(1, 0, 0));
                    }
                    if(map.GetTile<Tile>(new Vector3Int(x - 1, y, 0)) == null) { // check left
                        AddOutline(position, new Vector3(-1, 0, 0));
                    }
                    if(map.GetTile<Tile>(new Vector3Int(x, y + 1, 0)) == null) { // check up
                        AddOutline(position, new Vector3(0, 1, 0));
                    }
                    if(map.GetTile<Tile>(new Vector3Int(x, y - 1, 0)) == null) { // check down
                        AddOutline(position, new Vector3(0, -1, 0));
                    }
                }
            }
        }
    }

    // origin: this middle of the tile this is being placed on, direction: unit vector for which side to put outline on
    private void AddOutline(Vector3 origin, Vector3 direction) {
        float DIST_FROM_MID = 0.48f; // found through trial and error
        GameObject outline = Instantiate(OutlinePrefab);
        outline.transform.position = origin + direction * DIST_FROM_MID;

        // rotate if on the left or right
        if(direction.x != 0) {
            outline.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}

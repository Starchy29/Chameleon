using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// when starting a scene, this outlines all walls in the level in black
// also, this now adds foliage to the border too
public class WallOutliner : MonoBehaviour
{
    public GameObject OutlinePrefab;
    public GameObject FoliagePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // add outlines to wall
        Tilemap map = gameObject.GetComponent<Tilemap>();
        Vector3Int dims = map.size;
        const int BUFFER = 10; // extra tiles to check just in case
        for(int x = -dims.x / 2 - BUFFER; x < dims.x / 2 + BUFFER; x++) {
            for(int y = -dims.y / 2 - BUFFER; y < dims.y / 2 + BUFFER; y++) {
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

        // add foliage now
        const float OFFSET = 0.6f;
        float width = FoliagePrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        Vector3 endPoint = map.localBounds.max;
        Vector3 startPoint = map.localBounds.min;
        for(float y = startPoint.y; y <= endPoint.y; y += width) { // vertical sides
            AddFoliage(new Vector3(startPoint.x + OFFSET, y, 0), 0); // left
            AddFoliage(new Vector3(endPoint.x - OFFSET, y, 0), 180); // right
        }
        for(float x = startPoint.x; x <= endPoint.x; x += width) { // horizontal sides
            AddFoliage(new Vector3(x, startPoint.y + OFFSET, 0), 90); // bottom
            AddFoliage(new Vector3(x, endPoint.y - OFFSET, 0), 270); // top
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

    private void AddFoliage(Vector3 position, float rotation) {
        GameObject foliage = Instantiate(FoliagePrefab);
        foliage.transform.position = position;
        foliage.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}

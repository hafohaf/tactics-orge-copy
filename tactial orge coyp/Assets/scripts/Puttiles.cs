using UnityEngine;

public class PutTiles : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;

    // Entfernt: Start()

    [ContextMenu("Generate Grid In Editor")]
    void GenerateGrid()
    {
        // Alte Tiles löschen (nur wenn unter diesem Objekt als Kind)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Grid erzeugen
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Instantiate(tilePrefab, position, Quaternion.identity, transform);
            }
        }

        // Zusätzliche Tiles
        Instantiate(tilePrefab, new Vector3(2, 1, 1), Quaternion.identity, transform);
        Instantiate(tilePrefab, new Vector3(3, 2, 1), Quaternion.identity, transform);
    }
}

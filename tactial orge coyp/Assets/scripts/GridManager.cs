using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public GameObject tilePrefab;

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    void Start()
    {
        //GenerateGrid();
    }

   
    

 
}


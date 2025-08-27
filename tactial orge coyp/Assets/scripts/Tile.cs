using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool current = false;
    public bool selctable = false;
    public bool walkable = false;
    public bool target = false;


    public bool visited = false;
    public Tile Parent = null;
    public int distance = 0;
    public List<Tile> adjacencyList = new List<Tile>();


    


    public void Reset()
    {
        adjacencyList.Clear();
        selctable = false;
        //walkable = true;


        visited = false;
        Parent = null;
        distance = 0;
        Debug.Log("reset done");
    }


    public void CheckTile(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtens = new Vector3(0.25f,jumpHeight, 0.25f);
        Vector3 offset = new Vector3(0f,-0.5f,0f );
        Collider[] colliders = Physics.OverlapBox(transform.position + direction+offset, halfExtens);
        
        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjacencyList.Add(tile);
                    int i = 0;
                    while ( i < colliders.Length)
                    {
                        // Output all of the collider names
                        Debug.Log("Hit : " + colliders[i].name + i);
                        // Increase the number of Colliders in the array
                        i++;
                    }
                   // Debug.Log(adjacencyList);
                }
            }
        }
    }
   



    public void FindNeighbors(float jumpHeight)
    {
        Reset();
        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
        

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selctable&&!current)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}

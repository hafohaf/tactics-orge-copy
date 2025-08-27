using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    
    // ==== Bewegungseinstellungen ====
    public float moveSpeed = 2f;
    public int moveRange = 4;
    public float jumpHeight = 2f;
    public float jumpVelocity = 4.5f;
    public bool isMoving = false;

    // ==== Private Felder für Bewegung ====
    private Vector3 velocity = Vector3.zero;
    private Vector3 heading = Vector3.zero;
    private float halfHeight = 0f;
    private Vector3 jumpTarget;

    private bool isFallingDown = false;
    private bool isJumpingUp = false;
    private bool isMovingToEdge = false;

    // ==== Tile-System ====
    private GameObject[] tiles;
    private List<Tile> selectableTiles = new List<Tile>();
    private Stack<Tile> path = new Stack<Tile>();
    private Tile currentTile;

    // ====================== Unity ======================
    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y;
        
    }

    // ====================== Tile-Funktionen ======================
    /// <summary> Gibt das Tile unter dem gegebenen GameObject zurück. </summary>
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    /// <summary> Setzt das aktuelle Tile, auf dem sich die Einheit befindet. </summary>
    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);

        currentTile.current = true;
    }

    /// <summary> Erstellt für jedes Tile eine Adjazenzliste basierend auf Sprunghöhe. </summary>
    public void ComputeAdjacencyLists()
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight);
            
        }
    }

    /// <summary> Findet alle Tiles, die innerhalb der Bewegungsreichweite liegen. </summary>
    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();
        currentTile.visited = true;
        process.Enqueue(currentTile);
        

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();
            selectableTiles.Add(t);
            t.selctable = true;

            if (t.distance < moveRange)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.Parent = t;
                        tile.visited = true;
                        tile.distance = t.distance+1;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    /// <summary> Entfernt alle markierten Tiles nach dem Zug. </summary>
    protected void removeSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

     // ====================== Bewegung ======================
     /// <summary> Erstellt einen Pfad zur gewählten Ziel-Position. </summary>
     
    public void MoveToSelectedTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        isMoving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.Parent;
        }
    }

    /// <summary> Führt die Bewegung entlang des Pfades aus. </summary>

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.5f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                
                    Jump(target);
                
                else
                {
                    CalculateHeading(target);
                    setHorizotalVelocity();
                }
                
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            removeSelectableTiles();
            isMoving = false;
        }
    }

    /// <summary> Berechnet die Blickrichtung zum Ziel. </summary>
    
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();

    }

    /// <summary> Setzt horizontale Bewegungsgeschwindigkeit. </summary>
    
    void setHorizotalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    // ====================== Springen ======================


    void Jump(Vector3 target)
    {
        if (isFallingDown)
        {
            FallDownward(target);
        }
        else if (isJumpingUp)
        {
            Jumpupward(target);
        }
        else if (isMovingToEdge)
        {
            moveToEdge();
        }
        else
        {
            PreparJump(target);
        }
    }
    void PreparJump(Vector3 target)
    {
        float targetY = target.y;
        target.y = transform.position.y;
        CalculateHeading(target);
        if (transform.position.y > targetY)
        {
            isFallingDown = false;
            isJumpingUp = false;
            isMovingToEdge = true;
            jumpTarget = transform.position + (target -transform.position) / 2.0f;

        }
        else
        {
            isFallingDown = false;
            isJumpingUp = true;
            isMovingToEdge = false;

            velocity = heading * moveSpeed / 3.0f;
            float difference = targetY - transform.position.y;
            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }
        void FallDownward(Vector3 target)
        {
            velocity += Physics.gravity * Time.deltaTime;
            if (transform.position.y <= target.y)
            {
                Vector3 p = transform.position;
                p.y = target.y;
                transform.position = p;

                velocity = new Vector3();
            }
        }
        void Jumpupward(Vector3 target)
        {
            velocity += Physics.gravity * Time.deltaTime;
            if (transform.position.y > target.y)
            {
                isJumpingUp= false;
                isFallingDown = true;
            }
        }
        void moveToEdge()
        {
            if (Vector3.Distance(transform.position, jumpTarget) >= 0.5f)
            {
                setHorizotalVelocity();

            }
            else
            {
                isMovingToEdge = false;
                isFallingDown = true;
                velocity /= 3.0f;
                velocity.y = 1.5f;
            }
        }
    }


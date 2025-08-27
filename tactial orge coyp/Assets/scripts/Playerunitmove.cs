using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitMove : TacticsMove
{
    public GameObject cursor;

    
    public Vector3 stratCursorPosition;
    private Vector3 cursorTargetPosition;

    void Start()
    {
        
        // Spielerposition auf dem Grid abrufen (X/Z), Y ignorieren
        cursor=GameObject.FindGameObjectWithTag("Cursor");
        //Debug.Log(transform.position);
         
        SetcursorStratposion();
        // Höhe vom Boden bestimmen
       

        Init(); // TacticsMove Init
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!isMoving)
        {
            FindSelectableTiles();
            HandleCursorInput();
        }
        else
        {
            Move();
            SetcursorStratposion();
        }
    }

    // =================== Eingabe und Bewegung ===================

    private void HandleCursorInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

        if (direction != Vector3.zero)
        {
            cursor.transform.position += direction;
            Vector3 newCursorHeightoffset=GetTilePosition(cursor.transform.position);
            cursor.transform.position= newCursorHeightoffset;
           // cursorGridPosition = new Vector3( Mathf.Round(cursorGridPosition.x),0f,Mathf.Round(cursorGridPosition.z));

        }

        // ESC – Cursor zurück zur Spielerposition
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursor.transform.position = stratCursorPosition;

           
        }

        // J – Spieler zur Cursor-Position bewegen, wenn Tile erreichbar
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Physics.Raycast(cursor.transform.position, Vector3.down, out RaycastHit hit, 1.6f))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    Tile tile = hit.collider.GetComponent<Tile>();
                    if (tile.selctable)
                    {
                        MoveToSelectedTile(tile);
                    }
                }
            }
            
        }
    }

    /// <summary>
    /// Gibt die tatsächliche Position des Tiles (inkl. Höhe) für eine X/Z-Position zurück.
    /// </summary>
    private Vector3 GetTilePosition(Vector3 gridPos)
{
    RaycastHit hit;
    Vector3 rayOrigin = new Vector3(gridPos.x, 10f, gridPos.z);

    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 20f))
    {
        if (hit.collider.CompareTag("Tile"))
        {
            float y = hit.point.y+0.01f;
            //Debug.Log(y);
            return new Vector3(gridPos.x, y, gridPos.z);
        }
    }

    Debug.LogWarning("Kein Tile unter Cursor bei: " + gridPos);
    return new Vector3(gridPos.x, 0.5f, gridPos.z);
}

private void SetcursorStratposion()
{
    Vector3 offset = new Vector3(0f, 0.9f, 0f);
    stratCursorPosition=transform.position-offset;
    //Debug.Log(stratCursorPosition);
        
    cursor.transform.position=stratCursorPosition;
}
}

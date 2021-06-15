using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris3Dgame : MonoBehaviour
{
    float camYaw = -30;
    float camPitch = -30;
    float cameraDist = 30;
    Vector3 mousePrevPos;

    int gameSpeed = 100;
    int gameCountdown = 0;

    int points = 0;
    public UnityEngine.UI.Text pointsText;
    float boundsThickness = 0.2f;

    const int gridXZdims = 6;
    const int gridYdim = 10;
    List<List<List<int>>> grid = new List<List<List<int>>>();
    List<List<List<GameObject>>> gridObjs = new List<List<List<GameObject>>>();

    List<Vector3Int> curPiece = new List<Vector3Int>();
    Vector3 centroid = Vector3.zero;

    /*
    
    1.
     0
    000

    2.
    00
    0
    0
    
    3.
    0000
    
    4.
    00
    00
    
    5.
     00
    00
    
     */

    // Start is called before the first frame update
    void Start()
    {
        gameCountdown = gameSpeed;

        // allocate grid
        for (int i = 0; i < gridXZdims; ++i)
        {
            grid.Add(new List<List<int>>());
            for (int j = 0; j < gridYdim; ++j)
            {
                grid[i].Add(new List<int>());
                for (int k = 0; k < gridXZdims; ++k)
                {
                    grid[i][j].Add(0);
                }
            }
        }

        // allocate gridObjs
        for (int i = 0; i < gridXZdims; ++i)
        {
            gridObjs.Add(new List<List<GameObject>>());
            for (int j = 0; j < gridYdim; ++j)
            {
                gridObjs[i].Add(new List<GameObject>());
                for (int k = 0; k < gridXZdims; ++k)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(i, j, k);
                    gridObjs[i][j].Add(go);
                }
            }
        }

        {
            // x-axis
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims * 0.5f, 0, 0), new Vector3(0, 0, 90), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims * 0.5f, 0, gridXZdims), new Vector3(0, 0, 90), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims * 0.5f, gridYdim, gridXZdims), new Vector3(0, 0, 90), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims * 0.5f, gridYdim, 0), new Vector3(0, 0, 90), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);

            // z-axis
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(0, 0, gridXZdims * 0.5f), new Vector3(90, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims, 0, gridXZdims * 0.5f), new Vector3(90, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(gridXZdims, gridYdim, gridXZdims * 0.5f), new Vector3(90, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridXZdims, boundsThickness) * 0.5f, new Vector3(0, gridYdim, gridXZdims * 0.5f), new Vector3(90, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);

            // y-axis
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridYdim, boundsThickness) * 0.5f, new Vector3(0, gridYdim * 0.5f, 0), new Vector3(0, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridYdim, boundsThickness) * 0.5f, new Vector3(gridXZdims, gridYdim * 0.5f, 0), new Vector3(0, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridYdim, boundsThickness) * 0.5f, new Vector3(0, gridYdim * 0.5f, gridXZdims), new Vector3(0, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
            MakePrim(PrimitiveType.Cylinder, new Vector3(boundsThickness, gridYdim, boundsThickness) * 0.5f, new Vector3(gridXZdims, gridYdim * 0.5f, gridXZdims), new Vector3(0, 0, 0), new Vector3(-0.5f, -0.5f, -0.5f), Color.white);
        }

        CreateNewPiece();
    }

    GameObject MakePrim(PrimitiveType p, Vector3 scale, Vector3 pos, Vector3 rot, Vector3 offset, Color c)
    {
        var prim = GameObject.CreatePrimitive(p);
        prim.transform.localScale = scale;
        prim.transform.position = pos;
        prim.transform.position += offset;
        prim.transform.Rotate(rot.x, rot.y, rot.z);
        prim.GetComponent<Renderer>().material.color = c;
        return prim;
    }

    void AddPiecePart(int x, int y, int z, int pieceID)
    {
        grid[x][y][z] = pieceID;
        curPiece.Add(new Vector3Int(x,y,z));
    }

    private void CreateNewPiece()
    {
        curPiece.Clear();

        int pieceID = UnityEngine.Random.Range(1, 2);

        int a = gridXZdims / 2;
        int b = gridYdim - 1;
        int c = gridXZdims / 2;

        switch(pieceID)
        {
            case 1:
                // 2x2x2 cube
                AddPiecePart(a, b, c, pieceID);
                AddPiecePart(a - 1, b, c, pieceID);
                AddPiecePart(a, b - 1, c, pieceID);
                AddPiecePart(a, b, c - 1, pieceID);
                AddPiecePart(a - 1, b, c - 1, pieceID);
                AddPiecePart(a - 1, b - 1, c, pieceID);
                AddPiecePart(a, b - 1, c - 1, pieceID);
                AddPiecePart(a - 1, b - 1, c - 1, pieceID);
                break;

            case 2:
                // 3x3 plane
                AddPiecePart(a, b, c, pieceID);
                AddPiecePart(a - 1, b, c, pieceID);
                AddPiecePart(a, b, c - 1, pieceID);
                AddPiecePart(a - 1, b, c - 1, pieceID);
                AddPiecePart(a - 2, b, c, pieceID);
                AddPiecePart(a, b, c - 2, pieceID);
                AddPiecePart(a - 2, b, c - 2, pieceID);
                AddPiecePart(a - 2, b, c - 1, pieceID);
                AddPiecePart(a - 1, b, c - 2, pieceID);
                // one sticking down
                AddPiecePart(a - 1, b - 1, c - 1, pieceID);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameCountdown <= 0)
        {
            // move the current piece down
            MovePiece(0, -1, 0);

            gameCountdown = gameSpeed;
        }
        else
        {
            gameCountdown--;
        }

        // calculate the centroid of curPiece
        centroid = Vector3.zero;
        for (int i = 0; i < curPiece.Count; ++i)
        {
            centroid += new Vector3(curPiece[i].x, curPiece[i].y, curPiece[i].z);
        }
        centroid = new Vector3(centroid.x / (float)curPiece.Count, centroid.y / (float)curPiece.Count, centroid.z / (float)curPiece.Count);

        // check user input
        if (Input.GetKeyDown(KeyCode.A))
        {
            MovePiece(-1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MovePiece(1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MovePiece(0, 0, 1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MovePiece(0, 0, -1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotatePiece(0, 0, -1);
        }

        // sync grid and gridObjs
        for (int i = 0; i < gridXZdims; ++i)
        {
            for (int j = 0; j < gridYdim; ++j)
            {
                for (int k = 0; k < gridXZdims; ++k)
                {
                    gridObjs[i][j][k].SetActive(true);
                    Color newcol = Color.blue;
                    switch (grid[i][j][k])
                    {
                        case 0:
                            gridObjs[i][j][k].SetActive(false);
                            break;
                        case 1:
                            newcol = Color.blue;
                            break;
                        case 2:
                            newcol = Color.yellow;
                            break;
                        case 3:
                            newcol = Color.green;
                            break;
                        case 4:
                            newcol = Color.red;
                            break;
                        case 5:
                            newcol = Color.white;
                            break;
                    }
                    gridObjs[i][j][k].GetComponent<Renderer>().material.color = newcol;
                }
            }
        }

        UpdateCameraAngle();
    }

    private void UpdateCameraAngle()
    {
        if (curPiece.Count == 0)
            return;

        float rotateSpeed = 0.3f;
        float scrollSpeed = 0.3f;

        var mousePos = Input.mousePosition;

        cameraDist -= Input.mouseScrollDelta.y * scrollSpeed;

        if(Input.GetKey(KeyCode.Mouse0))
        {
            var mouseMoveDelta = mousePos - mousePrevPos;
            camPitch += mouseMoveDelta.y * rotateSpeed;
            if (camPitch > 89)
            {
                camPitch = 89;
            }
            if (camPitch < -89)
            {
                camPitch = -89;
            }
            camYaw += mouseMoveDelta.x * rotateSpeed;
        }
        Vector3 unitF = -Vector3.forward;
        Vector3 unitR = Vector3.right;
        unitF = Quaternion.AngleAxis(camYaw, Vector3.up) * unitF;
        unitR = Quaternion.AngleAxis(camYaw, Vector3.up) * unitR;

        unitF = Quaternion.AngleAxis(-camPitch, unitR) * unitF * cameraDist;

        Camera.main.transform.position = new Vector3(
            curPiece[0].x + unitF.x,
            curPiece[0].y + unitF.y,
            curPiece[0].z + unitF.z);

        Camera.main.transform.LookAt(centroid);

        mousePrevPos = mousePos;
    }

    private void RotatePiece(int dx, int dy, int dz)
    {
        var savedID = grid[curPiece[0].x][curPiece[0].y][curPiece[0].z];

        // remove old from grid (still have curPiece pointers)
        for (int i = 0; i < curPiece.Count; ++i)
        {
            grid[curPiece[i].x][curPiece[i].y][curPiece[i].z] = 0;
        }

        // check if we can move
        bool canMove = true;
        for (int i = 0; i < curPiece.Count; ++i)
        {
            // if we reach the bottom or there is a piece below
            if (curPiece[i].y + dy < 0 || curPiece[i].y + dy >= gridYdim ||
                curPiece[i].x + dx < 0 || curPiece[i].x + dx >= gridXZdims ||
                curPiece[i].z + dz < 0 || curPiece[i].z + dz >= gridXZdims ||
                grid[curPiece[i].x + dx][curPiece[i].y + dy][curPiece[i].z + dz] != 0)
            {
                canMove = false;
                break;
            }
        }

        // add it back
        int savedCount = curPiece.Count;
        for (int i = 0; i < savedCount; ++i)
        {
            AddPiecePart(
                curPiece[i].x + ((canMove) ? dx : 0),
                curPiece[i].y + ((canMove) ? dy : 0),
                curPiece[i].z + ((canMove) ? dz : 0), savedID);
        }
        curPiece.RemoveRange(0, savedCount);
    }

    private void MovePiece(int dx, int dy, int dz)
    {
        var savedID = grid[curPiece[0].x][curPiece[0].y][curPiece[0].z];

        // remove old from grid (still have curPiece pointers)
        for (int i = 0; i < curPiece.Count; ++i)
        {
            grid[curPiece[i].x][curPiece[i].y][curPiece[i].z] = 0;
        }

        // check if we can move
        bool canMove = true;
        for (int i = 0; i < curPiece.Count; ++i)
        {
            // if we reach the bottom or there is a piece below
            if (curPiece[i].y + dy < 0 || curPiece[i].y + dy >= gridYdim ||
                curPiece[i].x + dx < 0 || curPiece[i].x + dx >= gridXZdims ||
                curPiece[i].z + dz < 0 || curPiece[i].z + dz >= gridXZdims ||
                grid[curPiece[i].x + dx][curPiece[i].y + dy][curPiece[i].z + dz] != 0)
            {
                canMove = false;
                break;
            }
        }

        // add it back
        int savedCount = curPiece.Count;
        for (int i = 0; i < savedCount; ++i)
        {
            AddPiecePart(
                curPiece[i].x + ((canMove) ? dx : 0),
                curPiece[i].y + ((canMove) ? dy : 0),
                curPiece[i].z + ((canMove) ? dz : 0), savedID);
        }
        curPiece.RemoveRange(0, savedCount);

        if(!canMove && dy < 0)
        {
            CreateNewPiece();
            CheckForCompleteRows();
        }
    }

    private void CheckForCompleteRows()
    {
        List<int> rowsToRemove = new List<int>();
        int removeCount = 0;
        for (int i = 0; i < gridYdim; ++i)
        {
            bool rowComplete = true;
            for (int j = 0; j < gridXZdims; ++j)
            {
                for (int k = 0; k < gridXZdims; ++k)
                {
                    if(grid[j][i][k] == 0)
                    {
                        rowComplete = false;
                        j = gridXZdims;
                        k = gridXZdims;
                        break;
                    }
                }
            }
            if(rowComplete)
            {
                removeCount++;
            }
            if(i + removeCount < gridYdim)
            {
                rowsToRemove.Add(i + removeCount);
            }
        }

        // update grid to remove completed rows
        for (int i = 0; i < gridYdim; ++i)
        {
            // copy correct row to this row
            for (int j = 0; j < gridXZdims; ++j)
            {
                for (int k = 0; k < gridXZdims; ++k)
                {
                    if(i < rowsToRemove.Count)
                    {
                        grid[j][i][k] = grid[j][rowsToRemove[i]][k];
                    }
                }
            }
        }
        points += removeCount;
        pointsText.text = points.ToString();
    }

}

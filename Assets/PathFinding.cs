using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Hexagon startPoint;
    public Hexagon endPoint;
    public FollowPath character;

    List<Hexagon> openList;
    List<Hexagon> closedList;
    public List<Hexagon> path;
    
    Material PathMat;
    Material PathEndMat;
    Material NormMat;
    Material Ground;
    Material Water;
    Material Mountain;
    // Start is called before the first frame update

    public void Start()
    {
        openList = new List<Hexagon>();
        closedList = new List<Hexagon>();
        path = new List<Hexagon>();
        PathEndMat = Resources.Load<Material>("EndPoint");
        PathMat = Resources.Load<Material>("TileOuter");
        NormMat = Resources.Load<Material>("TileInnerDebug");
        Ground = Resources.Load<Material>("Ground");
        Water = Resources.Load<Material>("Water");
        Mountain = Resources.Load<Material>("Mountain");
    }

    public void ClearPathEnds()
    {
        startPoint = null;
        endPoint = null;
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (hit)
            {
                SetStart(hit.collider.gameObject);
                if (endPoint != null && startPoint != endPoint)
                {
                    FindPath();
                    character.SetPath(path);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (hit)
            {
                SetEnd(hit.collider.gameObject);
                if (startPoint != null && startPoint != endPoint)
                {
                    FindPath();
                    character.SetPath(path);
                }
            }
        }
    }

    public void TracePath(Hexagon hex, bool reversed)
    {
        path.Clear();
        Debug.Log("Pathing:");
        path.Add(hex);
        while(hex.parent != null)
        {
            path.Add(hex.parent);
            hex = hex.parent;
            Debug.Log(hex.totalMoveDifficulty);
        }
        foreach (Hexagon hexagon in path)
        {
            if (hexagon != startPoint && hexagon != endPoint)
                SetMaterial(hexagon, PathMat);
            hexagon.totalMoveDifficulty = float.MaxValue;
            hexagon.aproxMoveDistance = float.MaxValue;
            hexagon.parent = null;
        }
        foreach (Hexagon hexagon in closedList)
        {
            hexagon.totalMoveDifficulty = float.MaxValue;
            hexagon.aproxMoveDistance = float.MaxValue;
            hexagon.parent = null;
        }
        foreach (Hexagon hexagon in openList)
        {
            hexagon.totalMoveDifficulty = float.MaxValue;
            hexagon.aproxMoveDistance = float.MaxValue;
            hexagon.parent = null;

        }
        if (!reversed)
            path.Reverse();
    }

    public void FindPath()
    {
        bool Reversed = false;
        if(endPoint.position.x < startPoint.position.x)
        {
            Reversed = true;
            Hexagon tempHex = startPoint;
            startPoint = endPoint;
            endPoint = tempHex;
        }
        if (closedList.Count != 0)
        {
            foreach (Hexagon hexagon in path)
            {
                if (hexagon != path[path.Count-1])
                {
                    switch (hexagon.tileType)
                    {
                        case "Ground":
                            SetMaterial(hexagon, Ground);
                            break;
                        case "Water":
                            SetMaterial(hexagon, Water);
                            break;
                        case "Mountain":
                            SetMaterial(hexagon, Mountain);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        closedList.Clear();
        openList.Clear();
        startPoint.totalMoveDifficulty = 0;
        startPoint.aproxMoveDistance = (endPoint.position - startPoint.position).magnitude;
        openList.Add(startPoint);

        bool foundDest = false;
        while (openList.Count != 0 && !foundDest)
        {
            float Difficulty = float.MaxValue;
            openList.Sort((x, y) => x.totalMoveDifficulty.CompareTo(y.totalMoveDifficulty));

            Debug.Log(openList[0].connections.Length - 1);
            for (int i = 0; i < openList[0].connections.Length; i++)
            {
                if (openList[0].connections[i] != null && openList[0].connections[i].tileType != "Water" && openList[0].connections[i].tileType != "Mountain")
                {
                    if (openList[0].connections[i] == endPoint)
                    {
                        openList[0].connections[i].parent = openList[0];
                        closedList.Add(openList[0].connections[i]);
                        TracePath(openList[0].connections[i], Reversed);
                        foundDest = true;
                        return;
                    }
                    else
                    {
                        if (!closedList.Contains(openList[0].connections[i]))
                        {
                            Difficulty = openList[0].totalMoveDifficulty + openList[0].connections[i].moveDifficulty;
                            Debug.Log("Difficulty: " + Difficulty);

                            if (Difficulty < openList[0].connections[i].totalMoveDifficulty && openList[0].connections[i])
                            {
                                openList[0].connections[i].totalMoveDifficulty = Difficulty;
                                openList[0].connections[i].aproxMoveDistance = Difficulty + (endPoint.position - openList[0].connections[i].position).sqrMagnitude;
                                openList[0].connections[i].parent = openList[0];
                                openList.Add(openList[0].connections[i]);
                            }

                        }
                    }
                    //openList.Sort((x, y) => x.totalMoveDifficulty.CompareTo(y.totalMoveDifficulty));
                }
            }

            closedList.Add(openList[0]);
            openList.Remove(openList[0]);


        }
        if (!foundDest)
        {
            Debug.Log("Failed to find destination");
            return;
        }
        
    }

    public void SetStart(GameObject startHex)
    {
        if (startHex.transform.parent.GetComponent<Hexagon>() == endPoint)
            return;

        if (startPoint != null)
        {
            switch (startPoint.tileType)
            {
                case "Ground":
                    SetMaterial(startPoint, Ground);
                    break;
                case "Water":
                    SetMaterial(startPoint, Water);
                    break;
                case "Mountain":
                    SetMaterial(startPoint, Mountain);
                    break;
                default:
                    break;
            }
            
        }
        startPoint = startHex.transform.parent.GetComponent<Hexagon>();
        SetMaterial(startPoint, PathEndMat);
    }

    public void SetEnd(GameObject endHex)
    {
        if (endHex.transform.parent.GetComponent<Hexagon>() == startPoint)
            return;

        if (endPoint != null)
        {
            switch (endPoint.tileType)
            {
                case "Ground":
                    SetMaterial(endPoint, Ground);
                    break;
                case "Water":
                    SetMaterial(endPoint, Water);
                    break;
                case "Mountain":
                    SetMaterial(endPoint, Mountain);
                    break;
                default:
                    break;
            }
        }
        endPoint = endHex.transform.parent.GetComponent<Hexagon>();
        SetMaterial(endPoint, PathEndMat);
    }


    public void SetMaterial(Hexagon hex, Material Mat)
    {
        foreach (Transform t in hex.gameObject.transform)
        {
            if (t.tag == "TileInner")
                t.gameObject.GetComponentInChildren<SpriteRenderer>().material = Mat;
        }
    }
}

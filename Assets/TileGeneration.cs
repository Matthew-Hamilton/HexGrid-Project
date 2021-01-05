using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    public int size;
    public GameObject Tile;
    public PathFinding SetMaterial;


    public string Seed;

    public List<List<Hexagon>> HexConstruction;

    Material Mountain;
    Material Ground;
    Material Water;

    void Start()
    {

        Mountain = Resources.Load<Material>("Mountain");
        Ground = Resources.Load<Material>("Ground");
        Water = Resources.Load<Material>("Water");

        HexConstruction = new List<List<Hexagon>>();
    }


    void ConstructMap(int size)
    {
        HexConstruction.Clear();
        int variableSize = size;
        int hexHeight = 2 * size - 1;
        Vector2 startPos = new Vector2(0,0);
        bool backtrack = false;
        for(int i=0; i< hexHeight; i++)
        {
            List<Hexagon> level = new List<Hexagon>();
            level.Add(Instantiate(Tile, startPos, Quaternion.identity).GetComponent<Hexagon>());

            for (int j = 0; j < variableSize; j++)
            {
                if(j < variableSize -1)
                    level.Add(level[j].GenerateHex(0).GetComponent<Hexagon>());
                if (j != 0)
                    level[j].SetConnection(3, level[j - 1]);

            }
            if (variableSize < hexHeight && !backtrack)
            {
                variableSize++;
                startPos.y -= 1;
            }
            else
            {
                backtrack = true;
                variableSize--;
                startPos.y += 1;
            }
            startPos.x += 1.732051f;
            HexConstruction.Add(level);
            Debug.Log(level.Count);
        }
        Debug.Log("Help Me: " + HexConstruction[0].Count);
        ConnectMap(size, hexHeight);
    }

    void ConnectMap(int size, int hexHeight)
    {
        int variableSize = size;
        bool backtrack = false;

        //Loop through each collumn to connect tiles laterally
        for (int i = 0; i< hexHeight-1; i++)
        {

            //Loop through each tile in the collumn
            for(int j = 0; j< variableSize; j++)
            {
                //If not backtracking (eg. getting smaller) bottom right connection attached to the first(bottom) element of the next collumn and visa versa, while top right connection 
                if (!backtrack)
                {
                    HexConstruction[i][j].SetConnection(2, HexConstruction[i + 1][j]);
                    //if(j!= variableSize)
                        HexConstruction[i][j].SetConnection(1, HexConstruction[i + 1][j + 1]);

                }
                if(backtrack)
                {
                    
                    Debug.Log("Backtracking");
                    if (j == 0)
                    {
                        HexConstruction[i][j].SetConnection(1, HexConstruction[i + 1][j]);
                        continue;
                    }
                    if (j == variableSize-1)
                    {
                        HexConstruction[i][j].SetConnection(2, HexConstruction[i + 1][j - 1]);
                        variableSize--;
                        //HexConstruction[i][j].gameObject.active = false;
                        continue;
                    }

                    HexConstruction[i][j].SetConnection(1, HexConstruction[i + 1][j]);
                    HexConstruction[i][j].SetConnection(2, HexConstruction[i + 1][j - 1]);
                }

            }
            if(backtrack)
            {
                variableSize--;
            }
            if (variableSize < hexHeight-1 && !backtrack)
            {
                variableSize++;
            }
            else
            {
                variableSize++;
                backtrack = true;
            }
        }
    }

    public void Randomise()
    {

        Seed = Time.time.ToString();
        System.Random MyRandom = new System.Random(Seed.GetHashCode());
        foreach (List<Hexagon> hexList in HexConstruction)
        {
            foreach(Hexagon hex in hexList)
            {
                int numConnected = 0;
                foreach(Hexagon connectedHex in hex.connections)
                {
                    if (connectedHex != null)
                        numConnected++;
                }
                if(numConnected < 6)
                {
                    hex.tileType = "Mountain";
                }
                else
                {
                    hex.tileType = (MyRandom.Next(0, 100) < 50) ? "Water" : "Ground";
                }
            }
        }
    }

    public void Smooth()
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (List<Hexagon> hexList in HexConstruction)
            {
                foreach (Hexagon hex in hexList)
                {
                    int numGround = 0;
                    int numWater = 0;
                    if (hex.tileType == "Mountain")
                        continue;
                    foreach (Hexagon connectedHex in hex.connections)
                    {
                        if (connectedHex == null)
                            continue;
                        if (connectedHex.tileType == "Ground")
                            numGround++;
                        if (connectedHex.tileType == "Water")
                            numWater++;
                    }
                    if (numWater > numGround + 1)
                    {
                        hex.tileType = "Water";
                    }
                    if (numWater <= 2)
                    {
                        hex.tileType = "Ground";
                    }
                }
            }
        }
    }

    public void Render()
    {
        foreach (List<Hexagon> hexList in HexConstruction)
        {
            foreach (Hexagon hex in hexList)
            {
                switch(hex.tileType)
                {
                    case "Mountain":
                        SetMaterial.SetMaterial(hex, Mountain);
                        break;
                    case "Ground":
                        SetMaterial.SetMaterial(hex, Ground);
                        break;
                    case "Water":
                        SetMaterial.SetMaterial(hex, Water);
                        break;
                    default:
                        break;

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Hexagon hex = Tile.GetComponent<Hexagon>();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConstructMap(size);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetMaterial.ClearPathEnds();
            Randomise();
            Smooth();
            Render();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SetMaterial.ClearPathEnds();
            Smooth();
            Render();
        }
    }
}

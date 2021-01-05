using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class vector3Helper
{
    public static Vector2 xy(this Vector3 myVector3)
    {
        return new Vector2(myVector3.x, myVector3.y);
    }
}
public class Hexagon : MonoBehaviour
{ 
    public GameObject tileInner;
    public bool active = false;
    public GameObject Tile;
    public Hexagon[] connections;
    public Hexagon parent;
    public Vector2 position = Vector2.zero;
    public float totalMoveDifficulty = float.MaxValue;
    public float aproxMoveDistance = float.MaxValue;
    public int moveDifficulty = 1;
    public string tileType = "";

    void Start()
    {
        totalMoveDifficulty = float.MaxValue;
        aproxMoveDistance = float.MaxValue;
        moveDifficulty = 1;
    }

    public Hexagon(int _connection, Hexagon connectedHexagon, Vector2 _position, int _moveDifficulty)
    {
        connections = new Hexagon[6];
        connections[_connection] = connectedHexagon;
        position = _position;
        moveDifficulty = _moveDifficulty;
    }
    public Hexagon(int _connection, Hexagon connectedHexagon, Vector2 _position)
    {
        connections = new Hexagon[6];
        connections[_connection] = connectedHexagon;
        position = _position;
        moveDifficulty = 1;
        totalMoveDifficulty = float.MaxValue;
        aproxMoveDistance = float.MaxValue;
    }
    public Hexagon(Vector2 _position, int _moveDifficulty)
    {
        connections = new Hexagon[6];
        position = _position;
        moveDifficulty = _moveDifficulty;
        moveDifficulty = 1;
        totalMoveDifficulty = float.MaxValue;
        aproxMoveDistance = float.MaxValue;
    }
    public Hexagon(Vector2 _position)
    {
        connections = new Hexagon[6];
        position = _position;
        moveDifficulty = 1;
        totalMoveDifficulty = float.MaxValue;
        aproxMoveDistance = float.MaxValue;
}

    public void SetConnection(int Direction, Hexagon hex)
    {
        connections[Direction] = hex;
        switch(Direction)
        {
            case 0:
                hex.connections[3] = this;
                break;
            case 1:
                hex.connections[4] = this;
                break;
            case 2:
                hex.connections[5] = this;
                break;
            case 3:
                hex.connections[0] = this;
                break;
            case 4:
                hex.connections[1] = this;
                break;
            case 5:
                hex.connections[2] = this;
                break;
        }
    }

    public GameObject GenerateHex(int connectionDirection)
    {
        Hexagon otherScript = Tile.GetComponent<Hexagon>();
        switch(connectionDirection)
        {
            case 0:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(0, 2);
                return Instantiate(Tile, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            case 1:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(1.732051f, 1);
                return Instantiate(Tile, transform.position + new Vector3(1.732051f, 1, 0), Quaternion.identity);
            case 2:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(1.732051f, -1);
                return Instantiate(Tile, transform.position + new Vector3(1.732051f, -1, 0), Quaternion.identity);
            case 3:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(0, -2);
                return Instantiate(Tile, transform.position + new Vector3(0, -2, 0), Quaternion.identity);
            case 4:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(-1.732051f, -1);
                return Instantiate(Tile, transform.position + new Vector3(-1.732051f, -1, 0), Quaternion.identity);
            case 5:
                otherScript.connections = new Hexagon[6];
                otherScript.position = transform.position.xy() + new Vector2(-1.732051f, 1);
                return Instantiate(Tile, transform.position + new Vector3(-1.732051f, 1, 0), Quaternion.identity);
            default:
                return null;
        }
    }

}

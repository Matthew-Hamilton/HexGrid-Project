using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public PathFinding pathFind;
    public List<Hexagon> Path;
    int index = 0;
    [Range(1, 10)]
    public int speed = 1;
    public bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            Debug.Log(Path.Count);
            FollowThePath();
        }
    }
    public void SetPath(List<Hexagon> path)
    {
        index = 0;
        Path = path;
        done = false;
    }

    public void FollowThePath()
    {
        if (index != Path.Count)
        {
            if ((transform.position.xy() - new Vector2(Path[index].position.x, Path[index].position.y -2)).magnitude <= 0.1)
            {
                index++;
            }
            transform.position += (new Vector3(Path[index].position.x, Path[index].position.y -2, 0) - new Vector3(transform.position.x, transform.position.y, 0)).normalized * Time.deltaTime * speed;
        }
        else
        {
            done = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject BrickWall; // The wall prefab
    public int width = 10; // Width of the plane
    public int height = 10; // Height of the plane

    // Start is called before the first frame update
    void Start()
    {
        GenerateWalls();
    }

    void GenerateWalls()
    {
        // Instantiate walls on the perimeter
        for (int x = 0; x < width; x++)
        {
            InstantiateWall(new Vector3(x, 0, 0), Quaternion.Euler(0, 90, 0));
            InstantiateWall(new Vector3(x, 0, height - 1), Quaternion.Euler(0, 90, 0));
        }

        for (int y = 0; y < height; y++)
        {
            InstantiateWall(new Vector3(0, 0, y), Quaternion.Euler(0, 90, 0));
            InstantiateWall(new Vector3(width - 1, 0, y), Quaternion.Euler(0, 90, 0));
        }
    }

    void InstantiateWall(Vector3 position, Quaternion rotation)
    {
        GameObject newWall = Instantiate(BrickWall, position, rotation);
        newWall.transform.SetParent(transform); // Set the wall as a child of the object this script is attached to
    }
}


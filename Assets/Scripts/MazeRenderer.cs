using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public float size = 4f;

    // the following should be fixed
    public Transform wallPrefab = null;
    public Transform floorPrefab = null;

    // Start is called before the first frame update
    // void Start()
    // {
    //     var maze = MazeGenerator.Generate(width, height);
    //     Draw(maze);
    // }

    public GameObject Draw(Maze maze, List<Room> rooms)
    {

        int width = maze.width;
        int height = maze.height;

        wallPrefab = Resources.Load<Transform>("Prefabs/Wall");
        floorPrefab = Resources.Load<Transform>("Prefabs/Floor");

        GameObject mazeObject = new GameObject();

        var floor = Instantiate(floorPrefab, mazeObject.transform);
        floor.name = "Floor";
        floor.localPosition = new Vector3(-size / 2, -floor.localScale.y / 2, -size / 2);
        floor.localScale = new Vector3(width * size * floor.localScale.x, floor.localScale.y, height * size * floor.localScale.z);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < rooms.Count; k++) {
                    var room = rooms[k];
                    if (i >= room.bounds[0] && i <= room.bounds[1] && j >= room.bounds[2] && j <= room.bounds[3]) {
                        goto SKIP;
                    }
                }
                var cell = maze.cells[i, j];
                var position = new Vector3(-width / 2 + i, 1f, -height / 2 + j);
                if (width % 2 == 1) {
                    position.x -= 0.5f;
                }
                if (height % 2 == 1) {
                    position.z -= 0.5f;
                }
                position.x = position.x * size;
                position.z = position.z * size;

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, mazeObject.transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, size / 2);
                    topWall.localScale = new Vector3(topWall.localScale.x * size + size * topWall.localScale.z, topWall.localScale.y, topWall.localScale.z * size);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, mazeObject.transform) as Transform;
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(leftWall.localScale.x * size + size * leftWall.localScale.z, leftWall.localScale.y, leftWall.localScale.z * size);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, mazeObject.transform) as Transform;
                        rightWall.position = position + new Vector3(+size / 2, 0, 0);
                        rightWall.localScale = new Vector3(rightWall.localScale.x * size + size * rightWall.localScale.z, rightWall.localScale.y, rightWall.localScale.z * size);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, mazeObject.transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(bottomWall.localScale.x * size + size * bottomWall.localScale.z, bottomWall.localScale.y, bottomWall.localScale.z * size);
                    }
                }
                SKIP: ;
            }

        }
        return mazeObject;
    }
}
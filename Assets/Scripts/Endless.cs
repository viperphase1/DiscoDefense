using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Math = System.Math;
using Enum = System.Enum;

[System.Serializable]
public class Constraint {
    public Tower tower;
    public int towerCap = 0;
}

[System.Serializable]
public class RoundConstraint {
    public int round;
    public List<Constraint> constraints = new List<Constraint>();
}

public class Room {
    public int width;
    public int height;
    public int[] center;
    public int[] bounds;

    public Room(int roomWidth, int roomHeight, int mazeWidth, int mazeHeight) {
        Random rng = new Random();
        width = roomWidth;
        height = roomHeight;
        int margin_w = (int) Math.Ceiling((double) (width + 1) / 2);
        int margin_h = (int) Math.Ceiling((double) (height + 1) / 2);
        center = new int[] {rng.Next(margin_w, mazeWidth - margin_w), rng.Next(margin_h, mazeHeight - margin_h)};
        bounds = new int[] {center[0], center[0], center[1], center[1]};
        for (int i = 1; i < width; i+=2) {
            bounds[1]++;
        }
        for (int i = 2; i < width; i+=2) {
            bounds[0]--;
        }
        for (int i = 1; i < height; i+=2) {
            bounds[3]++;
        }
        for (int i = 2; i < height; i+=2) {
            bounds[2]--;
        }
    }
}

[System.Flags]
public enum PointType
{
    Vertex = 1,
    Cell = 2, 
    Room = 4,
    Center = 8,
    Guard = 16,
}

public class DropPoint {
    public Vector3 position;
    public PointType type;

    public DropPoint(Vector3 _position) {
        position = _position;
    }
}

public class Endless : MonoBehaviour
{
    Random rng = new Random();

    public int round = 1;
    public float size = 4f;
    public float towerHeight = 3f;
    public int probabilityRings = 3;
    public List<RoundConstraint> rounds = new List<RoundConstraint>();
    private GameObject player;
    private GameObject mazeContainer;
    private GameObject headContainer;
    private GameObject towerContainer;
    private Transform exitPrefab;
    private Tower[] towers;

    // TODO: create start menu
    // options:
    // resume at saved round
    // start over
    // leave

    // TODO: create pause menu
    // options:
    // resume
    // restart
    // quit (go back to start menu)
    // go back to main menu

    void Start() {
        towers = SOManager.GetAllInstances<Tower>();
        player = gameObject.transform.Find("Player").gameObject;
        mazeContainer = gameObject.transform.Find("MazeContainer").gameObject;
        headContainer = gameObject.transform.Find("HeadContainer").gameObject;
        towerContainer = gameObject.transform.Find("TowerContainer").gameObject;
        exitPrefab = Resources.Load<Transform>("Prefabs/Exit");
        loadPlayerData();
        startRound();
    }

    void loadPlayerData() {
        if (PlayerPrefs.HasKey("Round")) {
            round = PlayerPrefs.GetInt("Round");
        }
    }

    void startRound() {
        // TODO: unload previous round objects if any

        // add music
        MusicManager mm = gameObject.GetComponent<MusicManager>();
        mm.getTracks();
        mm.addMusicPlayer(player);
        MusicTrack track = mm.defaultTrack(round);
        mm.play();

        HeadBob bobber = headContainer.transform.Find("AnimationRig").GetComponent<HeadBob>();
        bobber.bpm = track.bpm;
        bobber.start();

        // calculate values
        int smallest_dim = Math.DivRem(round - 1, 3, out int r) + 3;
        int width = smallest_dim;
        int height = smallest_dim;
        if (r == 1) {
            width++;
        }
        if (r == 2) {
            height++;
        }
        // TODO: have rounds where the budget increases instead of the size of the maze
        // TODO: support a greater variety of aspect ratios

        int budget = width * height * 100;
        float secondsPerSquareUnit = 1f;
        int time = (int) (secondsPerSquareUnit * width * height * size);

        List<Room> rooms = new List<Room>();

        int num3x3Rooms = 0;
        int num5x5Rooms = 0;

        if ((width * height) >= 36) {
            num3x3Rooms = (int) Math.Ceiling((double) (width * height) / 36);
        }
        // After adding 3x3 rooms, 5x5 rooms remove too many walls
        // if ((width * height) >= 100) {
        //     num5x5Rooms = (int) Math.Ceiling((double) (width * height) / 100);
        // }

        for (int i = 0; i < num3x3Rooms; i++) {
            rooms.Add(new Room(3,3,width,height));
        }

        for (int i = 0; i < num5x5Rooms; i++) {
            rooms.Add(new Room(5,5,width,height));
        }

        // draw maze
        Debug.Log($"Width: {width}, Height: {height}");
        var maze = MazeGenerator.Generate(width, height);
        var mr = new MazeRenderer();
        mr.size = size;
        var mazeObject = mr.Draw(maze, rooms);
        mazeObject.transform.parent = mazeContainer.transform;

        Vector3 cellToPosition(int[] cell, float y = 0f) {
            return new Vector3(
                -(width * size) / 2 + cell[0] * size, 
                y, 
                -(height * size) / 2 + cell[1] * size
            );
        }

        Vector3 vertexToPosition(int[] vertex, float y = 0f) {
            return new Vector3(
                (width + 1) * size / -2 + vertex[0] * size, 
                y, 
                (height + 1) * size / -2 + vertex[1] * size
            );
        }
        
        // determine starting and ending points
        int latOrLong = rng.Next(0,2);
        int otherSide = rng.Next(0,2);
        int lockedDimension = latOrLong == 0 ? width : height;
        int unlockedDimension = latOrLong == 0 ? height : width;
        int[] startingPoint = {rng.Next(0, width), rng.Next(0, height)};
        startingPoint[latOrLong] = otherSide == 0 ? 0 : lockedDimension - 1;

        Debug.Log($"Starting point: {startingPoint[0]}, {startingPoint[1]}");
        
        player.transform.position = cellToPosition(startingPoint, player.transform.position.y);

        int[] exitPoint = {0,0};
        int midPoint = (int) Math.Ceiling((double) lockedDimension / 2);
        exitPoint[latOrLong] = rng.Next(otherSide == 1 ? 0 : midPoint, otherSide == 1 ? midPoint : lockedDimension);
        exitPoint[(latOrLong + 1) % 2] = rng.Next(0, unlockedDimension);

        Debug.Log($"Exit point: {exitPoint[0]}, {exitPoint[1]}");

        var exit = Instantiate(exitPrefab, transform);
        exit.transform.position = cellToPosition(exitPoint, exit.localScale.y / 2 + 0.2f);

        var floor = mazeObject.transform.Find("Floor") as Transform;
        headContainer.transform.position = new Vector3(floor.position.x, 10, floor.position.z + floor.localScale.z / 2 + 5);

        // TODO: add towers
        int maxIndex = Math.Min(round - 1, towers.Length - 1);
        int minPrice = 100000;
        for (int i = 0; i <= maxIndex; i++) {
            if (towers[i].price < minPrice) {
                minPrice = towers[i].price;
            }
        }
        List<Tower> selectedTowers = new List<Tower>();
        while (budget >= minPrice) {
            List<Tower> affordableTowers = new List<Tower>();
            for (int i = 0; i <= maxIndex; i++) {
                if (towers[i].price <= budget) {
                    affordableTowers.Add(towers[i]);
                }
            }
            int towerIndex = rng.Next(0, affordableTowers.Count);
            selectedTowers.Add(affordableTowers[towerIndex]);
            budget -= affordableTowers[towerIndex].price;
        }
        // default behavior without player death statistics
        int guardWeightScaleFactor = (int) Math.Floor((double) (width * height) / 10);
        // add a potential drop point for each cell in the maze
        List<DropPoint> cellPoints = new List<DropPoint>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int[] cell = {i, j};
                Vector3 cellPosition = cellToPosition(cell);
                DropPoint cp = new DropPoint(cellPosition);
                cp.type = PointType.Cell;
                int weight = 1;
                for (int k = 0; k < rooms.Count; k++) {
                    var room = rooms[k];
                    if (i >= room.bounds[0] && i <= room.bounds[1] && j >= room.bounds[2] && j <= room.bounds[3]) {
                        cp.type |= PointType.Room;
                        weight = 2;
                        if (cell == room.center) {
                            cp.type |= PointType.Center;
                            weight = room.width;
                            // end the loop early
                            k = rooms.Count;
                        }
                    }
                }
                // give more weight to cells close to the exit, the weight should scale with the area of the maze
                float exitProximity = (cellPosition - new Vector3(exit.transform.position.x, 0, exit.transform.position.z)).magnitude / size;
                if (exitProximity <= probabilityRings * Math.Sqrt(2)) {
                    weight = (probabilityRings + 1) - (int) Math.Floor(exitProximity);
                    cp.type |= PointType.Guard;
                    weight = weight * Math.Max(1, guardWeightScaleFactor);
                }
                for (int k = 1; k <= weight; k++) {
                    cellPoints.Add(cp);
                }
            }
        }
        // add a potential drop point for each vertex in the maze
        // I am doing this separately because the logic isn't quite the same
        List<DropPoint> vertexPoints = new List<DropPoint>();
        // there is one more vertex than cells in both directions
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                int[] vertex = {i, j};
                Vector3 vertexPosition = vertexToPosition(vertex);
                DropPoint vp = new DropPoint(vertexPosition);
                vp.type = PointType.Vertex;
                int weight = 1;
                for (int k = 0; k < rooms.Count; k++) {
                    var room = rooms[k];
                    if (i >= room.bounds[0] && i <= room.bounds[1] + 1 && j >= room.bounds[2] && j <= room.bounds[3] + 1) {
                        vp.type |= PointType.Room;
                        weight = 2;
                        k = rooms.Count;
                    }
                }
                // give more weight to vertices close to the exit, the weight should scale with the area of the maze
                float exitProximity = (vertexPosition - new Vector3(exit.transform.position.x, 0, exit.transform.position.z)).magnitude / size;
                if (exitProximity <= (probabilityRings + 2) * Math.Sqrt(2) / 2) {
                    weight = (probabilityRings + 1) - (int) Math.Floor(exitProximity);
                    vp.type |= PointType.Guard;
                    weight = weight * Math.Max(1, guardWeightScaleFactor);
                }
                for (int k = 1; k <= weight; k++) {
                    vertexPoints.Add(vp);
                }
            }
        }
        // the following has been absorbed into the loop above
        // add additional dropPoints for the cells of each room with more weight given to the center of the room
        // for (int i = 0; i < rooms.Count; i++) {
        //     Room room = rooms[i];
        //     for (int j = room.bounds[0] + 1; j <= room.bounds[1] - 1; j++) {
        //         for (int k = room.bounds[2] + 1; k <= room.bounds[3] - 1; k++) {
        //             int[] cell = {j,k};
        //             PointType type = PointType.Cell | PointType.Room;
        //             if (cell == room.center) {
        //                 type |= PointType.Center;
        //             }
        //             DropPoint p = new DropPoint(cellToPosition(cell));
        //             p.type = type;
        //             addPoint(p, cell == room.center ? room.width : 1);
        //         }
        //     }
        // }
        List<DropPoint> allPoints = new List<DropPoint>();
        allPoints.AddRange(cellPoints);
        allPoints.AddRange(vertexPoints);
        List<DropPoint> selectedPoints = new List<DropPoint>();
        // collect as many random spots in the maze as towers
        // we are keeping a separate list for these points in case we don't want to match towers to points by index
        int remainingCellCount = cellPoints.Count;
        for (int i = 0; i < selectedTowers.Count; i++) {
            DropPoint selectedPoint;
            Tower tower = selectedTowers[i];
            PointType types = tower.supportedPointTypes;
            if (types.HasFlag(PointType.Cell) && types.HasFlag(PointType.Vertex)) {
                selectedPoint = allPoints[rng.Next(0, allPoints.Count)];
            } else if (types.HasFlag(PointType.Cell)) {
                // select a point from the portion of allPoints that are cells
                selectedPoint = allPoints[rng.Next(0, remainingCellCount)];
            } else {
                // selectedPoint is a vertex
                // select a point from the portion of allPoints that are vertices
                selectedPoint = allPoints[rng.Next(remainingCellCount, allPoints.Count)];
            }
            selectedPoints.Add(selectedPoint);
            // IndexOf returns the index of the first instance
            int deleteIndex = allPoints.IndexOf(selectedPoint);
            // remove duplicates
            int deleteCount = 0;
            while(allPoints[deleteIndex] == selectedPoint) {
                allPoints.RemoveAt(deleteIndex);
                deleteCount++;
            }
            if (selectedPoint.type.HasFlag(PointType.Cell)) {
                remainingCellCount -= deleteCount;
            }
        }
        // originally I wanted to give stronger towers to drop points with a higher type
        // static int SortByRating(Tower t1, Tower t2)
        // {
        //     float rating1 = t1.towerPrefab.GetComponent<TowerBehavior>().getRating();
        //     float rating2 = t2.towerPrefab.GetComponent<TowerBehavior>().getRating();
        //     return rating1.CompareTo(rating2);
        // }
        // selectedTowers.Sort(SortByRating);
        // static int SortByType(DropPoint p1, DropPoint p2)
        // {
        //     return ((int) p1.type).CompareTo((int) p2.type);
        // }
        // selectedPoints.sort(SortByType);
        for (int i = 0; i < selectedTowers.Count; i++) {
            GameObject newTower = Instantiate(selectedTowers[i].towerPrefab, towerContainer.transform);
            TowerBehavior towerBehavior = newTower.GetComponent<TowerBehavior>();
            towerBehavior.radius *= size;
            newTower.transform.position = new Vector3(
                selectedPoints[i].position.x, 
                selectedTowers[i].towerHeight, 
                selectedPoints[i].position.z
            );
        }

        // TODO: apply upgrades

        // TODO: add powerups

        // TODO: start countdown

        // TODO: react to lose or win conditions

        // on player death save data about the player position at the point of death, the killing shot, and the towers nearby

    }
}

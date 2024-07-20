using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SentryBehavior : TowerBehavior
{
    Random rng = new Random();
    private Transform toWaypoint;
    private Transform fromWaypoint;
    public float speed = 0.01f;
    private Vector3 towerToPlayer;
    private float distanceToPlayer;
    LayerMask wpLayerMask;

    public override void Start() {
        base.Start();
        wpLayerMask = (1 << LayerMask.NameToLayer("Waypoints"));
    }

    private Transform getStartingPoint() {
        Collider[] waypointColliders = Physics.OverlapSphere(transform.position, 0.1f, wpLayerMask);
        return waypointColliders[0].transform;
    }

    private List<Transform> getNearbyWaypoints() {
        Collider[] waypointColliders = Physics.OverlapSphere(transform.position, scale, wpLayerMask);
        List<Transform> waypoints = new List<Transform>();
        foreach(Collider coll in waypointColliders) {
            if (coll.transform != toWaypoint) {
                waypoints.Add(coll.transform);
            }
        }
        return waypoints;
    }

    private List<Transform> getReachableWaypoints() {
        List<Transform> waypoints = getNearbyWaypoints();
        List<Transform> reachable = new List<Transform>();
        foreach(Transform waypoint in waypoints) {
            // prevent sentries from going to the same waypoint
            Transform reservedBy = waypoint.GetComponent<Waypoint>().reservedBy;
            if (reservedBy != null && reservedBy != transform) {
                continue;
            }
            // leave out any waypoints obstructed by a wall, the player, or another sentry
            bool obstruction = Physics.Raycast(transform.position, waypoint.position - transform.position, scale, ~(wpLayerMask | 1 << LayerMask.NameToLayer("Exit")));
            if (!obstruction) {
                reachable.Add(waypoint);
            }
        }
        return reachable;
    }

    public override void Update() {
        if (!fromWaypoint) {
            fromWaypoint = getStartingPoint();
            toWaypoint = fromWaypoint;
        }
        withinRange = false;
        towerToPlayer = playerCamera.transform.position - transform.position;
        distanceToPlayer = towerToPlayer.magnitude;
        float searchRadius = radius * scale;
        if (distanceToPlayer < searchRadius) {
            // need some way of excluding this tower from the raycast below
            gameObject.layer = LayerMask.NameToLayer("Temp");
            bool obstructed = Physics.Raycast(transform.position, towerToPlayer, distanceToPlayer, 1 << LayerMask.NameToLayer("Structures") | 1 << LayerMask.NameToLayer("Towers"));
            gameObject.layer = LayerMask.NameToLayer("Towers");
            withinRange = !obstructed;
        }
        if (distanceToPlayer > 1f + transform.GetComponent<SphereCollider>().radius * 2) {
            // move forward through the maze or follow the player if within range
            if (transform.position == toWaypoint.position) {
                toWaypoint.GetComponent<Waypoint>().reservedBy = null;
                Transform _fromWaypoint = fromWaypoint;
                fromWaypoint = toWaypoint;
                List<Transform> waypoints = getReachableWaypoints();
                if (waypoints.Count > 0) {
                    if (withinRange) {
                        // pick the waypoint closest to the player
                        // not convinced my layer mask logic is correct
                        float newDistanceToPlayer = distanceToPlayer;
                        foreach(Transform waypoint in waypoints) {
                            bool obstructed = Physics.Raycast(waypoint.position, playerCamera.position - waypoint.position, scale, 1 << LayerMask.NameToLayer("Structures") | 1 << LayerMask.NameToLayer("Towers"));
                            if (!obstructed) {
                                if (Vector3.Distance(waypoint.position, playerObject.position) < newDistanceToPlayer) {
                                    toWaypoint = waypoint;
                                    newDistanceToPlayer = Vector3.Distance(toWaypoint.position, playerCamera.transform.position);
                                }
                            }
                        }
                    } else {
                        // don't want to be able to go backwards unless necessary
                        if (waypoints.Count > 1) {
                            waypoints.Remove(_fromWaypoint);
                        }
                        // pick a random waypoint
                        toWaypoint = waypoints[rng.Next(0, waypoints.Count)];
                    }
                }
                toWaypoint.GetComponent<Waypoint>().reservedBy = transform;
            }
            float step = Time.deltaTime * speed;
            transform.position = Vector3.MoveTowards(transform.position, toWaypoint.position, step);
        }
        if (withinRange) {
            // play siren sound
            transform.forward = playerCamera.transform.position - transform.position;
        } else {
            if (toWaypoint.position != transform.position) {
                transform.LookAt(toWaypoint.position);
            }
        }
        fireOnInterval();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : FollowBehavior
{
    private LineRenderer lineRenderer;

    public override void Start() {
        base.Start();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    public override void tracking() {
        base.tracking();
        // check if the laser can make contact with a solid object
        int layerMask = LayerMaskHelper.GetLayerMask("Structures", "Towers", "Player Camera", "Accessories");
        bool hit = Physics.Raycast(spawnPoint.position, transform.rotation * Vector3.left, out RaycastHit hitInfo, Mathf.Infinity, layerMask);
        if (hit) {
            // Debug.Log("Laser can hit " + hitInfo.collider.transform.name);
            // if it can, set the starting and ending points of the laser so that it spans the space between the spawn point and the contact point
            lineRenderer.positionCount = 2;
            Vector3[] positions = {spawnPoint.position, hitInfo.point};
            lineRenderer.SetPositions(positions);
            if (Transform.ReferenceEquals(hitInfo.collider.transform, playerCamera)) {
                player.takeDamage(damage);
            }
        } else {
            // otherwise, we should not render the laser
            lineRenderer.positionCount = 0;
        }
    }

    public override void notTracking() {
        base.notTracking();
        lineRenderer.positionCount = 0;
    }
}

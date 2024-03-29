using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class TowerBehavior : MonoBehaviour
{
    public GameObject ammoPrefab;
    public float radius = 3f;
    // fireRate is in beats. A value of 2 means that this tower will fire every 2 beats
    public float fireRate = 2f;
    public float damage = 1f;
    public float velocity = 3f;
    [HideInInspector]
    public float scale = 1f;
    [HideInInspector]
    public MusicManager mm;

    public string slug = "Tower";
    protected bool withinRange = false;
    protected Transform spawnPoint;
    protected Transform player;
    protected Transform playerCamera;
    protected int lastInterval;

    public virtual void Start()
    {
        Transform root = transform;
        while (root.parent) {
            root = root.parent.transform;
        }
        player = Global.FindDeepChild(root, "Player");
        playerCamera = Global.FindDeepChild(root, "Main Camera");
        spawnPoint = Global.FindDeepChild(transform, "SpawnPoint");
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 distance = playerCamera.transform.position - transform.position;
        withinRange = distance.magnitude < radius * scale;
        if (withinRange) {
            // TODO: 
            transform.forward = playerCamera.transform.position - transform.position;
        }
        fireOnInterval();
    }

    public void fireOnInterval() {
        int interval = mm.getInterval(fireRate);
        if (interval != lastInterval) {
            lastInterval = interval;
            if (interval > 0) {
                fire();
            }
        }
    }

    public float getRating() {
        return radius * damage * 1 / fireRate;
    }

    public virtual void fire() {
        if (withinRange) {
            Debug.Log(slug + ": Fire");
            var ammo = Instantiate(ammoPrefab).transform;
            ammo.name = slug + "_" + "ammo";
            var ab = ammo.GetComponent<AmmoBehavior>();
            ab.player = player.GetComponent<Player>();
            ab.damage = damage;
            ammo.position = spawnPoint.position;
            Vector3 ammoToPlayer = playerCamera.transform.position - ammo.position;
            ammoToPlayer.Normalize();
            var rigidBodies = GetAllRigidBodies(ammo);
            foreach (Rigidbody rb in rigidBodies) {
                rb.velocity = ammoToPlayer * velocity;
            }
            // TODO: fire sfx
        }
    }
}

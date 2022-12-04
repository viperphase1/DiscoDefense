using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehavior : MonoBehaviour
{
    public GameObject ammoPrefab;
    public float radius = 3f;
    public float fireRate = 2f;
    public float damage = 1f;
    public float velocity = 3f;
    protected virtual string slug {get { return "Tower";} set {}}
    private Transform spawnPoint;
    private Transform player;
    private bool withinRange = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Transform root = transform;
        while (root.parent) {
            root = root.parent.transform;
        }
        player = Global.FindDeepChild(root, "Main Camera");
        spawnPoint = Global.FindDeepChild(transform, "SpawnPoint");
        StartCoroutine("fire");
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 distance = player.transform.position - transform.position;
        withinRange = distance.magnitude < radius;
        if (withinRange) {
            transform.forward = player.transform.position - transform.position;
        }
    }

    public float getRating() {
        return radius * damage * 1 / fireRate;
    }

    public virtual IEnumerator fire() {
        while(true) {
            if (withinRange) {
                Debug.Log("Fire");
                var ammo = Instantiate(ammoPrefab).transform;
                ammo.gameObject.layer = LayerMask.NameToLayer("Weapons");
                ammo.name = slug + "_" + "ammo";
                var ab = ammo.GetComponent<AmmoBehavior>();
                ab.damage = damage;
                ammo.position = spawnPoint.position;
                Vector3 toPlayer = player.transform.position - ammo.position;
                toPlayer.Normalize();
                ammo.GetComponent<Rigidbody>().velocity = toPlayer * velocity;
                // TODO: fire sfx
            }
            yield return new WaitForSeconds(fireRate);
        }
    }
}

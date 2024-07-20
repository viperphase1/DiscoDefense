using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Global;

public class TowerBehavior : MonoBehaviour
{
    public GameObject ammoPrefab;
    public AudioClip defaultSound;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public float radius = 3f;
    // fireRate is in beats. A value of 2 means that this tower will fire every 2 beats
    public float fireRate = 2f;
    public float damage = 1f;
    public float velocity = 3f;
    public float scale = 1f;
    [HideInInspector]
    public MusicManager mm;
    public string slug = "Tower";

    protected AudioSource audioSource;
    protected bool withinRange = false;
    protected Transform spawnPoint;
    protected Transform player;
    protected Transform playerCamera;
    protected int lastInterval;

    private List<Renderer> renderers = new List<Renderer>();

    public virtual void Start()
    {
        Transform root = transform;
        while (root.parent) {
            root = root.parent.transform;
        }
        player = Global.FindDeepChild(root, "Player");
        playerCamera = Global.FindDeepChild(root, "Main Camera");
        spawnPoint = Global.FindDeepChild(transform, "SpawnPoint");

        // add audio source at runtime
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = defaultSound;
        RouteAudioSourceToMixerGroup(audioSource, "SFX");
        TowerSoundEffect themedSound = theme.towerSoundEffects.FirstOrDefault(sfx => sfx.towerSlug == slug);
        if (themedSound != null) {
            audioSource.clip = themedSound.clip;
        }

        // apply default material
        renderers = DeepGetComponents<Renderer>(transform);
        foreach (Renderer renderer in renderers) {
            renderer.material = defaultMaterial;
        }

        // Add XR Grab Interactable component for highlighting
        // XRGrabInteractable interactable = gameObject.AddComponent<XRGrabInteractable>();

        // Configure hover events
        // interactable.hoverEntered.AddListener((hoverEvent) => OnHoverEnter());
        // interactable.hoverExited.AddListener((hoverEvent) => OnHoverExit());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 distance = playerCamera.transform.position - transform.position;
        withinRange = distance.magnitude < radius * scale;
        if (withinRange) {
            transform.forward = playerCamera.position - transform.position;
        }
        fireOnInterval();
    }

    public virtual void fireOnInterval() {
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
            audioSource.Play(0);
        }
    }

    public void OnHoverEnter()
    {
        foreach (Renderer renderer in renderers) {
            renderer.material = highlightMaterial;
        }
    }

    public void OnHoverExit()
    {
        foreach (Renderer renderer in renderers) {
            renderer.material = defaultMaterial;
        }
    }
}

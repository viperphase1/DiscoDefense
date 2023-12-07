using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class BurstAmmoBehavior : AmmoBehavior
{
    public override void OnDestroy() {
        if (destroyInProgress) return;
        destroyInProgress = true;
        Debug.Log("Using sub class OnDestroy");
        PointBurst(gameObject.transform.position, new Transform[]{transform}, 100.0f, 0.2f);
        StartCoroutine(DestroyBuffer(1));
    }

    IEnumerator DestroyBuffer(float waitTime){
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}

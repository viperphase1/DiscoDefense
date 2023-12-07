using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension{
 
    public static void Clear(this GameObject obj){
        foreach (Transform child in obj.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}

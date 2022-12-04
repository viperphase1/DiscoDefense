using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour{
    
    public int bpm = 120;
    public Animator anim;

    public void start(){
       StartCoroutine("bob"); 
    }

    IEnumerator bob (){
        while(true){
            anim.speed = bpm * 1/120;
            yield return new WaitForSeconds(0.00001f);
            anim.SetTrigger("Bob");
        }
    }

}

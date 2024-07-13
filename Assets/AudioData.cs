using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour{

public AudioSource audioSource;
public float[] samples = new float[512];
public int initialBlockAmount;
public GameObject defaultBlockRef;
public int columnSpacing;
public Material defaultMat;
public int divAmount;

// class with List of GameObjects to be used as the List type
[System.Serializable]
public class blocks{
    public List<GameObject> blockList = new List<GameObject>();
    public float freqAmp = 0;
}

//List using blocks as type (list of lists)
public List<blocks> listOfBlockLists = new List<blocks>();

private void Start() {

    for(int i = 0; i < samples.Length / divAmount; i++){
    if(listOfBlockLists.Count < samples.Length / divAmount){
        listOfBlockLists.Add(new blocks());
    }
    }
    for(int i = 0; i < listOfBlockLists.Count; i++){
        Vector3 center = transform.position;
        Vector3 pos = RandomCircle(center, listOfBlockLists.Count, i);
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center-pos);
        for(int j = 0; j < initialBlockAmount; j++){
            GameObject blockRef = Instantiate(defaultBlockRef, pos, rot);
            blockRef.transform.SetParent(transform);
            listOfBlockLists[i].blockList.Add(blockRef);
        }
    }
    int ID = 0;
    for(int i = 0; i < listOfBlockLists.Count; i++){
        for(int j = 0; j < listOfBlockLists[i].blockList.Count; j++){

            listOfBlockLists[i].blockList[j].transform.position = new Vector3(listOfBlockLists[i].blockList[j].transform.position.x, j * 5, listOfBlockLists[i].blockList[j].transform.position.z);
            
            GameObject clone = GameObject.Find(defaultBlockRef.name + "(Clone)");
            clone.transform.GetComponent<MeshRenderer>().material = defaultMat;
            clone.gameObject.name = ID.ToString();
            ID++;
        }
    }
}

Vector3 RandomCircle (Vector3 center ,float radius, int i){
    float ang = i * columnSpacing;
    Vector3 pos;
    pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
    pos.y = center.y;
    pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
    return pos;
}

private void Update() {
    if (audioSource) {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        float max = 0;
        float min = 0;
        for(int i = 0; i < samples.Length; i++){
            if(samples[i] > max){
                max = samples[i];
            }
            if(samples[i] < min) {
                min = samples[i];
            }
        }
        for(int i = 0; i < listOfBlockLists.Count; i++){
            listOfBlockLists[i].freqAmp = ((samples[i] - min) / (max - min))  * 200.0f;
            for(int j = 0; j < listOfBlockLists[i].blockList.Count; j++){
                if(j > Mathf.RoundToInt(samples[i] * 200.0f)){
                    listOfBlockLists[i].blockList[j].transform.GetComponent<MeshRenderer>().enabled = false;
                } else {
                    listOfBlockLists[i].blockList[j].transform.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }  
    } else {
        Debug.Log("No audio source to get samples from");
    }
}  

}


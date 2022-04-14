using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkToGround : MonoBehaviour
{
    // Start is called before the first frame update
    float destroyHeight;
  
    void Start()
    {
        if (this.gameObject.tag == "RagDollZombie")
        {
            Invoke("ReadyToSink",2f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SinkIntoGround()
    {
        this.transform.Translate(0f, -0.001f, 0f);
        if(this.transform.position.y<destroyHeight)
        {
            Destroy(gameObject);
        }
    }
    public void ReadyToSink()
    {
        destroyHeight = Terrain.activeTerrain.SampleHeight(this.transform.position)-5f;
        Collider[] colliderList = this.transform.GetComponentsInChildren<Collider>();
        foreach (Collider item in colliderList)
        {
            Destroy(item);
            
        }
        InvokeRepeating("SinkIntoGround", 5f, 0.1f);

    }
}

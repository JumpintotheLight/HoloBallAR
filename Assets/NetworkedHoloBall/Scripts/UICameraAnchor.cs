using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraAnchor : MonoBehaviour
{
    public Vector3 anchoredOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Camera.main.transform.position - anchoredOffset, Camera.main.transform.up, 0f);
        this.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
    }
}

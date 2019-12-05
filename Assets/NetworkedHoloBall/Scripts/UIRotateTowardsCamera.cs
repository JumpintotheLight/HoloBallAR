using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRotateTowardsCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
    }
}

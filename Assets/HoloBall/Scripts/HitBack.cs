using UnityEngine;
using System.Collections;

public class HitBack : MonoBehaviour
{

    public Transform head;

    void OnCollisionEnter()
    {
        Rigidbody r = BallManager.Instance.CurrentBall.GetComponent<Rigidbody>();
        r.angularVelocity = Vector3.zero;
        Vector3 dir = (head.position - r.transform.position).normalized;
        r.velocity = dir * 10f;
    }


}
using UnityEngine;
using System.Collections;

public class LoseCollider3D : MonoBehaviour {

    private Level_Manager levelManager;
    public GameObject prefab;

    void Start() {
        levelManager = GameObject.FindObjectOfType<Level_Manager>();
    }
	void OnTriggerEnter (Collider trigger) {
        // levelManager.LoadLevel("Lose");
        Debug.Log("You Lose!");
    }
	
	void OnCollisionEnter (Collision collision){
        if(collision.gameObject.tag == "Ball")
        {
            Destroy(collision.gameObject);

            // This should be placed in BallManager script
            //Instantiate(prefab, new Vector3(0, 0.8f, 2.5f), Quaternion.identity);



        }


    }
}

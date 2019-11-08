using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick3D : MonoBehaviour {
    private Level_Manager levelManager;
    public AudioClip crack;
    public Sprite[] hitSprites;
    private int timesHits;
    public static int breakableCount = 0;
    private bool isBreakable;
    public Material brokenmaterial;

    // Use this for initialization
    void Start () {
        isBreakable = (this.tag == "Breakable");
        if (isBreakable)
        {
            breakableCount++;
        }
        levelManager = GameObject.FindObjectOfType<Level_Manager>();
        timesHits = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
   
        }
    void OnCollisionEnter(Collision collision) {
        if (isBreakable) { HandleHits();
           AudioSource.PlayClipAtPoint(crack, transform.position);

        }
    }

    void SimulateWin() {
        levelManager.LoadNextLevel();
    }


    void HandleHits() {
        timesHits++;
        int maxHits = hitSprites.Length + 1;
        if (timesHits >= maxHits)
        {
            breakableCount--;
            levelManager.BrickDestroyed();
            Destroy(gameObject);
        }
        else
        {
            LoadSprites();
        }
    }
    void LoadSprites() {

        if(brokenmaterial != null)
        {
            this.gameObject.GetComponent<Renderer>().material = brokenmaterial;
        }
        // FIXME
        /*
        int spriteIndex = timesHits - 1;
        if (hitSprites[spriteIndex])
        { this.GetComponent<SpriteRenderer>().sprite = hitSprites[spriteIndex]; }
        */
    }

}

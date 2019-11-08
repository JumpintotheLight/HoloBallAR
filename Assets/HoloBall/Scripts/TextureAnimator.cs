using UnityEngine;
using System.Collections;

public class TextureAnimator : MonoBehaviour
{
    new Renderer renderer;
    public float speed = 1;
    // Use this for initialization
    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time / 10 * speed,0));

    }
}

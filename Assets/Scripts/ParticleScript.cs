using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public bool onFloor;
    public GlassScript glasScript;
    float clampPos;
    public int colorID;
    float lerpValue;
    bool destroy;
    public Renderer objectRenderer;
    Vector3 originalScale;    // orijinal boyutu saklamak için
    // Start is called before the first frame update
    void Start()
    {
        clampPos = 2;
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        lerpValue += Time.deltaTime * .1f;

        var xPos = Mathf.Clamp(transform.position.x, -clampPos, clampPos);
        var zPos = Mathf.Clamp(transform.position.z, clampPos, clampPos * 2);
        transform.position = new Vector3(xPos, transform.position.y, zPos);
        if (glasScript.isOpen)
        {
            onFloor = true;
        }
        if (destroy)
        {
            // Hedef boyut, orijinal boyutun 0.25 (yüzde 25) katı olsun:
            Vector3 targetScale = originalScale * 1.05f;

            // 3. parametre (lerpValue * 0.3f) 0 ile 1 arasında ilerledikçe
            // transform.localScale, originalScale'den targetScale'e doğru yaklaşacak.
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                lerpValue * 0.3f
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "floor")
        {
            onFloor = true;
        }
        else onFloor = false;
    }

    public void ChangeColor(Material newColor)
    {
        objectRenderer.sharedMaterial = newColor;
    }
    public IEnumerator DestroyParticles()
    {
        yield return new WaitForSecondsRealtime(2f);
        destroy = true;
        GetComponentInChildren<ParticleSystem>().Play();
        // Destroy(gameObject, 2f);

    }

}

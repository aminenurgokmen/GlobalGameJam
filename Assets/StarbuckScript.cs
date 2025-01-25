using System.Collections;
using UnityEngine;

public class StarbuckScript : MonoBehaviour
{
    public int starbucksID;
    public float moveDistance = 1f;

    // Hareketin süresi (saniye)
    public float moveDuration = 1f;

    // Objenin şuan hareket edip etmediğini takip ediyoruz
    private bool isMoving = false;
    public float moveSpeed = 1f;
    float startDelay = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(MoveBackward());
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Move()
    {
        StartCoroutine(MoveBackward());
    }
    public IEnumerator MoveBackward()
    {
        // Zaten hareket ediyorsa tekrar başlatma
        if (isMoving) yield break;
        isMoving = true;

        // Başlamadan önce 3 saniye bekle
        yield return new WaitForSeconds(startDelay);

        // Başlangıç pozisyonunu kaydet
        Vector3 startPosition = transform.position;
        float movedSoFar = 0f; // Ne kadar mesafe gidildi

        // Belirtilen mesafe kadar (moveDistance) gidene kadar döngüde kal
        while (movedSoFar < moveDistance)
        {
            // Bu karede gideceğimiz mesafe
            float moveStep = moveSpeed * Time.deltaTime;

            // Eğer kalan mesafe, bu karedeki adım uzunluğundan küçükse
            // sadece kalan mesafe kadar git, tam belirtilen noktada dur
            float remaining = moveDistance - movedSoFar;
            if (moveStep > remaining)
            {
                moveStep = remaining;
            }

            // Pozisyonu z ekseninde geriye doğru kaydır
            transform.Translate(Vector3.back * moveStep);

            // Kaç birim hareket ettik, topla
            movedSoFar += moveStep;

            yield return null; // Bir frame bekle
        }

        // Tam 1 birim (ya da moveDistance kadar) geriye geldiğinden emin ol
        transform.position = startPosition + Vector3.back * moveDistance;

        isMoving = false;
    }
}

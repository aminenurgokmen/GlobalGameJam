using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public static GameScript Instance;
    public Transform selectedGlass, otherGlass, oldGlass;
    public List<GlassScript> glassList;
    public List<Material> colorList;
    public bool onClick;

    public List<StarbuckScript> starbucksCups;
    private int currentStarbucksIndex = 0;
    public bool done;
    public GameObject floor;
    public AudioSource blop;

    // Kullanıcının tıklama yapıp yapamayacağını kontrol eden değişken
    public bool canClick = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        glassList = new List<GlassScript>(FindObjectsOfType<GlassScript>());
    }

    void Update()
    {
        // Null olan bardakları listeden temizle
        glassList.RemoveAll(item => item == null);

        if (glassList.Count <= 0)
        {
            UIManager.Instance.ShowSuccesPanel2();
        }

        if (currentStarbucksIndex >= starbucksCups.Count)
        {
            return;
        }

        // Fare tıklamasını kontrol ediyoruz
        if (Input.GetMouseButtonDown(0))
        {
            blop.Play();
            RaycastHit hit;

            // 1) Eğer animasyon sırasında (canClick = false) tıklanırsa:
            if (!canClick)
            {
                // Raycast ile tıklanan objeyi bul
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10, 1 << 6))
                {
                    // Tıklanan objede en az 6 çocuk varsa 6. çocuğu aktif et
                    if (hit.transform.childCount > 6)
                    {
                        hit.transform.GetChild(6).gameObject.SetActive(true);
                    }
                }
                // Tıklama işlemini burada sonlandırıyoruz (return)
                return;
            }

            // 2) Eğer canClick = true ise normal akış
            onClick = true;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10, 1 << 6))
            {
                // Bardağa tıklandığında yapılan işlemler
                if (selectedGlass == null)
                {
                    selectedGlass = hit.transform;
                    oldGlass = selectedGlass;
                    selectedGlass.GetComponent<GlassScript>().isOpen = true;
                }
                else if (selectedGlass == hit.transform)
                {
                    return;
                }
                else if (selectedGlass)
                {
                    selectedGlass = hit.transform;
                    otherGlass = selectedGlass;
                    selectedGlass.GetComponent<GlassScript>().isOpen = true;
                }
            }
        }
        else
        {
            onClick = false;
        }

        // Seçili bardak Starbucks rengine uyuyor mu?
        if (selectedGlass)
        {
            var glassScript = selectedGlass.GetComponent<GlassScript>();
            var currentStarbucks = starbucksCups[currentStarbucksIndex];

            if (glassScript.glassID == currentStarbucks.starbucksID)
            {
                glassScript.isOpen = true;
            }
            else
            {
                glassScript.isOpen = false;
                selectedGlass.GetComponent<Animator>().SetTrigger("shake");
                selectedGlass = null;
            }
        }

        // Merge kontrolü
        if (oldGlass && otherGlass)
        {
            GlassScript oldGS = oldGlass.GetComponent<GlassScript>();
            GlassScript otherGS = otherGlass.GetComponent<GlassScript>();

            if (oldGS.isOpen && otherGS.isOpen &&
                oldGS.glassID == otherGS.glassID &&
                !oldGS.occupied && !otherGS.occupied)
            {
                oldGS.merge = true;
                otherGS.merge = true;

                if (oldGS.merge && otherGS.merge)
                {
                    Debug.Log("Merge done!");

                    // Animasyon başlamadan önce tıklamaları kapat
                    canClick = false;

                    // 5 saniyelik animasyonu tetikleyelim
                    starbucksCups[currentStarbucksIndex]
                        .GetComponent<Animator>()
                        .SetTrigger("done");

                    floor.GetComponent<Animator>().SetTrigger("Open");

                    // 5 saniye sonra tıklamayı yeniden aç
                    StartCoroutine(WaitForAnimation(5f));

                    // Sıradaki Starbucks
                    currentStarbucksIndex++;

                    // Değişkenleri sıfırla
                    otherGlass = null;
                    oldGlass = null;
                    selectedGlass = null;

                    // Starbuck bardakları hareket ettirme (opsiyonel)
                    int moveCount = starbucksCups.Count;
                    for (int offset = 0; offset < moveCount; offset++)
                    {
                        int targetIndex = currentStarbucksIndex + offset;
                        if (targetIndex < starbucksCups.Count)
                        {
                            starbucksCups[targetIndex]
                                .GetComponent<StarbuckScript>()
                                .Move();
                        }
                    }
                }
            }
            else
            {
                oldGS.isOpen = false;
                otherGS.isOpen = false;
                selectedGlass = null;
                otherGlass = null;
                oldGlass = null;
            }
        }
    }

    // Belirli bir süre (ör. 5 sn) bekledikten sonra tıklamayı yeniden açan Coroutine
    private IEnumerator WaitForAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canClick = true;
    }
}

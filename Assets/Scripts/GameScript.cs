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
    // public GameObject starbucks;
    public List<StarbuckScript> starbucksCups; // Sahnede birden fazla Starbucks olabilir
    private int currentStarbucksIndex = 0;
    public bool done;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        glassList = new List<GlassScript>(FindObjectsOfType<GlassScript>());

    }

    // Update is called once per frame
    void Update()
    {
        glassList.RemoveAll(item => item == null);

        if (glassList.Count <= 0)
        {
            UIManager.Instance.ShowSuccesPanel2();
        }
        if (currentStarbucksIndex >= starbucksCups.Count)
        {
            // Tüm bardaklar tamamlandı, belki bir kazandı paneli vs.
            return;
        }
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            onClick = true;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10, 1 << 6))
            {

                // Barda�a t�kland�
                if (selectedGlass == null)
                {
                    // Se�ili bardak yoksa, se�ili barda�� ayarla
                    selectedGlass = hit.transform;
                    oldGlass = selectedGlass;
                    selectedGlass.GetComponent<GlassScript>().isOpen = true;
                }
                else if (selectedGlass == hit.transform)
                {
                    return;

                    // Se�ili barda�a tekrar t�kland�, barda�� eski yerine getir
                    selectedGlass.GetComponent<GlassScript>().isOpen = false;
                    selectedGlass = null;
                }
                else if (selectedGlass)
                {
                    // Ba�ka bir barda�a t�kland�, �nceki se�ili barda�� eski yerine getir
                    // selectedGlass.GetComponent<GlassScript>().isOpen = false;
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

        // Once you have a selected glass, check if it matches any Starbucks
        if (selectedGlass)
        {
            var glassScript = selectedGlass.GetComponent<GlassScript>();

            // O an doldurulması gereken Starbucks (sıralı)
            var currentStarbucks = starbucksCups[currentStarbucksIndex];

            // Eğer seçtiğimiz bardağın rengi, current Starbucks'ın ID’siyle eşleşiyorsa açılabilir
            if (glassScript.glassID == currentStarbucks.starbucksID)
            {
                // Renk uyuyor, bardağı açık tut
                glassScript.isOpen = true;
            }
            else
            {
                // Renk eşleşmiyorsa, shake animasyonu ve seçiliyi sıfırla
                glassScript.isOpen = false;
                selectedGlass.GetComponent<Animator>().SetTrigger("shake");
                selectedGlass = null;
            }
        }

        if (oldGlass && otherGlass)
        {
            GlassScript oldGS = oldGlass.GetComponent<GlassScript>();
            GlassScript otherGS = otherGlass.GetComponent<GlassScript>();

            // İkisi de open ve ID'leri eşleşiyorsa merge
            if (oldGS.isOpen && otherGS.isOpen &&
                oldGS.glassID == otherGS.glassID &&
                !oldGS.occupied && !otherGS.occupied)
            {
                oldGS.merge = true;
                otherGS.merge = true;

                if (oldGS.merge && otherGS.merge)
                {

                    // Birleştirme tamam
                    Debug.Log("Merge done!");

                    // Burada “currentStarbucksIndex”i arttırabiliriz 
                    // çünkü bu renk bardaklar tamamlandı.
                    starbucksCups[currentStarbucksIndex]
                        .GetComponent<Animator>()
                        .SetTrigger("done");

                    currentStarbucksIndex++; // Sıradaki Starbucks’a geç
                                             // starbucksCups[currentStarbucksIndex].GetComponent<StarbuckScript>().isMoving = true;

                    // Diğer değişkenleri sıfırla
                    otherGlass = null;
                    oldGlass = null;
                    selectedGlass = null;

                    int moveCount = starbucksCups.Count;

                    // currentStarbucksIndex’ten başlayıp moveCount kadar ilerle
                    for (int offset = 0; offset < moveCount; offset++)
                    {
                        int targetIndex = currentStarbucksIndex + offset;

                        // targetIndex dizin sınırlarını aşmıyorsa Move() çağır
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
                // Uymuyorsa kapat
                oldGS.isOpen = false;
                otherGS.isOpen = false;
                selectedGlass = null;
                otherGlass = null;
                oldGlass = null;
            }
        }


    }


}

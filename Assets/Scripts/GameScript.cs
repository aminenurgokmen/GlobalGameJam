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
    public GameObject starbucks;
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
        if (selectedGlass)
        {
            if (selectedGlass.GetComponent<GlassScript>().glassID == starbucks.GetComponent<StarbuckScript>().starbucksID)
            {
                selectedGlass.GetComponent<GlassScript>().isOpen = true;
            }
            else
            {
                selectedGlass.GetComponent<GlassScript>().isOpen = false;
                selectedGlass.GetComponent<Animator>().SetTrigger("shake");
                selectedGlass = null;
            }
        }

        if (oldGlass && otherGlass)
        {
            if (oldGlass.GetComponent<GlassScript>().isOpen && otherGlass.GetComponent<GlassScript>().isOpen
            && otherGlass.GetComponent<GlassScript>().glassID == oldGlass.GetComponent<GlassScript>().glassID &&
                !oldGlass.GetComponent<GlassScript>().occupied && !otherGlass.GetComponent<GlassScript>().occupied)
            {
                oldGlass.GetComponent<GlassScript>().merge = true;
                otherGlass.GetComponent<GlassScript>().merge = true;
                if (oldGlass.GetComponent<GlassScript>().merge == true && otherGlass.GetComponent<GlassScript>().merge == true)
                {

                    starbucks.GetComponent<Animator>().SetTrigger("done");
                    otherGlass = null;
                    oldGlass = null;
                    selectedGlass = null;
                }

            }
            else
            {
                oldGlass.GetComponent<GlassScript>().isOpen = false;
                otherGlass.GetComponent<GlassScript>().isOpen = false;
                selectedGlass = otherGlass = oldGlass = null;
            }
        }

        glassList.RemoveAll(item => item == null);

        // for (int i = 0; i < glassList.Count; i++)
        // {
        //     if (glassList[i].isOpen && glassList[(i + 1) % glassList.Count].isOpen && glassList[i].glassID == glassList[(i + 1) % glassList.Count].glassID && !glassList[(i + 1) % glassList.Count].occupied)
        //     {
        //         glassList[i].merge = true;
        //         glassList[(i + 1) % glassList.Count].merge = true;
        //         glassList.RemoveAll(item => item == null);
        //     }
        // }
        if (glassList.Count <= 0)
        {
            UIManager.Instance.ShowSuccesPanel2();
        }
    }


}

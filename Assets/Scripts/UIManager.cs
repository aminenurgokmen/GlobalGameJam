using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject succes, fail;
    float newDly;
    bool callSucces, callFail;
    public Text text;
    int i = 1;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (callFail || callSucces)
        {
            newDly += Time.deltaTime * 2;
            Debug.Log(newDly);

        }
        LevelInfo();

    }
    public void ShowSuccesPanel2()
    {
        StartCoroutine(ShowSuccesPanel());
    }
    IEnumerator ShowSuccesPanel()
    {
        yield return new WaitForSeconds(1f);
        succes.SetActive(true);
    }

    public void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            SceneManager.LoadScene(0);

        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }
    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void LevelInfo()
    {
        text.text = "LEVEL " + SceneManager.GetActiveScene().buildIndex.ToString();
    }
}

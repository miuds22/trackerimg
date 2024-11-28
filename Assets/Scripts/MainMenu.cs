using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ScenceManager.loadScence()
    }


    public void  LadScenceInt(int scenceIndex)
    {
        SceneManager.LoadScene(scenceIndex);
    }

    public void LadScenceStr(string scenceName)
    {
        SceneManager.LoadScene(scenceName);
    }
}

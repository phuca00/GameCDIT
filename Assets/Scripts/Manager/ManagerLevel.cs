using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLevel : MonoBehaviour
{
    public void Level1()
    {
        SceneManager.LoadScene("level1");   
    }

    public void Level2()
    {
        SceneManager.LoadScene("level2");
    }

    public void Level3()
    {
        SceneManager.LoadScene("level3");
    }

    public void Level4()
    {
        SceneManager.LoadScene("level4");
    }

    public void Level5()
    {
        SceneManager.LoadScene("level5");
    }

    public void Level6()
    {
        SceneManager.LoadScene("level6");
    }

    public void Level7()
    {
        SceneManager.LoadScene("level7");
    }

    public void Level8()
    {
        SceneManager.LoadScene("level8");
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}

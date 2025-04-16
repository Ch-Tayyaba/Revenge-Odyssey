using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exchangescene : MonoBehaviour
{
    public void Home()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Next()
    {
        SceneManager.LoadSceneAsync(1);
    }
}

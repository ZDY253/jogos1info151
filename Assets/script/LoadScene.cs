using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public string sceneName;


    public void LoadSceneByName()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}

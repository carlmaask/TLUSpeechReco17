using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    
    public void NewSceneBtn(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void QuitGameBtn()
    {
        Application.Quit();
    }
}
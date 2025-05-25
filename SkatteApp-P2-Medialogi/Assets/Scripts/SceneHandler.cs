using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
   public void SceneToChange(string scene)
    {
        SceneManager.LoadScene(scene);

    }
}

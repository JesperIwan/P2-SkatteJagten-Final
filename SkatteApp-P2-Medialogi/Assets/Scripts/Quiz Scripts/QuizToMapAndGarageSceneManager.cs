using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizMapAndGarageSceneManager : MonoBehaviour
{
    public int sceneIndexToLoad;
    public void OpenScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
    }
}

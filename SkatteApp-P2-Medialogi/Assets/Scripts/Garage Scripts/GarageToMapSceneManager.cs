using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageToMapSceneManager : MonoBehaviour
{
    public int sceneIndexToLoad;
    public void OpenScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
    }
}

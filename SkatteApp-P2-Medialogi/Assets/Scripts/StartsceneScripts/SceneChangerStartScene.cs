using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerStartScene : MonoBehaviour
{
    // This will be triggered by an Animation Event
    public void LoadSceneAfterAnimation()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(5);
        
    }
}


using UnityEngine;

public class MenuToSettingScript : MonoBehaviour
{
    public string sceneIndexToLoad;

    public void OpenScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
        AudioManager.Instance.PlaySFX("Click");
    }
}


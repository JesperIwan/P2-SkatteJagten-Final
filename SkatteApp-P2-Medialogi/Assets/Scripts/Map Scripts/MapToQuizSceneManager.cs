using UnityEngine;
using UnityEngine.SceneManagement;

public class MapToQuizSceneManager : MonoBehaviour
{
    public int sceneIndexToLoad;
    public int quizIndexToLoad;

    public void OpenScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
        QuizManager.quizNumber = quizIndexToLoad;
        AudioManager.Instance.PlaySFX("Click");
    }
}

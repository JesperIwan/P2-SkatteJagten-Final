using UnityEngine;

public class txtScript : MonoBehaviour
{
    public static txtScript Instance { get; private set; }

    public string name4File;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
    }
}

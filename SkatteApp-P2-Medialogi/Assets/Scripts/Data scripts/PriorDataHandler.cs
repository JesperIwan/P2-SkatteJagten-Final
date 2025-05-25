using UnityEngine;

public class PriorDataHandler : MonoBehaviour
{
    public static PriorDataHandler Instance { get; private set; }

    public float priorData;
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


    public void SetData(float setData) {  priorData = setData; }
}

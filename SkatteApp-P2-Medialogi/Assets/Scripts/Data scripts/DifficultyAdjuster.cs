using UnityEngine;

public class DifficultyAdjuster : MonoBehaviour
{
    public float prior;
    public float learningRate;
    float priorProbability;
    float posterior;  

    public DataStore data; 
    public QuizManager quizManager;
   

    public void Start() 
    {
        if (PriorDataHandler.Instance.priorData != null || PriorDataHandler.Instance.priorData > 0)
        {
            intialisePrior(PriorDataHandler.Instance.priorData);
        }
        else
        {
            intialisePrior(0.51f);
        }
    }
    

    public void intialisePrior(float startPrior) { priorProbability = startPrior; }   

    public void difficultyAdjust(bool awnser)
    {
        posterior = awnser ? 1 : 0; 

        prior = (1 - learningRate) * priorProbability + learningRate * posterior; 

        data.LogBayesian(prior);
        Debug.Log("Prior: " + prior);

        priorProbability = prior;
        quizManager.updatedPrior(priorProbability);

    }


}

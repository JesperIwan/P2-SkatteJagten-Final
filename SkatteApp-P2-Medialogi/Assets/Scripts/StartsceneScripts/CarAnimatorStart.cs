using UnityEngine;

public class StartCarAnimation : MonoBehaviour
{
    public Animator carAnimator;

    public void PlayCarAnimation()
    {
        carAnimator.Play("CarDriveAnimation");
        AudioManager.Instance.PlaySFX("CarToAnimation");
    }

}




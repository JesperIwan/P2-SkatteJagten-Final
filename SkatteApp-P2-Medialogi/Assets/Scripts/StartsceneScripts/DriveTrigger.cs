using UnityEngine;

public class DriveTrigger : MonoBehaviour
{
    public Animator carAnimator;

    public void Drive()
    {
        carAnimator.SetTrigger("DriveNow");
    }
}


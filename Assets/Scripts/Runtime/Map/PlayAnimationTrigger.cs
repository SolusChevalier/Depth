using UnityEngine;

public class PlayAnimationTrigger : MonoBehaviour
{
    public Animator targetAnimator;
    public string triggerName = "Play";
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            targetAnimator.SetTrigger(triggerName);
            hasPlayed = true;
        }
    }
}

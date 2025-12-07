using UnityEngine;

public class DeerNPC : MonoBehaviour
{
    public Animator animator;

    public void PlayCelebrate()
    {
        if (animator == null) return;

        animator.SetTrigger("Celebrate");
    }
}

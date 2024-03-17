using UnityEngine;

public class PlayerFingerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int FingerMovement = Animator.StringToHash("FingerMovement");

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            RestartAnimation();
        }
    }

    private void RestartAnimation()
    {
        animator.Play(FingerMovement, 0, 0f);
        animator.Update(0f);
    }
}

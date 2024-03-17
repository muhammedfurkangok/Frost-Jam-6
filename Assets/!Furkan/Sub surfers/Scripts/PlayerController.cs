using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Furkan.Sub_surfers.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rigidbody;

        [Header("Parameters")]
        [SerializeField] private float jumpPower = 1200;
        [SerializeField] private Ease rotationEase;
        [SerializeField] private float swipeSpeed = 20f;
        [SerializeField] private float rotationDuration = 0.23f;
        [SerializeField] private float rollDuration = 1.18f;
        [SerializeField] private float deltaXValue = 20;
        [SerializeField] private float obstacleHitWaitTime = 3f;

        [Header("Info - No Touch")]
        [SerializeField] private CharacterPositionStates characterPositionState = CharacterPositionStates.Mid;
        [SerializeField] private bool isObstacleHit;
        [SerializeField] private bool isJumping;
        [SerializeField] private bool isRolling;
        [SerializeField] private bool isGrounded = true;

        private Vector3 startPosition;
        private float newXPosition;

        private static readonly int SlideTrigger = Animator.StringToHash("Slide_Trigger");
        private static readonly int ObstacleHit = Animator.StringToHash("ObstacleHit");

        private bool swipeUp, swipeLeft, swipeRight, swipeDown;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (isObstacleHit) return;

            GetInput();

            Jump();

            if (swipeLeft || swipeRight) Move();
            if (isGrounded && swipeDown) Roll();
        }

        private void GetInput()
        {
            swipeLeft = Input.GetKeyDown(KeyCode.A);
            swipeRight = Input.GetKeyDown(KeyCode.D);
            swipeUp =  Input.GetKeyDown(KeyCode.W);
            swipeDown = Input.GetKeyDown(KeyCode.S);
        }

        private void Move()
        {
            DOTween.Kill(gameObject);

            if (swipeLeft)
            {
                if (characterPositionState == CharacterPositionStates.Mid)
                {
                    newXPosition -= deltaXValue;
                    characterPositionState = CharacterPositionStates.Left;

                    animator.Play("dodgeLeft");
                }

                else if (characterPositionState == CharacterPositionStates.Right)
                {
                    characterPositionState = CharacterPositionStates.Mid;
                    newXPosition = startPosition.x;

                    animator.Play("dodgeLeft");
                }

                transform.DORotate(new Vector3(0f, -45f, 0), rotationDuration);
            }

            else if (swipeRight)
            {
                if (characterPositionState == CharacterPositionStates.Mid)
                {
                    newXPosition += deltaXValue;
                    characterPositionState = CharacterPositionStates.Right;

                    animator.Play("dodgeRight");
                }

                else if (characterPositionState == CharacterPositionStates.Left)
                {
                    characterPositionState = CharacterPositionStates.Mid;
                    newXPosition = startPosition.x;

                    animator.Play("dodgeRight");
                }

                transform.DORotate(new Vector3(0f, 45f, 0), rotationDuration);
            }

            transform.DOMoveX(newXPosition, swipeSpeed).SetSpeedBased().onComplete += () => transform.DORotate(Vector3.zero, rotationDuration).SetEase(rotationEase);
        }

        private void Jump()
        {
            if (isGrounded)
            {
                if (swipeUp)
                {
                    animator.CrossFadeInFixedTime("Jump",0.1f);
                    rigidbody.AddForce(jumpPower * Vector3.up);
                    isJumping = true;
                }
                
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                {
                    animator.Play("Landing");
                    isJumping = false;
                }
            }
        }
        
        private async void Roll()
        {
            animator.SetTrigger(SlideTrigger);
            isRolling = true;
            await UniTask.WaitForSeconds(rollDuration);
            isRolling = false;
        }

        private async void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                isJumping = false;
            }

            if (other.gameObject.CompareTag("Obstacle"))
            {
                other.collider.GetComponent<Obstacle>().DestroyObstacle();

                DOTween.Kill(gameObject);
                animator.SetTrigger(ObstacleHit);
                isObstacleHit = true;
                await UniTask.WaitForSeconds(obstacleHitWaitTime);
                isObstacleHit = false;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
    }
}
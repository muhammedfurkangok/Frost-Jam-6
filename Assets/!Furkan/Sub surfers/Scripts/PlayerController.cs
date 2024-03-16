using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Furkan.Sub_surfers.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private CharacterPositionStates _characterPositionStates = CharacterPositionStates.Mid;
        [SerializeField] private Animator m_animator;
        [SerializeField] private float _jumpPower = 30;

        [SerializeField]  private float _swipeSpeed = 20;

        #endregion

        #region Private Variables

        private bool _swipeUp, _swipeLeft, _swipeRight, _swipeDown;

        private bool isJumping;
        private bool isRoll;
        private bool isGrounded = true;

        private float _xValue = 20;
       
        private float _newXPos = 0f;
        private float _lerp;
        private Rigidbody _rigidbody;
        private float ColHeight;
        private float ColCenterY;
        private float rollDuration = 1.18f;
       [SerializeField] private Ease rotEase;
       [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float rotationDuration = 0.1f;
        
        
        private static readonly int SlideTrigger = Animator.StringToHash("Slide_Trigger");

        #endregion


        #endregion


        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            m_animator = GetComponent<Animator>();
            transform.position = Vector3.zero;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            GetInput();
          if(  _swipeLeft||  _swipeRight) Move();
            Jump();
            
            if (isGrounded && _swipeDown) Roll();
        }

        private void GetInput()
        {
            _swipeLeft = Input.GetKeyDown(KeyCode.A);
            _swipeRight = Input.GetKeyDown(KeyCode.D);
            _swipeUp =  Input.GetKeyDown(KeyCode.W);
            _swipeDown = Input.GetKeyDown(KeyCode.S);
        }

        private void Move()
        {
            DOTween.KillAll();
            if(_swipeLeft )
            {
                if(_characterPositionStates == CharacterPositionStates.Mid)
                {
                    _newXPos = -_xValue;
                    _characterPositionStates = CharacterPositionStates.Left;
                    m_animator.Play("dodgeLeft");
                    
                }
                else if(_characterPositionStates == CharacterPositionStates.Right)
                {
                    _characterPositionStates = CharacterPositionStates.Mid;
                    _newXPos = 0f;
                    m_animator.Play("dodgeLeft");
                }

                transform.DORotate(new Vector3(0f, -45f, 0), rotationDuration);
            }
            else if(_swipeRight)
            {
                if(_characterPositionStates == CharacterPositionStates.Mid)
                {
                    _newXPos = _xValue;
                    _characterPositionStates = CharacterPositionStates.Right;
                    m_animator.Play("dodgeRight");
                }
                else if(_characterPositionStates == CharacterPositionStates.Left)
                {
                    _characterPositionStates = CharacterPositionStates.Mid;
                    _newXPos = 0f;
                    m_animator.Play("dodgeRight");
                }
                transform.DORotate(new Vector3(0f, 45f, 0), rotationDuration);
            }
          
            transform.DOMoveX(_newXPos, _swipeSpeed).SetSpeedBased().onComplete += () => transform.DORotate(Vector3.zero, rotationDuration).SetEase(rotEase);


        }

        private void Jump()
        {
            if (isGrounded)
            {
                if (_swipeUp)
                {
                    m_animator.CrossFadeInFixedTime("Jump",0.1f);
                    _rigidbody.AddForce(_jumpPower * Vector3.up);
                }
                
                if(m_animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                {
                    m_animator.Play("Landing");
                    isJumping = false;
                }
                
                if (_swipeUp)
                {
                    m_animator.CrossFadeInFixedTime("Jump",0.1f);
                    isJumping = true;
                }
            }
        }
        
        private async void Roll()
        {
            m_animator.SetTrigger(SlideTrigger);
            isRoll = true;
            await UniTask.WaitForSeconds(rollDuration);
            isRoll = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
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
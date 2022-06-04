using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JJ
{
    public class PlayerLocomotion : MonoBehaviour
    {
        CameraHandler cameraHandler;
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]//0.25
        float groundDetectionRayStartPoint = 0.5f;
        [SerializeField]//0.5
        float minimumDistanceNeededToBeginFall = 1.5f;
        [SerializeField]//0.05
        float groundDirectionRayDistance = 0.05f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement stats")]
        [SerializeField]
        float movementSpeed = 5.0f;
        [SerializeField]
        float sprintSpeed = 7.0f;
        [SerializeField]
        float rotationSpeed = 10.0f;
        [SerializeField]
        float fallingSpeed = 350.0f;
        [SerializeField]
        float fallingAccel = 1;

        private float prevInAirTimer;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }
        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent <InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            if(inputHandler.lockOnFlag)
            {
                if(inputHandler.sprintFlag || inputHandler.rollFlag)
                {
                    Vector3 targetDir = Vector3.zero;
                    targetDir = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                    targetDir += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                    targetDir.y = 0;

                    if (targetDir == Vector3.zero)
                    {
                        targetDir = transform.forward;
                    }
                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
                else
                {
                    Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }

            }
            else
            {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputHandler.moveAmount;

                targetDir = cameraObject.forward * inputHandler.vertical;
                targetDir += cameraObject.right * inputHandler.horizontal;

                targetDir.Normalize();
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                {
                    targetDir = myTransform.forward;
                }

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

                myTransform.rotation = targetRotation;
            }

        }

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;
            if (playerManager.isInteracting)
                return;
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if(inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                if (inputHandler.moveAmount < 0.5f)
                {
                    moveDirection *= speed; //suppposed to be walking speed
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }

            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;
            if(inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);

            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            }

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            //If player is interacting, disable rolling and sprinting
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if(inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    CalculateRotation();

                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;
            //If raycast comes out and hits sth directly in front of you, you can't move
            if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if(playerManager.isInAir)
            {
                if(inAirTimer > 1.0f && inAirTimer > prevInAirTimer * 2 && fallingAccel < 10)
                {
                    fallingAccel += 0.5f;
                    prevInAirTimer = inAirTimer;
                }
                float fallingVelocity = fallingSpeed * fallingAccel;
                rigidbody.AddForce(Vector3.down * fallingVelocity);
                //If u need to walk off an edge, you will hop off so u don't get stuck on an edge
                rigidbody.AddForce(moveDirection * fallingSpeed / 10.0f);
            }

            Vector3 direction = moveDirection;
            direction.Normalize();
            origin = origin + direction * groundDirectionRayDistance;

            targetPosition = myTransform.position;
            Debug.DrawRay(origin, Vector3.down * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
            bool groundDetected = (Physics.Raycast(origin, Vector3.down, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck));

            if(groundDetected)
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if(playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                        {
                            //Debug.Log("You were in the air for " + inAirTimer);
                            animatorHandler.PlayTargetAnimation("Landing", true);
                            inAirTimer = 0;
                            fallingAccel = 1;
                        }
                        else
                        {
                            animatorHandler.PlayTargetAnimation("Empty", false);
                            inAirTimer = 0;
                            fallingAccel = 1;
                        }
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }
                if(playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);

                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
                
                if(playerManager.isInAir && playerManager.isInteracting == false)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                
            }
            #region SG's code
            /*
            if (Physics.Raycast(origin, Vector3.down, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    // if player was in the air for more than 0.5s when the raycast detected the ground, play a landing animation
                    if (inAirTimer > 0.5f)
                    {
                        Debug.Log("You were in the air for " + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Landing", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }
                if (playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        Debug.Log("Player is Falling");
                        animatorHandler.PlayTargetAnimation("Falling", true);

                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }
            */
            #endregion
            if (playerManager.isGrounded)
            {
                if(playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }
            
            if(playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                myTransform.position = targetPosition;
            }
            
        }

        public void HandleJumping()
        {
            if (playerManager.isInteracting)
                return;
            if(inputHandler.jump_Input)
            {
                if(inputHandler.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;
                    animatorHandler.PlayTargetAnimation("Jump Forward", true);
                    CalculateRotation();

                }
            }
        }
        #endregion

        private void CalculateRotation()
        {
            moveDirection.y = 0;
            Quaternion rotation = Quaternion.LookRotation(moveDirection);
            myTransform.rotation = rotation;
        }
    }
}

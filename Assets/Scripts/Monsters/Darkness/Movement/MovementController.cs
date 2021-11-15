using UnityEngine;

namespace DarknessMinion.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField, Range(1, 25)]
        private float maxAccel, maxSpeed;
        [SerializeField]
        private Transform inputSpace;
        [SerializeField, Range(1, 360)]
        private float rotationSpeed;
        private Rigidbody rgdBod;
        
        private IInputInterpreter inputInterpreter;
        private Vector3 velocity;

        public float movementSpeedMax {
            get { return maxSpeed; }
        }
        
        public Vector2 inputDirection { get; private set; }

        private void Awake()
        {
            rgdBod = GetComponent<Rigidbody>();
            foreach (MonoBehaviour mon in GetComponents(typeof(MonoBehaviour)))
            {
                if (mon is IInputInterpreter)
                    inputInterpreter = (IInputInterpreter)mon;
            }
        }

        private void FixedUpdate()
        {
            Move();
        }

        public Vector2 GetPosition()
        {
            return transform.position.ToVector2();
        }

        public Vector2 GetVelocity()
        {
            return velocity.ToVector2();
        }

        public void RotateTowardsDirection(Vector3 mDir) 
        {
            Vector3 dir = Vector3.RotateTowards(transform.forward, mDir, 2.0f * Time.deltaTime * rotationSpeed, 0.1f);
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        }

        private void Move() //TODO decouple the heading from the forward. Move towards the desired velocity more gradually. Save the gradual velocity
        {
            if (inputInterpreter != null)
            {
                InputInfo inputInfo = inputInterpreter.GetInputInfo();
                
                inputDirection = inputInfo.inputDirection;
                if (inputDirection != Vector2.zero)
                {
                    float maxSpeedChange = maxAccel * Time.deltaTime;
                    Vector2 desiredVelocity;
                    if (inputSpace)
                        desiredVelocity = (inputSpace.TransformDirection(inputDirection.ToVector3()) * maxSpeed)
                            .ToVector2();
                    else desiredVelocity = inputDirection * (maxSpeed + inputInfo.maxSpeedModifier);

                    velocity = rgdBod.velocity;

                    velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
                    velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.y, maxSpeedChange);

                    rgdBod.velocity = new Vector3(velocity.x, rgdBod.velocity.y, velocity.z);
                }
                RotateTowardsDirection(velocity);
            }
            inputDirection = Vector2.zero;
        }
    }
}
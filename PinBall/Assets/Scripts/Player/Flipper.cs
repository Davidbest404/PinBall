using UnityEngine;

namespace Pinball.Player
{
    [RequireComponent(typeof(HingeJoint2D))]
    public class Flipper : MonoBehaviour
    {
        [Header("Управление")]
        [SerializeField] private KeyCode activateKey = KeyCode.Z;

        [Header("Мотор")]
        [SerializeField] private float activeSpeed = 2000f;
        [SerializeField] private float restSpeed = -2000f;

        private HingeJoint2D hinge;
        private JointMotor2D motor;

        private void Start()
        {
            hinge = GetComponent<HingeJoint2D>();
            motor = hinge.motor;
            motor.motorSpeed = restSpeed;
            hinge.motor = motor;
        }

        private void FixedUpdate()
        {
            motor.motorSpeed = Input.GetKey(activateKey) ? activeSpeed : restSpeed;
            hinge.motor = motor;
        }
    }
}
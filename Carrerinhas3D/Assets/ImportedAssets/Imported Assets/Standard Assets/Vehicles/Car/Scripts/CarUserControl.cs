using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private bool IsCarEnabled;
        private CarController CarControl; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            CarControl = GetComponent<CarController>();
        }

        public void SetCarEnabled(bool isEnabled)
        {
            this.IsCarEnabled = isEnabled;
        }

        public void ForceBrake()
        {
            CarControl.Move(1, 1, 1, 0.1f);
        }
        public void ForceStart()
        {
            CarControl.Move(0, -10, -10, 0);
        }


        private void FixedUpdate()
        {
            if (IsCarEnabled)
            {
                // pass the input to the car!
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                CarControl.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            }
            else
            {

                CarControl.Move(0, 0, 0, 0.1f);
            }
        }
    }
}

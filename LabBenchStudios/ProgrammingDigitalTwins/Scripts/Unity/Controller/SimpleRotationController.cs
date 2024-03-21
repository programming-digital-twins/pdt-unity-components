/**
 * MIT License
 * 
 * Copyright (c) 2024 Andrew D. King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;

using UnityEngine;

using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Common;

/**
 * Controller for managing rotational speed.
 * 
 */
namespace LabBenchStudios.Pdt.Unity.Controller
{
    public class SimpleRotationController : MonoBehaviour, IDataContextEventListener
    {
        public const int SECS_PER_MIN = 60;

        [SerializeField]
        public GameObject rotationHub;

        [SerializeField, Range(0.0f, 100000.0f)]
        private float rotationsPerMinute = 0.0f;

        public enum RotationDirection
        {
            Clockwise,
            Counterclockwise
        };

        [SerializeField]
        private RotationDirection rotationDirection = RotationDirection.Clockwise;

        public enum BrakingType
        {
            Instant,
            Timed,
            Algorithmic
        };

        [SerializeField]
        private BrakingType brakingType = BrakingType.Timed;

        [SerializeField]
        private Boolean isBrakeEngaged = false;

        [SerializeField]
        private GameObject rotatorActivationLight = null;

        [SerializeField]
        private Boolean enableRotation = true;

        [SerializeField]
        private Boolean enableBraking = true;

        [SerializeField]
        private float decelerateToZeroRpmSeconds = 0.0f;

        private float rotationsPerSecond = 0.0f;
        private int totalDegrees = 360;

        private Boolean enableActivationLight = true;
        private Boolean activationLightStatus = true;
        private Boolean rotationStatus = true;

        private Transform rotationHubTransform = null;
        private Vector3 rotationalVector = Vector3.zero;


        // internal methods

        void Awake()
        {
            if (this.rotationHub == null)
            {
                this.rotationHub = gameObject;
            }

            this.rotationHubTransform = this.rotationHub.transform;
            this.rotationsPerSecond =
                this.rotationsPerMinute > 0.0f ? this.rotationsPerMinute / SECS_PER_MIN : 0.0f;
        }

        void Update()
        {
            if (this.enableRotation)
            {
                this.activationLightStatus = true;
                this.rotationStatus = true;

                if (!this.isBrakeEngaged)
                {
                    if (this.rotationsPerSecond > 0.0f)
                    {
                        switch (this.rotationDirection)
                        {
                            case RotationDirection.Clockwise:
                                this.totalDegrees = -360;
                                break;
                            case RotationDirection.Counterclockwise:
                                this.totalDegrees = 360;
                                break;
                        }

                        // calculate rotationsPerSecond every update in case
                        // API call changes rotationsPerMinute dynamically
                        this.rotationsPerSecond = this.rotationsPerMinute / SECS_PER_MIN;
                        this.rotationHubTransform.Rotate(0, (this.rotationsPerSecond * this.totalDegrees) * Time.deltaTime, 0);
                    }
                }
                else
                {
                    this.rotationStatus = false;
                    this.activationLightStatus = false;
                }
            }
            else
            {
                this.activationLightStatus = false;
                this.rotationStatus = false;
            }

            if (this.activationLightStatus && this.rotatorActivationLight != null)
            {
                this.rotatorActivationLight.gameObject.SetActive(this.activationLightStatus);
            }
        }


        // public methods

        public void EnableBrakingSystem(bool enable)
        {
            this.isBrakeEngaged = enable;
        }

        public void SetRotationsPerMinute(float rpm)
        {
            if (rpm >= 0.0f)
            {
                this.rotationsPerMinute = rpm;
            }
        }

        public void HandleActuatorData(ActuatorData data)
        {
            if (data != null)
            {
                switch (data.GetTypeID())
                {
                    case ConfigConst.WIND_TURBINE_BRAKE_SYSTEM_ACTUATOR_TYPE:
                        this.EnableBrakingSystem((data.GetCommand() == ConfigConst.COMMAND_ON));
                        break;
                }
            }
        }

        public void HandleConnectionStateData(ConnectionStateData data)
        {
            // ignore
        }

        public void HandleMessageData(MessageData data)
        {
            // ignore
        }

        public void HandleSensorData(SensorData data)
        {
            if (data != null)
            {
                int typeID = data.GetDeviceType();

                Debug.Log($"Processing incoming rotation controller SensorData: {data.GetDeviceID()} - {typeID}");

                switch (typeID)
                {
                    case ConfigConst.WIND_TURBINE_ROTATIONAL_SPEED_SENSOR_TYPE:
                        //this.SetRotationsPerMinute((float)Math.Round(data.GetValue(), 1));
                        this.SetRotationsPerMinute(data.GetValue());
                        break;

                    case ConfigConst.IMPELLER_RPM_SENSOR_TYPE:
                        //this.SetRotationsPerMinute((float)Math.Round(data.GetValue(), 1));
                        this.SetRotationsPerMinute(data.GetValue());
                        break;
                }
            }
        }

        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            // ignore
        }


    }

}

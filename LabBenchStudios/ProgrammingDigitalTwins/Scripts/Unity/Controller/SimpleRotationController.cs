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

/**
 * Controller for managing rotational speed.
 * 
 */
namespace LabBenchStudios.Pdt.Unity.Controller
{
    public class SimpleRotationController : BaseAsyncDataMessageProcessor
    {
        public const float PI = 3.14159f;
        public const int SECS_PER_MIN = 60;

        [SerializeField]
        public GameObject rotationHub;

        [SerializeField, Range(0.0f, 100000.0f)]
        private float rotationsPerMinute = 5000.0f;

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

        void Start()
        {
            if (this.rotationHub == null)
            {
                this.rotationHub = gameObject;
                this.rotationHubTransform = rotationHub.transform;
            }
            else
            {
                this.rotationHubTransform = this.rotationHub.transform;
            }

            // set rotationsPerSecond initially
            this.rotationsPerSecond = this.rotationsPerMinute / SECS_PER_MIN;
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
                        this.rotationalVector.y = this.rotationsPerSecond * this.totalDegrees;
                        this.rotationHubTransform.Rotate(this.rotationalVector * Time.deltaTime);
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

        protected override void InitMessageHandler()
        {
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessMessageData(MessageData data)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessSensorData(SensorData data)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            throw new NotImplementedException();
        }
    }
}

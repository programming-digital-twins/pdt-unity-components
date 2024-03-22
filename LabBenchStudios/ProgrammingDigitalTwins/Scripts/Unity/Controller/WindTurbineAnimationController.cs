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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using System;

using UnityEngine;

/**
 * Controller for managing wind turbine rotational speed
 * and energy generation.
 * 
 * References:
 *  - https://www.weather.gov/epz/wxcalc_windconvert
 *  - https://www.energy.gov/eere/wind/how-wind-turbine-works-text-version
 *  - https://www.e-education.psu.edu/emsc297/node/649
 */
namespace LabBenchStudios.Pdt.Unity.Controller
{
    public class WindTurbineAnimationController : SimpleRotationController
    {
        public const float AIR_DENSITY = 1.225f;
        public const float DEFAULT_RADIUS = 4.2672f;

        // for further documentation on typical cut-out and cut-in speeds,
        // see https://www.energy.gov/eere/articles/how-do-wind-turbines-survive-severe-storms
        [SerializeField, Range(56.0f, 100.0f)]
        private float fullStopWindSpeed = 65.0f;

        [SerializeField, Range(40.0f, 55.0f)]
        private float cutOutWindSpeed = 55.0f;

        [SerializeField, Range(1.0f, 10.0f)]
        private float cutInWindSpeed = 5.0f;

        [SerializeField, Range(1.0f, 10.0f)]
        private float warningZoneWindSpeedRange = 5.0f;

        [SerializeField, Range(5.0f, 25.0f)]
        private float turbineDiameterInMeters = (DEFAULT_RADIUS * 2);

        [SerializeField, Range(0.0f, 50.0f)]
        private float windVelocity = 5.0f;

        private Renderer wtObjectRenderer = null;
        private Gradient wtGradient = null;


        // public methods


        // protected methods

        void Awake()
        {
            //this.InitColorGradients();
        }


        // private methods

        /*
        private void GenerateAndSendActuationEvent(float adjustedVal)
        {
            if (this.dtStateProcessor != null)
            {
                ActuatorData data = new();

                // note: recipient should be able to auto shut-off
                // hvac once desired temp is reached - it should not
                // have to rely on a follow up command from a remote
                // system (hosted within the DTA)
                data.SetName(ConfigConst.ACTUATOR_CMD);
                data.SetDeviceID(this.dtStateProcessor.GetDeviceID());
                data.SetTypeCategoryID(ConfigConst.ENV_TYPE_CATEGORY);
                data.SetTypeID(ConfigConst.HVAC_ACTUATOR_TYPE);
                data.SetCommand(ConfigConst.COMMAND_ON);
                data.SetValue(adjustedVal);

                // state processor will ensure the target device is set
                ResourceNameContainer resource =
                    this.dtStateProcessor.GenerateOutgoingStateUpdate(data);

                EventProcessor.GetInstance().ProcessStateUpdateToPhysicalThing(resource);
            }
            else
            {
                Debug.LogError("No Digital Twin State Processor set. Ignoring actuation event.");
            }
        }
        */

        private void InitColorGradients()
        {
            // get renderer and create gradient
            this.wtObjectRenderer = gameObject.GetComponent<Renderer>();
            this.wtGradient = new Gradient();

            // use three gradients: red (highest val), green (mid val), blue (low val)
            GradientColorKey[] colorKey = new GradientColorKey[3];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

            float danger = 1.0f;
            float warning = 0.5f;// this.nominalMid / this.thresholdHigh;
            float normal = 0.1f; // this.thresholdLow / this.thresholdHigh;

            if (danger >= 1.0f) danger = 1.0f;
            if (warning >= 1.0f) warning = 0.5f;
            if (normal >= 1.0f) normal = 0.1f;

            //
            // provision color keys
            //

            // as curValue approaches danger, color becomes more red
            colorKey[0].color = Color.red;
            colorKey[0].time = danger;

            // as curValue hovers at warning, color becomes more yellow
            colorKey[1].color = Color.yellow;
            colorKey[1].time = warning;

            // as curValue approaches normal, color moves back to grey / neutral
            colorKey[2].color = Color.grey;
            colorKey[2].time = normal;

            //
            // provision alpha keys
            //

            // as curValue moves from danger to warning to normal
            // alpha curValue renders color more translucent
            alphaKey[0].alpha = 0.85f;
            alphaKey[0].time = danger;
            alphaKey[1].alpha = 0.55f;
            alphaKey[1].time = warning;
            alphaKey[2].alpha = 0.25f;
            alphaKey[2].time = normal;

            this.wtGradient.SetKeys(colorKey, alphaKey);

            // set default color to configured mid-point
            //this.UpdateComponentState(this.nominalMid);
        }

        private void UpdateComponentState(float val)
        {
            if (this.wtObjectRenderer != null)
            {
                // scale curValue to something between 0.0f and 1.0f
                //float scaledVal = (val > 0.0f ? val / this.thresholdHigh : val);

                //this.wtObjectRenderer.material.color = this.wtGradient.Evaluate(scaledVal);
            }
        }

    }

}
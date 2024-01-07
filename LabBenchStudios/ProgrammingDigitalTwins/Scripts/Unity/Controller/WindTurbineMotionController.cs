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
    public class WindTurbineMotionController : SimpleRotationController
    {
        public const float AIR_DENSITY = 1.225f;
        public const float DEFAULT_RADIUS = 4.2672f;

        [SerializeField, Range(5.0f, 25.0f)]
        private float turbineDiameterInMeters = (DEFAULT_RADIUS * 2);

        [SerializeField, Range(0.0f, 50.0f)]
        private float windVelocity = 5.0f;

        private double sweptArea = 0.0;
        private double windPower = 0.0;

        // Power (W) = 1/2 * p * A * v^3
        //  where...
        //   W = Watts
        //   p = air density (kg/m^3)
        //   A = cross-sectional area of wind in m^2
        //   v = velocity of wind in m/s

        protected override void InitMessageHandler()
        {
            this.sweptArea = PI * Math.Pow(this.turbineDiameterInMeters / 2.0, 2.0);
        }

    }
}
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

using TMPro;

using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class FluidStorageSystemDashboardHandler : BaseAsyncDataMessageProcessor
    {
        [SerializeField]
        private GameObject fluidLevelDisplay = null;

        [SerializeField]
        private GameObject fluidCapacityDisplay = null;

        [SerializeField]
        private GameObject fluidTempDisplay = null;

        [SerializeField]
        private GameObject fluidFlowRateDisplay = null;

        [SerializeField]
        private GameObject filterLifeDisplay = null;

        private TMP_Text fluidLevelLog = null;
        private TMP_Text fluidCapacityLog = null;
        private TMP_Text fluidTempLog = null;
        private TMP_Text fluidFlowRateLog = null;
        private TMP_Text filterLifeLog = null;


        // protected

        protected override void InitMessageHandler()
        {
            try
            {
                this.fluidLevelLog = this.fluidLevelDisplay.GetComponent<TextMeshProUGUI>();
                this.fluidCapacityLog = this.fluidCapacityDisplay.GetComponent<TextMeshProUGUI>();
                this.fluidTempLog = this.fluidTempDisplay.GetComponent<TextMeshProUGUI>();
                this.fluidFlowRateLog = this.fluidFlowRateDisplay.GetComponent<TextMeshProUGUI>();
                this.filterLifeLog = this.filterLifeDisplay.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize fluid storage display text. Continuing without display data.");
            }
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            // nothing to do
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            // nothing to do
        }

        protected override void ProcessMessageData(MessageData data)
        {
            // nothing to do
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null)
            {
                int typeID = data.GetDeviceType();

                /*
                if (typeID == ConfigConst.TEMP_SENSOR_TYPE && this.envCurTemperatureLog != null)
                {
                    double cookedVal = Math.Round(data.GetValue(), 1);
                    this.envCurTemperatureLog.text = cookedVal.ToString();
                }

                if (typeID == ConfigConst.HUMIDITY_SENSOR_TYPE && this.envCurHumidityLog != null)
                {
                    double cookedVal = Math.Round(data.GetValue(), 1);
                    this.envCurHumidityLog.text = cookedVal.ToString();
                }

                if (typeID == ConfigConst.PRESSURE_SENSOR_TYPE && this.envCurPressureLog != null)
                {
                    double cookedVal = Math.Round(data.GetValue(), 1);
                    this.envCurPressureLog.text = cookedVal.ToString();
                }
                */
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // nothing to do
        }

    }
}

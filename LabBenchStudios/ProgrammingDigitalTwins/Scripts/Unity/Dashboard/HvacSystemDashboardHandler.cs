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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class HvacSystemDashboardHandler : BaseAsyncDataMessageProcessor
    {
        [SerializeField]
        private GameObject envCurTemperatureDisplay = null;

        [SerializeField]
        private GameObject envNewTemperatureDisplay = null;

        [SerializeField]
        private GameObject envCurHumidityDisplay = null;

        [SerializeField]
        private GameObject envCurPressureDisplay = null;

        private TMP_Text envCurTemperatureLog = null;
        private TMP_Text envNewTemperatureLog = null;
        private TMP_Text envCurHumidityLog = null;
        private TMP_Text envCurPressureLog = null;


        // protected

        protected override void InitMessageHandler()
        {
            try
            {
                this.envCurTemperatureLog = this.envCurTemperatureDisplay.GetComponent<TextMeshProUGUI>();
                this.envNewTemperatureLog = this.envNewTemperatureDisplay.GetComponent<TextMeshProUGUI>();
                this.envCurHumidityLog = this.envCurHumidityDisplay.GetComponent<TextMeshProUGUI>();
                this.envCurPressureLog = this.envCurPressureDisplay.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize env data display text. Continuing without display data.");
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
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // nothing to do
        }

    }
}

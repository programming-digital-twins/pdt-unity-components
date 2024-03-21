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
using LabBenchStudios.Pdt.Model;

using LabBenchStudios.Pdt.Unity.Common;
using System.Runtime.InteropServices.WindowsRuntime;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class PowerSystemDashboardHandler : BaseAsyncDataMessageProcessor, IDataContextExtendedListener
    {
        [SerializeField]
        private GameObject operationalStateDisplay = null;

        [SerializeField]
        private GameObject voltageOutputDisplay = null;

        [SerializeField]
        private GameObject wattageOutputDisplay = null;

        [SerializeField]
        private GameObject generatorTempDisplay = null;

        [SerializeField]
        private GameObject generatorDutyCycleInHoursDisplay = null;

        [SerializeField]
        private GameObject windSpeedDisplay = null;

        [SerializeField]
        private GameObject windTurbineRpmDisplay = null;

        private TMP_Text operationalStateLog = null;
        private TMP_Text voltageOutputLog = null;
        private TMP_Text wattageOutputLog = null;
        private TMP_Text generatorTempLog = null;
        private TMP_Text dutyCycleLog = null;
        private TMP_Text windSpeedLog = null;
        private TMP_Text windTurbineRpmLog = null;

        private IDigitalTwinStateProcessor windTurbineStateProcesor = null;

        private ThresholdCrossingContainer thresholdCrossingContainer = null;

        private float curOutputPower = 0.0f;
        private float curWindSpeed = 0.0f;
        private float curWindTurbineRpm = 0.0f;

        // public methods

        public ThresholdCrossingContainer GetThresholdCrossingContainer()
        {
            return this.thresholdCrossingContainer;
        }

        public void SetDigitalTwinStateProcessor(IDigitalTwinStateProcessor dtStateProcessor)
        {
            if (dtStateProcessor != null)
            {
                this.windTurbineStateProcesor = dtStateProcessor;
            }
        }


        // protected

        protected override void InitMessageHandler()
        {
            this.thresholdCrossingContainer = new ThresholdCrossingContainer();

            try
            {
                this.operationalStateLog = this.operationalStateDisplay?.GetComponent<TextMeshProUGUI>();
                this.voltageOutputLog = this.voltageOutputDisplay?.GetComponent<TextMeshProUGUI>();
                this.wattageOutputLog = this.wattageOutputDisplay?.GetComponent<TextMeshProUGUI>();
                this.generatorTempLog = this.generatorTempDisplay?.GetComponent<TextMeshProUGUI>();
                this.dutyCycleLog = this.generatorDutyCycleInHoursDisplay?.GetComponent<TextMeshProUGUI>();
                this.windSpeedLog = this.windSpeedDisplay?.GetComponent<TextMeshProUGUI>();
                this.windTurbineRpmLog = this.windTurbineRpmDisplay?.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize power data display text. Continuing without display data.");
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

                Debug.Log($"Processing incoming power system SensorData: {data.GetDeviceID()} - {typeID}");

                switch (typeID)
                {
                    case ConfigConst.WIND_TURBINE_POWER_OUTPUT_SENSOR_TYPE:
                        this.curOutputPower = (float) Math.Round(data.GetValue(), 1);
                        break;

                    case ConfigConst.WIND_TURBINE_AIR_SPEED_SENSOR_TYPE:
                        this.curWindSpeed = (float) Math.Round(data.GetValue(), 1);
                        break;

                    case ConfigConst.WIND_TURBINE_ROTATIONAL_SPEED_SENSOR_TYPE:
                        this.curWindTurbineRpm = (float) Math.Round(data.GetValue(), 1);
                        break;

                }

                if (this.voltageOutputLog != null) this.voltageOutputLog.text = "120.0";
                if (this.wattageOutputLog != null) this.wattageOutputLog.text = this.curOutputPower.ToString();
                if (this.windSpeedLog != null) this.windSpeedLog.text = this.curWindSpeed.ToString();
                if (this.windTurbineRpmLog != null) this.windTurbineRpmLog.text = this.curWindTurbineRpm.ToString();
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // nothing to do
        }

    }
}

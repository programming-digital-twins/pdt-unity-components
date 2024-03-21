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

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class HvacSystemDashboardHandler : BaseAsyncDataMessageProcessor, IDataContextExtendedListener
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

        private float requestedTemp = 0.0f;

        private float curTemp = 0.0f;
        private float curHumidity = 0.0f;
        private float curPressure = 0.0f;

        private IDigitalTwinStateProcessor thermostatStateProcessor = null;
        private IDigitalTwinStateProcessor humidifierStateProcessor = null;
        private IDigitalTwinStateProcessor barometerStateProcessor  = null;

        private ThresholdCrossingContainer thresholdCrossingContainer = null;

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ThresholdCrossingContainer GetThresholdCrossingContainer()
        {
            return this.thresholdCrossingContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtStateProcessor"></param>
        public void SetDigitalTwinStateProcessor(IDigitalTwinStateProcessor dtStateProcessor)
        {
            if (dtStateProcessor != null)
            {
                switch (dtStateProcessor.GetModelControllerID())
                {
                    case ModelNameUtil.DtmiControllerEnum.Barometer:
                        // set barometer state processor ref
                        this.barometerStateProcessor = dtStateProcessor;

                        // create humidifier state processor
                        this.humidifierStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.barometerStateProcessor,
                                this.humidifierStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Humidifier);

                        // create thermostat state processor
                        this.thermostatStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.barometerStateProcessor,
                                this.thermostatStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Thermostat);

                        break;

                    case ModelNameUtil.DtmiControllerEnum.Humidifier:
                        // set humidifier state processor ref
                        this.humidifierStateProcessor = dtStateProcessor;

                        // create barometer state processor
                        this.barometerStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.humidifierStateProcessor,
                                this.barometerStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Barometer);

                        // create thermostat state processor
                        this.thermostatStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.humidifierStateProcessor,
                                this.thermostatStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Thermostat);

                        break;

                    case ModelNameUtil.DtmiControllerEnum.Thermostat:
                        // set thermostat state processor ref
                        this.thermostatStateProcessor = dtStateProcessor;

                        // create barometer state processor
                        this.barometerStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.thermostatStateProcessor,
                                this.barometerStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Barometer);

                        // create humidifier state processor
                        this.humidifierStateProcessor =
                            base.CreateOrUpdateDigitalTwinStateProcessor(
                                this.thermostatStateProcessor,
                                this.humidifierStateProcessor,
                                ModelNameUtil.DtmiControllerEnum.Humidifier);

                        break;
                }
            }
        }

        // protected methods

        protected override void InitMessageHandler()
        {
            this.thresholdCrossingContainer = new ThresholdCrossingContainer();

            try
            {
                this.envCurTemperatureLog = this.envCurTemperatureDisplay?.GetComponent<TextMeshProUGUI>();
                this.envNewTemperatureLog = this.envNewTemperatureDisplay?.GetComponent<TextMeshProUGUI>();
                this.envCurHumidityLog    = this.envCurHumidityDisplay?.GetComponent<TextMeshProUGUI>();
                this.envCurPressureLog    = this.envCurPressureDisplay?.GetComponent<TextMeshProUGUI>();
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

                Debug.Log($"Processing incoming thermostat SensorData: {data.GetDeviceID()} - {typeID}");

                switch (typeID)
                {
                    case ConfigConst.TEMP_SENSOR_TYPE:
                        this.curTemp = (float) Math.Round(data.GetValue(), 1);
                        break;

                    case ConfigConst.HUMIDITY_SENSOR_TYPE:
                        this.curHumidity = (float) Math.Round(data.GetValue(), 1);
                        break;

                    case ConfigConst.PRESSURE_SENSOR_TYPE:
                        this.curPressure = (float) Math.Round(data.GetValue(), 1);
                        break;

                }

                if (this.envCurTemperatureLog != null) this.envCurTemperatureLog.text = this.curTemp.ToString();
                if (this.envCurHumidityLog != null) this.envCurHumidityLog.text = this.curHumidity.ToString();
                if (this.envCurPressureLog != null) this.envCurPressureLog.text = this.curPressure.ToString();
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // nothing to do
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adjustedVal"></param>
        private void GenerateAndSendActuationEvent(IDigitalTwinStateProcessor dtStateProcessor, float adjustedVal)
        {
            if (dtStateProcessor != null)
            {
                ActuatorData data = new();

                // note: recipient should be able to auto shut-off
                // hvac once desired temp is reached - it should not
                // have to rely on a follow up command from a remote
                // system (hosted within the DTA)
                data.SetName(ConfigConst.ACTUATOR_CMD);
                data.SetDeviceID(dtStateProcessor.GetDeviceID());
                data.SetTypeCategoryID(ConfigConst.ENV_TYPE_CATEGORY);
                data.SetTypeID(ConfigConst.HVAC_ACTUATOR_TYPE);
                data.SetCommand(ConfigConst.COMMAND_ON);
                data.SetValue(adjustedVal);

                // state processor will ensure the target device is set
                ResourceNameContainer resource =
                    dtStateProcessor.GenerateOutgoingStateUpdate(data);

                EventProcessor.GetInstance().ProcessStateUpdateToPhysicalThing(resource);
            }
            else
            {
                Debug.LogError("No Digital Twin State Processor set. Ignoring actuation event.");
            }
        }

    }

}

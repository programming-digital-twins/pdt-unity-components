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
using UnityEngine.UI;

using TMPro;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;

using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class DigitalTwinStateHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private int modelVersion = 1;

        [SerializeField]
        private ModelConst.DtmiControllerEnum modelType = ModelConst.DtmiControllerEnum.Custom;

        [SerializeField]
        private GameObject dtmiHudPanel = null;

        [SerializeField]
        private GameObject dtmiHudPanelModelIDObject = null;

        [SerializeField]
        private GameObject dtmiHudPanelModelNameObject = null;

        [SerializeField]
        private GameObject dtmiHudPanelContentObject = null;

        [SerializeField]
        private GameObject dtmiHudPanelCloseButtonObject = null;

        [SerializeField]
        private GameObject dtmiHudPanelShowButtonObject = null;

        [SerializeField]
        private GameObject deviceTwinUpdatePropertiesButtonObject = null;

        [SerializeField]
        private GameObject devicePropertiesContentObject = null;

        [SerializeField]
        private GameObject deviceModelNameObject = null;

        [SerializeField]
        private GameObject deviceModelIDObject = null;

        [SerializeField]
        private GameObject deviceConnectionStateLabelObject = null;

        [SerializeField]
        private GameObject deviceStateImageObject = null;

        [SerializeField]
        private GameObject deviceStateContentObject = null;

        [SerializeField]
        private GameObject deviceStateStopButtonObject = null;

        [SerializeField]
        private GameObject deviceStateStartButtonObject = null;

        [SerializeField]
        private GameObject deviceStateUpdateButtonObject = null;

        [SerializeField]
        private GameObject deviceStatePauseTelemetryButtonObject = null;

        [SerializeField]
        private GameObject deviceStatusPanelCloseButtonObject = null;

        private TMP_Text deviceConnStateLabelText = null;
        private TMP_Text device = null;
        private TMP_Text deviceModelID = null;
        private TMP_Text deviceModelName = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathEntryText = null;

        private TMP_Text dtmiHudPanelModelID = null;
        private TMP_Text dtmiHudPanelModelName = null;

        private Image deviceStateLabelImage = null;

        private Button startDeviceButton = null;
        private Button stopDeviceButton = null;
        private Button showDtmiHudPanelButton = null;
        private Button closeDtmiHudPanelButton = null;
        private Button updateDeviceButton = null;
        private Button updateTwinPropertiesButton = null;
        private Button pauseDeviceTelemetryButton = null;
        private Button closeMgmtDialogButton = null;

        private bool hasDtmiHudPanel = false;
        private bool hasDtmiModelJsonContainer = false;
        private bool hasDevicePropertiesContainer = false;
        private bool hasDeviceTelemetryContainer = false;

        private string dtmiURI  = ModelConst.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelConst.IOT_MODEL_CONTEXT_NAME;

        // public methods (button interactions)

        public void HideDtmiHudPanel()
        {
            if (this.hasDtmiHudPanel)
            {
                this.dtmiHudPanel.SetActive(false);
            }
        }

        public void ShowDtmiHudPanel()
        {
            if (this.hasDtmiHudPanel)
            {
                this.dtmiHudPanel.SetActive(true);
            }
        }

        public void UpdateTwinProperties()
        {
            // TODO: implement this
        }

        public void StartPhysicalDevice()
        {
            // TODO: implement this
        }

        public void StopPhysicalDevice()
        {
            // TODO: implement this
        }

        public void UpdatePhysicalDevice()
        {
            // TODO: implement this
        }

        public void PauseDeviceTelemetry()
        {
            // TODO: implement this
        }

        // public callback methods

        public void LogDebugMessage(string message)
        {
            base.HandleDebugLogMessage(message);
        }

        public void LogErrorMessage(string message, Exception ex)
        {
            base.HandleErrorLogMessage(message);
        }

        public void LogWarningMessage(string message)
        {
            base.HandleWarningLogMessage(message);
        }

        public void OnMessagingSystemDataReceived(ActuatorData data)
        {
            base.HandleActuatorData(data);
        }

        public void OnMessagingSystemDataReceived(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }

        public void OnMessagingSystemDataReceived(SensorData data)
        {
            base.HandleSensorData(data);
        }

        public void OnMessagingSystemDataReceived(SystemPerformanceData data)
        {
            base.HandleSystemPerformanceData(data);
        }

        public void OnMessagingSystemDataSent(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }

        public void OnMessagingSystemStatusUpdate(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }


        // protected

        protected override void InitMessageHandler()
        {
            try
            {
                this.dtmiURI  = ModelConst.CreateModelID(this.modelType, this.modelVersion);
                this.dtmiName = ModelConst.GetNameFromDtmiURI(this.dtmiURI);

                //this.gameObject.SetActive(false);

                if (this.dtmiHudPanel != null)
                {
                    this.hasDtmiHudPanel = true;
                    //this.dtmiHudPanel.SetActive(false);
                }

                if (this.dtmiHudPanelCloseButtonObject != null)
                {
                    this.closeDtmiHudPanelButton = this.dtmiHudPanelCloseButtonObject.GetComponent<Button>();
                    
                    if (this.closeDtmiHudPanelButton != null)
                    {
                        this.closeDtmiHudPanelButton.onClick.AddListener(() => HideDtmiHudPanel());
                    }    
                }

                if (this.dtmiHudPanelShowButtonObject != null)
                {
                    this.showDtmiHudPanelButton = this.dtmiHudPanelShowButtonObject.GetComponent<Button>();

                    if (this.showDtmiHudPanelButton != null)
                    {
                        this.showDtmiHudPanelButton.onClick.AddListener(() => ShowDtmiHudPanel());
                    }
                }

                if (this.deviceConnectionStateLabelObject != null)
                {
                    this.deviceConnStateLabelText = this.deviceConnectionStateLabelObject.GetComponent<TextMeshProUGUI>();
                }

                if (this.deviceStateImageObject != null)
                {
                    this.deviceStateLabelImage = this.deviceStateImageObject.GetComponent<Image>();
                }

                if (this.deviceModelNameObject != null)
                {
                    this.deviceModelName = this.deviceModelNameObject.GetComponent<TextMeshProUGUI>();
                    this.deviceModelName.text = this.dtmiName;
                }

                if (this.deviceModelIDObject != null)
                {
                    this.deviceModelID = this.deviceModelIDObject.GetComponent<TextMeshProUGUI>();
                    this.deviceModelID.text = this.dtmiURI;
                }

                if (this.dtmiHudPanelModelNameObject != null)
                {
                    this.dtmiHudPanelModelName = this.dtmiHudPanelModelNameObject.GetComponent<TextMeshProUGUI>();
                    this.dtmiHudPanelModelName.text = this.dtmiName;
                }

                if (this.dtmiHudPanelModelIDObject != null)
                {
                    this.dtmiHudPanelModelID = this.dtmiHudPanelModelIDObject.GetComponent<TextMeshProUGUI>();
                    this.dtmiHudPanelModelID.text = this.dtmiURI;
                }

                if (this.devicePropertiesContentObject != null)
                {
                    this.hasDevicePropertiesContainer = true;
                }

                if (this.deviceStateContentObject != null)
                {
                    this.hasDeviceTelemetryContainer = true;
                }

                if (this.dtmiHudPanelContentObject != null)
                {
                    this.hasDtmiModelJsonContainer = true;
                }

                if (this.deviceTwinUpdatePropertiesButtonObject != null)
                {
                    this.updateTwinPropertiesButton = this.deviceTwinUpdatePropertiesButtonObject.GetComponent<Button>();

                    if (this.updateTwinPropertiesButton != null)
                    {
                        this.updateTwinPropertiesButton.onClick.AddListener(() => this.UpdateTwinProperties());
                    }
                }

                if (this.deviceStateStartButtonObject != null)
                {
                    this.startDeviceButton = this.deviceStateStartButtonObject.GetComponent<Button>();

                    if (this.startDeviceButton != null)
                    {
                        this.startDeviceButton.onClick.AddListener(() => this.StartPhysicalDevice());
                    }
                }

                if (this.deviceStateStopButtonObject != null)
                {
                    this.stopDeviceButton = this.deviceStateStopButtonObject.GetComponent<Button>();

                    if (this.stopDeviceButton != null)
                    {
                        this.stopDeviceButton.onClick.AddListener(() => this.StopPhysicalDevice());
                    }
                }

                if (this.deviceStateUpdateButtonObject != null)
                {
                    this.updateDeviceButton = this.deviceStateUpdateButtonObject.GetComponent<Button>();

                    if (this.updateDeviceButton != null)
                    {
                        this.updateDeviceButton.onClick.AddListener(() => this.UpdatePhysicalDevice());
                    }
                }

                if (this.deviceStatePauseTelemetryButtonObject != null)
                {
                    this.pauseDeviceTelemetryButton = this.deviceStatePauseTelemetryButtonObject.GetComponent<Button>();

                    if (this.pauseDeviceTelemetryButton != null)
                    {
                        this.pauseDeviceTelemetryButton.onClick.AddListener(() => this.PauseDeviceTelemetry());
                    }
                }

                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize digital twin HUD. Continuing without display data: {ex.StackTrace}");
            }
        }

        protected new void ProcessDebugLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        protected new void ProcessWarningLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        protected new void ProcessErrorLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            if (data != null)
            {
                String jsonData = DataUtil.ActuatorDataToJson(data);
            }
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (data != null)
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.deviceConnStateLabelText != null) this.deviceConnStateLabelText.text = connStateMsg;
            }
        }

        protected override void ProcessMessageData(MessageData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

    }
}

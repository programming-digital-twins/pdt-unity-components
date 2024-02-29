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
using System.Collections.Generic;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class DigitalTwinStateHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private int modelVersion = 1;

        [SerializeField]
        private ModelNameUtil.DtmiControllerEnum modelType = ModelNameUtil.DtmiControllerEnum.Custom;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject refreshDeviceIDButtonObject = null;

        [SerializeField]
        private GameObject deviceIDSelectorObject = null;

        [SerializeField]
        private GameObject modelPanel = null;

        [SerializeField]
        private GameObject modelPanelIDObject = null;

        [SerializeField]
        private GameObject modelPanelNameObject = null;

        [SerializeField]
        private GameObject modelPanelContentObject = null;

        [SerializeField]
        private GameObject modelPanelCloseButtonObject = null;

        [SerializeField]
        private GameObject modelPanelShowButtonObject = null;

        [SerializeField]
        private GameObject statusPanel = null;

        [SerializeField]
        private GameObject statusPanelUpdatePropsButtonObject = null;

        [SerializeField]
        private GameObject statusPanelPropsContentObject = null;

        [SerializeField]
        private GameObject statusPanelNameObject = null;

        [SerializeField]
        private GameObject statusPanelIDObject = null;

        [SerializeField]
        private GameObject statusPanelConnStateLabelObject = null;

        [SerializeField]
        private GameObject statusPanelStateImageObject = null;

        [SerializeField]
        private GameObject statusPanelContentObject = null;

        [SerializeField]
        private GameObject statusPanelStopDeviceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelStartDeviceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelUpdateDeviceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelPauseDeviceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelCloseButtonObject = null;

        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text device = null;
        private TMP_Text statusPanelID = null;
        private TMP_Text statusPanelName = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathEntryText = null;

        private TMP_Text modelPanelID = null;
        private TMP_Text modelPanelName = null;
        private TMP_Text modelContentText = null;

        private TMP_Dropdown deviceIDSelector = null;

        private Image statusPanelStateImage = null;

        private Button refreshDeviceIDButton = null;
        private Button startDeviceButton = null;
        private Button stopDeviceButton = null;
        private Button updateDeviceButton = null;
        private Button pauseDeviceButton = null;
        private Button showModelPanelButton = null;
        private Button closeModelPanelButton = null;
        private Button closeStatusPanelButton = null;
        private Button updateTwinPropertiesButton = null;

        private bool hasModelPanel = false;
        private bool hasModelPanelJsonContainer = false;
        private bool isModelPanelActive = false;

        private bool hasStatusPanel = false;
        private bool hasStatusPanelPropsContainer = false;
        private bool hasStatusPanelTelemetryContainer = false;

        private string dtmiURI  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        private string deviceID = ConfigConst.NOT_SET;

        private DigitalTwinModelState digitalTwinModelState = null;


        // public methods (button interactions)

        public void CloseStatusPanel()
        {
            if (this.hasStatusPanel)
            {
                this.statusPanel.SetActive(false);
            }
        }

        public void CloseModelPanel()
        {
            if (this.hasModelPanel)
            {
                this.modelPanel.SetActive(false);
            }
        }

        public void OnDeviceIDSelected()
        {
            if (this.deviceIDSelector != null)
            {
                this.deviceID = this.deviceIDSelector.captionText.text;
                //int selectionIndex = this.deviceIDSelector.value;
                //this.deviceID = this.deviceIDSelector.options[selectionIndex].text;
            }
            
            this.deviceIDText.text = this.deviceID;

            // should already be created by now - if not, the deviceID
            // will be applied as soon as the model manager creates the
            // referential state object
            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.SetConnectedDeviceID(this.deviceID);
            }

            // update connection state - by the time we process the device ID in this
            // UI component, the target remote device may have already sent it's
            // connection state update, so we need to check if the local cache
            // has one we can process
            this.UpdateConnectionState();
        }

        public void UpdateDeviceIDList()
        {
            if (this.deviceIDSelector != null)
            {
                List<string> deviceIDList = EventProcessor.GetInstance().GetAllKnownDeviceIDs();

                if (deviceIDList != null && deviceIDList.Count > 0)
                {
                    this.deviceIDSelector.ClearOptions();
                    this.deviceIDSelector.AddOptions(deviceIDList);
                }
            }
        }

        public void UpdateModelPanelVisibility()
        {
            if (this.hasModelPanel)
            {
                this.isModelPanelActive = ! this.isModelPanelActive;
                this.modelPanel.SetActive(this.isModelPanelActive);
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

        public void OnModelUpdateEvent()
        {
            EventProcessor eventProcessor = EventProcessor.GetInstance();
            DigitalTwinModelManager dtModelManager = eventProcessor.GetDigitalTwinModelManager();

            if (dtModelManager != null)
            {
                string rawModelJson = dtModelManager.GetRawModelJson(this.modelType);

                this.digitalTwinModelState =
                    dtModelManager.CreateModelState(
                        this.modelType, (IDataContextEventListener)this, rawModelJson);

                this.digitalTwinModelState.SetConnectedDeviceID(this.deviceID);

                this.modelContentText.text = rawModelJson;

                Debug.Log($"Created model state with URI {this.dtmiURI} and name {this.dtmiName}");
                Debug.Log($"Raw DTDL JSON\n==========\n{rawModelJson}\n==========\n");
            }
        }


        // protected

        private void InitStatusPanelControls()
        {
            if (this.statusPanel != null )
            {
                this.hasStatusPanel = true;
            }

            if (this.deviceIDObject != null)
            {
                this.deviceIDText = this.deviceIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.deviceIDSelectorObject != null)
            {
                this.deviceIDSelector = this.deviceIDSelectorObject.GetComponent<TMP_Dropdown>();

                this.deviceIDSelector.onValueChanged.AddListener(
                    delegate { this.OnDeviceIDSelected(); }
                );

                // fire the listener to set deviceID and sync label
                //this.UpdateDeviceIDList();

                //this.deviceIDSelector.Select();
                //this.OnDeviceIDSelected();
            }

            if (this.statusPanelConnStateLabelObject != null)
            {
                this.connStateLabelText = this.statusPanelConnStateLabelObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelStateImageObject != null)
            {
                this.statusPanelStateImage = this.statusPanelStateImageObject.GetComponent<Image>();
            }

            if (this.statusPanelNameObject != null)
            {
                this.statusPanelName = this.statusPanelNameObject.GetComponent<TextMeshProUGUI>();
                this.statusPanelName.text = this.dtmiName;
            }

            if (this.statusPanelIDObject != null)
            {
                this.statusPanelID = this.statusPanelIDObject.GetComponent<TextMeshProUGUI>();
                this.statusPanelID.text = this.dtmiURI;
            }

            if (this.statusPanelPropsContentObject != null)
            {
                this.hasStatusPanelPropsContainer = true;
            }

            if (this.statusPanelContentObject != null)
            {
                this.hasStatusPanelTelemetryContainer = true;
            }

            // init buttons
            if (this.refreshDeviceIDButtonObject != null)
            {
                this.refreshDeviceIDButton = this.refreshDeviceIDButtonObject.GetComponent<Button>();

                if (this.refreshDeviceIDButton != null)
                {
                    this.refreshDeviceIDButton.onClick.AddListener(() => this.UpdateDeviceIDList());
                }
            }

            if (this.statusPanelCloseButtonObject != null)
            {
                this.closeStatusPanelButton = this.statusPanelCloseButtonObject.GetComponent<Button>();

                if (this.closeStatusPanelButton != null)
                {
                    this.closeStatusPanelButton.onClick.AddListener(() => this.CloseStatusPanel());
                }
            }

            if (this.statusPanelUpdatePropsButtonObject != null)
            {
                this.updateTwinPropertiesButton = this.statusPanelUpdatePropsButtonObject.GetComponent<Button>();

                if (this.updateTwinPropertiesButton != null)
                {
                    this.updateTwinPropertiesButton.onClick.AddListener(() => this.UpdateTwinProperties());
                }
            }

            if (this.statusPanelStartDeviceButtonObject != null)
            {
                this.startDeviceButton = this.statusPanelStartDeviceButtonObject.GetComponent<Button>();

                if (this.startDeviceButton != null)
                {
                    this.startDeviceButton.onClick.AddListener(() => this.StartPhysicalDevice());
                }
            }

            if (this.statusPanelStopDeviceButtonObject != null)
            {
                this.stopDeviceButton = this.statusPanelStopDeviceButtonObject.GetComponent<Button>();

                if (this.stopDeviceButton != null)
                {
                    this.stopDeviceButton.onClick.AddListener(() => this.StopPhysicalDevice());
                }
            }

            if (this.statusPanelUpdateDeviceButtonObject != null)
            {
                this.updateDeviceButton = this.statusPanelUpdateDeviceButtonObject.GetComponent<Button>();

                if (this.updateDeviceButton != null)
                {
                    this.updateDeviceButton.onClick.AddListener(() => this.UpdatePhysicalDevice());
                }
            }

            if (this.statusPanelPauseDeviceButtonObject != null)
            {
                this.pauseDeviceButton = this.statusPanelPauseDeviceButtonObject.GetComponent<Button>();

                if (this.pauseDeviceButton != null)
                {
                    this.pauseDeviceButton.onClick.AddListener(() => this.PauseDeviceTelemetry());
                }
            }
        }

        private void InitModelPanelControls()
        {
            if (this.modelPanel != null)
            {
                this.hasModelPanel = true;
                this.modelPanel.SetActive(false);
            }

            if (this.modelPanelNameObject != null)
            {
                this.modelPanelName = this.modelPanelNameObject.GetComponent<TextMeshProUGUI>();
                this.modelPanelName.text = this.dtmiName;
            }

            if (this.modelPanelIDObject != null)
            {
                this.modelPanelID = this.modelPanelIDObject.GetComponent<TextMeshProUGUI>();
                this.modelPanelID.text = this.dtmiURI;
            }

            if (this.modelPanelContentObject != null)
            {
                this.modelContentText = this.modelPanelContentObject.GetComponent<TextMeshProUGUI>();

                if (this.modelContentText != null) this.hasModelPanelJsonContainer = true;
            }

            // init buttons
            if (this.modelPanelCloseButtonObject != null)
            {
                this.closeModelPanelButton = this.modelPanelCloseButtonObject.GetComponent<Button>();

                if (this.closeModelPanelButton != null)
                {
                    this.closeModelPanelButton.onClick.AddListener(() => this.CloseModelPanel());
                }
            }

            if (this.modelPanelShowButtonObject != null)
            {
                this.showModelPanelButton = this.modelPanelShowButtonObject.GetComponent<Button>();

                if (this.showModelPanelButton != null)
                {
                    this.showModelPanelButton.onClick.AddListener(() => this.UpdateModelPanelVisibility());
                }
            }
        }

        protected override void InitMessageHandler()
        {
            try
            {
                // set DTMI labels first
                this.dtmiURI  = ModelNameUtil.CreateModelID(this.modelType, this.modelVersion);
                this.dtmiName = ModelNameUtil.GetNameFromDtmiURI(this.dtmiURI);

                // init controls second
                this.InitStatusPanelControls();
                this.InitModelPanelControls();

                // update device ID list third
                this.UpdateDeviceIDList();

                // register for events fourth
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
            if (this.IsMyMessage(data))
            {
                String jsonData = DataUtil.ActuatorDataToJson(data);
            }
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            this.UpdateDeviceIDList();

            if (this.IsMyMessage(data))
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.connStateLabelText != null) this.connStateLabelText.text = connStateMsg;
            }
        }

        protected override void ProcessMessageData(MessageData data)
        {
            if (this.IsMyMessage(data))
            {
                // nothing to do
            }
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (this.IsMyMessage(data))
            {
                // nothing to do
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.IsMyMessage(data))
            {
                // nothing to do
            }
        }

        // private methods

        private bool IsMyMessage(IotDataContext data)
        {
            return (data != null && data.GetDeviceID().Equals(this.deviceID));
        }

        private void UpdateConnectionState()
        {
            ConnectionStateData connStateData = EventProcessor.GetInstance().GetConnectionState(this.deviceID);

            if (connStateData != null)
            {
                this.OnMessagingSystemStatusUpdate(connStateData);
            }
            else
            {
                Debug.LogWarning($"No cached connection state for {this.deviceID}");
            }
        }

    }
}

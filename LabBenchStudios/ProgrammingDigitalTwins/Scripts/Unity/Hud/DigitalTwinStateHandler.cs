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
using Unity.VisualScripting;
using System.Text;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class DigitalTwinStateHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private int modelVersion = 1;

        [SerializeField]
        private ModelNameUtil.DtmiControllerEnum controllerID = ModelNameUtil.DtmiControllerEnum.Custom;

        [SerializeField]
        private bool useGuidInInstanceKey = false;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject provisionDeviceTwinButtonObject = null;

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
        private GameObject statusPanelStateContentObject = null;

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
        private TMP_Text statusContentText = null;
        private TMP_Text propsContentText = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathEntryText = null;

        private TMP_Text modelPanelID = null;
        private TMP_Text modelPanelName = null;
        private TMP_Text modelContentText = null;

        private TMP_Dropdown deviceIDSelector = null;

        private Image statusPanelStateImage = null;

        private Button provisionDeviceTwinButton = null;

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

        private string dtmiUri  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;

        private string modelProps = "";
        private string modelTelemetries = "";

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
                this.locationID = this.deviceID;
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

            // allow the twin to be provisioned
            if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = true;

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
            this.UpdateModelRawData();
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
                this.statusPanelID.text = this.dtmiUri;
            }

            if (this.statusPanelPropsContentObject != null)
            {
                this.hasStatusPanelPropsContainer = true;

                this.propsContentText = this.statusPanelPropsContentObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelStateContentObject != null)
            {
                this.hasStatusPanelTelemetryContainer = true;

                this.statusContentText = this.statusPanelStateContentObject.GetComponent<TextMeshProUGUI>();
            }

            // init buttons
            if (this.provisionDeviceTwinButtonObject != null)
            {
                this.provisionDeviceTwinButton = this.provisionDeviceTwinButtonObject.GetComponent<Button>();

                if (this.provisionDeviceTwinButton != null)
                {
                    if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = false;
                    this.provisionDeviceTwinButton.onClick.AddListener(() => this.ProvisionModelState());
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
                this.modelPanelID.text = this.dtmiUri;
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

            // pull in any loaded DTDL data - this can be mapped via the
            // pre-provisioned ID that maps to the DTMI for the asset
            this.UpdateModelRawData();
        }

        protected override void InitMessageHandler()
        {
            try
            {
                // set DTMI labels first
                this.dtmiUri  = ModelNameUtil.CreateModelID(this.controllerID, this.modelVersion);
                this.dtmiName = ModelNameUtil.GetNameFromDtmiURI(this.dtmiUri);

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
                StringBuilder sb = new StringBuilder(this.modelTelemetries);

                sb.Append("\n==========");
                sb.Append($"\nSensor Name: {data.GetName()}");
                sb.Append($"\nSensor Value: {data.GetValue()}");
                sb.Append("\n==========");
                sb.Append($"\nIncoming Key: {ModelNameUtil.GenerateDataSyncKey(data)}");
                sb.Append($"\nData Sync Key: {this.digitalTwinModelState.GetDataSyncKey()}");
                sb.Append($"\nModel Sync Key: {this.digitalTwinModelState.GetModelSyncKey()}");

                this.statusContentText.text = sb.ToString();
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.IsMyMessage(data))
            {
                StringBuilder sb = new StringBuilder(this.modelTelemetries);

                sb.Append("\n==========");
                sb.Append($"\nCPU Util: {data.GetCpuUtilization()}");
                sb.Append($"\nMem Util: {data.GetMemoryUtilization()}");
                sb.Append($"\nDisk Util: {data.GetDiskUtilization()}");

                this.statusContentText.text = sb.ToString();
            }
        }

        // private methods

        private bool IsMyMessage(IotDataContext data)
        {
            return true;
            //return (data != null && data.GetDeviceID().Equals(this.deviceID));
        }

        private void ProvisionModelState()
        {
            DigitalTwinModelManager dtModelManager =
                EventProcessor.GetInstance().GetDigitalTwinModelManager();

            if (dtModelManager != null)
            {
                Debug.LogError($"NORMAL: Provisioning device instance with URI {this.dtmiUri} and name {this.dtmiName}");
                this.digitalTwinModelState =
                    dtModelManager.CreateModelState(
                        this.deviceID,
                        this.locationID,
                        this.useGuidInInstanceKey,
                        this.controllerID,
                        (IDataContextEventListener) this);

                this.OnModelUpdateEvent();

                Debug.LogError($"NORMAL: Created model state with URI {this.dtmiUri} and instance {this.digitalTwinModelState.GetModelSyncKey()}");
                Debug.LogError($"NORMAL: Raw DTDL JSON\n==========\n{this.digitalTwinModelState.GetRawModelJson()}\n==========\n");

                // once provisioned, we're done with the provisioning button
                if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = false;
            }
        }

        private void UpdateConnectionState()
        {
            ConnectionStateData connStateData =
                EventProcessor.GetInstance().GetConnectionState(this.deviceID);

            if (connStateData != null)
            {
                this.OnMessagingSystemStatusUpdate(connStateData);
            }
            else
            {
                Debug.LogWarning($"No cached connection state for {this.deviceID}");
            }
        }

        private void UpdateModelRawData()
        {
            // it's possible the model state object hasn't
            // been provisioned yet
            Debug.LogError("NORMAL: Updating JSON raw model data...");

            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.BuildModelData();

                if (this.modelContentText != null)
                {
                    this.modelContentText.text =
                        this.digitalTwinModelState.GetRawModelJson();
                }

                List<string> propKeys = this.digitalTwinModelState.GetModelPropertyKeys();
                StringBuilder propKeysStr = new StringBuilder();

                foreach (string key in propKeys)
                {
                    propKeysStr.Append(key).Append("\n");
                }

                this.modelProps = propKeysStr.ToString();
                this.propsContentText.text = this.modelProps;

                Debug.LogError(
                    $"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.propsContentText.text}");

                List<string> telemetryKeys = this.digitalTwinModelState.GetModelPropertyTelemetryKeys();
                StringBuilder telemetryKeysStr = new StringBuilder();

                foreach (string key in telemetryKeys)
                {
                    telemetryKeysStr.Append(key).Append("\n");
                }

                this.modelTelemetries = telemetryKeysStr.ToString();
                this.statusContentText.text = this.modelTelemetries;

                Debug.LogError(
                    $"Telemetry Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.statusContentText.text}");
            }
            else
            {
                if (this.modelContentText != null)
                {
                    this.modelContentText.text =
                        EventProcessor.GetInstance().GetDigitalTwinModelManager().GetRawModelJson(this.controllerID);
                }
            }
        }

        private void UpdateTwinProperties()
        {

        }

    }
}

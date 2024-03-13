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
        private GameObject propsPanelShowButtonObject = null;

        [SerializeField]
        private GameObject statusPanelPropsContentObject = null;

        [SerializeField]
        private GameObject statusPanelNameObject = null;

        [SerializeField]
        private GameObject statusPanelIDObject = null;

        [SerializeField]
        private GameObject statusPanelCommandResourceObject = null;

        [SerializeField]
        private GameObject statusPanelConnStateLabelObject = null;

        [SerializeField]
        private GameObject statusPanelStateImageObject = null;

        [SerializeField]
        private GameObject statusPanelStateContentObject = null;

        [SerializeField]
        private GameObject pauseTelemetryButtonObject = null;

        [SerializeField]
        private GameObject resumeTelemetryButtonObject = null;

        [SerializeField]
        private GameObject updateDeviceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelCloseButtonObject = null;

        [SerializeField]
        private GameObject propsEditorPanel = null;

        [SerializeField]
        private GameObject animationListenerContainer = null;

        [SerializeField]
        private GameObject eventListenerContainer = null;

        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text deviceCmdResourceText = null;
        private TMP_Text deviceName = null;
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

        private Button resumeTelemetryButton = null;
        private Button pauseTelemetryButton = null;
        private Button updateDeviceButton = null;
        private Button showModelPanelButton = null;
        private Button closeModelPanelButton = null;
        private Button closeStatusPanelButton = null;
        private Button showPropsPanelButton = null;

        private bool hasModelPanel = false;
        private bool hasModelPanelJsonContainer = false;
        private bool isModelPanelActive = false;

        private bool hasStatusPanel = false;
        private bool hasStatusPanelPropsContainer = false;
        private bool hasStatusPanelTelemetryContainer = false;

        private bool hasPropsEditorPanel = false;
        private bool isPropsEditorPanelActive = false;

        private bool enableIncomingTelemetry = true;

        private string dtmiUri  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;

        private string cmdResourceName =
            ConfigConst.PRODUCT_NAME + "/" + ConfigConst.EDGE_DEVICE + "/" + ConfigConst.ACTUATOR_CMD;

        private string modelProps = "";
        private string modelTelemetries = "";

        private DigitalTwinModelState digitalTwinModelState = null;
        private DigitalTwinPropertiesHandler digitalTwinPropsHandler = null;

        private ResourceNameContainer cmdResource = null;

        private IDataContextEventListener animationListener = null;
        private IDataContextExtendedListener eventListener = null;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void CloseStatusPanel()
        {
            if (this.hasStatusPanel)
            {
                this.statusPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseModelPanel()
        {
            if (this.hasModelPanel)
            {
                this.modelPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePropsEditorPanel()
        {
            if (this.hasPropsEditorPanel)
            {
                this.propsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnDeviceIDSelected()
        {
            if (this.deviceIDSelector != null)
            {
                this.deviceID = this.deviceIDSelector.captionText.text;
                this.locationID = this.deviceID;
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

            // update connection state - by the time we process the deviceName ID in this
            // UI component, the target remote deviceName may have already sent it's
            // connection state update, so we need to check if the local cache
            // has one we can process
            this.UpdateConnectionState();
            this.UpdateCommandResourceName();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void UpdateModelPanelVisibility()
        {
            if (this.hasModelPanel)
            {
                this.isModelPanelActive = ! this.isModelPanelActive;
                this.modelPanel.SetActive(this.isModelPanelActive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePropsEditorPanelVisibility()
        {
            if (this.hasPropsEditorPanel)
            {
                this.isPropsEditorPanelActive = !this.isPropsEditorPanelActive;
                this.propsEditorPanel.SetActive(this.isPropsEditorPanelActive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PauseIncomingTelemetry()
        {
            this.enableIncomingTelemetry = false;

            if (this.pauseTelemetryButton != null) this.pauseTelemetryButton.interactable = false;
            if (this.resumeTelemetryButton != null) this.resumeTelemetryButton.interactable = true;

            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.EnableIncomingTelemetryProcessing(this.enableIncomingTelemetry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResumeIncomingTelemetry()
        {
            this.enableIncomingTelemetry = true;

            if (this.pauseTelemetryButton != null) this.pauseTelemetryButton.interactable = true;
            if (this.resumeTelemetryButton != null) this.resumeTelemetryButton.interactable = false;

            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.EnableIncomingTelemetryProcessing(this.enableIncomingTelemetry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePhysicalDevice()
        {
            // first: set the command resource name text
            //        this is automatically set initially, but the user can
            //        manually override (eventually - future update), so check
            //        the current value and apply it to the locally stored
            //        cmdResourceName string
            if (this.deviceCmdResourceText != null)
            {
                this.cmdResourceName = this.deviceCmdResourceText.text;
            }

            // second: generate the commands to be send the target physical device
            //         this will take any writeable properties and convert any
            //         deltas (from any recent change) into ActuatorData instances
            //         for (eventual) transmission to the target physical device;
            //         for each ActuatorData instance, a ResourceNameContainer will
            //         be created with the cmdResourceName as the target resource and
            //         the ActuatorData as the IotDataContext instance
            List<ResourceNameContainer> deviceCmdResourceList = this.GenerateDeviceCommands();

            // third: send each generated ResourceNameContainer on its way
            //        this will iterate over the device command list and send each
            //        one to the target physical device;
            //        a future update may permit all to be sent in one message
            if (deviceCmdResourceList != null)
            {
                foreach (ResourceNameContainer resource in deviceCmdResourceList)
                {
                    EventProcessor.GetInstance().ProcessStateUpdateToPhysicalThing(resource);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
            this.UpdateModelDataAndProperties();
        }


        // protected

        /// <summary>
        /// 
        /// </summary>
        private void InitPropsEditorPanelControls()
        {
            if (this.propsEditorPanel != null)
            {
                this.digitalTwinPropsHandler = this.propsEditorPanel.GetComponent<DigitalTwinPropertiesHandler>();

                this.hasPropsEditorPanel = true;
                this.propsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

            if (this.statusPanelCommandResourceObject != null)
            {
                this.deviceCmdResourceText = this.statusPanelCommandResourceObject.GetComponent<TextMeshProUGUI>();
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

            if (this.animationListenerContainer != null)
            {
                try
                {
                    this.animationListener = this.animationListenerContainer.GetComponent<IDataContextEventListener>();
                }
                catch (Exception e)
                {
                    this.animationListener = null;

                    Debug.LogError(
                        "Can't find IDataContextEventListener reference in animation listener container GameObject. Ignoring.");
                }
            }

            if (this.eventListenerContainer != null)
            {
                try
                {
                    this.eventListener = this.eventListenerContainer.GetComponent<IDataContextExtendedListener>();
                }
                catch (Exception e)
                {
                    this.eventListener = null;

                    Debug.LogError(
                        "Can't find IDataContextExtendedListener reference in event listener container GameObject. Ignoring.");
                }
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

            if (this.propsPanelShowButtonObject != null)
            {
                this.showPropsPanelButton = this.propsPanelShowButtonObject.GetComponent<Button>();

                if (this.showPropsPanelButton != null)
                {
                    this.showPropsPanelButton.onClick.AddListener(() => this.UpdatePropsEditorPanelVisibility());
                }
            }

            if (this.resumeTelemetryButtonObject != null)
            {
                this.resumeTelemetryButton = this.resumeTelemetryButtonObject.GetComponent<Button>();

                if (this.resumeTelemetryButton != null)
                {
                    this.resumeTelemetryButton.onClick.AddListener(() => this.ResumeIncomingTelemetry());
                }
            }

            if (this.pauseTelemetryButtonObject != null)
            {
                this.pauseTelemetryButton = this.pauseTelemetryButtonObject.GetComponent<Button>();

                if (this.pauseTelemetryButton != null)
                {
                    this.pauseTelemetryButton.onClick.AddListener(() => this.PauseIncomingTelemetry());
                }
            }

            if (this.updateDeviceButtonObject != null)
            {
                this.updateDeviceButton = this.updateDeviceButtonObject.GetComponent<Button>();

                if (this.updateDeviceButton != null)
                {
                    this.updateDeviceButton.onClick.AddListener(() => this.UpdatePhysicalDevice());
                }
            }

            // start in 'resume incoming telemetry processing' state
            this.ResumeIncomingTelemetry();
        }

        /// <summary>
        /// 
        /// </summary>
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
            this.UpdateModelDataAndProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                // first: set DTMI labels
                this.dtmiUri  = ModelNameUtil.CreateModelID(this.controllerID, this.modelVersion);
                this.dtmiName = ModelNameUtil.GetNameFromDtmiURI(this.dtmiUri);

                // second: init individual panels and their control
                this.InitStatusPanelControls();
                this.InitModelPanelControls();
                this.InitPropsEditorPanelControls();

                // third: update deviceName ID list
                this.UpdateDeviceIDList();

                // fourth: update the command resource name
                this.UpdateCommandResourceName();

                // fifth: other init steps (if needed - future)

                // finally: register for events
                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize digital twin HUD. Continuing without display data: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessDebugLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessWarningLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessErrorLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessActuatorData(ActuatorData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                String jsonData = DataUtil.ActuatorDataToJson(data);

                if (this.animationListener != null)
                {
                    this.animationListener.HandleActuatorData(data);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleActuatorData(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            this.UpdateDeviceIDList();

            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.connStateLabelText != null) this.connStateLabelText.text = connStateMsg;

                if (this.animationListener != null)
                {
                    this.animationListener.HandleConnectionStateData(data);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleConnectionStateData(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessMessageData(MessageData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                // nothing to do

                if (this.animationListener != null)
                {
                    this.animationListener.HandleMessageData(data);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleMessageData(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSensorData(SensorData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
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

                if (this.animationListener != null)
                {
                    this.animationListener.HandleSensorData(data);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleSensorData(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                StringBuilder sb = new StringBuilder(this.modelTelemetries);

                sb.Append("\n==========");
                sb.Append($"\nCPU Util: {data.GetCpuUtilization()}");
                sb.Append($"\nMem Util: {data.GetMemoryUtilization()}");
                sb.Append($"\nDisk Util: {data.GetDiskUtilization()}");

                this.statusContentText.text = sb.ToString();

                if (this.animationListener != null)
                {
                    this.animationListener.HandleSystemPerformanceData(data);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleSystemPerformanceData(data);
                }
            }
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ResourceNameContainer> GenerateDeviceCommands()
        {
            // IFF we have any changed properties, create a new list
            List<ResourceNameContainer> deviceCmdResourceList = null;

            // TODO: implement this - the following is just a template
            ActuatorData data = new();
            ResourceNameContainer deviceCmdResource = new ResourceNameContainer(this.cmdResource);
            deviceCmdResource.DataContext = data;
            
            // TODO: for now, list is null
            return deviceCmdResourceList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsIncomingTelemetryProcessingEnabled(IotDataContext data)
        {
            // placeholder for further narrowing of IotDataContext data processing
            // for now, this is moot - the model manager distributes updates to the
            // appropriate model state, which in turn notifies its listener (this)
            // of those updates
            return this.enableIncomingTelemetry;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProvisionModelState()
        {
            DigitalTwinModelManager dtModelManager =
                EventProcessor.GetInstance().GetDigitalTwinModelManager();

            if (dtModelManager != null)
            {
                Debug.Log(
                    $"NORMAL: Provisioning DT model state instance with " +
                    $"\n\tURI = {this.dtmiUri}" +
                    $"\n\tName = {this.dtmiName}" +
                    $"\n\tDevice ID = {this.deviceID}" +
                    $"\n\tLocation ID = {this.locationID}" +
                    $"\n\tController ID = {this.controllerID}");

                if (this.digitalTwinModelState == null)
                {
                    this.digitalTwinModelState =
                        dtModelManager.CreateModelState(
                            this.deviceID,
                            this.locationID,
                            this.useGuidInInstanceKey,
                            this.controllerID,
                            (IDataContextEventListener) this);
                }
                else
                {
                    this.digitalTwinModelState.UpdateConnectionState(this.deviceID, this.locationID);

                    dtModelManager.UpdateModelState(this.digitalTwinModelState);
                }

                if (this.digitalTwinPropsHandler != null)
                {
                    this.digitalTwinPropsHandler.SetDigitalTwinModelState(this.digitalTwinModelState);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.SetDigitalTwinStateProcessor(this.digitalTwinModelState);
                }

                this.OnModelUpdateEvent();

                Debug.Log($"NORMAL: Created model state with URI {this.dtmiUri} and instance {this.digitalTwinModelState.GetModelSyncKey()}");
                Debug.Log($"NORMAL: Raw DTDL JSON\n==========\n{this.digitalTwinModelState.GetModelJson()}\n==========\n");

                // once provisioned, we're done with the provisioning button
                if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void UpdateModelJson()
        {
            if (this.modelContentText != null)
            {
                DigitalTwinModelManager modelMgr = EventProcessor.GetInstance().GetDigitalTwinModelManager();

                if (this.digitalTwinModelState != null)
                {
                    Debug.Log($"Updating model JSON via DT Model State instance: {this.digitalTwinModelState.GetModelControllerID()}");
                    this.modelContentText.text =
                        this.digitalTwinModelState.GetModelJson();
                }
                else
                {
                    Debug.Log($"Updating model JSON via DT Model Manager (state not yet created): {this.controllerID}");
                    this.modelContentText.text =
                        modelMgr.GetDigitalTwinModelJson(this.controllerID);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateModelDataAndProperties()
        {
            // it's possible the model state object hasn't been provisioned yet;
            // however, we may still be able to load the model's JSON, as it's
            // retrieved via the controller ID, which we might already know
            Debug.Log("NORMAL: Updating digital twin model JSON data...");

            this.UpdateModelJson();

            // if the model state has already been created,
            // (re) build the model data and display it
            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.BuildModelData();

                List<string> propKeys = this.digitalTwinModelState.GetModelPropertyKeys();
                StringBuilder propKeysStr = new StringBuilder();

                foreach (string key in propKeys)
                {
                    propKeysStr.Append(key).Append("\n");
                }

                this.modelProps = propKeysStr.ToString();
                this.propsContentText.text = this.modelProps;

                Debug.Log(
                    $"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.propsContentText.text}");

                List<string> telemetryKeys = this.digitalTwinModelState.GetModelPropertyTelemetryKeys();
                StringBuilder telemetryKeysStr = new StringBuilder();

                foreach (string key in telemetryKeys)
                {
                    telemetryKeysStr.Append(key).Append("\n");
                }

                this.modelTelemetries = telemetryKeysStr.ToString();
                this.statusContentText.text = this.modelTelemetries;

                Debug.Log(
                    $"Telemetry Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.statusContentText.text}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCommandResourceName()
        {
            if (this.cmdResource == null)
            {
                this.cmdResource = new ResourceNameContainer(this.deviceID, ConfigConst.ACTUATOR_CMD);
            }
            else
            {
                this.cmdResource.DeviceName = this.deviceID;
                this.cmdResource.InitFullResourceName();
            }

            this.cmdResourceName = this.cmdResource.GetFullResourceName();

            if (this.deviceCmdResourceText != null)
            {
                this.deviceCmdResourceText.text = this.cmdResourceName;
            }

            if (this.digitalTwinPropsHandler != null)
            {
                this.digitalTwinPropsHandler.SetDigitalTwinCommandResource(this.cmdResource);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateTwinProperties()
        {
            // TODO: this will be involved - move to a separate class
        }

    }
}

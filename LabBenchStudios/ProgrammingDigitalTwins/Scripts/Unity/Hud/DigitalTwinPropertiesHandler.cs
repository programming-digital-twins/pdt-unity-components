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
    public class DigitalTwinPropertiesHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private GameObject connStateObject = null;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject deviceModelNameObject = null;

        [SerializeField]
        private GameObject deviceModelIDObject = null;

        [SerializeField]
        private GameObject deviceCommandResourceObject = null;

        [SerializeField]
        private GameObject contentObject = null;

        [SerializeField]
        private GameObject closePanelButtonObject = null;

        [SerializeField]
        private GameObject savePropsButtonObject = null;

        [SerializeField]
        private GameObject sendCommandButtonObject = null;

        [SerializeField]
        private GameObject eventListenerContainer = null;

        private GameObject propsPanel = null;

        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text deviceModelNameText = null;
        private TMP_Text deviceModelIDText = null;
        private TMP_Text deviceCmdResourceText = null;
        private TMP_Text propsContentText = null;

        private Button closePanelButton = null;
        private Button savePropsButton = null;
        private Button sendCommandButton = null;

        private bool hasPropertiesPanel = false;

        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;
        private string dtmiUri  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;

        private string cmdResourceName =
            ConfigConst.PRODUCT_NAME + "/" + ConfigConst.EDGE_DEVICE + "/" + ConfigConst.ACTUATOR_CMD;

        private string modelProps = "";

        private DigitalTwinModelState digitalTwinModelState = null;

        private ResourceNameContainer cmdResource = null;

        private IDataContextExtendedListener eventListener = null;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void ClosePropertiesPanel()
        {
            if (this.hasPropertiesPanel)
            {
                this.propsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveProperties()
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendDeviceCommand()
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

        public void SetDigitalTwinCommandResource(ResourceNameContainer resource)
        {
            if (resource != null)
            {
                this.cmdResource = resource;

                this.UpdateCommandResource();
            }
        }

        public void SetDigitalTwinModelState(DigitalTwinModelState dtModelState)
        {
            if (dtModelState != null)
            {
                this.digitalTwinModelState = dtModelState;

                this.UpdateModelDataAndProperties();
            }
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
        private void InitPropertiesPanelControls()
        {
            this.propsPanel = gameObject;

            if (this.deviceIDObject != null)
            {
                this.deviceIDText = this.deviceIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.deviceCommandResourceObject != null)
            {
                this.deviceCmdResourceText = this.deviceCommandResourceObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.connStateObject != null)
            {
                this.connStateLabelText = this.connStateObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.deviceModelNameObject != null)
            {
                this.deviceModelNameText = this.deviceModelNameObject.GetComponent<TextMeshProUGUI>();
                this.deviceModelNameText.text = this.dtmiName;
            }

            if (this.deviceModelIDObject != null)
            {
                this.deviceModelIDText = this.deviceModelIDObject.GetComponent<TextMeshProUGUI>();
                this.deviceModelIDText.text = this.dtmiUri;
            }

            if (this.contentObject != null)
            {
                this.hasPropertiesPanel = true;

                this.propsContentText = this.contentObject.GetComponent<TextMeshProUGUI>();
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
            if (this.closePanelButtonObject != null)
            {
                this.closePanelButton = this.closePanelButtonObject.GetComponent<Button>();

                if (this.closePanelButton != null)
                {
                    this.closePanelButton.onClick.AddListener(() => this.ClosePropertiesPanel());
                }
            }

            if (this.sendCommandButtonObject != null)
            {
                this.sendCommandButton = this.sendCommandButtonObject.GetComponent<Button>();

                if (this.sendCommandButton != null)
                {
                    this.sendCommandButton.onClick.AddListener(() => this.SendDeviceCommand());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                // first: init controls
                this.InitPropertiesPanelControls();

                // second: update the command resource name
                this.UpdateModelDataAndProperties();

                // third: other init steps (if needed - future)

                // finally: register for events
                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize digital twin props editor HUD. Continuing without display data: {ex.StackTrace}");
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
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (this.connStateLabelText != null)
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.connStateLabelText != null) this.connStateLabelText.text = connStateMsg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessMessageData(MessageData data)
        {
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSensorData(SensorData data)
        {
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // ignore
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
        private void UpdateCommandResource()
        {
            if (this.deviceCmdResourceText != null)
            {
                if (this.cmdResource != null)
                {
                    this.deviceCmdResourceText.text = this.cmdResource.GetFullResourceName();
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
            Debug.Log("NORMAL: Updating digital twin model properties...");

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

                this.deviceID = this.digitalTwinModelState.GetDeviceID();
                this.dtmiUri = this.digitalTwinModelState.GetModelID();
                this.dtmiName = this.digitalTwinModelState.GetModelControllerID().ToString();

                if (this.deviceIDText != null) this.deviceIDText.text = this.deviceID;
                if (this.deviceModelIDText != null) this.deviceModelIDText.text = this.dtmiUri;
                if (this.deviceModelNameText != null) this.deviceModelNameText.text = this.dtmiName;

                this.modelProps = propKeysStr.ToString();

                if (this.propsContentText != null) this.propsContentText.text = this.modelProps;

                this.UpdateCommandResource();

                Debug.Log(
                    $"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.propsContentText.text}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateTwinProperties()
        {
            // TODO: implement this
        }

    }
}

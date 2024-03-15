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
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;

using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Hud
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
        private GameObject commandGuiTemplate = null;

        [SerializeField]
        private GameObject valuePropsGuiTemplate = null;

        [SerializeField]
        private GameObject togglePropGuiTemplate = null;

        [SerializeField]
        private GameObject messagePropGuidTemplate = null;

        [SerializeField]
        private GameObject schedulePropGuidTemplate = null;

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

        private float verticalAnchorDelta = 400.0f;
        //private float verticalAnchorDelta = 0.25f;

        private List<GameObject> digitalTwinGuiPropsList = null;
        private List<IPropertyManagementController> propertyUpdateHandlerList = null;

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
        public void SendDeviceCommands()
        {
            // first: set the command resource name text
            //        this is automatically set initially, but the user can
            //        manually override (eventually - future update), so check
            //        the current curValue and apply it to the locally stored
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
                    Debug.Log($"Sending command to device: {resource}");
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
                    this.sendCommandButton.onClick.AddListener(() => this.SendDeviceCommands());
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
                this.digitalTwinGuiPropsList = new List<GameObject>();
                this.propertyUpdateHandlerList = new List<IPropertyManagementController>();

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
            if (this.propertyUpdateHandlerList.Count > 0)
            {
                // IFF we have any changed properties, create a new list
                List<ResourceNameContainer> deviceCmdResourceList = new List<ResourceNameContainer>();

                // TODO: implement this - the following is just a template
                foreach (IPropertyManagementController propsUpdateHandler in this.propertyUpdateHandlerList)
                {
                    if (propsUpdateHandler.IsChanged())
                    {
                        ResourceNameContainer deviceCmdResource = new ResourceNameContainer(this.cmdResource);
                        deviceCmdResource.DataContext = propsUpdateHandler.GenerateCommand();

                        Debug.LogWarning($"Resource: {deviceCmdResource}; Data: {deviceCmdResource.DataContext}");
                        deviceCmdResourceList.Add(deviceCmdResource);
                    }
                }

                return deviceCmdResourceList;
            }

            return null;
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
                this.ClearDigitalTwinProperties();

                this.digitalTwinModelState.BuildModelData();

                List<string> propKeys = this.digitalTwinModelState.GetModelPropertyKeys();
                StringBuilder propKeysStr = new StringBuilder();

                float curYPosDelta = 0.0f;

                foreach (string key in propKeys)
                {
                    DigitalTwinProperty property = this.digitalTwinModelState.GetModelProperty(key);

                    // should never be null, but check anyway
                    if (property != null)
                    {
                        // only writeable properties for this handler
                        // the model should never indicate telemetry or
                        // commands are 'writeable', but double check anyway
                        if (property.IsPropertyWriteable() &&
                            ! property.IsPropertyTelemetry() &&
                            ! property.IsCommand())
                        {
                            this.RenderDigitalTwinProperty(property, curYPosDelta);
                            curYPosDelta += this.verticalAnchorDelta;
                        }
                        else if (property.IsCommand())
                        {
                            // commands get special treatment
                            this.CreateDigitalTwinCommandProperty(property, curYPosDelta);
                            curYPosDelta += this.verticalAnchorDelta;
                        }

                        // incoming telemetry is currently processed via the event manager
                        // and the state handler using the incoming IotDataContext, but not
                        // directly validated against the digital twin thing's model
                        //
                        // a future update to the DTA may eventually use the model to validate
                        // incoming telemetry using the specified schema type; however, any
                        // requisite updates are probably best handled by the state handler
                        // directly, as this handler is for manipulating properties and
                        // commands only
                    }
                }

                this.deviceID = this.digitalTwinModelState.GetDeviceID();
                this.dtmiUri = this.digitalTwinModelState.GetModelID();
                this.dtmiName = this.digitalTwinModelState.GetModelControllerID().ToString();

                if (this.deviceIDText != null) this.deviceIDText.text = this.deviceID;
                if (this.deviceModelIDText != null) this.deviceModelIDText.text = this.dtmiUri;
                if (this.deviceModelNameText != null) this.deviceModelNameText.text = this.dtmiName;

                this.UpdateCommandResource();

                Debug.Log($"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearDigitalTwinProperties()
        {
            // clear the controller list first
            this.propertyUpdateHandlerList.Clear();

            // now destroy all the props objects
            foreach (GameObject go in this.digitalTwinGuiPropsList)
            {
                go.SetActive(false);

                Destroy(go);
            }

            this.digitalTwinGuiPropsList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private void RenderDigitalTwinProperty(DigitalTwinProperty property, float yPosDelta)
        {
            // create the object
            switch (property.GetPropertyType())
            {
                // for now, a limited number of property types are supported - this can
                // be relatively easily extended with new prefabs that render the properties
                // mapped by current values in ModelNameUtil.DtmiPropertyTypeEnum, and those
                // that may be added in the future
                //
                // each enum -> one prefab (set of GUI controls)
                case ModelNameUtil.DtmiPropertyTypeEnum.Count:
                    this.CreateDigitalTwinValueProperty(property, yPosDelta);
                    break;

                case ModelNameUtil.DtmiPropertyTypeEnum.Value:
                    this.CreateDigitalTwinValueProperty(property, yPosDelta);
                    break;

                case ModelNameUtil.DtmiPropertyTypeEnum.Toggle:
                    this.CreateDigitalTwinToggleProperty(property, yPosDelta);
                    break;

                case ModelNameUtil.DtmiPropertyTypeEnum.Message:
                    this.CreateDigitalTwinMessageProperty(property, yPosDelta);
                    break;

                case ModelNameUtil.DtmiPropertyTypeEnum.Schedule:
                    this.CreateDigitalTwinScheduleProperty(property, yPosDelta);
                    break;

                case ModelNameUtil.DtmiPropertyTypeEnum.Undefined:
                    this.CreateDigitalTwinMessageProperty(property, yPosDelta);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="yPosDelta"></param>
        private void AddDigitalTwinGuiProperty(DigitalTwinProperty property, GameObject go, float yPosDelta)
        {
            if (go != null)
            {
                // set the property label
                try
                {
                    Transform t = go.transform.Find("PropsLabel");
                    TMP_Text guiLabel = t.GetComponent<TextMeshProUGUI>();
                    guiLabel.text = property.GetDisplayName();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Can't find properties label for GameObject {go}");
                }

                // adjust location

                RectTransform propPosObj = go.GetComponent<RectTransform>();
                float xPos = propPosObj.anchoredPosition.x - 50.0f;

                if (this.contentObject != null)
                {
                    RectTransform parentPosObj = this.contentObject.GetComponent<RectTransform>();
                    xPos = parentPosObj.anchoredPosition.x - 5.0f;
                }

                Vector2 anchorPos = new Vector2(xPos, (propPosObj.anchoredPosition.y - yPosDelta));
                propPosObj.anchoredPosition = anchorPos;

                // activate component
                go.SetActive(true);

                Debug.Log($"Created GUI for property. Loc: {anchorPos}. Prop: {property}");

                // add to internal list
                this.digitalTwinGuiPropsList.Add(go);

                // retrieve the controller for each property and set its references
                // as it needs to know about the property and how to (potentially)
                // generate an ActuatorData command (if one is requested)
                try
                {
                    IPropertyManagementController propsUpdateHandler =
                        go.GetComponent<IPropertyManagementController>();

                    if (propsUpdateHandler != null )
                    {
                        propsUpdateHandler.InitPropertyController(this.digitalTwinModelState, property);

                        this.propertyUpdateHandlerList.Add(propsUpdateHandler);
                    }
                }
                catch (Exception e)
                {
                    // each GameObject in this call should have an IPropertyManagementController;
                    // however, if not, just ignore and log a message
                    Debug.LogError($"No IPropertyManagementController for GameObject and property: {property}");
                }
            }
            else
            {
                Debug.LogError($"GUI GameObject for property is null: {property}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinCommandProperty(DigitalTwinProperty property, float yPosDelta)
        {
            if (this.commandGuiTemplate != null)
            {
                GameObject go = Instantiate(this.commandGuiTemplate, this.contentObject.transform);

                this.AddDigitalTwinGuiProperty(property, go, yPosDelta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinValueProperty(DigitalTwinProperty property, float yPosDelta)
        {
            if (this.valuePropsGuiTemplate != null)
            {
                GameObject go = Instantiate(this.valuePropsGuiTemplate, this.contentObject.transform);

                this.AddDigitalTwinGuiProperty(property, go, yPosDelta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinToggleProperty(DigitalTwinProperty property, float yPosDelta)
        {
            if (this.togglePropGuiTemplate != null)
            {
                GameObject go = Instantiate(this.togglePropGuiTemplate, this.contentObject.transform);

                this.AddDigitalTwinGuiProperty(property, go, yPosDelta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinMessageProperty(DigitalTwinProperty property, float yPosDelta)
        {
            if (this.messagePropGuidTemplate != null)
            {
                GameObject go = Instantiate(this.messagePropGuidTemplate, this.contentObject.transform);

                this.AddDigitalTwinGuiProperty(property, go, yPosDelta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinScheduleProperty(DigitalTwinProperty property, float yPosDelta)
        {
            if (this.schedulePropGuidTemplate != null)
            {
                GameObject go = Instantiate(this.schedulePropGuidTemplate, this.contentObject.transform);

                this.AddDigitalTwinGuiProperty(property, go, yPosDelta);
            }
        }

    }

}

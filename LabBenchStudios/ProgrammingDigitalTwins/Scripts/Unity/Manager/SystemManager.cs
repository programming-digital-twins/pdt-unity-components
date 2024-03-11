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

using UnityEngine;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Data;

using System.Collections.Generic;
using System;

namespace LabBenchStudios.Pdt.Unity.Manager
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemManager : MonoBehaviour, IRemoteStateProcessor
    {
        [SerializeField]
        private bool enableLogging = false;

        [SerializeField]
        private bool allowRemoteCommandTriggers = true;

        [SerializeField, Range(1.0f, 60.0f)]
        private float maxRemoteCommandsPerMinute = 12.0f;

        [SerializeField]
        private bool addSampleRemoteCommandTriggers = true;

        [SerializeField, Range(10.0f, 60.0f)]
        private float sampleRemoteCommandTriggerFirings = 10.0f;

        [SerializeField]
        private bool enableLiveDataConnection = true;

        [SerializeField]
        private bool checkMessagingConnectionStatus = true;

        [SerializeField]
        private bool enableStoredDataConnection = false;

        [SerializeField]
        private bool connectToMqttAsap = false;

        [SerializeField]
        private string messagingTopicPrefixName = ConfigConst.PRODUCT_NAME;

        [SerializeField]
        private string messagingHostName = ConfigConst.DEFAULT_HOST;
        
        [SerializeField]
        private int messagingHostPort = ConfigConst.DEFAULT_MQTT_PORT;

        [SerializeField]
        private string tsdbHostName = ConfigConst.DEFAULT_HOST;

        [SerializeField]
        private int tsdbHostPort = ConfigConst.DEFAULT_TSDB_PORT;

        [SerializeField]
        private string clientToken = ConfigConst.NOT_SET;

        [SerializeField]
        private string organizationID = ConfigConst.NOT_SET;

        [SerializeField]
        private string envSensorDataBucket = ConfigConst.NOT_SET;

        [SerializeField]
        private string sysPerformanceDataBucket = ConfigConst.NOT_SET;

        [SerializeField]
        private string sysStateDataBucket = ConfigConst.NOT_SET;

        [SerializeField]
        private string commandDataBucket = ConfigConst.NOT_SET;

        private bool isInitialized = false;
        private float delayBetweenRemoteCommands = 5.0f; // default 5 seconds

        private IPubSubConnector mqttClient = null;
        private IPersistenceConnector tsdbClient = null;

        private EventProcessor eventProcessor = null;

        private Queue<ResourceNameContainer> remoteCmdDataQueue = null;


        // internal

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Debug.unityLogger.logEnabled = this.enableLogging;

            this.eventProcessor = EventProcessor.GetInstance();
            this.eventProcessor.SetRemoteCommandProcessor(this);

            this.delayBetweenRemoteCommands = 60.0f / this.maxRemoteCommandsPerMinute;
            this.isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            this.InitManager();
            this.StartManager();
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDestroy()
        {
            this.StopManager();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            // nothing to do
        }

        // public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        public void HandleRemoteCommandRequest(ResourceNameContainer resource)
        {
            if (resource != null)
            {
                // STEP 1: validate the request - ensure it's legit
                // this simple validation just ensures it's an
                // actuation command (ActuatorData instance)
                bool isCmd = resource.IsActuationResource;

                Debug.Log("Is Actuator Command? " + isCmd);

                // STEP 2: Enqueue the request
                if (isCmd) this.remoteCmdDataQueue.Enqueue(resource);

                // STEP 3: Process the request (send to the IPubSubConnector)

                // this is automatically handled in the ProcessRemoteCommands
                // co-routine, invoked every this.delayBetweenRemoteCommands seconds
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartManager()
        {
            if (this.connectToMqttAsap)
            {
                this.EnableLiveDataFeed(true);
            }

            if (this.allowRemoteCommandTriggers)
            {
                // first execution after 1.0 seconds; repeats every this.delayBetweenRemoteCommands seconds
                InvokeRepeating(
                    nameof(ProcessRemoteCommands), 1.0f, this.delayBetweenRemoteCommands);
            }

            if (this.addSampleRemoteCommandTriggers)
            {
                // for testing only - send a dummy actuation command every 'n' seconds
                InvokeRepeating(
                    nameof(AddSampleRemoteCommandToQueue), 1.0f, this.sampleRemoteCommandTriggerFirings);
            }

            if (this.checkMessagingConnectionStatus)
            {
                InvokeRepeating(
                    nameof(CheckMessagingConnection), 5.0f, 5.0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopManager()
        {
            this.EnableLiveDataFeed(false);
            this.EnableSimulatedDataFeed(false);

            // cancel all InvokeRepeating calls
            CancelInvoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableLiveDataFeed(bool enable)
        {
            if (enable)
            {
                this.EnableSimulatedDataFeed(false);

                if (!this.mqttClient.IsClientConnected())
                {
                    Debug.Log("Connecting MQTT client to host.");
                    this.mqttClient.ConnectClient();
                }
                else
                {
                    Debug.Log("MQTT client already connected to host.");
                }
            }
            else
            {
                if (this.mqttClient.IsClientConnected())
                {
                    Debug.Log("Disconnecting MQTT client from host.");
                    this.mqttClient.DisconnectClient();
                }
                else
                {
                    Debug.Log("MQTT client already disconnected from host.");
                }
            }

            this.EnableIncomingVirtualThingUpdates(enable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableSimulatedDataFeed(bool enable)
        {
            if (enable)
            {
                this.EnableLiveDataFeed(false);

                // TODO: implement this
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableIncomingVirtualThingUpdates(bool enable)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableOutoingPhysicalThingUpdates(bool enable)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public bool SendStateUpdateToPhysicalThing(ResourceNameContainer resource)
        {
            if (resource != null)
            {
                Debug.Log($"Sending remote command to {this.messagingHostName}: {resource.DeviceName}");

                this.mqttClient.PublishMessage(resource);
            }

            return true;
        }

        // private

        /// <summary>
        /// 
        /// </summary>
        private void CheckMessagingConnection()
        {
            if (this.mqttClient != null)
            {
                ConnectionStateData connStateData =
                    new ConnectionStateData(
                        ConfigConst.PRODUCT_NAME,
                        ConfigConst.PRODUCT_NAME,
                        this.messagingHostName,
                        this.messagingHostPort);

                if (this.mqttClient.IsClientConnected())
                {
                    connStateData.SetMessage("Connected");
                    connStateData.SetIsClientConnectedFlag(true);
                    connStateData.SetStatusCode(ConfigConst.CONN_SUCCESS_STATUS_CODE);
                }
                else
                {
                    connStateData.SetMessage("Disconnected");
                    connStateData.SetIsClientDisconnectedFlag(true);
                    connStateData.SetStatusCode(ConfigConst.DISCONN_SUCCESS_STATUS_CODE);
                }

                this.eventProcessor.OnMessagingSystemStatusUpdate(connStateData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddSampleRemoteCommandToQueue()
        {
            // TODO: this is a simplistic way to pass data
            // back to the EDA - for now, we're just using
            // the 'EDGE_DEVICE' const
            string deviceName = ConfigConst.EDGE_DEVICE;
            string resourceName = ConfigConst.ACTUATOR_CMD;

            ActuatorData data =
                new ActuatorData(
                    ConfigConst.HUMIDIFIER_ACTUATOR_NAME,
                    deviceName,
                    ConfigConst.ENV_TYPE_CATEGORY,
                    ConfigConst.HUMIDIFIER_ACTUATOR_TYPE);

            data.SetCommand(ConfigConst.COMMAND_ON);
            data.SetValue(this.GenerateRandomTestFloat(35.0f, 65.0f));
            data.SetStateData("Unity DT Remote Command");

            ResourceNameContainer resource =
                new ResourceNameContainer(deviceName, resourceName, data);
            resource.ProductPrefix = ConfigConst.PRODUCT_NAME;

            this.HandleRemoteCommandRequest(resource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private float GenerateRandomTestFloat(float min, float max)
        {
            System.Random rnd = new System.Random();

            double delta = max - min;
            double rndDbl = rnd.NextDouble();
            double rndVal = (rndDbl * delta) + min;
        
            Debug.Log("Actuator Value: " + rndVal);
            return (float) rndVal;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessRemoteCommands()
        {
            Debug.Log("Checking if remote command available to send to deviceName(s)...");

            ResourceNameContainer resource = this.GetRemoteCommandFromQueue();

            this.SendStateUpdateToPhysicalThing(resource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ResourceNameContainer GetRemoteCommandFromQueue()
        {
            if (this.remoteCmdDataQueue.Count > 0)
            {
                return this.remoteCmdDataQueue.Dequeue();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitManager()
        {
            string msg = "System manager initializing...";

            Debug.Log(msg);

            this.remoteCmdDataQueue = new Queue<ResourceNameContainer>();

            this.mqttClient =
                new MqttClientManagedConnector(
                    this.messagingHostName, this.messagingHostPort, null, this.messagingTopicPrefixName, this.eventProcessor);
        }

    }
}

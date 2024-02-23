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
    public class SystemManager : MonoBehaviour
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
        private bool enableStoredDataConnection = false;

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


        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Debug.unityLogger.logEnabled = this.enableLogging;

            this.eventProcessor = EventProcessor.GetInstance();
            this.eventProcessor.SetRemoteCommandProcessor(this);

            this.delayBetweenRemoteCommands = 60.0f / this.maxRemoteCommandsPerMinute;
            this.isInitialized = true;
        }

        void Start()
        {
            this.InitManager();
            this.StartManager();

            if (this.allowRemoteCommandTriggers)
            {
                // first execution after 1.0 seconds; repeats every this.delayBetweenRemoteCommands seconds
                InvokeRepeating(
                    "ProcessRemoteCommands", 1.0f, this.delayBetweenRemoteCommands);
            }

            if (this.addSampleRemoteCommandTriggers)
            {
                // for testing only - send a dummy actuation command every 'n' seconds
                InvokeRepeating(
                    "AddSampleRemoteCommandToQueue", 1.0f, this.sampleRemoteCommandTriggerFirings);
            }
        }

        void OnDestroy()
        {
            this.StopManager();
        }

        // Update is called once per frame
        void Update()
        {
            // nothing to do
        }

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

        public void StartManager()
        {
            this.mqttClient.ConnectClient();
        }

        public void StopManager()
        {
            this.mqttClient.DisconnectClient();
        }


        // private

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

        private float GenerateRandomTestFloat(float min, float max)
        {
            System.Random rnd = new System.Random();

            double delta = max - min;
            double rndDbl = rnd.NextDouble();
            double rndVal = (rndDbl * delta) + min;
        
            Debug.Log("Actuator Value: " + rndVal);
            return (float) rndVal;
        }

        private void ProcessRemoteCommands()
        {
            Debug.Log("Checking if remote command available to send to device(s)...");

            ResourceNameContainer resource = this.GetRemoteCommandFromQueue();

            if (resource != null)
            {
                Debug.Log("Sending remote command to " + this.messagingHostName + ": " + resource.DeviceName);

                this.mqttClient.SendRemoteCommand(resource);
            }
        }

        private ResourceNameContainer GetRemoteCommandFromQueue()
        {
            if (this.remoteCmdDataQueue.Count > 0)
            {
                return this.remoteCmdDataQueue.Dequeue();
            }

            return null;
        }

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

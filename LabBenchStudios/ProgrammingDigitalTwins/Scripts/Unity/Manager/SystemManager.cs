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

namespace LabBenchStudios.Pdt.Unity.Manager
{
    public class SystemManager : MonoBehaviour
    {
        [SerializeField]
        private bool enableLiveDataConnection = true;

        [SerializeField]
        private bool enableStoredDataConnection = false;

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

        private IPubSubConnector mqttClient = null;
        private IPersistenceConnector tsdbClient = null;

        private EventProcessor eventProcessor = null;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            this.eventProcessor = EventProcessor.GetInstance();
            this.isInitialized = true;
        }

        void Start()
        {
            this.InitManager();
            this.StartManager();
        }

        void OnDestroy()
        {
            this.StopManager();
        }

        // Update is called once per frame
        void Update()
        {

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

        private void InitManager()
        {
            string msg = "System manager initializing...";

            Debug.LogWarning(msg);

            this.mqttClient = new MqttClientManagedConnector(this.messagingHostName, this.messagingHostPort, null, this.eventProcessor);
        }
    }
}

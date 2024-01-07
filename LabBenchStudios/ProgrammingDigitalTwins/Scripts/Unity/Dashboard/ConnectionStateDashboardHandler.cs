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
using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class ConnectionStateDashboardHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private GameObject connectionStateDisplay = null;

        [SerializeField]
        private GameObject connectionHostNameDisplay = null;

        [SerializeField]
        private GameObject connectionMsgInDisplay = null;

        [SerializeField]
        private GameObject connectionMsgOutDisplay = null;

        [SerializeField]
        private GameObject rawMsgDataDisplay = null;

        [SerializeField]
        private GameObject formattedMsgDataDisplay = null;

        private EventProcessor eventProcessor = null;

        private TMP_Text connStateLog = null;
        private TMP_Text connHostNameLog = null;
        private TMP_Text connMsgInLog = null;
        private TMP_Text connMsgOutLog = null;
        private TMP_Text rawMsgDataLog = null;
        private TMP_Text formattedMsgDataLog = null;


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
                if (this.connectionStateDisplay != null)
                {
                    this.connStateLog = this.connectionStateDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.connectionStateDisplay != null)
                {
                    this.connHostNameLog = this.connectionHostNameDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.connectionMsgInDisplay != null)
                {
                    this.connMsgInLog = this.connectionMsgInDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.connectionMsgOutDisplay != null)
                {
                    this.connMsgOutLog = this.connectionMsgOutDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.rawMsgDataDisplay != null)
                {
                    this.rawMsgDataLog = this.rawMsgDataDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.formattedMsgDataDisplay != null)
                {
                    this.formattedMsgDataLog = this.formattedMsgDataDisplay.GetComponent<TextMeshProUGUI>();
                }

                this.eventProcessor = EventProcessor.GetInstance();

                this.eventProcessor.RegisterListener((ISystemStatusEventListener) this);
                this.eventProcessor.RegisterListener((IDataContextEventListener) this);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize connection state display text. Continuing without display data.");
            }
        }

        protected new void ProcessDebugLogMessage(string message)
        {
            if (message != null)
            {
                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = message;
            }
        }

        protected new void ProcessWarningLogMessage(string message)
        {
            if (message != null)
            {
                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = message;
            }
        }

        protected new void ProcessErrorLogMessage(string message)
        {
            if (message != null)
            {
                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = message;
            }
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            if (data != null)
            {
                String jsonData = DataUtil.ActuatorDataToJson(data);

                if (this.rawMsgDataLog != null) this.rawMsgDataLog.text = jsonData;
            }
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (data != null)
            {
                String connStateMsg = "Checking...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                if (data.IsClientDisconnected()) connStateMsg = "Disconnected";

                if (this.connStateLog != null) this.connStateLog.text = connStateMsg;
                if (this.connHostNameLog != null) this.connHostNameLog.text = data.GetHostName();
                if (this.connMsgInLog != null) this.connMsgInLog.text = data.GetMessageInCount().ToString();
                if (this.connMsgOutLog != null) this.connMsgOutLog.text = data.GetMessageOutCount().ToString();
            }
        }

        protected override void ProcessMessageData(MessageData data)
        {
            if (data != null)
            {
                if (this.rawMsgDataLog != null) this.rawMsgDataLog.text = data.GetMessageData();

                String formattedMsg = "Source: " + data.GetName() + " - " + data.GetDeviceID();

                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = formattedMsg;
            }
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null)
            {
                String jsonData = DataUtil.SensorDataToJson(data);

                if (this.rawMsgDataLog != null) this.rawMsgDataLog.text = jsonData;

                String formattedMsg = "Source: " + data.GetName() + " - " + data.GetDeviceID();

                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = formattedMsg;
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null)
            {
                String jsonData = DataUtil.SystemPerformanceDataToJson(data);

                if (this.rawMsgDataLog != null) this.rawMsgDataLog.text = jsonData;

                String formattedMsg = "Source: " + data.GetName() + " - " + data.GetDeviceID();

                if (this.formattedMsgDataLog != null) this.formattedMsgDataLog.text = formattedMsg;
            }
        }

    }
}

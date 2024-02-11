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

using UnityEngine;

using TMPro;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Unity.Common
{
    public abstract class BaseAsyncDataMessageProcessor : MonoBehaviour, IDataContextEventListener
    {
        [SerializeField]
        private Boolean enableDebugLogProcessing = false;

        [SerializeField]
        private Boolean enableWarningLogProcessing = false;

        [SerializeField]
        private Boolean enableErrorLogProcessing = false;

        [SerializeField]
        private Boolean enableMessageDataProcessing = false;

        [SerializeField]
        private Boolean enableActuatorDataProcessing = false;

        [SerializeField]
        private Boolean enableConnectionStateDataProcessing = false;

        [SerializeField]
        private Boolean enableSensorDataProcessing = false;

        [SerializeField]
        private Boolean enableSystemPerformanceDataProcessing = false;

        [SerializeField]
        private Boolean enableTimeAndDateUpdates = true;

        [SerializeField]
        private GameObject timeDisplay = null;

        [SerializeField]
        private GameObject dateDisplay = null;

        private bool isInitialized = false;

        private EventProcessor eventProcessor = null;

        private TMP_Text timeLog = null;
        private TMP_Text dateLog = null;

        private Queue<string> debugLogQueue = null;
        private Queue<string> warningLogQueue = null;
        private Queue<string> errorLogQueue = null;

        private Queue<MessageData> msgDataQueue = null;
        private Queue<ActuatorData> actuatorDataQueue = null;
        private Queue<ConnectionStateData> connStateDataQueue = null;
        private Queue<SensorData> sensorDataQueue = null;
        private Queue<SystemPerformanceData> sysPerfDataQueue = null;

        void Awake()
        {
            this.debugLogQueue = new Queue<string>();
            this.warningLogQueue = new Queue<string>();
            this.errorLogQueue = new Queue<string>();

            this.msgDataQueue = new Queue<MessageData>();
            this.actuatorDataQueue = new Queue<ActuatorData>();
            this.connStateDataQueue = new Queue<ConnectionStateData>();
            this.sensorDataQueue = new Queue<SensorData>();
            this.sysPerfDataQueue = new Queue<SystemPerformanceData>();

            if (this.timeDisplay != null)
            {
                this.timeLog = this.timeDisplay.GetComponent<TextMeshProUGUI>();
            }

            if (this.dateDisplay != null)
            {
                this.dateLog = this.dateDisplay.GetComponent<TextMeshProUGUI>();
            }

            this.eventProcessor = EventProcessor.GetInstance();
            this.eventProcessor.RegisterListener((IDataContextEventListener) this);

            this.InitMessageHandler();
            this.isInitialized = true;
        }

        void Start()
        {
            if (this.enableTimeAndDateUpdates)
            {
                InvokeRepeating("UpdateTimeAndDate", 1.0f, 1.0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.enableDebugLogProcessing)
            {
                this.ProcessDebugLogMessage(this.GetDebugLogMessageFromQueue());
            }

            if (this.enableWarningLogProcessing)
            {
                this.ProcessWarningLogMessage(this.GetWarningLogMessageFromQueue());
            }

            if (this.enableErrorLogProcessing)
            {
                this.ProcessErrorLogMessage(this.GetErrorLogMessageFromQueue());
            }

            if (this.enableActuatorDataProcessing)
            {
                this.ProcessActuatorData(this.GetActuatorDataFromQueue());
            }

            if (this.enableConnectionStateDataProcessing)
            {
                this.ProcessConnectionStateData(this.GetConnectionStateDataFromQueue());
            }

            if (this.enableMessageDataProcessing)
            {
                this.ProcessMessageData(this.GetMessageDataFromQueue());
            }

            if (this.enableSensorDataProcessing)
            {
                this.ProcessSensorData(this.GetSensorDataFromQueue());
            }

            if (this.enableSystemPerformanceDataProcessing)
            {
                this.ProcessSystemPerformanceData(this.GetSystemPerformanceDataFromQueue());
            }
        }

        // public callback methods

        public void HandleDebugLogMessage(string message)
        {
            if (this.enableDebugLogProcessing && message != null)
            {
                this.debugLogQueue.Enqueue(message);
            }
        }

        public void HandleWarningLogMessage(string message)
        {
            if (this.enableWarningLogProcessing && message != null)
            {
                this.warningLogQueue.Enqueue(message);
            }
        }

        public void HandleErrorLogMessage(string message)
        {
            if (this.enableErrorLogProcessing && message != null)
            {
                this.errorLogQueue.Enqueue(message);
            }
        }

        public void HandleActuatorData(ActuatorData data)
        {
            if (this.enableActuatorDataProcessing && data != null)
            {
                this.actuatorDataQueue.Enqueue(data);
            }
        }

        public void HandleConnectionStateData(ConnectionStateData data)
        {
            if (this.enableConnectionStateDataProcessing && data != null)
            {
                this.connStateDataQueue.Enqueue(data);
            }
        }

        public void HandleMessageData(MessageData data)
        {
            if (this.enableMessageDataProcessing && data != null)
            {
                this.msgDataQueue.Enqueue(data);
            }
        }

        public void HandleSensorData(SensorData data)
        {
            if (this.enableSensorDataProcessing && data != null)
            {
                this.sensorDataQueue.Enqueue(data);
            }
        }

        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.enableSystemPerformanceDataProcessing && data != null)
            {
                this.sysPerfDataQueue.Enqueue(data);
            }
        }

        // protected methods

        protected void ProcessDebugLogMessage(string message)
        {
            if (message != null) Debug.Log(message);
        }

        protected void ProcessWarningLogMessage(string message)
        {
            if (message != null) Debug.LogWarning(message);
        }

        protected void ProcessErrorLogMessage(string message)
        {
            if (message != null) Debug.LogError(message);
        }

        protected void RegisterForSystemStatusEvents(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.eventProcessor.RegisterListener(listener);
            }
        }

        // protected template methods

        protected abstract void InitMessageHandler();

        protected abstract void ProcessActuatorData(ActuatorData data);

        protected abstract void ProcessConnectionStateData(ConnectionStateData data);

        protected abstract void ProcessMessageData(MessageData data);

        protected abstract void ProcessSensorData(SensorData data);

        protected abstract void ProcessSystemPerformanceData(SystemPerformanceData data);

        // private methods

        private string GetDebugLogMessageFromQueue()
        {
            string msg = null;

            if (this.debugLogQueue.Count > 0)
            {
                msg = this.debugLogQueue.Dequeue();
            }

            return msg;
        }

        private string GetWarningLogMessageFromQueue()
        {
            string msg = null;

            if (this.warningLogQueue.Count > 0)
            {
                msg = this.warningLogQueue.Dequeue();
            }

            return msg;
        }

        private string GetErrorLogMessageFromQueue()
        {
            string msg = null;

            if (this.errorLogQueue.Count > 0)
            {
                msg = this.errorLogQueue.Dequeue();
            }

            return msg;
        }

        private ActuatorData GetActuatorDataFromQueue()
        {
            ActuatorData data = null;

            if (this.actuatorDataQueue.Count > 0)
            {
                data = this.actuatorDataQueue.Dequeue();
            }

            return data;
        }

        private ConnectionStateData GetConnectionStateDataFromQueue()
        {
            ConnectionStateData data = null;

            if (this.connStateDataQueue.Count > 0)
            {
                data = this.connStateDataQueue.Dequeue();
            }

            return data;
        }

        private MessageData GetMessageDataFromQueue()
        {
            MessageData data = null;

            if (this.msgDataQueue.Count > 0)
            {
                data = this.msgDataQueue.Dequeue();
            }

            return data;
        }

        private SensorData GetSensorDataFromQueue()
        {
            SensorData data = null;

            if (this.sensorDataQueue.Count > 0)
            {
                data = this.sensorDataQueue.Dequeue();
            }

            return data;
        }

        private SystemPerformanceData GetSystemPerformanceDataFromQueue()
        {
            SystemPerformanceData data = null;

            if (this.sysPerfDataQueue.Count > 0)
            {
                data = this.sysPerfDataQueue.Dequeue();
            }

            return data;
        }

        private void UpdateTimeAndDate()
        {
            DateTime dateTime = DateTime.Now;

            if (this.timeLog != null)
            {
                this.timeLog.text = dateTime.ToString("t");// "h:MM:ss tt");
            }

            if (this.dateLog != null)
            {
                this.dateLog.text = dateTime.ToString("ddd, MMM dd yyyy");
            }
        }

    }
}

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
using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class StateManagementHudHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private GameObject liveDataStatusDisplay = null;

        [SerializeField]
        private GameObject liveDataEnableButtonObject = null;

        [SerializeField]
        private GameObject simDataStatusDisplay = null;

        [SerializeField]
        private GameObject simDataEnableButtonObject = null;

        [SerializeField]
        private GameObject modelDataLoadStatusDisplay = null;

        [SerializeField]
        private GameObject filePathEntryStatusDisplay = null;

        [SerializeField]
        private GameObject modelLoadButtonObject = null;

        private TMP_Text liveDataStatusText = null;
        private TMP_Text simDataStatusText = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathEntryText = null;

        private Button startLiveDataFeedButton = null;
        private Button startSimDataFeedButton = null;
        private Button loadModelDataButton = null;

        private TMP_Text startLiveDataFeedButtonText = null;
        private TMP_Text startSimDataFeedButtonText = null;
        private TMP_Text loadModelDataButtonText = null;

        private string dtdlModelPath =
            Application.dataPath + "/LabBenchStudios/ProgrammingDigitalTwins/Models/Dtdl";

        private bool isLiveDataEngaged = false;
        private bool isSimDataEngaged = false;
        private bool isLoadModelDataEngaged = false;


        // public methods (button interactions)

        public void StartLiveDataFeed(bool enable)
        {
            if (this.startLiveDataFeedButton != null)
            {
                if (enable && ! this.isLiveDataEngaged)
                {
                    this.StartSimDataFeed(false);

                    this.startLiveDataFeedButtonText.text = "Stop Live";
                    this.liveDataStatusText.text = "Live Data Started";
                }
                else if (! enable || this.isLiveDataEngaged)
                {
                    this.startLiveDataFeedButtonText.text = "Start Live";
                    this.liveDataStatusText.text = "Live Data Stopped";

                    enable = false;
                }

                EventProcessor eventProcessor = EventProcessor.GetInstance();
                eventProcessor.ProcessLiveDataFeedEngageRequest(enable);

                this.isLiveDataEngaged = enable;
            }
        }

        public void StartSimDataFeed(bool enable)
        {
            if (this.startSimDataFeedButton != null)
            {
                if (enable && ! this.isSimDataEngaged)
                {
                    this.StartLiveDataFeed(false);

                    this.startSimDataFeedButtonText.text = "Stop Sim";
                    this.simDataStatusText.text = "Sim Data Started";
                }
                else if (! enable || this.isSimDataEngaged)
                {
                    this.startSimDataFeedButtonText.text = "Start Sim";
                    this.simDataStatusText.text = "Sim Data Stopped";

                    enable = false;
                }

                EventProcessor eventProcessor = EventProcessor.GetInstance();
                eventProcessor.ProcessSimulatedDataFeedEngageRequest(enable);

                this.isSimDataEngaged = enable;
            }
        }

        public void InitializeAndLoadModelData()
        {
            try
            {
                Debug.Log($"Attempting to init and load DTDL model data: {this.dtdlModelPath}");

                EventProcessor eventProcessor = EventProcessor.GetInstance();
                DigitalTwinModelManager dtModelManager = eventProcessor.GetDigitalTwinModelManager();

                if (dtModelManager == null)
                {
                    Debug.Log("Creating DT Model Manager and registering with EventProcessor...");

                    this.modelDataLoadStatusText.text = "Loading model data...";

                    dtModelManager = new DigitalTwinModelManager(this.dtdlModelPath);
                    eventProcessor.RegisterDigitalTwinModelManager(dtModelManager);

                    if (dtModelManager.HasSuccessfulDataLoad())
                    {
                        eventProcessor.OnModelUpdateEvent();

                        Debug.Log($"Loaded DTDL model data: {this.dtdlModelPath}");
                        this.modelDataLoadStatusText.text = "Loaded model data.";
                    }
                    else
                    {
                        Debug.LogError($"Failed to load DTDL model data: {this.dtdlModelPath}");
                        this.modelDataLoadStatusText.text = "Failed to load model data!";
                    }
                }
                else
                {
                    this.modelDataLoadStatusText.text = "Reloading model data...";
                    if (dtModelManager.ReloadDtdlModels(this.dtdlModelPath))
                    {
                        Debug.Log($"Reloaded DTDL model data: {this.dtdlModelPath}");
                        this.modelDataLoadStatusText.text = "Reloaded model data.";
                    }
                    else
                    {
                        Debug.LogError($"Failed to reload DTDL model data: {this.dtdlModelPath}");
                        this.modelDataLoadStatusText.text = "Failed to reload model data!";
                    }
                }
            }
            catch (Exception e)
            {
                this.modelDataLoadStatusText.text = "Failed to load model data.";
                Debug.LogError($"Failed to load DTDL files from {this.dtdlModelPath}");
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

        public void OnModelUpdateEvent()
        {
            // nothing to do
        }


        // protected

        protected override void InitMessageHandler()
        {
            try
            {
                if (this.liveDataStatusDisplay != null)
                {
                    this.liveDataStatusText = this.liveDataStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.liveDataEnableButtonObject != null)
                {
                    this.startLiveDataFeedButton = this.liveDataEnableButtonObject.GetComponent<Button>();
                    
                    if (this.startLiveDataFeedButton != null)
                    {
                        this.startLiveDataFeedButton.onClick.AddListener(() => StartLiveDataFeed(true));
                        this.startLiveDataFeedButtonText =
                            this.startLiveDataFeedButton.GetComponentInChildren<TextMeshProUGUI>();
                    }    
                }

                if (this.simDataStatusDisplay != null)
                {
                    this.simDataStatusText = this.simDataStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.simDataEnableButtonObject != null)
                {
                    this.startSimDataFeedButton = this.simDataEnableButtonObject.GetComponent<Button>();

                    if (this.startSimDataFeedButton != null)
                    {
                        this.startSimDataFeedButton.onClick.AddListener(() => StartSimDataFeed(true));
                        this.startSimDataFeedButtonText =
                            this.startSimDataFeedButton.GetComponentInChildren<TextMeshProUGUI>();
                    }
                }

                if (this.modelDataLoadStatusDisplay != null)
                {
                    this.modelDataLoadStatusText = this.modelDataLoadStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.filePathEntryStatusDisplay != null)
                {
                    this.filePathEntryText = this.filePathEntryStatusDisplay.GetComponent<TextMeshProUGUI>();

                    if (this.filePathEntryText != null)
                    {
                        this.filePathEntryText.text = this.dtdlModelPath;
                    }
                }

                if (this.modelLoadButtonObject != null)
                {
                    this.loadModelDataButton = this.modelLoadButtonObject.GetComponent<Button>();

                    if (this.loadModelDataButton != null)
                    {
                        this.loadModelDataButton.onClick.AddListener(() => this.InitializeAndLoadModelData());
                        this.loadModelDataButtonText =
                            this.loadModelDataButton.GetComponentInChildren<TextMeshProUGUI>();
                    }
                }

                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
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
            if (data != null)
            {
                String jsonData = DataUtil.ActuatorDataToJson(data);
            }
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (data != null)
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.liveDataStatusText != null) this.liveDataStatusText.text = connStateMsg;
            }
        }

        protected override void ProcessMessageData(MessageData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null)
            {
                // nothing to do
            }
        }

    }
}

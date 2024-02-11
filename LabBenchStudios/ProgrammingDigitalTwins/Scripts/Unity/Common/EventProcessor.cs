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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Unity.Common
{
    /**
     * This class handles the registration of various event listeners and
     * the distribution of incoming events (of various types) to all
     * registered event listeners.
     * 
     * It is designed to be instanced once by the system manager, and then
     * accessed via the Singleton-like 'GetInstance()' methods.
     * 
     * It is NOT designed to be used across scenes (yet).
     * 
     */
    public class EventProcessor : MonoBehaviour, ISystemStatusEventListener
    {
        private static string _GUID = null;
        private static bool _IS_TERMINATED = false;

        private static System.Object _LOCK_OBJ = new System.Object();

        private static EventProcessor _INSTANCE = CreateInstance();

        private static EventProcessor CreateInstance()
        {
            lock (_LOCK_OBJ)
            {
                if (_IS_TERMINATED)
                {
                    return null;
                }

                if (_INSTANCE == null)
                {
                    Debug.Log("Creating instance of EventProcessor.");

                    _GUID = System.Guid.NewGuid().ToString();

                    return new EventProcessor();
                }
                else
                {
                    return _INSTANCE;
                }
            }
        }

        public static EventProcessor GetInstance()
        {
            return _INSTANCE;
        }


        // private member vars

        private List<IDataContextEventListener> dataContextEventListenerList = null;
        private List<ISystemStatusEventListener> systemStatusEventListenerList = null;


        // constructors

        private EventProcessor()
        {
            this.dataContextEventListenerList = new List<IDataContextEventListener>();
            this.systemStatusEventListenerList = new List<ISystemStatusEventListener>();
        }


        // instance methods

        public void OnDestroy()
        {
            _IS_TERMINATED = true;
            _INSTANCE = null;
        }

        public void ClearAllListeners()
        {
            this.dataContextEventListenerList.Clear();
            this.systemStatusEventListenerList.Clear();
        }

        public string GetGuid()
        {
            return _GUID;
        }

        public void RegisterListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.dataContextEventListenerList.Add(listener);
            }
        }

        public void RegisterListener(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.systemStatusEventListenerList.Add(listener);
            }
        }

        public void LogDebugMessage(string message)
        {
            if (! string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogDebugMessage(message);
                    }
                }
            }
        }

        public void LogWarningMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogWarningMessage(message);
                    }
                }
            }
        }

        public void LogErrorMessage(string message, Exception ex)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogErrorMessage(message, ex);
                    }
                }
            }
        }

        public void OnMessagingSystemDataReceived(ActuatorData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.dataContextEventListenerList)
                {
                    listener.HandleActuatorData(data);
                }
            }
        }

        public void OnMessagingSystemDataReceived(ConnectionStateData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.systemStatusEventListenerList)
                {
                    listener.OnMessagingSystemDataReceived(data);
                }
            }
        }

        public void OnMessagingSystemDataReceived(SensorData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.dataContextEventListenerList)
                {
                    listener.HandleSensorData(data);
                }
            }
        }

        public void OnMessagingSystemDataReceived(SystemPerformanceData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.dataContextEventListenerList)
                {
                    listener.HandleSystemPerformanceData(data);
                }
            }
        }

        public void OnMessagingSystemDataSent(ConnectionStateData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.systemStatusEventListenerList)
                {
                    listener.OnMessagingSystemDataSent(data);
                }
            }
        }

        public void OnMessagingSystemStatusUpdate(ConnectionStateData data)
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.systemStatusEventListenerList)
                {
                    listener.OnMessagingSystemStatusUpdate(data);
                }
            }
        }


        // private methods


    }
}


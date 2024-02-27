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
using UnityEngine.InputSystem;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Unity.Model
{
    public class PrefabModelContext : BaseAsyncDataMessageProcessor
    {
        protected override void InitMessageHandler()
        {
            throw new System.NotImplementedException();
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void ProcessMessageData(MessageData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void ProcessSensorData(SensorData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            throw new System.NotImplementedException();
        }

        private void Awake()
        {
        }

        void OnEnable()
        {
        }

        void Update()
        {
        }
    }
}
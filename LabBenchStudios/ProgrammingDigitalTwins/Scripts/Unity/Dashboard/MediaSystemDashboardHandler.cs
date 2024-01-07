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
    public class MediaSystemDashboardHandler : BaseAsyncDataMessageProcessor
    {
        [SerializeField]
        private GameObject AudioVolumeDisplay = null;

        [SerializeField]
        private GameObject RoomNameDisplay = null;

        private TMP_Text audioVolumeLog = null;


        // protected

        protected override void InitMessageHandler()
        {
            try
            {
                this.audioVolumeLog = this.AudioVolumeDisplay.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize media data display text. Continuing without display data.");
            }
        }

        protected override void ProcessActuatorData(ActuatorData data)
        {
            // nothing to do
        }

        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            // nothing to do
        }

        protected override void ProcessMessageData(MessageData data)
        {
            // nothing to do
        }

        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null && this.audioVolumeLog != null)
            {
                int typeID = data.GetDeviceType();

                if (typeID == ConfigConst.MEDIA_DEVICE_TYPE)
                {
                    double cookedVal = Math.Round(data.GetValue(), 1);
                    this.audioVolumeLog.text = cookedVal.ToString();
                }
            }
        }

        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // nothing to do
        }

    }
}

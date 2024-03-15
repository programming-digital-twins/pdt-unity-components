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

using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using System.Globalization;

/**
 * Controller for managing impeller turbine rotational speed.
 * 
 */
namespace LabBenchStudios.Pdt.Unity.Controller
{
    public class PropertyUpdateHandler : MonoBehaviour, IPropertyManagementController
    {
        [SerializeField]
        private GameObject propertyLabelObject = null;

        [SerializeField]
        private GameObject propertyValueObject = null;

        [SerializeField]
        private GameObject propertyToggleObject = null;

        [SerializeField]
        private GameObject propertyMessageObject = null;

        private TMP_Text propertyLabel = null;
        private TMP_Text propertyMessage = null;
        private Text propertyValue = null;
        private Toggle propertyToggle = null;

        private float value = 0.0f;
        private int command = 0;
        private bool isSelected = false;
        private string msgState = ConfigConst.NOT_SET;

        private IotDataContext dataContext = null;

        private string name = ConfigConst.NOT_SET;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;
        private int typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        private DigitalTwinProperty digitalTwinProperty;

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            if (this.propertyLabelObject != null)
            {
                this.propertyLabel = this.propertyLabelObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.propertyMessageObject != null)
            {
                this.propertyMessage = this.propertyMessageObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.propertyToggleObject != null)
            {
                this.propertyToggle = this.propertyToggleObject.GetComponent<Toggle>();
            }

            if (this.propertyValueObject != null)
            {
                this.propertyValue = this.propertyValueObject.GetComponent<Text>();
            }

            this.UpdateLocalProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLabel()
        {
            return this.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return this.msgState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCommand()
        {
            if (this.digitalTwinProperty != null)
            {
                return this.digitalTwinProperty.IsCommand();
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return this.isSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActuatorData GenerateCommand()
        {
            this.UpdateLocalProperties();

            /*
            ActuatorData data =
                new ActuatorData(this.name, this.deviceID, this.typeCategoryID, this.typeID);
            */

            ActuatorData data = new ActuatorData();

            data.UpdateData(this.dataContext);
            data.SetCommand(this.command);
            data.SetValue(this.value);

            Debug.Log($"Generated Outgoing Command: {data}");

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DigitalTwinProperty GetDigitalTwinProperty()
        {
            this.UpdateLocalProperties();

            return this.digitalTwinProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetPropertyJson()
        {
            Debug.LogWarning("Not yet implemented.");

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digitalTwinProperty"></param>
        public void InitPropertyController(DigitalTwinModelState modelState, DigitalTwinProperty digitalTwinProperty)
        {
            if (digitalTwinProperty != null)
            {
                this.digitalTwinProperty = digitalTwinProperty;
            }

            if (modelState != null)
            {
                this.name = modelState.GetName();
                this.deviceID = modelState.GetDeviceID();
                this.locationID = modelState.GetLocationID();
                this.typeCategoryID = modelState.GetTypeCategoryID();
                this.typeID = modelState.GetTypeID();

                Debug.Log(
                    "Props state:" +
                    $"\n\tname:          {this.name}" +
                    $"\n\tDeviceID:      {this.deviceID}" +
                    $"\n\tlocationID:    {this.locationID}" +
                    $"\n\typeCategoryID: {this.typeCategoryID}" +
                    $"\n\typeID:         {this.typeID}");

                this.dataContext =
                    new IotDataContext(this.name, this.deviceID, this.typeCategoryID, this.typeID);
            }

            this.UpdateLocalProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void ValidateCommandResponse(ActuatorData data)
        {
            if (data != null)
            {
                // TODO: implement this
            }
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLocalProperties()
        {
            /*
            if (this.propertyLabel != null)
            {
                this.name = this.propertyLabel.text;
            }
            */

            if (this.propertyMessage != null)
            {
                this.msgState = this.propertyMessage.text;
            }

            if (this.propertyToggle != null)
            {
                this.isSelected = this.propertyToggle.isOn;

                if (this.isSelected)
                {
                    this.command = ConfigConst.COMMAND_ON;
                }
                else
                {
                    this.command = ConfigConst.COMMAND_OFF;
                }
            }

            if (this.propertyValue != null && ! string.IsNullOrEmpty(this.propertyValue.text))
            {
                string valueStr = this.propertyValue.text.Trim();

                try
                {
                    if (float.TryParse(valueStr, out float f)) { this.value = f; }
                    if (double.TryParse(valueStr, out double d)) { this.value = (float) d; }
                    if (int.TryParse(valueStr, out int i)) { this.value = (float) i; }

                    Debug.Log($"Attempted to parse {this.propertyValue.text} into {this.value}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Can't parse value entry - not a float: --{valueStr}--\n{e}");
                }
            }
        }

    }

}

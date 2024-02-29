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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;
using System.IO;

namespace LabBenchStudios.Pdt.Unity.Common
{
    public static class DigitalTwinUtil
    {
        //////////
        // 
        // Consts and utility (static) methods for values / paths / etc.
        //
        //
        public static readonly string RELATIVE_PDT_PATH =
            "/LabBenchStudios/ProgrammingDigitalTwins";

        public static readonly string RELATIVE_DATA_PATH =
            DigitalTwinUtil.RELATIVE_PDT_PATH + "/Data";

        public static readonly string RELATIVE_STATE_DATA_PATH =
            DigitalTwinUtil.RELATIVE_DATA_PATH + "/State";

        public static readonly string RELATIVE_MODELS_PATH =
            DigitalTwinUtil.RELATIVE_PDT_PATH + "/Models";

        public static readonly string RELATIVE_DTDL_MODELS_PATH =
            DigitalTwinUtil.RELATIVE_MODELS_PATH + "/Dtdl";

        public static readonly string STATE_DATA_EXT = ".dat";

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <returns>The absolute path as a string</returns>
        public static string GetDtdlModelsPath()
        {
            string path = Application.dataPath + DigitalTwinUtil.RELATIVE_DTDL_MODELS_PATH;

            if (! Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);

                if (di != null)
                {
                    Debug.Log($"Created DTDL model path directory {di.FullName} at {di.CreationTime}");
                }
                else
                {
                    Debug.LogError($"Failed to create DTDL model path directory {path}. DTDL JSON will not load properly.");
                }
            }

            return path;
        }

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <returns>The absolute path as a string</returns>
        public static string GetStateDataPath()
        {
            string path = Application.dataPath + DigitalTwinUtil.RELATIVE_STATE_DATA_PATH;

            if (! Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);

                if (di != null)
                {
                    Debug.Log($"Created state data path directory {di.FullName} at {di.CreationTime}");
                }
                else
                {
                    Debug.LogError($"Failed to create state data path directory {path}. System updates will not be saved!");
                }
            }

            return path;
        }

    }
}

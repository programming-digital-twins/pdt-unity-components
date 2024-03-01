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
using System;

namespace LabBenchStudios.Pdt.Unity.Model
{
    public class DigitalTwinCollisionHandler : MonoBehaviour
    {
        [SerializeField]
        private string playerTagName = "Player";

        [SerializeField]
        private GameObject digitalTwinDisplayMgr = null;

        [SerializeField]
        private bool fadeFast = true;

        private Canvas digitalTwinDisplayMgrCanvas = null;
        private CanvasGroup digitalTwinDisplayMgrCanvasGroup = null;

        private bool hasDisplayCanvas = false;
        private bool hasDisplayCanvasGroup = false;

        private bool fadeInCanvasGroup   = false;
        private bool fadeOutCanvasGroup  = false;

        private float fadeNormalMultiple = 1.0f;
        private float fadeInMultiple     = 2.0f;
        private float fadeOutMultiple    = 5.0f;

        private void Awake()
        {
            if (this.digitalTwinDisplayMgr != null)
            {
                this.digitalTwinDisplayMgrCanvas = this.digitalTwinDisplayMgr.GetComponent<Canvas>();
                this.digitalTwinDisplayMgrCanvasGroup = this.digitalTwinDisplayMgr.GetComponent<CanvasGroup>();

                if (this.digitalTwinDisplayMgrCanvas != null )
                {
                    this.hasDisplayCanvas = true;
                    this.digitalTwinDisplayMgrCanvas.enabled = false;
                }

                if (this.digitalTwinDisplayMgrCanvasGroup != null)
                {
                    this.hasDisplayCanvasGroup = true;
                    this.digitalTwinDisplayMgrCanvasGroup.alpha = 0;
                }
            }
        }

        private void OnTriggerEnter(Collider exogenousCollider)
        {
            Debug.Log($"Trigger ENTER called: {exogenousCollider.tag}");

            if (exogenousCollider.tag.Equals(this.playerTagName))
            {
                Debug.Log($"Enabling Digital Twin HUD...");

                // enable the canvas straight away - it will fade in shortly
                if (this.hasDisplayCanvas)
                {
                    this.digitalTwinDisplayMgrCanvas.enabled = true;
                }

                if (this.hasDisplayCanvasGroup)
                {
                    this.fadeInCanvasGroup = true;
                }
            }
        }

        private void OnTriggerExit(Collider exogenousCollider)
        {
            Debug.Log($"Trigger EXIT called: {exogenousCollider.tag}");

            if (exogenousCollider.tag.Equals(this.playerTagName))
            {
                Debug.Log($"Disabling Digital Twin HUD...");

                // wait to disable the canvas until fade out is complete
                if (this.hasDisplayCanvasGroup)
                {
                    this.fadeOutCanvasGroup = true;
                }
            }
        }

        private void Update()
        {
            if (this.hasDisplayCanvasGroup)
            {
                if (this.fadeInCanvasGroup &&
                    this.digitalTwinDisplayMgrCanvasGroup.alpha < 1)
                {
                    float fadeMultiple =
                        (this.fadeFast) ? this.fadeInMultiple : this.fadeNormalMultiple;

                    this.digitalTwinDisplayMgrCanvasGroup.alpha +=
                        (Time.deltaTime * fadeMultiple);

                    if (this.digitalTwinDisplayMgrCanvasGroup.alpha >= 1)
                    {
                        this.fadeInCanvasGroup = false;
                        this.digitalTwinDisplayMgrCanvasGroup.alpha = 1;
                    }
                }

                if (this.fadeOutCanvasGroup &&
                    this.digitalTwinDisplayMgrCanvasGroup.alpha >= 0)
                {
                    float fadeMultiple =
                        (this.fadeFast) ? this.fadeOutMultiple : this.fadeNormalMultiple;

                    this.digitalTwinDisplayMgrCanvasGroup.alpha -=
                        (Time.deltaTime * fadeMultiple);

                    if (this.digitalTwinDisplayMgrCanvasGroup.alpha <= 0)
                    {
                        this.fadeOutCanvasGroup = false;
                        this.digitalTwinDisplayMgrCanvasGroup.alpha = 0;

                        // fade out complete, so disable the canvas
                        // it will be re-enabled (with alpha == 0)
                        // when the collider is triggered next
                        if (this.hasDisplayCanvas)
                        {
                            this.digitalTwinDisplayMgrCanvas.enabled = false;
                        }
                    }
                }
            }
        }

    }
}

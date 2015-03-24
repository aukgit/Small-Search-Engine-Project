using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SearchEngine.Modules {
    class FlashPropertise {

        private AxShockwaveFlashObjects.AxShockwaveFlash flash;

        public AxShockwaveFlashObjects.AxShockwaveFlash FlashObject {
            get {
                return flash;
            }
            set {
                flash = value;
            }
        }

        public static void addListItemStatic(string Label, string Location, AxShockwaveFlashObjects.AxShockwaveFlash flx) {
            string req = "<invoke name='addListItem' returnType='void'><arguments><string>" + Label + "</string><string>" + Location + "</string></arguments></invoke>";
            CallFunction(req, flx);
        }
        public void addListItem(string Label, string Location) {
            string req = "<invoke name='addListItem' returnType='void'><arguments><string>" + Label + "</string><string>" + Location + "</string></arguments></invoke>";
            CallFunction(req);
        }
        public string getListItemAt(int Index) {
            string req = "<invoke name='getListItemAt' returnType='String'><arguments><int>" + Index.ToString() + "</int></arguments></invoke>";
            return CallFunctionReturn(req);
        }

        public void clearList() {
            string req = "<invoke name='clearListItem' returnType='void'></invoke>";
            CallFunction(req);
        }



        public FlashPropertise(AxShockwaveFlashObjects.AxShockwaveFlash flashx = null) {
            flash = flashx;
        }

        #region Flash Functions

        public string GetSearchText() {
            string req = "<invoke name='getSearchText' returnType='String'></invoke>";
            return CallFunctionReturn(req);
        }

        

        /// <summary>
        /// Call flash inside methods in a thread safe way.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="flash"></param>
        public void CallFunction(string req) {
            if (flash == null) {
                return;
            }

            if (flash.InvokeRequired) {
                flash.Invoke(new MethodInvoker(delegate { flash.CallFunction(req); }));
                return;
            }
            flash.CallFunction(req);
        }

        /// <summary>
        /// Call flash inside methods in a thread safe way.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="flash"></param>
        public string CallFunctionReturn(string req) {
            if (flash == null) {
                return "";
            }

            string str = "";

            if (flash.InvokeRequired) {
                flash.Invoke(new MethodInvoker(delegate { str = flash.CallFunction(req); }));
                return str;
            }
            str = flash.CallFunction(req);

            return str;
        }

        /// <summary>
        /// Call indeterminate progress to 
        /// show on the flash screen..
        /// mostly when processing folders.
        /// It will also start the global timn counter in flash.
        /// </summary>
        /// <param name="label">Name of Processing</param>
        /// <param name="flash"></param>
        public void StartInditerminateSateProcessing(string label) {
            //string requestHideInditerminateProgressor = "<invoke name='hideIndtProgressor' returnType='void'></invoke>";
            new Thread(() => {
                string requestInditerminateProgressor = "<invoke name='setIndtProgressor' returnType='void'><arguments><string>" + label + "</string></arguments></invoke>";
                CallFunction(requestInditerminateProgressor);
            }).Start();
        }

        /// <summary>
        /// Call indeterminate progress to stop 
        /// show on the flash screen
        /// </summary>
        /// <param name="flash"></param>
        public void StopInditerminateSateProcessing() {
            new Thread(() => {

                string requestHideInditerminateProgressor = "<invoke name='hideIndtProgressor' returnType='void'></invoke>";
                CallFunction(requestHideInditerminateProgressor);
            }).Start();
        }

        /// <summary>
        /// Stop Global Timer Count.
        /// </summary>
        /// <param name="flash"></param>
        public void FlashStopGlobalTimer() {
            new Thread(() => {

                string request = "<invoke name='stopGlobalTimer' returnType='void'></invoke>";
                CallFunction(request);
            }).Start();
        }

        /// <summary>
        /// set progress in the flash player.
        /// If no flash player then do not set the progress.
        /// </summary>
        /// <param name="value">between 0 and 100, 0 or 100 = hide</param>
        public void setProgress(float value) {
            new Thread(() => {
                string request = "<invoke name='setBarProgress' returnType='void'><arguments><number>" + value.ToString() + "</number></arguments></invoke>";
                CallFunction(request);
            }).Start();
        }

        /// <summary>
        /// Call flash inside methods in a thread safe way.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="flash"></param>
        public static void CallFunction(string req, AxShockwaveFlashObjects.AxShockwaveFlash flashobj2 = null) {
            if (flashobj2 == null) {
                return;
            }

            if (flashobj2.InvokeRequired) {
                flashobj2.Invoke(new MethodInvoker(delegate { flashobj2.CallFunction(req); }));
                return;
            }
            flashobj2.CallFunction(req);
        }

        /// <summary>
        /// set progress in the flash player.
        /// If no flash player then do not set the progress.
        /// </summary>
        /// <param name="value">between 0 and 100, 0 or 100 = hide</param>
        public static void setProgress(float value, AxShockwaveFlashObjects.AxShockwaveFlash flashobj2 = null) {
            //new Thread(() => {
            string request = "<invoke name='setBarProgress' returnType='void'><arguments><number>" + value.ToString() + "</number></arguments></invoke>";
            CallFunction(request, flashobj2);
            //}).Start();
        }
        #endregion

    }
}

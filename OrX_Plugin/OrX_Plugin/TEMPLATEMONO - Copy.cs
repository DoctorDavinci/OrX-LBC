using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSP.UI;
using UnityEngine;
using KSP.UI.Screens;
using System.IO;
using System.Reflection;
using BDArmory;
using BDArmory.Control;


namespace OrX
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class TEMPLATEMONO : MonoBehaviour
    {

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static TEMPLATEMONO instance;

        private void Awake()
        {
            /*
            GameEvents.onPhysicsEaseStart.Add(CheckStartingVessel);
            GameEvents.onVesselChange.Add(VesselCheck);

            if (instance) Destroy(instance);
            instance = this;
            */
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {

            }
        }



        #region Core
        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param name="msg"></param>
        /// 

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 8, ScreenMessageStyle.UPPER_CENTER));
        }

        #endregion

    }
}


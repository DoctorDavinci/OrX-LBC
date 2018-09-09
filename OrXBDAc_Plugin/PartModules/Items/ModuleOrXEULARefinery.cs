using OrXBDAc.missions;
using System.Collections;
using UnityEngine;

namespace OrXBDAc.parts
{
    public class ModuleOrXEULARefinery : PartModule
    {

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "EXTRACT SALT"),
         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool extractSalt = false;

        private bool triggered = false;


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "GET GPS"),
         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "", enabledText = "")]
        public bool getGPS = false;

        //---------------------------------------------------------------------

        [KSPAction("EULA Refinery Toggle")]
        public void actionToggleEULAR(KSPActionParam param)
        {
            extractSalt = !extractSalt;
            triggered = false;
        }

        [KSPAction("EULA Refinery On")]
        public void actionEULAROn(KSPActionParam param)
        {
            extractSalt = true;
            triggered = false;
        }

        [KSPAction("EULA Refinery Off")]
        public void actionEULAROff(KSPActionParam param)
        {
            extractSalt = false;
        }

        [KSPAction("GET GPS")]
        public void actionGetGPS(KSPActionParam param)
        {
            GetGPS();
        }

        //---------------------------------------------------------------------

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && this.vessel.Landed)
            {
                if (extractSalt)
                {
                    if (!triggered)
                    {
                        OrX_Log.instance.drillPresent = true;
                        triggered = true;
                        StartCoroutine(AddSaltRoutine());

                    }
                }
                else
                {
                    if (triggered)
                    {
                        OrX_Log.instance.drillPresent = false;
                        triggered = false;
                    }
                }

                if (getGPS)
                {
                    GetGPS();
                }
            }
        }

        IEnumerator AddSaltRoutine()
        {

            yield return new WaitForSeconds(2);

            GenerateSalt();
            triggered = false;

        }

        private void GenerateSalt()
        {
            int saltToAdd = 0;

            float RequiredORE = Time.deltaTime * 10;
            float _AcquiredORE = part.RequestResource("Ore", RequiredORE);

            if (_AcquiredORE * 1000 <= 1)
            {
                saltToAdd = 1;
            }

            KerbinMissions.instance.saltTotal += saltToAdd;

        }

        private void GetGPS()
        {
            double lat = this.vessel.latitude;
            double lon = this.vessel.longitude;
            double alt = this.vessel.altitude;

            Debug.Log("[OrX EULA Refinery] GPS Latitude: " + lat);
            Debug.Log("[OrX EULA Refinery] GPS Longitude: " + lon);
            Debug.Log("[OrX EULA Refinery] GPS Altitude: " + alt);

            ScreenMsg("[OrX EULA Refinery] GPS Latitude: " + lat);
            ScreenMsg("[OrX EULA Refinery] GPS Longitude: " + lon);
            ScreenMsg("[OrX EULA Refinery] GPS Altitude: " + alt);

            getGPS = false;
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 3, ScreenMessageStyle.UPPER_CENTER));
        }

    }
}
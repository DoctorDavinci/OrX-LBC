﻿using BDArmory.Core.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OrX.missions;

namespace OrX.parts
{
    public class ModuleOrXTardis : PartModule
    {
        private static string modName = "[ModuleOrXTardis]";

        KeyCode cam;

        public bool triggered = false;

        [KSPField(isPersistant = true)]
        public bool launchSiteChanged = false;
        [KSPField(isPersistant = true)]
        public bool KSC = false;
        [KSPField(isPersistant = true)]
        public bool WaldosIsland = false;
        [KSPField(isPersistant = true)]
        public bool Baikerbanur = false;
        [KSPField(isPersistant = true)]
        public bool Pyramids = false;

        public int salt = 0;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "GET GPS"),
         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "", enabledText = "")]
        public bool getGPS = false;

        public override void OnStart(StartState state)
        {
            part.force_activate();
            GameEvents.onVesselWasModified.Add(ReconfigureEvent);
            recalcSurfaceArea();
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (getGPS)
                {
                    GetGPS();
                }
            }
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && !this.vessel.HoldPhysics)
            {
                if (launchSiteChanged && !triggered)
                {
                    triggered = true;
                    launchSiteChanged = false;
                    
                    StartTravel();
                }

                if (IsTransitioning())
                {
                    recalcCloak = false;
                    calcNewCloakLevel();

                    foreach (Part p in vessel.parts)
                    {
                        if (selfCloak || (p != part))
                        {
                            p.SetOpacity(visiblilityLevel);
                            SetRenderAndShadowStates(p, visiblilityLevel > shadowCutoff, visiblilityLevel > RENDER_THRESHOLD);
                        }
                    }
                }
            }
            base.OnUpdate();
        }

        private int count = 0;

        public void OnDestroy()
        {
            GameEvents.onVesselWasModified.Remove(ReconfigureEvent);
        }

        public void StartTravel()
        {
            OrXVesselSwitcher.instance.missions = true;
            OrXVesselSwitcher.instance.missionRunning = true;
            Missions.instance.spawnCount = 0;
            vessel.ctrlState.mainThrottle = 0.5f;
            StartCoroutine(TravelTimer());
        }

        IEnumerator TravelTimer()
        {
            OrX_Log.instance.LockKeyboard();
            cam = GameSettings.CAMERA_MODE.primary.code;
            GameSettings.CAMERA_MODE.primary.code = KeyCode.None;
            yield return new WaitForSeconds(5);
            StartEngines();
            vessel.ctrlState.mainThrottle = 0.5f;
            cloakOn = true;
            UpdateCloakField(null, null);
            yield return new WaitForSeconds(4);
            IVA(FlightGlobals.ActiveVessel);
            Travel();
            yield return new WaitForSeconds(5);
            CameraManager.Instance.SetCameraFlight();
            yield return new WaitForSeconds(1);
            cloakOn = false;
            UpdateCloakField(null, null);
            yield return new WaitForSeconds(4);
            StopEngines();
            GameSettings.CAMERA_MODE.primary.code = cam;
            OrX_Log.instance.UnlockKeyboard();
        }

        public void Travel()
        {
            if (KSC)
            {
                KSC = false;
                Missions.instance.KSC = true;
                MissionLauncher.instance.LaunchToKSC();
            }

            if (WaldosIsland)
            {
                WaldosIsland = false;
                Missions.instance.waldosIsland = true;
                MissionLauncher.instance.LaunchToWaldosIsland();
            }

            if (Baikerbanur)
            {
                Baikerbanur = false;
                Missions.instance.Baikerbanur = true;
                MissionLauncher.instance.LaunchToBaikerbanur();
            }

            if (Pyramids)
            {
                Pyramids = false;
                Missions.instance.Pyramids = true;
                MissionLauncher.instance.LaunchToPyramids();
            }
        }

        private void StartEngines()
        {
            if  (HighLogic.LoadedSceneIsFlight)
            {
                var enginesFX = this.part.FindModuleImplementing<ModuleEnginesFX>();
                var anim = this.part.FindModuleImplementing<FXModuleAnimateThrottle>();

                if (enginesFX != null)
                {
                    enginesFX.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesFX.currentThrottle = 1;
                    enginesFX.Activate();
                    vessel.ctrlState.mainThrottle = 1;
                }
            }
        }

        private void StopEngines()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                var enginesFX = this.part.FindModuleImplementing<ModuleEnginesFX>();

                if (enginesFX != null)
                {
                    enginesFX.ShutdownAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesFX.currentThrottle = 0;
                    enginesFX.Shutdown();
                    vessel.ctrlState.mainThrottle = 0;

                }
            }
        }

        private void GetGPS()
        {
            double lat = this.vessel.latitude;
            double lon = this.vessel.longitude;
            double alt = this.vessel.altitude;

            count += 1;

            Debug.Log("[OrX Tardis] Location #" + count);
            Debug.Log("[OrX Tardis] GPS Latitude: " + lat);
            Debug.Log("[OrX Tardis] GPS Longitude: " + lon);
            Debug.Log("[OrX Tardis] GPS Altitude: " + alt);

            ScreenMsg("[OrX Tardis] GPS Latitude: " + lat);
            ScreenMsg("[OrX Tardis] GPS Longitude: " + lon);
            ScreenMsg("[OrX Tardis] GPS Altitude: " + alt);

            getGPS = false;
        }

        #region IVA

        Kerbal EVAKerbal = null;

        List<InternalSeat> VesselSeats(Vessel vessel)
        {
            bool _hasOnlyPlaceholder;
            return VesselSeats(vessel, true, out _hasOnlyPlaceholder);
        }

        List<InternalSeat> VesselSeats(Vessel vessel, bool withPlaceholder, out bool hasOnlyPlaceholder)
        {
            int _index = 0;
            hasOnlyPlaceholder = true;
            List<Part> _parts = vessel.parts;
            List<InternalSeat> _result = new List<InternalSeat>();
            for (int _i = _parts.Count - 1; _i >= 0; --_i)
            {
                Part _part = _parts[_i];
                if (_part.internalModel != null)
                {
                    if (_part.internalModel.internalName != "Placeholder" || withPlaceholder)
                    {
                        hasOnlyPlaceholder = false;
                        List<InternalSeat> _seats = _part.internalModel.seats;
                        for (int _j = _seats.Count - 1; _j >= 0; --_j)
                        {
                            InternalSeat _seat = _seats[_j];
                            if (_seat.taken && _seat.kerbalRef != null)
                            {
                                Kerbal _kerbal = _seat.kerbalRef;
                                if (_kerbal.state == Kerbal.States.ALIVE || _kerbal.state == Kerbal.States.NO_SIGNAL)
                                {
                                    if (_part.partInfo.category == PartCategories.Pods)
                                    {
                                        _result.Insert(_index, _seat);
                                        _index++;
                                    }
                                    else
                                    {
                                        _result.Add(_seat);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return _result;
        }

        bool VesselHasSeatTaken(Vessel vessel, out Kerbal IVAKerbal, out bool hasOnlyPlaceholder)
        {
            hasOnlyPlaceholder = true;
            bool _result = false;
            Kerbal _first = null;
            Kerbal _firstPilot = null;
            List<InternalSeat> _seats = VesselSeats(vessel);
            for (int _i = _seats.Count - 1; _i >= 0; --_i)
            {
                Kerbal _kerbal = _seats[_i].kerbalRef;
                if (_first == null)
                {
                    _first = _kerbal;
                }
                if (_firstPilot == null && _kerbal.protoCrewMember.experienceTrait.Title == "Pilot")
                {
                    _firstPilot = _kerbal;
                }
                _result = true;
            }
            IVAKerbal = (_firstPilot != null ? _firstPilot : _first);
            return _result;
        }

        bool VesselHasCrewAlive(Vessel vessel)
        {
            Kerbal _first;
            return VesselHasCrewAlive(vessel, out _first);
        }

        bool VesselHasCrewAlive(Vessel vessel, out Kerbal first)
        {
            bool _crewAreLoaded;
            return VesselHasCrewAlive(vessel, out first, out _crewAreLoaded);
        }

        bool VesselHasCrewAlive(Vessel vessel, out Kerbal first, out bool crewAreLoaded)
        {
            crewAreLoaded = true;
            first = null;
            List<ProtoCrewMember> _crews = vessel.GetVesselCrew();
            for (int _i = _crews.Count - 1; _i >= 0; --_i)
            {
                Kerbal _kerbal = _crews[_i].KerbalRef;
                if (_kerbal.state == Kerbal.States.ALIVE || _kerbal.state == Kerbal.States.NO_SIGNAL)
                {
                    first = _kerbal;
                    crewAreLoaded = true;
                    return true;
                }
                if (_kerbal.state == Kerbal.States.NO_SIGNAL)
                {
                    crewAreLoaded = false;
                }
            }
            return false;
        }

        bool isGoneIVA = false;

        bool KerbalIsOnVessel(Vessel vessel, Kerbal kerbal)
        {
            if (kerbal == null)
            {
                return false;
            }
            return vessel.GetVesselCrew().Contains(kerbal.protoCrewMember);
        }

        private void IVA(Vessel vessel)
        {
            if (!HighLogic.CurrentGame.Parameters.Flight.CanIVA || !FlightGlobals.ready)
            {
                return;
            }

            if (vessel.GetCrewCount() > 0)
            {
                bool _VesselhasOnlyPlaceholder;
                Kerbal _IVAKerbal;
                if (VesselHasSeatTaken(vessel, out _IVAKerbal, out _VesselhasOnlyPlaceholder))
                {
                    if (EVAKerbal != null)
                    {
                        if (vessel.GetVesselCrew().Contains(EVAKerbal.protoCrewMember))
                        {
                            _IVAKerbal = EVAKerbal;
                        }
                        else
                        {
                            EVAKerbal = null;
                        }
                    }
                    if (_IVAKerbal != null)
                    {
                        if (KerbalIsOnVessel(vessel, _IVAKerbal))
                        {
                            isGoneIVA = CameraManager.Instance.SetCameraIVA(_IVAKerbal, true);
                        }
                        else
                        {
                            isGoneIVA = CameraManager.Instance.SetCameraIVA();
                        }
                    }
                    else
                    {
                        isGoneIVA = CameraManager.Instance.SetCameraIVA();
                    }
                }
                else if (_VesselhasOnlyPlaceholder)
                {
                    return;
                }
            }
            else
            {
                return;
            }
            if (isGoneIVA)
            {
                EVAKerbal = null;
            }
        }

        #endregion

        #region Resources

        /// <summary>
        /// Resources
        /// </summary>
        /// 
        protected void drawEC()
        {
            RequiredEC = Time.deltaTime * (1 - visiblilityLevel) * (float)Math.Pow(surfaceAreaToCloak * ECPerSec, areaExponet);

            float AcquiredEC = part.RequestResource("ElectricCharge", RequiredEC);
            if (AcquiredEC < RequiredEC * 0.8f)
            {
                if (vessel.isActiveVessel)
                {
                    ScreenMsg("Not Enough Electrical Charge");
                }
                disengageCloak();
            }

            foreach (var p in vessel.parts)
            {
                double totalAmount = 0;
                double maxAmount = 0;
                PartResource r = p.Resources.Where(n => n.resourceName == "ElectricCharge").FirstOrDefault();
                if (r != null)
                {
                    totalAmount += r.amount;
                    maxAmount += r.maxAmount;
                    if (totalAmount < maxAmount * 0.02)
                    {
                        if (vessel.isActiveVessel)
                        {
                            ScreenMsg("Not Enough Electrical Charge");
                        }
                        disengageCloak();
                        StartCoroutine(CoolDownRoutine());
                    }
                }
            }
        }

        private void checkresourceAvailable()
        {
            foreach (var p in vessel.parts)
            {
                double totalAmount = 0;
                double maxAmount = 0;
                PartResource r = p.Resources.Where(n => n.resourceName == "ElectricCharge").FirstOrDefault();
                if (r != null)
                {
                    totalAmount += r.amount;
                    maxAmount += r.maxAmount;
                    if (totalAmount < maxAmount * 0.2)
                    {
                        resourceAvailable = false;
                    }
                    else
                    {
                        resourceAvailable = true;
                    }
                }
            }
        }

        IEnumerator PauseRoutine()
        {
            pauseRoutine = true;
            yield return new WaitForSeconds(10);
            pauseRoutine = false;
            StartCoroutine(CoolDownRoutine());
        }

        IEnumerator CoolDownRoutine()
        {
            coolDown = true;
            yield return new WaitForSeconds(10);
            coolDown = false;
        }

        #endregion

        #region Cloak
        /// <summary>
        /// Cloak code
        /// </summary>
        /// 

        //[KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Auto Deploy"),
        // UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool cloakOn = false;

        private float maxfade = 0f; // invisible:0 to uncloaked:1
        private float surfaceAreaToCloak = 0.0f;
        private float RequiredEC = 0.0f;

        private static float UNCLOAKED = 1.0f;
        private static float RENDER_THRESHOLD = 0.0f;
        private float fadePerTime = 0.5f;
        private bool currentShadowState = true;
        private bool pauseRoutine;
        private bool coolDown = false;
        private bool resourceAvailable;
        private bool fullRenderHide = true;
        private bool recalcCloak = true;
        private float visiblilityLevel = UNCLOAKED;
        private float areaExponet = 0.5f;
        private float ECPerSec = 1.0f; // Electric charge per second
        private float fadeTime = 3; // In seconds
        private float shadowCutoff = 0.0f;
        private bool selfCloak = true;

        //---------------------------------------------------------------------

        [KSPAction("Cloak Toggle")]
        public void actionToggleCloak(KSPActionParam param)
        {
            cloakOn = !cloakOn;
            UpdateCloakField(null, null);
        }

        [KSPAction("Cloak On")]
        public void actionCloakOn(KSPActionParam param)
        {
            cloakOn = true;
            UpdateCloakField(null, null);
        }

        [KSPAction("Cloak Off")]
        public void actionCloakOff(KSPActionParam param)
        {
            cloakOn = false;
            UpdateCloakField(null, null);
        }

        //---------------------------------------------------------------------

        //---------------------------------------------------------------------



        public void ToggleVoid()
        {
            if (cloakOn)
            {
                cloakOn = false;
                disengageCloak();
            }

            if (!cloakOn)
            {
                cloakOn = true;
                engageCloak();
            }
        }

        public void engageCloak()
        {
            if (vessel.isActiveVessel)
            {
                ScreenMsg("Entering the Void");
            }
            cloakOn = true;
            UpdateCloakField(null, null);
        }

        public void disengageCloak()
        {
            if (cloakOn)
            {
                if (vessel.isActiveVessel)
                {
                    ScreenMsg("Active Camouflage Disengaging");
                }
                cloakOn = false;
                UpdateCloakField(null, null);
            }
        }

        protected void UpdateSelfCloakField(BaseField field, object oldValueObj)
        {
            if (selfCloak)
            {
                SetRenderAndShadowStates(part, visiblilityLevel > shadowCutoff, visiblilityLevel > RENDER_THRESHOLD);
            }
            else
            {
                SetRenderAndShadowStates(part, true, true);
            }
            recalcCloak = true;
        }

        public void UpdateCloakField(BaseField field, object oldValueObj)
        {
            // Update in case its been changed
            calcFadeTime();
            recalcSurfaceArea();
            recalcCloak = true;
        }

        private void calcFadeTime()
        {
            // In case fadeTime == 0
            try
            { fadePerTime = (1 - maxfade) / fadeTime; }
            catch (Exception)
            { fadePerTime = 10.0f; }
        }

        private void recalcSurfaceArea()
        {
            Part p;

            if (vessel != null)
            {
                surfaceAreaToCloak = 0.0f;
                for (int i = 0; i < vessel.parts.Count; i++)
                {
                    p = vessel.parts[i];
                    if (p != null)
                        if (selfCloak || (p != part))
                            surfaceAreaToCloak = (float)(surfaceAreaToCloak + p.skinExposedArea);
                }
            }
        }

        private void SetRenderAndShadowStates(Part p, bool shadowsState, bool renderState)
        {
            if (p.gameObject != null)
            {
                int i;

                MeshRenderer[] MRs = p.GetComponentsInChildren<MeshRenderer>();
                for (i = 0; i < MRs.GetLength(0); i++)
                    MRs[i].enabled = renderState;// || !fullRenderHide;

                SkinnedMeshRenderer[] SMRs = p.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (i = 0; i < SMRs.GetLength(0); i++)
                    SMRs[i].enabled = renderState;// || !fullRenderHide;

                if (shadowsState != currentShadowState)
                {
                    for (i = 0; i < MRs.GetLength(0); i++)
                    {
                        if (shadowsState)
                            MRs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        else
                            MRs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    for (i = 0; i < SMRs.GetLength(0); i++)
                    {
                        if (shadowsState)
                            SMRs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        else
                            SMRs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    currentShadowState = shadowsState;
                }
            }
        }

        private void ReconfigureEvent(Vessel v)
        {
            if (v == null) { return; }

            if (v == vessel)
            {   // This is the cloaking vessel - recalc EC required based on new configuration (unless this is a dock event)
                recalcCloak = true;
                recalcSurfaceArea();
            }
            else
            {   // This is the added/removed part - reset it to normal
                ModuleOrXTardis mc = null;
                foreach (Part p in v.parts)
                    if ((p != null) &&
                        ((p != part) || selfCloak))
                    {
                        //p.setOpacity(UNCLOAKED); // 1.1.3
                        p.SetOpacity(UNCLOAKED); // 1.2.2 and up
                        SetRenderAndShadowStates(p, true, true);
                        Debug.Log(modName + "Uncloak " + p.name);

                        // If the other vessel has a cloak device let it know it needs to do a refresh
                        mc = p.FindModuleImplementing<ModuleOrXTardis>();
                        if (mc != null)
                            mc.recalcCloak = true;
                    }
            }
        }

        protected void calcNewCloakLevel()
        {
            calcFadeTime();
            float delta = Time.deltaTime * fadePerTime;
            if (cloakOn && (visiblilityLevel > maxfade))
                delta = -delta;

            visiblilityLevel = visiblilityLevel + delta;
            visiblilityLevel = Mathf.Clamp(visiblilityLevel, maxfade, UNCLOAKED);
        }

        protected bool IsTransitioning()
        {
            return (cloakOn && (visiblilityLevel > maxfade)) ||     // Cloaking in progress
                   (!cloakOn && (visiblilityLevel < UNCLOAKED)) ||  // Uncloaking in progress
                   recalcCloak;                                     // A forced refresh 
        }
        #endregion

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 3, ScreenMessageStyle.UPPER_LEFT));
        }

    }
}
using System;
using System.Collections;
using UnityEngine;
using OrX.spawn;
using OrX.missions;
using System.Collections.Generic;

namespace OrX.parts
{
    public class ModuleOrXHole : ModuleCommand
    {
//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "ORKIFY NEARBY KERBALS"),
//         UI_Toggle(scene = UI_Scene.All, disabledText = "", enabledText = "")]
//        public bool orkifyKerbals = false;

//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "ORKIFIED"),
//         UI_Toggle(scene = UI_Scene.All, disabledText = "", enabledText = "")]
        public bool orkified = false;

//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "DENY CONTROL"),
//         UI_Toggle(scene = UI_Scene.All, disabledText = "", enabledText = "")]
        public bool denied = false;

//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "EJECT ORX"),
//         UI_Toggle(scene = UI_Scene.All, disabledText = "", enabledText = "")]
        public bool eject = false;

        public int crewCheck = 0;
        private bool seeking = false;
        private int average = 0;
        private bool checkedCrew = false;
        public bool orxDetected = true;

        public string enemyName = string.Empty;
        private string OrXname = "OrX";

        public bool empty = false;

        private ProtoCrewMember kerbal;
        private bool ejecting = false;
        private bool setup = true;
        private bool paused = false;

        GameObject hole = null;
        BoxCollider holeCollider = null;

        [KSPAction("Eject OrX", KSPActionGroup.Abort)]
        public void EjectAction(KSPActionParam param)
        {
            EjectCheck();
        }

        public override void OnStart(StartState state)
        {
            GameEvents.onVesselWasModified.Add(ReconfigureEvent);
            recalcSurfaceArea();
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
                vessel.name = "OrX Hole";

                if (!cloakOn)
                {
                    engageCloak();
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
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

            base.OnFixedUpdate();
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && this.vessel.Landed)
            {
                if (setup)
                {
                    setup = false;
                    CrewCheck();
                }
            }
        }

        public void CrewCheck()
        {
            int random = new System.Random().Next(2, 8);
            int random2 = new System.Random().Next(2, 5);
            average = (random * random2) / 4 * Missions.instance.level;
            Debug.Log("[OrX Hole] OrX to Spawn = " + average);
            StartCoroutine(EjectionRoutine());
            StartCoroutine(UpdateListRoutine());
        }

        IEnumerator UpdateListRoutine()
        {
            yield return new WaitForSeconds(2);
        }


        public void Eject()
        {
            if (cloakOn)
            {
                disengageCloak();
                Debug.Log("[OrX Hole] De-Cloaking ...");
            }

            OrXSpawn.instance.SpawnCoords = this.vessel.GetWorldPos3D();
            OrXSpawn.instance.SpawnOrX();
        }

        IEnumerator EjectionRoutine()
        {
            Eject();
            yield return new WaitForSeconds(1.5f);
            EjectCheck();
        }

        public void EjectCheck()
        {
            average -= 1;

            if (average <= 1)
            {
                Debug.Log("[OrX Hole] Hole is empty ... [EjectCheck();]");
                StartCoroutine(ChekForOrX());
            }
            else
            {
                Debug.Log("[OrX Hole] " + crewCount + " OrX left to Spawn ...");
                StartCoroutine(EjectionRoutine());
            }
        }

        IEnumerator ChekForOrX()
        {
            yield return new WaitForSeconds(30);
            EmptyRoutine();
        }

        public void EmptyRoutine()
        {
            var count = 0;

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                List<ModuleOrX>.Enumerator orx = v.Current.FindPartModulesImplementing<ModuleOrX>().GetEnumerator();
                while (orx.MoveNext())
                {
                    if (orx.Current == null) continue;

                    if (!orx.Current.player)
                    {
                        count += 1;
                        orxDetected = true;
                        StartCoroutine(ChekForOrX());
                        break;
                    }
                }
                orx.Dispose();

            }
            v.Dispose();

            if (count == 0)
            {
                orxDetected = false;
                SpawnLootBox.instance.SpawnCoords = this.part.vessel.GetWorldPos3D();
                SpawnLootBox.instance.CheckSpawnTimer();
                //this.part.Die();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Cloak
        /// <summary>
        /// Cloak code
        /// </summary>
        /// 



        /////////////////////////////////////////////////////////////////////////

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = false)]
        public bool autoDeploy = true;

        private float maxfade = 0.0f; // invisible:0 to uncloaked:1
        private float surfaceAreaToCloak = 0.0f;

        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = false)]
        public bool cloakOn = true;

        private static float UNCLOAKED = 1.0f;
        private static float RENDER_THRESHOLD = 0.0f;
        private float fadePerTime = 0.5f;
        private bool currentShadowState = true;
        private bool recalcCloak = true;
        private float visiblilityLevel = UNCLOAKED;
        private float fadeTime = 2f; // In seconds
        private float shadowCutoff = 0.0f;
        private bool selfCloak = true;


        //////////////////////////////////////////////////////////////////////////////


        public void engageCloak()
        {
            cloakOn = true;
            UpdateCloakField(null, null);
        }

        public void disengageCloak()
        {
            cloakOn = false;
            UpdateCloakField(null, null);
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
                ModuleOrXHole mc = null;
                foreach (Part p in v.parts)
                    if ((p != null) &&
                        ((p != part) || selfCloak))
                    {
                        //p.setOpacity(UNCLOAKED); // 1.1.3
                        p.SetOpacity(UNCLOAKED); // 1.2.2 and up
                        SetRenderAndShadowStates(p, true, true);

                        // If the other vessel has a cloak device let it know it needs to do a refresh
                        mc = p.FindModuleImplementing<ModuleOrXHole>();
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


    }
}

using System.Collections;
using UnityEngine;
using BDArmory;
using BDArmory.Control;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using KSP.UI.Screens;
using BDArmory.UI;
using BDArmory.Modules;
using BDArmory.Core;
using BDArmory.Radar;
using BDArmory.Parts;

namespace OrX.parts
{
    public class ModuleOrXMissileFire : MissileFire
    {
        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>
        public bool setup = true;
        public bool denied = false;
        public int parts = 0;


        IEnumerator SetupRoutine()
        {
            parts = this.vessel.parts.Count;

            Debug.Log("[OrX Module Missile Fire] Weapon Manager ..................... Setup Initiated");

            part.force_activate();

            UpdateList();

            yield return new WaitForFixedUpdate();

            if (weaponArray.Length > 0) selectedWeapon = weaponArray[weaponIndex];

            UpdateMaxGuardRange();

            Debug.Log("[OrX Module Missile Fire] Weapon Manager ....... Adding to BDAc");

            UpdateList();

            yield return new WaitForFixedUpdate();

            Debug.Log("[OrX Module Missile Fire] Weapon Manager ....... Game Events");
            GameEvents.onVesselCreate.Add(OnVesselCreate);
            GameEvents.onPartDie.Add(OnPartDie);
            RefreshModules();

            yield return new WaitForFixedUpdate();
            //ToggleTeam();

            UpdateList();

            yield return new WaitForFixedUpdate();

            var orx = this.vessel.FindPartModuleImplementing<ModuleOrX>();
            if (orx != null)
            {
                if (orx.player)
                {
                    team = false;
                }
                else
                {
                    team = true;
                }
            }

            //ToggleTeam();
            Debug.Log("[OrX Module Missile Fire] Weapon Manager Team = " + team);

        }

        void OnVesselCreate(Vessel v)
        {
            RefreshModules();
        }

        void RefreshModules()
        {
            radars = vessel.FindPartModulesImplementing<ModuleRadar>();

            List<ModuleRadar>.Enumerator rad = radars.GetEnumerator();
            while (rad.MoveNext())
            {
                if (rad.Current == null) continue;
                rad.Current.EnsureVesselRadarData();
                if (rad.Current.radarEnabled) rad.Current.EnableRadar();
            }
            rad.Dispose();

            jammers = vessel.FindPartModulesImplementing<ModuleECMJammer>();
            targetingPods = vessel.FindPartModulesImplementing<ModuleTargetingCamera>();
            wmModules = vessel.FindPartModulesImplementing<IBDWMModule>();
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (setup)
                {
                    setup = false;
                    StartCoroutine(SetupRoutine());
                }

                if (!this.vessel.isEVA)
                {
                    Destroy(this);
                }
            }
        }

        public void Refresh()
        {
            StartCoroutine(SetupRoutine());
        }

        void OnDestroy()
        {
            RefreshModules();
            UpdateList();

            GameEvents.onPartDie.Remove(OnPartDie);
        }

        void OnPartDie(Part p = null)
        {
            if (p == part)
            {
                try
                {
                    GameEvents.onPartDie.Remove(OnPartDie);
                }
                catch (Exception e)
                {
                    if (BDArmorySettings.DRAW_DEBUG_LABELS)
                    {
                        Debug.Log("[BDArmory]: Error OnPartDie: " + e.Message);
                    }
                }
            }
            RefreshModules();
            UpdateList();
        }
    }
}
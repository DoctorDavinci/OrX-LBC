
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using BDArmory.UI;
using BDArmory.Modules;
using BDArmory.Core;

namespace OrXBDAc.parts
{
    public class ModuleOrXMissileFire : MissileFire
    {
        public bool setup = true;
        public bool denied = false;
        public int parts = 0;

        IEnumerator SetupRoutine()
        {
            Debug.Log("[OrX Module Missile Fire] Weapon Manager ..................... Setup Initiated");
            part.force_activate();
            parts = this.vessel.parts.Count;
            UpdateList();

            yield return new WaitForFixedUpdate();

            if (weaponArray.Length > 0) selectedWeapon = weaponArray[weaponIndex];
            UpdateMaxGuardRange();
            UpdateList();

            yield return new WaitForFixedUpdate();

            GameEvents.onVesselCreate.Add(OnVesselCreate);
            GameEvents.onPartDie.Add(OnPartDie);
            RefreshModules();

            yield return new WaitForFixedUpdate();

            UpdateList();

            var orx = this.vessel.FindPartModuleImplementing<ModuleOrXBDAc>();
            if (orx != null)
            {
                if (orx.player)
                {
                    team = false;
                    guardMode = true;
                }
                else
                {
                    team = true;
                    guardMode = true;
                }
            }
            else
            {
                team = true;
                guardMode = true;
            }

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
            //base.OnUpdate();
        }

        public override void OnFixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (!this.vessel.HoldPhysics)
                {
                    if (setup)
                    {
                        setup = false;
                        StartCoroutine(SetupRoutine());
                    }
                    else
                    {
                        if (!this.vessel.isEVA)
                        {
                            Destroy(this);
                        }
                    }
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
using BDArmory.Modules;
using BDArmory.Core.Module;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace OrXBDAc.parts
{
    public class ModuleOrXWeapon : PartModule
    {
        private float hp = 0.0f;
        private float HPcheck = 0.0f;
        private int count = 2;

        private HitpointTracker hpTracker;
        private HitpointTracker GetHP()
        {
            HitpointTracker h = null;

            h = part.FindModuleImplementing<HitpointTracker>();

            return h;
        }

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                hpTracker = GetHP();
                hpTracker.maxHitPoints = 10000;
                hpTracker.Hitpoints = 10000;
                HPcheck = hpTracker.maxHitPoints;
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {           
                if (count == 0)
                {
                    CheckHP();
                }
                else
                {
                    count -= 1;
                }
            }
        }

        private void CheckHP()
        {
            hpTracker = GetHP();
            hp = hpTracker.Hitpoints;
            count = 2;

            if (hp <= 10000)
            {
                var OrX = part.FindModuleImplementing<ModuleOrXBDAc>();

                if (OrX != null)
                {
                    var _hpToRemove = hp - HPcheck;
                    OrX.hpToRemove = _hpToRemove;
                    hpTracker.Hitpoints = hpTracker.maxHitPoints;
                }
            }

            if (this.part.vessel.vesselName.Contains("Debris"))
            {
                this.part.Die();
            }
        }
    }
}
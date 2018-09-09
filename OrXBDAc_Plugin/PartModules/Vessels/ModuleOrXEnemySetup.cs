using System;
using System.Collections;
using System.Collections.Generic;
using BDArmory.Modules;
using UnityEngine;

namespace OrXBDAc.parts
{
    public class ModuleOrXEnemySetup : PartModule
    {
        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
                detonating = false;
            }
            base.OnStart(state);
        }

        private bool detonating = false;

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!detonating)
                {
                    detonating = true;
                    StartCoroutine(CountDownRoutine());
                }
            }
        }

        public int seconds = 1;

        IEnumerator CountDownRoutine()
        {
            foreach (Part p in vessel.parts)
            {
                var engines = p.FindModuleImplementing<ModuleEngines>();
                var enginesFX = p.FindModuleImplementing<ModuleEnginesFX>();

                if (engines != null)
                {
                    p.force_activate();
                    engines.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    engines.Activate();
                }

                if (enginesFX != null)
                {
                    p.force_activate();
                    enginesFX.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesFX.Activate();
                }
            }

            var wmPart = vessel.FindPartModuleImplementing<MissileFire>();
            if (wmPart != null)
            {
                wmPart.part.force_activate();
                wmPart.guardMode = true;
                wmPart.team = true;
            }

            var PAI = vessel.FindPartModuleImplementing<BDModulePilotAI>();
            if (PAI != null)
            {
                PAI.part.force_activate();
                if (!PAI.pilotEnabled)
                {
                    PAI.ActivatePilot();
                }
            }

            var SAI = vessel.FindPartModuleImplementing<BDModulePilotAI>();
            if (SAI != null)
            {
                SAI.part.force_activate();
                if (!SAI.pilotEnabled)
                {
                    SAI.ActivatePilot();
                }
            }
            yield return new WaitForSeconds(seconds);

            Destroy(this);
        }
    }
}

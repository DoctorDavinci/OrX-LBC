using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrXBDAc.parts
{
    public class ModuleOrXDestroyVessel : PartModule
    {
        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
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
            yield return new WaitForSeconds(seconds);
            this.vessel.DestroyVesselComponents();
            this.vessel.Die();
        }
    }
}

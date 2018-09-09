using BDArmory.Modules;
using BDArmory.Core.Module;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace OrX.parts
{
    public class ModuleOrXWeapon : PartModule
    {
        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
            }
            base.OnStart(state);
        }
        /*
        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (this.vessel.parts.Count <= 2)
                {
                    this.part.Die();
                }
            }
        }*/
    }
}
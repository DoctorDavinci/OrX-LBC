
using OrX.parts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrX
{
    public class ModuleOrXEVAWaldo : KerbalEVA
    {

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "RESET KERBAL"),
         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "False", enabledText = "True")]
        public bool setup = true;

        StartState ready;
        
        public override void OnStart(StartState state)
        {
            ready = state;

            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
            }
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (setup)
                {
                    if (setup)
                    {
                        setup = false;
                        StartCoroutine(DelayRoutine());
                    }
                }
            }
            base.OnUpdate();
        }

        IEnumerator DelayRoutine()
        {
            yield return new WaitForSeconds(1);
            base.OnStart(ready);
        }
    }
}

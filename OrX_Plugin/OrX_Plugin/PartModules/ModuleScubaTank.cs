
using System.Collections;
using UnityEngine;

namespace OrX.parts
{
    public class ModuleScubaTank : PartModule
    {

        public float oxygenMax = 1000;
        public float oxygen = 1000;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
            }
            base.OnStart(state);
        }

        public override void OnFixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    if (part.vessel.altitude <= -1)
                    {
                        CheckOxy();
                    }
                }
            }
            base.OnFixedUpdate();
        }


        private void CheckOxy()
        {
            var OrX = part.vessel.FindPartModuleImplementing<ModuleOrX>();
            if (OrX.oxygen <= 100)
            {
                if (oxygen >= 0)
                {
                    var _oxy = 100 - OrX.oxygen;
                    OrX.oxygen += _oxy;
                    oxygen -= _oxy;
                }
            }
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 3f, ScreenMessageStyle.UPPER_CENTER));
        }

        private void ScreenMsg2(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 0.015f, ScreenMessageStyle.UPPER_RIGHT));
        }

    }
}
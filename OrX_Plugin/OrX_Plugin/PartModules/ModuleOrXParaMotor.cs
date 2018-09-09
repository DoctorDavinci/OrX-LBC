
using System.Collections;
using UnityEngine;

namespace OrX.parts
{
    public class ModuleOrXParaMotor : ModuleEngines
    {

//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "GROUND LAUNCH"),
//         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "", enabledText = "True")]
//        public bool groundTakeOff = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "THROTTLE %"),
         UI_FloatRange(controlEnabled = true, scene = UI_Scene.All, minValue = 0, maxValue = 100, stepIncrement = 1f)]
        public float _throttle = 0.0f;

        public float jumpForce = 0.0f;
        private bool takingOff = false;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
                Setup();
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    if (!takingOff)
                    {
                        StartCoroutine(GroundLaunch());
                    }
                    else
                    {
                        GetParaProp();
                    }

                    if (part.vessel.Landed && takingOff)
                    {
                        CheckChute();
                    }
                }
            }
        }

        private void Setup()
        {
            var chute = vessel.FindPartModuleImplementing<ModuleEvaChute>();
            chute.enabled = false;
        }

        private void CheckChute()
        {
            var chute = vessel.FindPartModuleImplementing<ModuleEvaChute>();
            takingOff = false;

            if (chute.enabled)
            {
                chute.AllowRepack(true);
                chute.Repack();
                chute.enabled = false;
            }
        }

        IEnumerator GroundLaunch()
        {
            var kerbal = part.vessel.FindPartModuleImplementing<KerbalEVA>();
            var chute = part.vessel.FindPartModuleImplementing<ModuleEvaChute>();
            
            if (kerbal.maxJumpForce <= 5)
            {
                kerbal.maxJumpForce = 5;
            }
            
            if (!part.vessel.Landed)
            {
                chute.enabled = true;
                takingOff = true;
                _throttle = 100;
                var throttle = _throttle / 100;
                currentThrottle = throttle;
                yield return new WaitForSeconds(1.5f);
                chute.Deploy();
            }
        }

        private void GetParaProp()
        {
            var throttle = _throttle / 100;
            currentThrottle = throttle;
        }
    }
}
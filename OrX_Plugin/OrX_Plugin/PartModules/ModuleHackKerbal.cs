using UnityEngine;
using System.Diagnostics;

namespace OrX.parts
{
    public class ModuleHackKerbal : PartModule
    {
        private KerbalEVA kerbal;
        private KerbalEVA kerbalControl()
        {
            KerbalEVA kControl = null;

            kControl = part.FindModuleImplementing<KerbalEVA>();

            return kControl;
        }

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Splat/Recovery MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool splatRecovery = false;


        #region Recovery and Splat
        /// <summary>
        /// Recovery and Splat
        /// </summary>

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "isRagdoll"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool isRagdoll = false;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "canRecover"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool canRecover = false;
/*
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "recoverTime"),
         UI_FloatRange(minValue = 0.0f, maxValue = 200, stepIncrement = 1f, scene = UI_Scene.All)]
        public double recoverTime = 0.0f;
        */
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "recoverThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float recoverThreshold = 0.0f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "splatEnabled"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool splatEnabled = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "splatSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2000, stepIncrement = 10f, scene = UI_Scene.All)]
        public float splatSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "splatThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2000, stepIncrement = 10f, scene = UI_Scene.All)]
        public float splatThreshold = 0.0f;

        #endregion


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Jetpack MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool jetPack = false;


        #region Jetpack
        /// <summary>
        /// 
        /// </summary>

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "JetpackDeployed"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool JetpackDeployed = false;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "JetpackIsThrusting"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool JetpackIsThrusting = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "PropellantConsumption"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.005f, scene = UI_Scene.All)]
        public float PropellantConsumption = 0.0f;
/*
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "propellantResourceDefaultAmount"),
         UI_FloatRange(minValue = 0.0f, maxValue = 200, stepIncrement = 1f, scene = UI_Scene.All)]
        public double propellantResourceDefaultAmount = 0.0f;
*/
        #endregion


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Bounds MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool bounds = false;

        #region Bounds
        /// <summary>
        /// Bounds
        /// </summary>
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundAttack"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundAttack = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundFallThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundFallThreshold = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundForce"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundForce = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundFrequency"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundFrequency = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundRelease"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundRelease = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundSharpness"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float boundSharpness = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float boundSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "boundThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float boundThreshold = 0.0f;

        #endregion


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kerbal Stats MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool clamber = false;



        #region Clamber and Reach
        /// <summary>
        /// Clamber and Reach
        /// </summary>
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "autoGrabLadderOnStart"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool autoGrabLadderOnStart = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "clamberMaxAlt"),
         UI_FloatRange(minValue = 0.0f, maxValue = 200, stepIncrement = 1f, scene = UI_Scene.All)]
        public float clamberMaxAlt = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "clamberReach"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float clamberReach = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "clamberStandoff"),
         UI_FloatRange(minValue = 0.0f, maxValue = 2, stepIncrement = 0.05f, scene = UI_Scene.All)]
        public float clamberStandoff = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "flagReach"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float flagReach = 0.0f;

        #endregion


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kd Ki Kp iC MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool kdkikpic = false;


        #region Kd Ki Kp
        /// <summary>
        /// Kd Ki Kp
        /// </summary>
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Kd"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.005f, scene = UI_Scene.All)]
        public float Kd = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Ki"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.005f, scene = UI_Scene.All)]
        public float Ki = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Kp"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.005f, scene = UI_Scene.All)]
        public float Kp = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "iC"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.001f, scene = UI_Scene.All)]
        public float iC = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "linPower"),
         UI_FloatRange(minValue = 0.0f, maxValue = 100, stepIncrement = 1f, scene = UI_Scene.All)]
        public float linPower = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "onFallHeightFromTerrain"),
         UI_FloatRange(minValue = 0.0f, maxValue = 5, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float onFallHeightFromTerrain = 0.0f;

        #endregion

/*
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kerbal Stats MENU"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool kerbalStats = false;
*/
        #region Kerbal Stats
        /// <summary>
        /// Kerbal Stats
        /// </summary>
        /// 

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "initialMasSalt"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.125f, scene = UI_Scene.All)]
        public float initialMass = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "massMultiplier"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.01f, scene = UI_Scene.All)]
        public float massMultiplier = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "minWalkingGee"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.01f, scene = UI_Scene.All)]
        public float minWalkingGee = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "minRunningGee"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.01f, scene = UI_Scene.All)]
        public float minRunningGee = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "hopThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 100, stepIncrement = 1f, scene = UI_Scene.All)]
        public float hopThreshold = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "maxJumpForce"),
         UI_FloatRange(minValue = 0.0f, maxValue = 200, stepIncrement = 1f, scene = UI_Scene.All)]
        public float maxJumpForce = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "ladderClimbSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float ladderClimbSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "ladderPushoffForce"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float ladderPushoffForce = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "rotPower"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 1f, scene = UI_Scene.All)]
        public float rotPower = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "runSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 50, stepIncrement = 1f, scene = UI_Scene.All)]
        public float runSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "strafeSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float strafeSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "stumbleThreshold"),
         UI_FloatRange(minValue = 0.0f, maxValue = 200, stepIncrement = 1f, scene = UI_Scene.All)]
        public float stumbleThreshold = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "swimSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float swimSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "turnRate"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float turnRate = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "walkSpeed"),
         UI_FloatRange(minValue = 0.0f, maxValue = 10, stepIncrement = 0.1f, scene = UI_Scene.All)]
        public float walkSpeed = 0.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "waterAngularDragMultiplier"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1, stepIncrement = 0.01f, scene = UI_Scene.All)]
        public float waterAngularDragMultiplier = 0.0f;

        #endregion


        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "UPDATE KERBAL"),
         UI_Toggle(scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool updateKerbal = false;


        public override void OnStart(StartState state)
        {
            GetKerbalControls();
            LogKerbalStats();
            base.OnStart(state);
        }

        public void Update()
        {
            if (updateKerbal)
            {
                UpdateKerbalControls();
            }
            updateButtons();
        }

        private void LogKerbalStats()
        {
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] Getting Kerbal Stats ............................");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] isRagdoll ................... |" + isRagdoll + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] canRecover ................... |" + canRecover + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] recoverThreshold ................... |" + recoverThreshold + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] splatEnabled ................... |" + splatEnabled + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] splatSpeed ................... |" + splatEnabled + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] splatThreshold ................... |" + splatThreshold + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] ......................................................... |");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] JetpackDeployed ................... |" + JetpackDeployed + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] JetpackIsThrusting ................... |" + JetpackIsThrusting + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] PropellantConsumption ................... |" + PropellantConsumption + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] ......................................................... |");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundFallThreshold ................... |" + boundFallThreshold + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundForce ................... |" + boundForce + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundFrequency ................... |" + boundFrequency + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundRelease ................... |" + boundRelease + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundSharpness ................... |" + boundSharpness + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundSpeed ................... |" + boundSpeed + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundThreshold ................... |" + boundThreshold + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] ......................................................... |");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");
            UnityEngine.Debug.Log("[MODULEHACKKERBAL] boundAttack ................... |" + boundAttack + "|");

        }

        private void updateButtons()
        {
            if (splatRecovery)
            {
                Fields["isRagdoll"].guiActiveEditor = true;
                Fields["isRagdoll"].guiActive = true;

                Fields["canRecover"].guiActiveEditor = true;
                Fields["canRecover"].guiActive = true;
/*
                Fields["recoverTime"].guiActiveEditor = true;
                Fields["recoverTime"].guiActive = true;
*/
                Fields["recoverThreshold"].guiActiveEditor = true;
                Fields["recoverThreshold"].guiActive = true;

                Fields["splatEnabled"].guiActiveEditor = true;
                Fields["splatEnabled"].guiActive = true;

                Fields["splatSpeed"].guiActiveEditor = true;
                Fields["splatSpeed"].guiActive = true;

                Fields["splatThreshold"].guiActiveEditor = true;
                Fields["splatThreshold"].guiActive = true;

            }
            else
            {
                Fields["isRagdoll"].guiActiveEditor = false;
                Fields["isRagdoll"].guiActive = false;

                Fields["canRecover"].guiActiveEditor = false;
                Fields["canRecover"].guiActive = false;
/*
                Fields["recoverTime"].guiActiveEditor = false;
                Fields["recoverTime"].guiActive = false;
*/
                Fields["recoverThreshold"].guiActiveEditor = false;
                Fields["recoverThreshold"].guiActive = false;

                Fields["splatEnabled"].guiActiveEditor = false;
                Fields["splatEnabled"].guiActive = false;

                Fields["splatSpeed"].guiActiveEditor = false;
                Fields["splatSpeed"].guiActive = false;

                Fields["splatThreshold"].guiActiveEditor = false;
                Fields["splatThreshold"].guiActive = false;
            }

            if (jetPack)
            {
                Fields["JetpackDeployed"].guiActiveEditor = true;
                Fields["JetpackDeployed"].guiActive = true;

                Fields["JetpackIsThrusting"].guiActiveEditor = true;
                Fields["JetpackIsThrusting"].guiActive = true;

                Fields["PropellantConsumption"].guiActiveEditor = true;
                Fields["PropellantConsumption"].guiActive = true;
/*
                Fields["propellantResourceDefaultAmount"].guiActiveEditor = true;
                Fields["propellantResourceDefaultAmount"].guiActive = true;
*/
            }
            else
            {
                Fields["JetpackDeployed"].guiActiveEditor = false;
                Fields["JetpackDeployed"].guiActive = false;

                Fields["JetpackIsThrusting"].guiActiveEditor = false;
                Fields["JetpackIsThrusting"].guiActive = false;

                Fields["PropellantConsumption"].guiActiveEditor = false;
                Fields["PropellantConsumption"].guiActive = false;
/*
                Fields["propellantResourceDefaultAmount"].guiActiveEditor = false;
                Fields["propellantResourceDefaultAmount"].guiActive = false;
                */
            }

            if (bounds)
            {
                Fields["boundAttack"].guiActiveEditor = true;
                Fields["boundAttack"].guiActive = true;

                Fields["boundFallThreshold"].guiActiveEditor = true;
                Fields["boundFallThreshold"].guiActive = true;

                Fields["boundForce"].guiActiveEditor = true;
                Fields["boundForce"].guiActive = true;

                Fields["boundFrequency"].guiActiveEditor = true;
                Fields["boundFrequency"].guiActive = true;

                Fields["boundRelease"].guiActiveEditor = true;
                Fields["boundRelease"].guiActive = true;

                Fields["boundSharpness"].guiActiveEditor = true;
                Fields["boundSharpness"].guiActive = true;

                Fields["boundSpeed"].guiActiveEditor = true;
                Fields["boundSpeed"].guiActive = true;

                Fields["boundThreshold"].guiActiveEditor = true;
                Fields["boundThreshold"].guiActive = true;
            }
            else
            {
                Fields["boundAttack"].guiActiveEditor = false;
                Fields["boundAttack"].guiActive = false;

                Fields["boundFallThreshold"].guiActiveEditor = false;
                Fields["boundFallThreshold"].guiActive = false;

                Fields["boundForce"].guiActiveEditor = false;
                Fields["boundForce"].guiActive = false;

                Fields["boundFrequency"].guiActiveEditor = false;
                Fields["boundFrequency"].guiActive = false;

                Fields["boundRelease"].guiActiveEditor = false;
                Fields["boundRelease"].guiActive = false;

                Fields["boundSharpness"].guiActiveEditor = false;
                Fields["boundSharpness"].guiActive = false;

                Fields["boundSpeed"].guiActiveEditor = false;
                Fields["boundSpeed"].guiActive = false;

                Fields["boundThreshold"].guiActiveEditor = false;
                Fields["boundThreshold"].guiActive = false;
            }

            if (clamber)
            {
                Fields["autoGrabLadderOnStart"].guiActiveEditor = true;
                Fields["autoGrabLadderOnStart"].guiActive = true;

                Fields["clamberMaxAlt"].guiActiveEditor = true;
                Fields["clamberMaxAlt"].guiActive = true;

                Fields["clamberReach"].guiActiveEditor = true;
                Fields["clamberReach"].guiActive = true;

                Fields["clamberStandoff"].guiActiveEditor = true;
                Fields["clamberStandoff"].guiActive = true;

                Fields["flagReach"].guiActiveEditor = true;
                Fields["flagReach"].guiActive = true;

                Fields["initialMasSalt"].guiActiveEditor = true;
                Fields["initialMasSalt"].guiActive = true;

                Fields["massMultiplier"].guiActiveEditor = true;
                Fields["massMultiplier"].guiActive = true;

                Fields["minRunningGee"].guiActiveEditor = true;
                Fields["minRunningGee"].guiActive = true;

                Fields["minWalkingGee"].guiActiveEditor = true;
                Fields["minWalkingGee"].guiActive = true;

                Fields["hopThreshold"].guiActiveEditor = true;
                Fields["hopThreshold"].guiActive = true;

                Fields["maxJumpForce"].guiActiveEditor = true;
                Fields["maxJumpForce"].guiActive = true;

                Fields["ladderClimbSpeed"].guiActiveEditor = true;
                Fields["ladderClimbSpeed"].guiActive = true;

                Fields["ladderPushoffForce"].guiActiveEditor = true;
                Fields["ladderPushoffForce"].guiActive = true;

                Fields["rotPower"].guiActiveEditor = true;
                Fields["rotPower"].guiActive = true;

                Fields["runSpeed"].guiActiveEditor = true;
                Fields["runSpeed"].guiActive = true;

                Fields["strafeSpeed"].guiActiveEditor = true;
                Fields["strafeSpeed"].guiActive = true;

                Fields["stumbleThreshold"].guiActiveEditor = true;
                Fields["stumbleThreshold"].guiActive = true;

                Fields["swimSpeed"].guiActiveEditor = true;
                Fields["swimSpeed"].guiActive = true;

                Fields["turnRate"].guiActiveEditor = true;
                Fields["turnRate"].guiActive = true;

                Fields["walkSpeed"].guiActiveEditor = true;
                Fields["walkSpeed"].guiActive = true;

                Fields["waterAngularDragMultiplier"].guiActiveEditor = true;
                Fields["waterAngularDragMultiplier"].guiActive = true;
            }
            else
            {
                Fields["autoGrabLadderOnStart"].guiActiveEditor = false;
                Fields["autoGrabLadderOnStart"].guiActive = false;

                Fields["clamberMaxAlt"].guiActiveEditor = false;
                Fields["clamberMaxAlt"].guiActive = false;

                Fields["clamberReach"].guiActiveEditor = false;
                Fields["clamberReach"].guiActive = false;

                Fields["clamberStandoff"].guiActiveEditor = false;
                Fields["clamberStandoff"].guiActive = false;

                Fields["flagReach"].guiActiveEditor = false;
                Fields["flagReach"].guiActive = false;

                Fields["initialMasSalt"].guiActiveEditor = false;
                Fields["initialMasSalt"].guiActive = false;

                Fields["massMultiplier"].guiActiveEditor = false;
                Fields["massMultiplier"].guiActive = false;

                Fields["minRunningGee"].guiActiveEditor = false;
                Fields["minRunningGee"].guiActive = false;

                Fields["minWalkingGee"].guiActiveEditor = false;
                Fields["minWalkingGee"].guiActive = false;

                Fields["hopThreshold"].guiActiveEditor = false;
                Fields["hopThreshold"].guiActive = false;

                Fields["maxJumpForce"].guiActiveEditor = false;
                Fields["maxJumpForce"].guiActive = false;

                Fields["ladderClimbSpeed"].guiActiveEditor = false;
                Fields["ladderClimbSpeed"].guiActive = false;

                Fields["ladderPushoffForce"].guiActiveEditor = false;
                Fields["ladderPushoffForce"].guiActive = false;

                Fields["rotPower"].guiActiveEditor = false;
                Fields["rotPower"].guiActive = false;

                Fields["runSpeed"].guiActiveEditor = false;
                Fields["runSpeed"].guiActive = false;

                Fields["strafeSpeed"].guiActiveEditor = false;
                Fields["strafeSpeed"].guiActive = false;

                Fields["stumbleThreshold"].guiActiveEditor = false;
                Fields["stumbleThreshold"].guiActive = false;

                Fields["swimSpeed"].guiActiveEditor = false;
                Fields["swimSpeed"].guiActive = false;

                Fields["turnRate"].guiActiveEditor = false;
                Fields["turnRate"].guiActive = false;

                Fields["walkSpeed"].guiActiveEditor = false;
                Fields["walkSpeed"].guiActive = false;

                Fields["waterAngularDragMultiplier"].guiActiveEditor = false;
                Fields["waterAngularDragMultiplier"].guiActive = false;

            }

            if (kdkikpic)
            {
                Fields["Kd"].guiActiveEditor = true;
                Fields["Kd"].guiActive = true;

                Fields["Ki"].guiActiveEditor = true;
                Fields["Ki"].guiActive = true;

                Fields["Kp"].guiActiveEditor = true;
                Fields["Kp"].guiActive = true;

                Fields["iC"].guiActiveEditor = true;
                Fields["iC"].guiActive = true;

                Fields["lastBoundStep"].guiActiveEditor = true;
                Fields["lastBoundStep"].guiActive = true;

                Fields["linPower"].guiActiveEditor = true;
                Fields["linPower"].guiActive = true;

                Fields["onFallHeightFromTerrain"].guiActiveEditor = true;
                Fields["onFallHeightFromTerrain"].guiActive = true;

            }
            else
            {
                Fields["Kd"].guiActiveEditor = false;
                Fields["Kd"].guiActive = false;

                Fields["Ki"].guiActiveEditor = false;
                Fields["Ki"].guiActive = false;

                Fields["Kp"].guiActiveEditor = false;
                Fields["Kp"].guiActive = false;

                Fields["iC"].guiActiveEditor = false;
                Fields["iC"].guiActive = false;

                Fields["lastBoundStep"].guiActiveEditor = false;
                Fields["lastBoundStep"].guiActive = false;

                Fields["linPower"].guiActiveEditor = false;
                Fields["linPower"].guiActive = false;

                Fields["onFallHeightFromTerrain"].guiActiveEditor = false;
                Fields["onFallHeightFromTerrain"].guiActive = false;
            }
        }

        private void GetKerbalControls()
        {
            kerbal = kerbalControl();

            kerbal.autoGrabLadderOnStart = autoGrabLadderOnStart;

            isRagdoll = kerbal.isRagdoll;
            canRecover = kerbal.canRecover;
            //recoverTime = kerbal.recoverTime;
            recoverThreshold = kerbal.recoverThreshold;
            splatEnabled = kerbal.splatEnabled;
            splatSpeed = kerbal.splatSpeed;
            splatThreshold = kerbal.splatThreshold;

            JetpackDeployed = kerbal.JetpackDeployed;
            JetpackIsThrusting = kerbal.JetpackIsThrusting;
            PropellantConsumption = kerbal.PropellantConsumption;
            //propellantResourceDefaultAmount = kerbal.propellantResourceDefaultAmount;

            boundAttack = kerbal.boundAttack;
            boundFallThreshold = kerbal.boundFallThreshold;
            boundForce = kerbal.boundForce;
            boundFrequency = kerbal.boundFrequency;
            boundRelease = kerbal.boundRelease;
            boundSharpness = kerbal.boundSharpness;
            boundSpeed = kerbal.boundSpeed;
            boundThreshold = kerbal.boundThreshold;

            clamberMaxAlt = kerbal.clamberMaxAlt;
            clamberReach = kerbal.clamberReach;
            clamberStandoff = kerbal.clamberStandoff;
            flagReach = kerbal.flagReach;
            hopThreshold = kerbal.hopThreshold;

            Kd = kerbal.Kd;
            Ki = kerbal.Ki = Ki;
            Kp = kerbal.Kp;
            iC = kerbal.iC;
//            lastBoundStep = kerbal.lastBoundStep;
            linPower = kerbal.linPower;
            onFallHeightFromTerrain = kerbal.onFallHeightFromTerrain;

            maxJumpForce = kerbal.maxJumpForce;
            minRunningGee = kerbal.minRunningGee;
            minWalkingGee = kerbal.minWalkingGee;
            initialMass = kerbal.initialMass;
            massMultiplier = kerbal.massMultiplier;
            ladderClimbSpeed = kerbal.ladderClimbSpeed;
            ladderPushoffForce = kerbal.ladderPushoffForce;
            rotPower = kerbal.rotPower;
            runSpeed = kerbal.runSpeed;
            strafeSpeed = kerbal.strafeSpeed;
            stumbleThreshold = kerbal.stumbleThreshold;
            swimSpeed = kerbal.swimSpeed;
            turnRate = kerbal.turnRate;
            walkSpeed = kerbal.walkSpeed;
            waterAngularDragMultiplier = kerbal.waterAngularDragMultiplier;

        }

        private void UpdateKerbalControls()
        {
            kerbal = kerbalControl();

            kerbal.autoGrabLadderOnStart = autoGrabLadderOnStart;

            kerbal.isRagdoll = isRagdoll;
            kerbal.canRecover = canRecover;
            //kerbal.recoverTime = recoverTime;
            kerbal.recoverThreshold = recoverThreshold;
            kerbal.splatEnabled = splatEnabled;
            kerbal.splatSpeed = splatSpeed;
            kerbal.splatThreshold = splatThreshold;

            kerbal.JetpackDeployed = JetpackDeployed;
            kerbal.JetpackIsThrusting = JetpackIsThrusting;
            kerbal.PropellantConsumption = PropellantConsumption;
            //kerbal.propellantResourceDefaultAmount = propellantResourceDefaultAmount;

            kerbal.boundAttack = boundAttack;
            kerbal.boundFallThreshold = boundFallThreshold;
            kerbal.boundForce = boundForce;
            kerbal.boundFrequency = boundFrequency;
            kerbal.boundRelease = boundRelease;
            kerbal.boundSharpness = boundSharpness;
            kerbal.boundSpeed = boundSpeed;
            kerbal.boundThreshold = boundThreshold;

            kerbal.clamberMaxAlt = clamberMaxAlt;
            kerbal.clamberReach = clamberReach;
            kerbal.clamberStandoff = clamberStandoff;
            kerbal.flagReach = flagReach;
            kerbal.hopThreshold = hopThreshold;

            kerbal.Kd = Kd;
            kerbal.Ki = Ki;
            kerbal.Kp = Kp;
            kerbal.iC = iC;
//            kerbal.lastBoundStep = lastBoundStep;
            kerbal.linPower = linPower;
            kerbal.onFallHeightFromTerrain = onFallHeightFromTerrain;

            kerbal.maxJumpForce = maxJumpForce;
            kerbal.minRunningGee = minRunningGee;
            kerbal.minWalkingGee = minWalkingGee;
            kerbal.initialMass = initialMass;
            kerbal.massMultiplier = massMultiplier;
            kerbal.ladderClimbSpeed = ladderClimbSpeed;
            kerbal.ladderPushoffForce = ladderPushoffForce;
            kerbal.rotPower = rotPower;
            kerbal.runSpeed = runSpeed;
            kerbal.strafeSpeed = strafeSpeed;
            kerbal.stumbleThreshold = stumbleThreshold;
            kerbal.swimSpeed = swimSpeed;
            kerbal.turnRate = turnRate;
            kerbal.walkSpeed = walkSpeed;
            kerbal.waterAngularDragMultiplier = waterAngularDragMultiplier;
        }
    }
}
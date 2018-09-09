using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace OrX.parts
{
    public class ModuleOrX : PartModule, IPartMassModifier
    {

        #region Fields

        public bool medPack = false;
        public int medPackLevel = 0;
        public bool walkSpeedUp = false;
        public float walkSpeedUpLevel = 0;
        public bool runSpeedUp = false;
        public float runSpeedUpLevel = 0;
        public bool jumpForceUp = false;
        public float jumpForceUpLevel = 0;
        public bool swimSpeedUp = false;
        public float swimSpeedUpLevel = 0;
        public bool strafeSpeedUp = false;
        public float strafeSpeedUpLevel = 0;

        public bool toggleJetPack = false;

        public bool trimUp = false;
        public bool trimDown = false;

        [KSPField(isPersistant = true)]
        public float _walkSpeed = 0.0f;

        [KSPField(isPersistant = true)]
        public float _runSpeed = 0.0f;

        [KSPField(isPersistant = true)]
        public float _strafeSpeed = 0.0f;

        [KSPField(isPersistant = true)]
        public float _maxJumpForce = 0.0f;

        [KSPField(isPersistant = true)]
        public float _swimSpeed = 0.0f;

        private bool toggle = false;

        public float trimModifier = 2;
        public bool resetTrim = false;
        private bool trimming = false;
        private float trimModCheck = 0.0f;

        [KSPField(isPersistant = true)]
        public float oxygenMax = 100.0f;

        [KSPField(isPersistant = true)]
        public float oxygen = 100.0f;

        [KSPField(isPersistant = true)]
        public bool showHelmet = false;

        [KSPField(isPersistant = true)]
        public float level = 1;

        public string kerbalName = "";

        [KSPField(isPersistant = true)]
        public bool repack = false;

        public KerbalEVA kerbal;
        private KerbalEVA GetKerbal()
        {
            KerbalEVA k = null;

            k = part.FindModuleImplementing<KerbalEVA>();

            return k;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    part.force_activate();

                    kerbalName = this.vessel.vesselName;
                    kerbal = GetKerbal();

                    _maxJumpForce = kerbal.maxJumpForce;
                    _walkSpeed = kerbal.walkSpeed;
                    _runSpeed = kerbal.runSpeed;
                    _strafeSpeed = kerbal.strafeSpeed;
                    _swimSpeed = kerbal.swimSpeed;
                }
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && !vessel.HoldPhysics)
            {
                if (part.vessel.isEVA)
                {
                    if (vessel.isActiveVessel)
                    {
                        if (vessel.Splashed)
                        {
                            checkTrim();

                            if (part.vessel.altitude <= -1)
                            {
                                oxyCheck();
                            }
                        }
                        else
                        {
                            massModifier = 0;
                        }

                        if (toggleJetPack && !toggle)
                        {
                            StartCoroutine(ToggleJetPackRoutine());
                        }

                        if (OrX_Scuba.instance.guiopen)
                        {
                            if (!trimming)
                            {
                                if (Input.GetKeyDown(KeyCode.LeftShift))
                                    massModifier = 0;

                                if (Input.GetKeyDown(KeyCode.LeftControl))
                                    massModifier = 10;

                                if (Input.GetKeyDown(KeyCode.Q))
                                    trimUp = true;

                                if (Input.GetKeyDown(KeyCode.E))
                                    trimDown = true;
                            }
                        }
                    }
                    else
                    {
                        if (vessel.Splashed)
                        {
                            if (BDArmoryExtensions.BDArmoryIsInstalled())
                            {
                                double targetAlt = FlightGlobals.ActiveVessel.altitude;

                                if (this.vessel.altitude >= FlightGlobals.ActiveVessel.altitude)
                                {
                                    trimDown = true;
                                }
                                else
                                {
                                    trimUp = true;
                                }
                            }

                            checkTrim();

                            if (part.vessel.altitude <= -1)
                            {
                                oxyCheck();
                            }
                        }
                        else
                        {
                            massModifier = 0;
                        }
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region SCUBA KERB ETC...

        //////////////////////////////////////////////////////////////////////////////
        // JETPACK, CHUTE AND SCUBA KERB
        //////////////////////////////////////////////////////////////////////////////

        IEnumerator ToggleJetPackRoutine()
        {
            toggle = true;
            kerbal = GetKerbal();
            kerbal.ToggleJetpack();
            yield return new WaitForSeconds(1);
            toggleJetPack = false;
            toggle = false;
        }

        private void CheckChute()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                var EVAchute = p.vessel.FindPartModuleImplementing<ModuleEvaChute>();

                if (EVAchute != null)
                {
                    EVAchute.AllowRepack(true);
                    EVAchute.Repack();
                }
            }
        }

        private void oxyCheck()
        {
            if (part.vessel.altitude <= -1 && this.vessel.Splashed)
            {
                oxygen -= 0.015f;
            }
            else
            {
                if (oxygen <= 100f)
                {
                    oxygen += 0.1f;
                }
            }

            if (oxygen <= 10)
            {
                if (oxygen <= 1)
                {
                    kerbal = GetKerbal();
                    kerbal.isRagdoll = true;
                }

                // Adjust movement speed if oxy is low
            }
        }

        public void checkTrim()
        {
            if (trimUp && !trimming)
            {
                StartCoroutine(TrimUp());
            }

            if (trimDown && !trimming)
            {
                StartCoroutine(TrimDown());
            }
        }

        IEnumerator TrimDown()
        {
            trimming = true;

            var _trimModifier = trimModifier + trimModCheck;
            massModifier = _trimModifier;
            trimModCheck = massModifier;
            yield return new WaitForSeconds(0.25f);
            trimDown = false;
            trimming = false;
        }

        IEnumerator TrimUp()
        {
            trimming = true;

            var _trimModifier = trimModCheck - trimModifier;
            if (_trimModifier >= 0)
            {
                massModifier = _trimModifier;
                trimModCheck = massModifier;
                yield return new WaitForEndOfFrame();
                trimUp = false;
                trimming = false;
            }
            else
            {
                massModifier = 0;
                trimModCheck = massModifier;
                yield return new WaitForEndOfFrame();
                trimUp = false;
                trimming = false;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Core

        public static bool Awaken(PartModule module)
        {
            if (module == null)
                return false;
            object[] paramList = new object[] { };
            MethodInfo awakeMethod = typeof(PartModule).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
            //MethodInfo awakeOnStart = typeof(PartModule).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);

            if (awakeMethod == null)
                return false;

            //if (awakeOnStart == null)
            //    return false;

            awakeMethod.Invoke(module, paramList);
            return true;
        }

        public static bool OnStarten(PartModule module)
        {
            if (module == null)
                return false;
            object[] paramOnStart = new object[] { };
            //MethodInfo awakeMethod = typeof(PartModule).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo awakeOnStart = typeof(PartModule).GetMethod("OnStart", BindingFlags.Instance);

            //if (awakeMethod == null)
            //    return false;

            if (awakeOnStart == null)
                return false;

            awakeOnStart.Invoke(module, paramOnStart);
            return true;
        }

        public void LaunchSplashed()
        {
            this.vessel.Landed = false;
            this.vessel.Splashed = true;
            this.vessel.situation = Vessel.Situations.SPLASHED;
        }

        public void LaunchOrbit()
        {
            this.vessel.Landed = false;
            this.vessel.Splashed = false;
            this.vessel.situation = Vessel.Situations.ORBITING;
        }

        private float massModifier = 0.0f;

        public void setMassModifier(float massModifier)
        {
            this.massModifier = massModifier;
        }

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
        {
            return defaultMass * massModifier;
        }

        public ModifierChangeWhen GetModuleMassChangeWhen()
        {
            return ModifierChangeWhen.CONSTANTLY;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

    }
}


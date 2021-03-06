﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrXBDAc.parts;
using BDArmory.Modules;
using System;
using System.Linq;
using OrXBDAc.missions;

namespace OrXBDAc
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
    public class OrX_Log : MonoBehaviour
    {
        public static OrX_Log instance;

        public bool debug = true;
        public bool ironKerbal = false;
        public bool survival = false;
        public bool win = false;

        private int debris = 0;

        public bool ejecting = false;
        public bool drillPresent = false;
        public bool orxHoleReady = false;

        /// <summary>
        /// //////////////////////////////////////////////////////////////////
        /// </summary>
        /// 

        private void Awake()
        {
            DontDestroyOnLoad(this);
            instance = this;
            GetSounds();
            CheckInstalledMods();
            GameEvents.onVesselChange.Add(VesselCheck);
        }

        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                if (ironKerbal)
                {
                    ironKerbal = false;
                }

                if (!setup)
                {
                    setup = true;
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (setup && FlightGlobals.ready)
                {
                    setup = false;
                    //devKitInstalled = OrXDevKitIsInstalled();
                    playerVessel = FlightGlobals.ActiveVessel.vesselName;
                    playerVessel01 = playerVessel;
                    CheckVessel();
                }

                if (FlightGlobals.ActiveVessel == null)
                {
                    FindPlayerVessel();
                }
            }
        }

        /*
        public bool OrXDevKitIsInstalled()
        {
            try
            {
                PartExtensions = AssemblyLoader.loadedAssemblies
                     .Where(a => a.name.Contains("OrX.DevKit")).SelectMany(a => a.assembly.GetExportedTypes())
                     .SingleOrDefault(t => t.FullName == "OrX.DevKit.BaseFilterDev");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        */
        #region Core

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

        #region Player Vessel List

        public bool playerVesselChecked = false;

        [KSPField(isPersistant = true)]
        public string playerVessel = "";
        [KSPField(isPersistant = true)]
        public string playerVessel01 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel02 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel03 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel04 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel05 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel06 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel07 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel08 = "";
        [KSPField(isPersistant = true)]
        public string playerVessel09 = "";

        private bool pv01 = false;
        private bool pv02 = false;
        private bool pv03 = false;
        private bool pv04 = false;
        private bool pv05 = false;
        private bool pv06 = false;
        private bool pv07 = false;
        private bool pv08 = false;
        private bool pv09 = false;

        private void CheckVesselList()
        {
            Debug.Log("[OrX LOG]: Checking Player Vessel List ...........");

            pv01 = false;
            pv02 = false;
            pv03 = false;
            pv04 = false;
            pv05 = false;
            pv06 = false;
            pv07 = false;
            pv08 = false;
            pv09 = false;

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;

                if (v.Current.vesselName == playerVessel01)
                {
                    pv01 = true;
                }
                else
                {
                    if (v.Current.vesselName == playerVessel02)
                    {
                        pv02 = true;
                    }
                    else
                    {
                        if (v.Current.vesselName == playerVessel03)
                        {
                            pv03 = true;

                        }
                        else
                        {
                            if (v.Current.vesselName == playerVessel04)
                            {
                                pv04 = true;
                            }
                            else
                            {
                                if (v.Current.vesselName == playerVessel05)
                                {
                                    pv05 = true;
                                }
                                else
                                {
                                    if (v.Current.vesselName == playerVessel06)
                                    {
                                        pv06 = true;
                                    }
                                    else
                                    {
                                        if (v.Current.vesselName == playerVessel07)
                                        {
                                            pv07 = true;
                                        }
                                        else
                                        {
                                            if (v.Current.vesselName == playerVessel08)
                                            {
                                                pv08 = true;
                                            }
                                            else
                                            {
                                                if (v.Current.vesselName == playerVessel09)
                                                {
                                                    pv09 = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            v.Dispose();


            if (!pv01 && playerVessel01 != "")
            {
                playerVessel01 = "";
                pv01 = false;
            }

            if (!pv02 && playerVessel02 != "")
            {
                playerVessel02 = "";
                pv02 = false;
            }

            if (!pv03 && playerVessel03 != "")
            {
                playerVessel03 = "";
                pv03 = false;
            }

            if (!pv04 && playerVessel04 != "")
            {
                playerVessel04 = "";
                pv04 = false;
            }

            if (!pv05 && playerVessel05 != "")
            {
                playerVessel05 = "";
                pv05 = false;
            }

            if (!pv06 && playerVessel06 != "")
            {
                playerVessel06 = "";
                pv06 = false;
            }

            if (!pv07 && playerVessel07 != "")
            {
                playerVessel07 = "";
                pv07 = false;
            }

            if (!pv08 && playerVessel08 != "")
            {
                playerVessel08 = "";
                pv08 = false;
            }

            if (!pv09 && playerVessel09 != "")
            {
                playerVessel09 = "";
                pv09 = false;
            }

            playerVesselChecked = false;
        }

        public void LogPlayerVessel()
        {
            if (playerVessel01 == "")
            {
                playerVessel01 = playerVessel;
            }
            else
            {
                if (playerVessel02 == "")
                {
                    playerVessel02 = playerVessel;
                }
                else
                {
                    if (playerVessel03 == "")
                    {
                        playerVessel03 = playerVessel;
                    }
                    else
                    {
                        if (playerVessel04 == "")
                        {
                            playerVessel04 = playerVessel;
                        }
                        else
                        {
                            if (playerVessel05 == "")
                            {
                                playerVessel05 = playerVessel;
                            }
                            else
                            {
                                if (playerVessel06 == "")
                                {
                                    playerVessel06 = playerVessel;
                                }
                                else
                                {
                                    if (playerVessel07 == "")
                                    {
                                        playerVessel07 = playerVessel;
                                    }
                                    else
                                    {
                                        if (playerVessel08 == "")
                                        {
                                            playerVessel08 = playerVessel;
                                        }
                                        else
                                        {
                                            if (playerVessel09 == "")
                                            {
                                                playerVessel09 = playerVessel;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CheckVesselList();
        }

        public void FindPlayerVessel() // Find player vessel and switch to it if player was killed
        {
            var count = 0;
            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;

                if (count == 0) // check for player kerbals and switch to the first one found if available
                {
                    var kerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>(); 

                    if (kerbal != null)
                    {
                        if (kerbal.player)
                        {
                            FlightGlobals.ForceSetActiveVessel(v.Current);
                            FlightInputHandler.ResumeVesselCtrlState(v.Current);
                            count = 1;
                            break;
                        }
                    }
                }
            }
            v.Dispose();

            if (count == 0) // if previous list above doesn't find a player kerbal then check for player vessels and switch to the first one found if available
            {
                List<Vessel>.Enumerator vv = FlightGlobals.Vessels.GetEnumerator();
                while (vv.MoveNext())
                {
                    if (vv.Current == null) continue;
                    if (!vv.Current.loaded || vv.Current.packed) continue;

                    if (count == 0)
                    {
                        var wmPart = vv.Current.FindPartModuleImplementing<MissileFire>();

                        if (wmPart != null)
                        {
                            if (!wmPart.team)
                            {
                                FlightGlobals.ForceSetActiveVessel(vv.Current);
                                FlightInputHandler.ResumeVesselCtrlState(vv.Current);
                                count = 1;
                                break;
                            }
                        }
                    }
                }
                vv.Dispose();
            }

            if (count == 0) // if no player kerbals or vessels are found Game Over !!!!!!!
            {
                GameOver();
            }
        }

        public void CheckVessel()
        {
            Debug.Log("[OrX LOG]: Checking Vessel ...........");

            if (!FlightGlobals.ActiveVessel.vesselName.Contains("OrX"))
            {
                if (FlightGlobals.ActiveVessel.vesselName != "Brute")
                {
                    if (FlightGlobals.ActiveVessel.vesselName != "StayPunkd")
                    {
                        if (FlightGlobals.ActiveVessel.vesselName != "OrX Tank")
                        {
                            if (FlightGlobals.ActiveVessel.vesselName != "Waldo")
                            {
                                if (FlightGlobals.ActiveVessel.isEVA)
                                {
                                    var orx = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXBDAc>();

                                    if (orx.player)
                                    {
                                        playerVessel = FlightGlobals.ActiveVessel.vesselName;
                                        LogPlayerVessel();
                                    }
                                }
                                else
                                {
                                    var wmPart = FlightGlobals.ActiveVessel.FindPartModuleImplementing<MissileFire>();

                                    if (wmPart != null)
                                    {
                                        playerVessel = FlightGlobals.ActiveVessel.vesselName;
                                        LogPlayerVessel();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void VesselCheck(Vessel v)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (!playerVesselChecked)
                {
                    playerVesselChecked = true;
                    Debug.Log("[OrX Log - ACTIVE VESSEL NAME] Checking if vessel is player vessel ......................");

                    Debug.Log("[OrX Log - ACTIVE VESSEL NAME] |" + FlightGlobals.ActiveVessel.vesselName + "| ...............");

                    if (FlightGlobals.ActiveVessel.vesselName != playerVessel)
                    {
                        Debug.Log("[OrX Log - ACTIVE VESSEL NAME] Vessel name doesn't match logged player vessel name ... Searching player vessel list");

                        if (FlightGlobals.ActiveVessel.vesselName != playerVessel01)
                        {
                            if (FlightGlobals.ActiveVessel.vesselName != playerVessel02)
                            {
                                if (FlightGlobals.ActiveVessel.vesselName != playerVessel03)
                                {
                                    if (FlightGlobals.ActiveVessel.vesselName != playerVessel04)
                                    {
                                        if (FlightGlobals.ActiveVessel.vesselName != playerVessel05)
                                        {
                                            if (FlightGlobals.ActiveVessel.vesselName != playerVessel06)
                                            {
                                                if (FlightGlobals.ActiveVessel.vesselName != playerVessel07)
                                                {
                                                    if (FlightGlobals.ActiveVessel.vesselName != playerVessel08)
                                                    {
                                                        if (FlightGlobals.ActiveVessel.vesselName != playerVessel09)
                                                        {
                                                            CheckVessel();
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 9 .......");
                                                            playerVessel = FlightGlobals.ActiveVessel.vesselName;
                                                            playerVesselChecked = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 8 .......");
                                                        playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                                    }

                                                }
                                                else
                                                {
                                                    Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 7  .......");
                                                    playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 6 .......");
                                                playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                            }
                                        }
                                        else
                                        {
                                            Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 5 .......");
                                            playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 4 .......");
                                        playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                    }

                                }
                                else
                                {
                                    Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 3 .......");
                                    playerVessel = FlightGlobals.ActiveVessel.vesselName;

                                }
                            }
                            else
                            {
                                Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 2 .......");
                                playerVessel = FlightGlobals.ActiveVessel.vesselName;

                            }
                        }
                        else
                        {
                            Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is logged vessel 1 .......");
                            playerVessel = FlightGlobals.ActiveVessel.vesselName;
                        }
                    }
                    else
                    {
                        Debug.Log("[OrX Log - ACTIVE VESSEL NAME] " + FlightGlobals.ActiveVessel.vesselName + " is player vessel .......");
                    }
                }
            }
        }

        #endregion

        #region Setup

        private KeyCode next;
        private KeyCode prev;
        private KeyCode next2;
        private KeyCode prev2;

        [KSPField(isPersistant = true)]
        private bool keysSet = true;

        private bool setup = true;
        private bool setFocusKeys = true;
        public bool modeReset = false;
        public bool launchChanged = false;

        private string KerbalFoundries = " KF-ALG-Large";
        private string SM_AFVs = "BradleyHull";
        private string Malfunc = "thephalanx";
        private string DCK_FT = "bahaBGM86B";
        private string KTech = "RGturret";
        private string EnemyMine = "EMHedgeTypelauncher";
        private string OrXDevKit = "ORXDEVDUMMY";

        public bool kf = false;
        public bool smAFVs = false;
        public bool malfunc = false;
        public bool dckft = false;
        public bool ktech = false;
        public bool em = false;

        public Type PartExtensions { get; private set; }

        public bool devKitInstalled = false;

        private string lockID = "";

        /// <summary>
        /// ///////////////////////////////////////////////////////
        /// 
        /// </summary>

        private void CheckInstalledMods()
        {
            List<AvailablePart> availablePart = PartLoader.LoadedPartsList;
            foreach (AvailablePart AP in availablePart)
            {
                if (AP.partPrefab.name == OrXDevKit)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found OrX Dev Kit .......................");

                    devKitInstalled = true;
                }

                if (AP.partPrefab.name == KerbalFoundries)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found Kerbal Foundries .......................");

                    kf = true;
                }

                if (AP.partPrefab.name == SM_AFVs)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found SM AFVs .......................");

                    smAFVs = true;
                }

                if (AP.partPrefab.name == Malfunc)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found Malfunc Industries .......................");

                    malfunc = true;
                }

                if (AP.partPrefab.name == DCK_FT)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found DCK FutureTech .......................");

                    dckft = true;
                }

                if (AP.partPrefab.name == KTech)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found K-Tech .......................");

                    ktech = true;
                }

                if (AP.partPrefab.name == EnemyMine)
                {
                    Debug.Log("[OrX Log - CheckInstalledMods] Found Enemy Mine .......................");

                    em = true;
                }
            }

            if (!kf)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] Kerbal Foundries not found .......................");
            }

            if (!smAFVs)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] SMAFVs not found .......................");
            }

            if (!malfunc)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] Malfunc not found .......................");
            }

            if (!dckft)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] DCK FutureTech not found .......................");
            }

            if (!ktech)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] K-Tech not found .......................");
            }

            if (!em)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] Enemy Mine not found .......................");
            }

            if (!devKitInstalled)
            {
                Debug.Log("[OrX Log - CheckInstalledMods] OrX Dev Kit not found .......................");
            }

        }

        public void SetFocusKeys()
        {
            if (setFocusKeys)
            {
                setFocusKeys = false;
                next = GameSettings.FOCUS_NEXT_VESSEL.primary.code;
                prev = GameSettings.FOCUS_PREV_VESSEL.primary.code;

                next2 = GameSettings.FOCUS_NEXT_VESSEL.secondary.code;
                prev2 = GameSettings.FOCUS_PREV_VESSEL.secondary.code;
            }

            keysSet = false;
            Debug.Log("[OrX LOG]: Setting Vessel Focus Hotkeys");

            GameSettings.FOCUS_NEXT_VESSEL.primary.code = KeyCode.None;
            GameSettings.FOCUS_PREV_VESSEL.primary.code = KeyCode.None;
            Debug.Log("[OrX LOG]: " + next + " changed to None" + GameSettings.FOCUS_PREV_VESSEL.primary.code);

            GameSettings.FOCUS_NEXT_VESSEL.secondary.code = KeyCode.None;
            GameSettings.FOCUS_PREV_VESSEL.secondary.code = KeyCode.None;
            Debug.Log("[OrX LOG]: " + next2 + " changed to None" + GameSettings.FOCUS_PREV_VESSEL.secondary.code);
        }

        public void ResetFocusKeys()
        {
            keysSet = true;

            Debug.Log("[OrX LOG]: Resetting Vessel Focus Hotkeys");

            GameSettings.FOCUS_NEXT_VESSEL.primary.code = next;
            GameSettings.FOCUS_PREV_VESSEL.primary.code = prev;
            Debug.Log("[OrX LOG]: " + next + " re-enabled ............................");


            GameSettings.FOCUS_NEXT_VESSEL.secondary.code = next2;
            GameSettings.FOCUS_PREV_VESSEL.secondary.code = prev2;
            Debug.Log("[OrX LOG]: " + next2 + " re-enabled ............................");

        }

        public void LockKeyboard()
        {
            InputLockManager.SetControlLock(ControlTypes.ACTIONS_ALL, lockID);
        }

        public void UnlockKeyboard()
        {
            InputLockManager.RemoveControlLock(lockID);
        }

        public void GameOver()
        {
            ironKerbal = false;
            OrX_Controls.instance.DisableGuiOrXControl();
            if (win)
            {
                ScreenMsg("<color=#cc4500ff><b>MISSION COMPLETE</b></color>");
                // PLAY WINNING SOUNDS
            }
            else
            {
                ScreenMsg("<color=#cc4500ff><b>YOU DIED .... GAME OVER</b></color>");
                HighLogic.bufferedLoad(5, false);
                // PLAY FAILURE SOUNDS
            }
        }

        public void ResetMode()
        {
            modeReset = true;
            StartCoroutine(ModeResetRoutine());
        }

        IEnumerator ModeResetRoutine()
        {
            yield return new WaitForSeconds(30);
            modeReset = false;
        }

        #endregion

        #region Sounds
        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param name="msg"></param>
        /// 

        public void PlayOrders()
        {
            int random = new System.Random().Next(0, 10);
            if (random <= 5)
            {
                sound_SpawnOrXOrders.Play();
            }
            else
            {
                sound_SpawnOrXThing.Play();
            }
        }

        IEnumerator BruteRandomSound()
        {

            int random = new System.Random().Next(1, 4);

            yield return new WaitForEndOfFrame();

            if (random == 1)
            {

            }

            if (random == 2)
            {

            }

            if (random == 3)
            {

            }

            if (random == 4)
            {

            }
        }

        public void stayPunkdRandomSound()
        {
            StartCoroutine(stayPunkdSoundRoutine());
        }

        IEnumerator stayPunkdSoundRoutine()
        {
            int random = new System.Random().Next(1, 4);

            yield return new WaitForSeconds(2);

            if (random == 1)
            {
                //sound_SpawnOrXOrders.Play();
            }

            if (random == 2)
            {
                //sound_OrXSheBitch.Play();
            }

            if (random == 3)
            {
                //sound_OrXSpinachChin.Play();
            }

            if (random == 4)
            {
                //sound_OrXSheBitch.Play();

            }
        }

        //public AudioSource sound_OrXBoomstick;
        //public AudioSource sound_OrXHailToTheKing;
        public AudioSource sound_SpawnOrXHole;
        public AudioSource sound_SpawnOrXHolyHole;
        //public AudioSource sound_OrXFatality;
        //public AudioSource sound_OrXGroovy;
        //public AudioSource sound_OrXFinishHim;
        //public AudioSource sound_OrXSpinachChin;
        //public AudioSource sound_OrXSheBitch;
        //public AudioSource sound_OrXShutTheDoor;
        //public AudioSource sound_SpawnOrXRevenge;
        public AudioSource sound_SpawnOrXOrders;
        //public AudioSource sound_SpawnOrXNeeds;
        //public AudioSource sound_SpawnOrXWhatsThat;
        public AudioSource sound_SpawnOrXThing;

        private void GetSounds()
        {
            Debug.LogWarning("[OrX Log] GETTING SOUNDS .................");

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// SPAWN
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /*
            sound_SpawnOrXRevenge = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXRevenge.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXRevenge");

            sound_SpawnOrXRevenge.loop = false;
            sound_SpawnOrXRevenge.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXRevenge.dopplerLevel = 0f;
            sound_SpawnOrXRevenge.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXRevenge.minDistance = 0.5f;
            sound_SpawnOrXRevenge.maxDistance = 2f;
            */

            sound_SpawnOrXOrders = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXOrders.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXOrders");

            sound_SpawnOrXOrders.loop = false;
            sound_SpawnOrXOrders.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXOrders.dopplerLevel = 0f;
            sound_SpawnOrXOrders.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXOrders.minDistance = 0.5f;
            sound_SpawnOrXOrders.maxDistance = 2f;


            /*
            sound_SpawnOrXNeeds = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXNeeds.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXNeeds");

            sound_SpawnOrXNeeds.loop = false;
            sound_SpawnOrXNeeds.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXNeeds.dopplerLevel = 0f;
            sound_SpawnOrXNeeds.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXNeeds.minDistance = 0.5f;
            sound_SpawnOrXNeeds.maxDistance = 2f;

            sound_SpawnOrXWhatsThat = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXWhatsThat.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXWhatsThat");

            sound_SpawnOrXWhatsThat.loop = false;
            sound_SpawnOrXWhatsThat.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXWhatsThat.dopplerLevel = 0f;
            sound_SpawnOrXWhatsThat.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXWhatsThat.minDistance = 0.5f;
            sound_SpawnOrXWhatsThat.maxDistance = 2f;
            */



            sound_SpawnOrXThing = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXThing.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXThing");

            sound_SpawnOrXThing.loop = false;
            sound_SpawnOrXThing.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXThing.dopplerLevel = 0f;
            sound_SpawnOrXThing.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXThing.minDistance = 0.5f;
            sound_SpawnOrXThing.maxDistance = 2f;



            /*
            //////////////////////////////////////////////////////////////////////////////////

            sound_OrXShutTheDoor = gameObject.AddComponent<AudioSource>();
            sound_OrXShutTheDoor.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXShutTheDoor");

            sound_OrXShutTheDoor.loop = false;
            sound_OrXShutTheDoor.volume = GameSettings.AMBIENCE_VOLUME;
            sound_OrXShutTheDoor.dopplerLevel = 0f;
            sound_OrXShutTheDoor.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXShutTheDoor.minDistance = 0.5f;
            sound_OrXShutTheDoor.maxDistance = 1f;

            sound_OrXSheBitch = gameObject.AddComponent<AudioSource>();
            sound_OrXSheBitch.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXSheBitch");

            sound_OrXSheBitch.loop = false;
            sound_OrXSheBitch.volume = GameSettings.AMBIENCE_VOLUME;
            sound_OrXSheBitch.dopplerLevel = 0f;
            sound_OrXSheBitch.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXSheBitch.minDistance = 0.5f;
            sound_OrXSheBitch.maxDistance = 1f;

            sound_OrXFinishHim = gameObject.AddComponent<AudioSource>();
            sound_OrXFinishHim.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXFinishHim");

            sound_OrXFinishHim.loop = false;
            sound_OrXFinishHim.volume = GameSettings.AMBIENCE_VOLUME;
            sound_OrXFinishHim.dopplerLevel = 0f;
            sound_OrXFinishHim.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXFinishHim.minDistance = 0.5f;
            sound_OrXFinishHim.maxDistance = 1f;

            sound_OrXBoomstick = gameObject.AddComponent<AudioSource>();
            sound_OrXBoomstick.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXBoomstick");

            sound_OrXBoomstick.loop = false;
            sound_OrXBoomstick.volume = GameSettings.AMBIENCE_VOLUME;
            sound_OrXBoomstick.dopplerLevel = 0f;
            sound_OrXBoomstick.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXBoomstick.minDistance = 0.5f;
            sound_OrXBoomstick.maxDistance = 1f;

            sound_OrXHailToTheKing = gameObject.AddComponent<AudioSource>();
            sound_OrXHailToTheKing.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXHailToTheKing");

            sound_OrXHailToTheKing.loop = false;
            sound_OrXHailToTheKing.volume = GameSettings.AMBIENCE_VOLUME;
            sound_OrXHailToTheKing.dopplerLevel = 0f;
            sound_OrXHailToTheKing.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXHailToTheKing.minDistance = 0.5f;
            sound_OrXHailToTheKing.maxDistance = 1f;


            sound_OrXFatality = gameObject.AddComponent<AudioSource>();
            sound_OrXFatality.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXFatality");

            sound_OrXFatality.loop = false;
            sound_OrXFatality.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_OrXFatality.dopplerLevel = 0f;
            sound_OrXFatality.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXFatality.minDistance = 0.5f;
            sound_OrXFatality.maxDistance = 2f;
            sound_OrXGroovy = gameObject.AddComponent<AudioSource>();
            sound_OrXGroovy.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXGroovy");

            sound_OrXGroovy.loop = false;
            sound_OrXGroovy.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_OrXGroovy.dopplerLevel = 0f;
            sound_OrXGroovy.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXGroovy.minDistance = 0.5f;
            sound_OrXGroovy.maxDistance = 2f;

            sound_OrXSpinachChin = gameObject.AddComponent<AudioSource>();
            sound_OrXSpinachChin.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_OrXSpinachChin");

            sound_OrXSpinachChin.loop = false;
            sound_OrXSpinachChin.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_OrXSpinachChin.dopplerLevel = 0f;
            sound_OrXSpinachChin.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_OrXSpinachChin.minDistance = 0.5f;
            sound_OrXSpinachChin.maxDistance = 2f;

    */
            sound_SpawnOrXHole = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXHole.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXHole");

            sound_SpawnOrXHole.loop = false;
            sound_SpawnOrXHole.volume = GameSettings.AMBIENCE_VOLUME;
            sound_SpawnOrXHole.dopplerLevel = 0f;
            sound_SpawnOrXHole.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXHole.minDistance = 0.5f;
            sound_SpawnOrXHole.maxDistance = 1f;

            sound_SpawnOrXHolyHole = gameObject.AddComponent<AudioSource>();
            sound_SpawnOrXHolyHole.clip = GameDatabase.Instance.GetAudioClip("OrX/Sounds/sound_SpawnOrXHolyHole");

            sound_SpawnOrXHolyHole.loop = false;
            sound_SpawnOrXHolyHole.volume = GameSettings.AMBIENCE_VOLUME * 1.5f;
            sound_SpawnOrXHolyHole.dopplerLevel = 0f;
            sound_SpawnOrXHolyHole.rolloffMode = AudioRolloffMode.Logarithmic;
            sound_SpawnOrXHolyHole.minDistance = 0.5f;
            sound_SpawnOrXHolyHole.maxDistance = 2f;


        }

        #endregion

        #endregion
    }
}


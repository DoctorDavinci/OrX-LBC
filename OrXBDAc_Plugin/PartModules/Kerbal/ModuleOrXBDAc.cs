using BDArmory.Core.Module;
using System.Collections;
using UnityEngine;
using BDArmory.Modules;
using OrXBDAc.chase;
using OrXBDAc.missions;
using System.Collections.Generic;
using System.Reflection;
using OrXBDAc.spawn;
using System;

namespace OrXBDAc.parts
{
    public class ModuleOrXBDAc : PartModule
    {

        #region Fields

        private int orxSalt = 15;
        private int bruteSalt = 40;
        private int stayPunkdSalt = 75;
        private int waldoSalt = 100;

        public bool launchSiteChanged = false;
        public bool beach = false;
        public bool islandBeach = false;
        public bool ironKerbal = false;

        private bool bomberDelay = true;

        public bool team = false;
        public bool scale = false;
        public bool brutify = false;
        public bool punkify = false;
        public bool goWaldo = false;
        public bool resetHead = false;
        private bool sizeCheck = false;
        private bool headReset = false;

        public Transform headBone = null;

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

        [KSPField(isPersistant = true)]
        public bool brute = false;
        [KSPField(isPersistant = true)]
        public bool stayPunkd = false;
        [KSPField(isPersistant = true)]
        public bool waldo = false;
        [KSPField(isPersistant = true)]
        public bool orx = false;
        [KSPField(isPersistant = true)]
        public bool player = true;
        [KSPField(isPersistant = true)]
        public bool infected = true;

        private string OrXname = "OrX";
        private string Waldoname = "Waldo";
        private string Brutename = "Brute";
        private string StayPunkdname = "Stay Punkd";
        private string OrX44 = "OrX 44";
        private string OrXThompson = "OrX Thompson";
        private string OrXKUzi = "OrX KUzi";

        public bool toggleJetPack = false;
        public bool manualWM = false;

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

        // WW2 Helmet
        public float walkSpeed = 3.0f;
        public float runSpeed = 8;
        public float strafeSpeed = 2f;
        public float maxJumpForce = 1f;

        // Race Helmet
        public float walkSpeed_ = 5f;
        public float runSpeed_ = 15;
        public float strafeSpeed_ = 5f;
        public float maxJumpForce_ = 15f;

        private bool pause = false;
        private bool toggle = false;

        [KSPField(isPersistant = true)]
        public bool guard = false;

        private bool teamCheck = true;
        private bool helmCheck = true;
        public bool RaceHelm = false;
        public bool ww2Helm = false;
        public bool OrXHelm = false;
        public bool checkHelm = false;

        private bool bomberCheck = false;
        private bool bomber = false;

        [KSPField(isPersistant = true)]
        public bool wmAdded = false;

        [KSPField(isPersistant = true)]
        public bool wmAddedCheck = true;

        [KSPField(isPersistant = true)]
        public bool orxCheck = true;

        public bool chasing = false;
        public bool attacking = false;

        [KSPField(isPersistant = true)]
        private bool arm = true;

        public float trimModifier = 2;
        public bool resetTrim = false;
        private bool trimming = false;
        private float trimModCheck = 0.0f;

        [KSPField(isPersistant = true)]
        public float oxygenMax = 100.0f;

        [KSPField(isPersistant = true)]
        public float oxygen = 100.0f;

        public float hpDamage = 5f;

        [KSPField(isPersistant = true)]
        public float hp = 0.0f;

        [KSPField(isPersistant = true)]
        public float hpMax = 0.0f;

        public float hpToAdd = 0.0f;
        public float hpToRemove = 0.0f;

        [KSPField(isPersistant = true)]
        private float hpCheck = 0.0f;

        private double targetOrX = 0;

        [KSPField(isPersistant = true)]
        private bool helmetRemoved = false;

        [KSPField(isPersistant = true)]
        public bool showHelmet = false;

        [KSPField(isPersistant = true)]
        public float level = 1;

        [KSPField(isPersistant = true)]
        public bool stayPunkdCheck = false;

        public Vector3d target;

        public bool saltDrill = false;
        public bool waldoGroundTarget = false;

        Transform savedTransform;

        public string kerbalName = "";

        GameObject kerbalLarge;

        private bool finishHim = true;
        private bool setup = true;

        [KSPField(isPersistant = true)]
        public bool repack = false;

        public bool guardChecked = false;
        public bool predator = false;

        public bool checkHPstarted = false;
        public bool HPCheckStarted = false;

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    part.force_activate();

                    explosive = Getexplosive();
                    kerbalName = this.vessel.vesselName;
                    hpTracker = GetHP();
                    kerbal = GetKerbal();

                    hpTracker.maxHitPoints = 100;
                    hpTracker.Hitpoints = 100;
                    hp = hpTracker.Hitpoints;
                    hpMax = hpTracker.maxHitPoints;
                    hpCheck = hp;

                    kerbal.stumbleThreshold = 50;
                    kerbal.recoverTime = 0.1;
                    kerbal.splatSpeed = 100;
                    kerbal.splatThreshold = 100;

                    _maxJumpForce = kerbal.maxJumpForce;
                    _walkSpeed = kerbal.walkSpeed;
                    _runSpeed = kerbal.runSpeed;
                    _strafeSpeed = kerbal.strafeSpeed;
                    _swimSpeed = kerbal.swimSpeed;
                    kerbal.swimSpeed = _swimSpeed * 3;
                    //GetHeadBone();

                    chasing = false;

                    if (!player)
                    {
                        if (waldo || this.vessel.vesselName == Waldoname)
                        {
                            vessel.vesselName = Waldoname;
                            waldo = true;
                            player = false;
                            team = true;
                        }

                        if (brute || this.vessel.vesselName == Brutename)
                        {
                            vessel.vesselName = Brutename;
                            brute = true;
                            player = false;
                            team = true;

                        }

                        if (stayPunkd || this.vessel.vesselName == StayPunkdname)
                        {
                            vessel.vesselName = StayPunkdname;
                            stayPunkd = true;
                            player = false;
                            team = true;

                        }

                        if (orx || this.vessel.vesselName == OrXname)
                        {
                            vessel.vesselName = OrXname;
                            orx = true;
                            player = false;
                            team = true;
                        }

                        var awaken = this.vessel.FindPartModuleImplementing<KerbalEVA>();
                        Awaken(awaken);
                        OnStarten(awaken);
                    }
                    else
                    {
                        player = true;
                        team = false;
                        orx = false;
                        stayPunkd = false;
                        brute = false;
                        waldo = false;

                    }
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
                    if (!player)
                    {
                        if (setup)
                        {
                            setup = false;
                            Setup();
                        }

                        if (part.vessel.LandedOrSplashed)
                        {
                            ChaseCheck();
                        }
                    }

                    if (!checkHPstarted && !HPCheckStarted)
                    {
                        HPCheckStarted = true;
                        checkHPstarted = true;
                        StartCoroutine(checkHP());
                    }

                    if (vessel.isActiveVessel)
                    {
                        if (player)
                        {
                            if (!sizeCheck)
                            {
                                if (scale)
                                {
                                    GoBig();
                                }
                            }
                            else
                            {
                                if (resetHead && !headReset)
                                {
                                    headReset = true;
                                    //GetHeadBone();
                                }
                            }
                        }
                        else
                        {
                            OrX_Log.instance.FindPlayerVessel();
                        }
                    }

                    if (vessel.atmDensity > 0.0007 && FlightGlobals.currentMainBody.atmosphereContainsOxygen)
                    {
                        if (!helmetRemoved)
                        {
                            RemoveHelmet();
                        }
                    }
                    else
                    {
                        if (helmetRemoved)
                        {
                            ShowHelmet();
                        }
                    }
                }
                else
                {
                    CureInfected();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region BDAc Interface

        public BDExplosivePart explosive;
        private BDExplosivePart Getexplosive()
        {
            BDExplosivePart e = null;

            e = part.FindModuleImplementing<BDExplosivePart>();

            return e;
        }

        //////////////////////////////////////////////////////////////////////////////
        // LEVEL UP, SETUP AND HEALTH
        //////////////////////////////////////////////////////////////////////////////

        private void Setup()
        {
            if (!player)
            {
                if (vessel.vesselName == Waldoname || waldo)
                {
                    this.part.AddModule("ModuleOrXMissileFire");

                    Debug.Log("[OrX Module] Weapon Manager Added to Waldo");

                    vessel.vesselName = Waldoname;
                    waldo = true;
                    KerbinMissions.instance.saltTotal += waldoSalt;
                    StartCoroutine(WaldoSetup());
                }

                if (vessel.vesselName == Brutename || brute)
                {
                    vessel.vesselName = Brutename;
                    brute = true;
                    KerbinMissions.instance.saltTotal += bruteSalt;
                    StartCoroutine(BruteSetup());
                }

                if (vessel.vesselName == StayPunkdname || stayPunkd)
                {
                    vessel.vesselName = StayPunkdname;
                    stayPunkd = true;
                    KerbinMissions.instance.saltTotal += stayPunkdSalt;
                    StartCoroutine(stayPunkdSetup());
                }

                if (vessel.vesselName == OrXname || orx)
                {
                    this.part.AddModule("ModuleOrXMissileFire");

                    Debug.Log("[OrX Module] Weapon Manager Added to OrX");

                    vessel.vesselName = OrXname;
                    orx = true;
                    KerbinMissions.instance.saltTotal += orxSalt;
                    StartCoroutine(UpdateorxRoutine());
                }
            }
        }

        public void LevelUP()
        {
            if (level != KerbinMissions.instance.level)
            {
                hpTracker = GetHP();
                //kerbal = GetKerbal();
                level = KerbinMissions.instance.level;
                /*
                var mjf = _maxJumpForce + 0.1f;
                _maxJumpForce = mjf;
                var ws = _walkSpeed + 0.2f;
                _walkSpeed = ws;
                var rs = _runSpeed + 0.2f;
                _runSpeed = rs;
                var ss = _strafeSpeed + 0.2f;
                _strafeSpeed = ss;
                var swim = _swimSpeed + 0.2f;
                _swimSpeed = swim;
                */
                //kerbal.maxJumpForce = _maxJumpForce;
                //kerbal.walkSpeed = _walkSpeed;
                //kerbal.runSpeed = _runSpeed;
                //kerbal.strafeSpeed = _strafeSpeed;
                //kerbal.swimSpeed = _swimSpeed;

                hpTracker.maxHitPoints = hpMax + (10 * (level - 1));
                hpMax = hpTracker.maxHitPoints;
                hpTracker.Hitpoints = hpMax;
                hp = hpTracker.Hitpoints;
            }
        }

        public HitpointTracker hpTracker;
        private HitpointTracker GetHP()
        {
            HitpointTracker hp = null;

            hp = part.FindModuleImplementing<HitpointTracker>();

            return hp;
        }

        public bool lowHP1 = false;

        IEnumerator checkHP()
        {
            kerbal = GetKerbal();

            hpTracker = GetHP();
            hp = hpTracker.Hitpoints;
            var averagedHP = hpTracker.Hitpoints / hpTracker.maxHitPoints;

            if (hpToRemove <= 0)
            {
                hpTracker.Hitpoints += hpToRemove;
                hpToRemove = 0;
            }

            if (hpToAdd >= 0)
            {
                hpTracker.Hitpoints += hpToAdd;
                hpToAdd = 0;
            }

            if (hp <= 0)
            {
                this.part.explode();
            }
            
            if (!player)
            {
                if (averagedHP <= 0.35f)
                {
                    if (!lowHP1)
                    {

                        if (averagedHP <= 0.2f)
                        {
                            kerbal = GetKerbal();
                            lowHP1 = true;
                            kerbal.walkSpeed -= _walkSpeed / 3;
                            kerbal.runSpeed -= _runSpeed / 3;
                        }
                        else
                        {
                            kerbal.walkSpeed -= _walkSpeed / 2;
                            kerbal.runSpeed -= _runSpeed / 2;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1);
            
            if (checkHPstarted)
            {
                StartCoroutine(checkHP());
            }
        }

        public void MediPack()
        {
            hpTracker = GetHP();

            if (medPack)
            {
                medPack = false;

                if (hpTracker.Hitpoints <= hpTracker.maxHitPoints - (25 * medPackLevel))
                {
                    hpTracker.Hitpoints += 25 * medPackLevel;
                }
                else
                {
                    hpTracker.Hitpoints = hpTracker.maxHitPoints;
                }
            }
        }

        public void CureInfected()
        {
            player = true;
            team = false;
            orx = false;
            stayPunkd = false;
            brute = false;
            waldo = false;

            var wmPart = this.vessel.FindPartModuleImplementing<ModuleOrXMissileFire>();
            if (wmPart != null)
            {
                Destroy(wmPart);
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        // TARGETING CHECK - DISABLED
        //////////////////////////////////////////////////////////////////////////////

        private void CheckTargetRoutine()
        {
            var t = FlightCamera.fetch.Target;

            if (t == null)
            {
                SetTargetRoutine();
            }
        }

        private void SetTargetRoutine()
        {
            var targetOrX = 0.0f;

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                if (v.Current.vesselName == "Waldo" && targetOrX == 0)
                {
                    targetOrX += 1;
                    FlightCamera.SetTarget(v.Current);
                }
            }
            v.Dispose();
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #region Waldo

        IEnumerator WaldoSetup()
        {
            if (!brute && !stayPunkd)
            {
                Debug.Log("[OrX Module] WALDO ........ Infecting Kerbal ...............");
                yield return new WaitForEndOfFrame();

                var wmPart = vessel.FindPartModuleImplementing<ModuleOrXMissileFire>();
                if (wmPart != null)
                {
                    wmPart.team = true;
                    wmPart.guardMode = true;
                }
                finishHim = false;
                team = true;
                yield return new WaitForSeconds(1);

                this.vessel.vesselName = "Waldo";
                KerbinMissions.instance.waldo = true;
                kerbalName = this.vessel.vesselName;
                level = KerbinMissions.instance.level;
                yield return new WaitForEndOfFrame();

                Debug.Log("[OrX Module] WALDO KERBAL ........Resizing " + part.name);

                // ADD CODE FOR HEAD ATTACH HERE ... BEFORE KERBAL HAS BEEN RESIZED
                //            ModuleBruteHead.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                //            ModuleBruteHead.instance.CheckSpawnTimer();

                yield return new WaitForEndOfFrame();

                kerbalLarge = this.part.gameObject;
                kerbalLarge.transform.localScale += new Vector3(4, 4, 4);

                hpTracker = GetHP();
                kerbal = GetKerbal();

                hpTracker.maxHitPoints = 100 + (25 * level);
                hpTracker.Hitpoints += hpTracker.maxHitPoints;

                kerbal.runSpeed = _runSpeed * 0.9f;
                kerbal.walkSpeed = _walkSpeed * 1.2f;

                //            kerbal.minRunningGee = 10;
                //            kerbal.boundForce = 2;
                //            kerbal.boundSpeed = 0.2f;

                //kerbal.massMultiplier = 100000;
                //            kerbal.swimSpeed = 0.4f;

                var _hpDamage = hpDamage * level * 10;
                hpDamage = _hpDamage;

                stayPunkdCheck = true;
                kerbal.minRunningGee = 10;

                //StartCoroutine(WaldoChase());
                //WaldoHPCheck();

                if (!player)
                {
                    part.OnJustAboutToBeDestroyed += DisableGui;
                    EnableGui();
                }
            }
        }

        public void ToggleWaldo()
        {
            if (!waldo)
            {
                waldo = true;
                WaldoHP();
            }
            else
            {
                stayPunkd = false;
                StartCoroutine(ResetKerbSize());
            }
        }

        private void WaldoRoutine()
        {
            if (!attacking)
            {
                StartCoroutine(DrawHP());
            }

            if (!chasing)
            {
                chasing = true;
                StartCoroutine(WaldoChase());
            }
        }

        private void WaldoAttackRoutine()
        {
            attacking = true;

            StartCoroutine(WaldoUpdateHP());

            var random = new System.Random().Next(0, 100);

            if (waldoGroundTarget)
            {
                Debug.Log("[OrX Module - Waldo] Ground Vessel Detected ............. waldoGroundTarget");

                if (KerbinMissions.instance.Baikerbanur)
                {
                    SpawnWaldoAttack.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnWaldoAttack.instance.sg02 = true;
                    SpawnWaldoAttack.instance.CheckSpawnTimer();

                    StartCoroutine(WaldoAttackDelay());
                }
                else
                {
                    SpawnWaldoAttack.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                    SpawnWaldoAttack.instance.sg01 = true;
                    SpawnWaldoAttack.instance.CheckSpawnTimer();

                    StartCoroutine(WaldoAttackDelay());
                }

            }
            else
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    if (FlightGlobals.ActiveVessel.vesselName != "PREDATOR")
                    {

                        Debug.Log("[OrX Module - Waldo] EVA Detected ............. FlightGlobals.ActiveVessel");

                        if (random <= 50)
                        {
                            SpawnWaldoAttack.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                            SpawnWaldoAttack.instance.sg01 = true;
                            SpawnWaldoAttack.instance.CheckSpawnTimer();
                            StartCoroutine(WaldoAttackDelay());

                        }
                        else
                        {
                            SpawnWaldoAttack.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                            SpawnWaldoAttack.instance.sg01 = true;
                            SpawnWaldoAttack.instance.CheckSpawnTimer();
                            StartCoroutine(WaldoAttackDelay());

                        }

                    }
                    else
                    {
                        StartCoroutine(WaldoAttackDelay());
                    }
                }
                else
                {
                    Debug.Log("[OrX Module - Waldo] Ground Vessel Detected ........... FlightGlobals.ActiveVessel");

                    if (random <= 50)
                    {
                        SpawnWaldoAttack.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                        SpawnWaldoAttack.instance.sg01 = true;
                        SpawnWaldoAttack.instance.CheckSpawnTimer();
                        StartCoroutine(WaldoAttackDelay());


                    }
                    else
                    {
                        SpawnWaldoAttack.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                        SpawnWaldoAttack.instance.sg01 = true;
                        SpawnWaldoAttack.instance.CheckSpawnTimer();
                        StartCoroutine(WaldoAttackDelay());

                    }
                }
            }
        }

        IEnumerator WaldoAttackDelay()
        {
            hpTracker = GetHP();

            if (hpTracker.Hitpoints <= (hpTracker.maxHitPoints / 3) * 2)
            {
                var random = new System.Random().Next(5, 10);
                yield return new WaitForSeconds(random);
                StartCoroutine(WaldoChase());

            }
            else
            {
                if (hpTracker.Hitpoints <= hpTracker.maxHitPoints / 3)
                {
                    var random = new System.Random().Next(3, 8);
                    yield return new WaitForSeconds(random);
                    StartCoroutine(WaldoChase());

                }
                else
                {
                    var random = new System.Random().Next(4, 10);
                    yield return new WaitForSeconds(random);
                    StartCoroutine(WaldoChase());

                }
            }
        }

        IEnumerator WaldoChase()
        {
            double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());

            if (targetDistance >= 2000)
            {
                this.vessel.DestroyVesselComponents();
                this.vessel.Die();
            }

            yield return new WaitForSeconds(1);
            level = KerbinMissions.instance.level;
            var levelCalc = level * 0.05f;
            var count = 0;
            waldoGroundTarget = false;
            kerbal = GetKerbal();

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                try
                {
                    if (count == 0)
                    {
                        if (v.Current == null) continue;
                        if (!v.Current.loaded || v.Current.packed) continue;

                        if (!v.Current.isEVA)
                        {
                            var wmPart = v.Current.FindPartModuleImplementing<MissileFire>();

                            if (wmPart != null)
                            {
                                if (wmPart.team != team)
                                {
                                    count = 1;
                                    waldoGroundTarget = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            v.Dispose();

            if (waldoGroundTarget)
            {
                var chase = part.FindModuleImplementing<OrXchaseModule>();
                //kerbal.minRunningGee = 1;
                chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                chase.Patrol();
                //chase.PatrolRun();

                yield return new WaitForSeconds(10);

                chase.Stay();
                chase.Wait();

                kerbal.minRunningGee = 10;

                yield return new WaitForEndOfFrame();

                WaldoAttackRoutine();
            }
            else
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    if (FlightGlobals.ActiveVessel.vesselName != "PREDATOR")
                    {
                        kerbal = GetKerbal();
                        kerbal.minRunningGee = 10;

                        var chase = part.FindModuleImplementing<OrXchaseModule>();
                        chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        chase.Patrol();
                        yield return new WaitForSeconds(10);

                        chase.Stay();
                        chase.Wait();

                        yield return new WaitForEndOfFrame();

                        WaldoAttackRoutine();
                    }
                    else
                    {
                        var chase = part.FindModuleImplementing<OrXchaseModule>();

                        chase.targetCoords = this.vessel.GetWorldPos3D();
                        chase.Patrol();
                        chase.Stay();

                        WaldoAttackRoutine();

                    }
                }
                else
                {
                    var chase = part.FindModuleImplementing<OrXchaseModule>();

                    //kerbal.minRunningGee = 1;
                    chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();

                    chase.Patrol();
                    //chase.PatrolRun();

                    yield return new WaitForSeconds(3);

                    chase.Stay();
                    chase.Wait();

                    kerbal.minRunningGee = 10;

                    yield return new WaitForEndOfFrame();

                    WaldoAttackRoutine();
                }
            }
        }

        public void WaldoHP()
        {
            //stayPunkd = true;
            waldo = true;

            if (this.vessel.Landed)
            {
                StartCoroutine(WaldoSetup());
            }
            else
            {
                StartCoroutine(WaldoDelay());
            }
        }

        IEnumerator WaldoDelay()
        {
            yield return new WaitForSeconds(2);
            WaldoHP();
        }

        IEnumerator WaldoUpdateHP()
        {
            hpTracker = GetHP();

            if (hpTracker.Hitpoints <= hpTracker.maxHitPoints / 3 && finishHim)
            {
                if (KerbinMissions.instance.level <= 11)
                {
                    finishHim = false;
                    //OrX_Log.instance.sound_OrXFinishHim.Play();

                }
            }

            if (hpTracker.Hitpoints <= hpTracker.maxHitPoints / 5)
            {
                DisableGui();
                explosive = Getexplosive();
                explosive.tntMass = level;
                Explode();
                //OrX_Log.instance.sound_OrXFinishHim.Play();
            }

            yield return new WaitForSeconds(5);

            //WaldoHPCheck();
        }

        void WaldoHPCheck()
        {
            if (finishHim)
            {
                StartCoroutine(WaldoUpdateHP());
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #region Waldo GUI
        /// <summary>
        /// GUI
        /// </summary>

        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public bool GuiEnabledOrX_WaldoHP = false;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        public float _hp = 0;
        private float _oxygen = 0.0f;

        private void Start()
        {
            hpTracker = GetHP();
            _windowRect = new Rect((Screen.width / 4) * 3 - WindowWidth - 20, 10, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXWaldoHP);
            GameEvents.onShowUI.Add(GameUiEnableOrXWaldoHP);
            _gameUiToggle = true;
            _hp = hp;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrX_WaldoHP && _gameUiToggle)
            {
                _windowRect = GUI.Window(93635702, _windowRect, GuiWindowOrX_WaldoHP, "");
            }
        }

        private void GuiWindowOrX_WaldoHP(int OrX_WaldoHP)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawHPNum(line);
            DrawTitleHP(line);
            line++;
            DrawHP(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void EnableGui()
        {
            OrX_Controls.instance.waldo = true;
            OrX_Controls.instance.WaldoToggle();
            GuiEnabledOrX_WaldoHP = true;
            Debug.Log("[OrX]: Showing Waldo HP GUI");
        }

        private void DisableGui()
        {
            OrX_Controls.instance.waldo = false;
            OrX_Controls.instance.WaldoToggle();
            GuiEnabledOrX_WaldoHP = false;
            Debug.Log("[OrX]: Hiding Waldo HP GUI");
        }

        private void GameUiEnableOrXWaldoHP()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXWaldoHP()
        {
            _gameUiToggle = false;
        }

        private void DrawHPNum(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "HP: " + hpTracker.Hitpoints + " / " + hpTracker.maxHitPoints, titleStyle);
        }

        private void DrawTitleHP(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "WALDO'S HIT POINT %",
                titleStyle);
        }

        private void DrawHP(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "0");
            GUI.Label(new Rect(30, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(90, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(150, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(175, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "100");
            _hp = GUI.HorizontalSlider(saveRect, hp, 0, hpMax);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #region Stay Punkd

        IEnumerator stayPunkdSetup()
        {
            if (!brute && !waldo)
            {
                Debug.Log("[OrX Module] STAY PUNKD ........ Infecting Kerbal ...............");

                yield return new WaitForSeconds(1);
                var wmPart = vessel.FindPartModuleImplementing<MissileFire>();
                if (wmPart != null)
                {
                    wmPart.team = true;
                    wmPart.guardMode = true;
                }
                finishHim = false;
                team = true;

                this.vessel.vesselName = "OrX Stay Punkd";
                kerbalName = this.vessel.vesselName;
                level = KerbinMissions.instance.level;
                yield return new WaitForEndOfFrame();

                /*
                Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL ........Resizing " + part.name);

                List<Part>.Enumerator p = this.vessel.parts.GetEnumerator();
                while (p.MoveNext())
                {
                    if (p.Current == null) continue;
                    if (p.Current.packed) continue;

                    kerbalLarge = p.Current.gameObject;
                    savedTransform = kerbalLarge.transform;
                    kerbalLarge.transform.localScale += new Vector3(8, 8, 8);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........layer " + kerbalLarge.layer);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........transform.parent " + kerbalLarge.transform.parent);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........transform.position " + kerbalLarge.transform.position);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........transform.right " + kerbalLarge.transform.right);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........transform.root " + kerbalLarge.transform.root);
                    Debug.Log("[OrX Module] STAY PUNKD MARSHMALLOW KERBAL .. " + p.Current.name + "  ........transform.rotation " + kerbalLarge.transform.rotation);

                }
                p.Dispose();
                */

                hpTracker = GetHP();
                kerbal = GetKerbal();

                hpTracker.maxHitPoints = 10 + (5 * level);
                hpTracker.Hitpoints += hpTracker.maxHitPoints;

                kerbal.walkSpeed = _walkSpeed; // * 1.2f;

                //kerbal.boundForce = 2;
                //kerbal.boundSpeed = 0.2f;

                //kerbal.massMultiplier = 100000;
                //            kerbal.swimSpeed = 0.4f;

                var _hpDamage = hpDamage * level * 5;
                hpDamage = _hpDamage;


                stayPunkdCheck = true;
                kerbal.minRunningGee = 10;

                if (!player)
                {
                    Chase();
                }
            }
        }

        public void TogglestayPunkd()
        {
            if (!stayPunkd)
            {
                stayPunkd = true;
                stayPunkdHP();
            }
            else
            {
                stayPunkd = false;
                StartCoroutine(ResetKerbSize());
            }
        }

        private void StayPunkdRoutine()
        {
            if (!attacking)
            {
                StartCoroutine(DrawHP());
            }

            if (!chasing)
            {
                chasing = true;
                StartCoroutine(ChaseOrX());
            }
        }

        public void stayPunkdHP()
        {
            stayPunkd = true;

            if (this.vessel.Landed)
            {
                StartCoroutine(stayPunkdSetup());
            }
            else
            {
                StartCoroutine(stayPunkdDelay());
            }
        }

        IEnumerator stayPunkdDelay()
        {
            yield return new WaitForSeconds(2);
            stayPunkdHP();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #region Brute

        IEnumerator BruteSetup()
        {
            if (!stayPunkd && !waldo)
            {
                Debug.Log("[OrX Module] BRUTE ........ Infecting Kerbal ...............");

                yield return new WaitForSeconds(1);
                var wmPart = vessel.FindPartModuleImplementing<MissileFire>();
                if (wmPart != null)
                {
                    wmPart.team = true;
                    wmPart.guardMode = true;
                }
                finishHim = false;
                team = true;

                this.vessel.vesselName = "Brute";
                kerbalName = this.vessel.vesselName;
                level = KerbinMissions.instance.level;
                yield return new WaitForEndOfFrame();

                //Debug.Log("[OrX Module] BRUTE KERBAL ........Resizing " + part.name);

                // ADD CODE FOR HEAD ATTACH HERE ... BEFORE KERBAL HAS BEEN RESIZED
                //            ModuleBruteHead.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                //            ModuleBruteHead.instance.CheckSpawnTimer();
                /*
                yield return new WaitForEndOfFrame();

                List<Part>.Enumerator p = this.vessel.parts.GetEnumerator();
                while (p.MoveNext())
                {
                    if (p.Current == null) continue;
                    if (p.Current.packed) continue;

                    kerbalLarge = p.Current.gameObject;
                    savedTransform = kerbalLarge.transform;
                    kerbalLarge.transform.localScale += new Vector3(3, 3, 3);

                }
                p.Dispose();

                */

                hpTracker = GetHP();
                kerbal = GetKerbal();

                hpTracker.maxHitPoints = 5 + (3 * level);
                hpTracker.Hitpoints += hpTracker.maxHitPoints;

                kerbal.runSpeed = _runSpeed * 0.8f;
                kerbal.walkSpeed = _walkSpeed; // * 0.9f;

                kerbal.minRunningGee = 10;
                //            kerbal.boundForce = 2;
                //            kerbal.boundSpeed = 0.2f;

                //kerbal.massMultiplier = 50000;
                //            kerbal.swimSpeed = 0.4f;

                var _hpDamage = hpDamage + (level * 2);
                hpDamage = _hpDamage;

                stayPunkdCheck = true;

                if (!player)
                {
                    Chase();
                }
            }
        }

        public void ToggleBrute()
        {
            if (!brute)
            {
                brute = true;
                BruteHP();
            }
            else
            {
                brute = false;
                StartCoroutine(ResetKerbSize());
            }
        }

        private void BruteRoutine()
        {
            if (!attacking)
            {
                StartCoroutine(DrawHP());
            }

            if (!chasing)
            {
                chasing = true;
                StartCoroutine(ChaseOrX());
            }
        }

        public void BruteHP()
        {
            brute = true;

            if (this.vessel.Landed)
            {
                StartCoroutine(BruteSetup());
            }
            else
            {
                StartCoroutine(BruteDelay());
            }
        }

        IEnumerator BruteDelay()
        {
            yield return new WaitForSeconds(2);
            BruteHP();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #region OrX

        IEnumerator UpdateorxRoutine()
        {
            if (!player)
            {
                Debug.Log("[OrX Module] OrX ................ Infecting Kerbal ...............");
                yield return new WaitForEndOfFrame();

                var wmPart = this.vessel.FindPartModuleImplementing<ModuleOrXMissileFire>();
                if (wmPart != null)
                {
                    wmPart.team = true;
                    wmPart.guardMode = true;
                }

                SetupBomber();

                yield return new WaitForSeconds(1);
                level = KerbinMissions.instance.level;
                team = true;

                kerbal = GetKerbal();
                kerbal.maxJumpForce = 1;

                if (level <= 5)
                {
                    kerbal.walkSpeed = (_walkSpeed * 0.75f) + (level * 0.05f);
                    kerbal.runSpeed = (_runSpeed * 0.75f) + (level * 0.05f);
                }
                else
                {
                    kerbal.walkSpeed = _walkSpeed;
                    kerbal.runSpeed = _runSpeed;
                }

                kerbal.stumbleThreshold = 50;
                hpTracker = GetHP();
                hpTracker.maxHitPoints = 5 + (level * 2);
                hpTracker.Hitpoints = hpTracker.maxHitPoints;
                hpTracker.Armor = 1;
                hpTracker.ArmorThickness = 1;
            }
        }

        private void OrXRoutine()
        {
            if (!player)
            {
                if (!attacking)
                {
                    StartCoroutine(DrawHP());
                }

                if (!chasing)
                {
                    chasing = true;
                    StartCoroutine(ChaseOrX());
                }
            }
        }

        public void Chase()
        {
            if (!player)
            {
                level = KerbinMissions.instance.level;
                var levelCalc = level * 0.05f;
                team = true;

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    var chase = part.FindModuleImplementing<OrXchaseModule>();
                    chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    chase.Patrol();
                    chase.PatrolRun();
                }
                else
                {
                    var chase = part.FindModuleImplementing<OrXchaseModule>();
                    chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    if (!stayPunkd && !brute)
                    {
                        chase.Patrol();
                        chase.PatrolRun();
                    }
                    else
                    {
                        chase.Patrol();
                        chase.PatrolRun();
                    }
                }
            }
        }

        public void CheckGuard()
        {
            var wmPart = this.vessel.FindPartModuleImplementing<MissileFire>();

            if (!wmPart.guardMode)
            {
                guard = true;
                wmPart.guardMode = true;
            }
            else
            {
                guard = false;
                wmPart.guardMode = false;
            }
        }

        private void SetupBomber()
        {
            var random = new System.Random().Next(0, 100);

            if (random <= 20)
            {
                bomber = true;
                part.AddModule("BDExplosivePart");
            }
        }

        private void CheckBomber()
        {
            hpTracker = GetHP();

            if (hpTracker.Hitpoints <= hpTracker.maxHitPoints * 0.5)
            {
                DetonateMineRoutine();
            }
        }

        IEnumerator ChaseOrX()
        {
            if (HighLogic.LoadedSceneIsFlight && !player)
            {
                chasing = true;

                double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());

                if (targetDistance >= 2000 || this.vessel.heightFromTerrain <= 0 || this.vessel.Splashed)
                {
                    this.vessel.DestroyVesselComponents();
                    this.vessel.Die();
                }


                yield return new WaitForSeconds(5);

                if (kerbal.isRagdoll)
                {
                    //Debug.Log("[OrX Module] " + this.vessel.vesselName + " Recovering from ragdoll .....................");

                    kerbal.RecoverFromRagdoll();
                }

                yield return new WaitForEndOfFrame();
                targetOrX = 0.0f;

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    var chase = this.vessel.FindPartModuleImplementing<OrXchaseModule>();

                    Debug.Log("[OrX Module] " + FlightGlobals.ActiveVessel.vesselName + " Detected ..................... CHASING");

                    targetOrX += 1;
                    Chase();
                }
                else
                {
                    Debug.Log("[OrX Module] No Player Kerbal Detected ......");

                    if (targetOrX == 0)
                    {
                        Debug.Log("[OrX Module] Active Vessel " + FlightGlobals.ActiveVessel.vesselName + " found ....................");
                        //var chase = this.vessel.FindPartModuleImplementing<OrXchaseModule>();

                        targetOrX += 1;

                        Chase();

                        //var targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        //chase.targetCoords = targetCoords;
                        //chase.Patrol();
                        //chase.PatrolRun();
                        //Debug.Log("[OrX Module] GPS Location of Active Vessel recorded: CHASING " + FlightGlobals.ActiveVessel.vesselName);

                    }
                }

                yield return new WaitForSeconds(10);

                chasing = false;
            }
        }

        IEnumerator DetonateMineRoutine()
        {
            explosive = Getexplosive();
            explosive.tntMass = 1;
            explosive.ArmAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            explosive.DetonateAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            yield return new WaitForEndOfFrame();
            part.explode();
        }

        public void Explode()
        {
            if (bomber)
            {
                StartCoroutine(DetonateMineRoutine());
            }
        }

        IEnumerator DrawHP()
        {
            if (!player)
            {
                var count = 0;
                var reset = 0;
                attacking = true;

                if (stayPunkd)
                {
                    reset = 8;

                    List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
                    while (v.MoveNext())
                    {
                        if (v.Current == null) continue;
                        if (!v.Current.loaded || v.Current.packed) continue;

                        if (v.Current.isEVA && count == 0)
                        {
                            double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                            if (targetDistance <= 2)
                            {
                                var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>();

                                if (OrXKerbal.player)
                                {
                                    Debug.Log("[OrX Module - stayPunkd] Applying HP Damage to " + v.Current.vesselName + " ....................");
                                    count += 1;

                                    OrXKerbal.hpToRemove -= hpDamage * 4;

                                    Debug.Log("[OrX Module - stayPunkd] Punting " + v.Current.vesselName + " ....................");
                                    float speed = 100;
                                    v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;

                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (count == 0)
                            {
                                double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                if (targetDistance <= 5)
                                {
                                    var lootBox = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                                    var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();

                                    if (tardis == null && lootBox == null)
                                    {
                                        count += 1;
                                        float speed = 200;
                                        v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;
                                        Debug.Log("[OrX Module - stayPunkd] Punting " + v.Current.vesselName + " ....................");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    v.Dispose();
                }
                else
                {
                    if (brute)
                    {
                        reset = 4;

                        List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
                        while (v.MoveNext())
                        {
                            if (v.Current == null) continue;
                            if (!v.Current.loaded || v.Current.packed) continue;

                            if (v.Current.isEVA && count == 0)
                            {
                                var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>();

                                if (OrXKerbal.player)
                                {
                                    double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                    if (targetDistance <= 2)
                                    {
                                        Debug.Log("[OrX Module - Brute] Applying HP Damage to " + v.Current.vesselName + " ....................");

                                        count += 1;
                                        OrXKerbal.hpToRemove -= hpDamage * 2;

                                        Debug.Log("[OrX Module - Brute] Punting " + v.Current.vesselName + " ....................");
                                        float speed = 10;
                                        v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (count == 0)
                                {
                                    double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                    if (targetDistance <= 5)
                                    {
                                        var lootBox = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                                        var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();

                                        if (tardis == null && lootBox == null)
                                        {
                                            count += 1;
                                            Debug.Log("[OrX Module - Brute] Punting " + v.Current.vesselName + " ....................");

                                            float speed = 50;
                                            v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        v.Dispose();
                    }
                    else
                    {
                        if (!waldo)
                        {
                            reset = 3;

                            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
                            while (v.MoveNext())
                            {
                                if (v.Current == null) continue;
                                if (!v.Current.loaded || v.Current.packed) continue;
                                if (v.Current.parts.Count >= 1)
                                {
                                    double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                    if (targetDistance <= 10)
                                    {
                                        if (v.Current.isEVA && count == 0)
                                        {
                                            var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>();

                                            if (OrXKerbal.player)
                                            {
                                                if (targetDistance <= 2)
                                                {
                                                    Debug.Log("[OrX Module - OrX] Applying HP Damage to " + v.Current.vesselName + " ....................");
                                                    OrXKerbal.hpToRemove -= hpDamage;
                                                    count += 1;

                                                    float speed = 5;
                                                    Debug.Log("[OrX Module - OrX] Punting " + v.Current.vesselName + " ....................");

                                                    v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;

                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (count == 0)
                                            {
                                                var lootBox = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                                                var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();

                                                if (tardis == null && lootBox == null)
                                                {
                                                    count += 1;
                                                    Debug.Log("[OrX Module - OrX] Punting " + v.Current.vesselName + " ....................");

                                                    float speed = 30;
                                                    v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;
                                                    Explode();

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            v.Dispose();
                        }
                        else
                        {
                            reset = 5;

                            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
                            while (v.MoveNext())
                            {
                                if (v.Current == null) continue;
                                if (!v.Current.loaded || v.Current.packed) continue;

                                double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                if (targetDistance <= 10)
                                {
                                    if (v.Current.isEVA && count == 0)
                                    {
                                        var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>();

                                        if (OrXKerbal != null && OrXKerbal.player)
                                        {
                                            Debug.Log("[OrX Module - Waldo] Applying HP Damage to " + v.Current.vesselName + " ....................");

                                            count += 1;

                                            OrXKerbal.hpToRemove -= hpDamage;

                                            float speed = 100;
                                            Debug.Log("[OrX Module - Waldo] Punting " + v.Current.vesselName + " ....................");

                                            v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (count == 0)
                                        {
                                            var lootBox = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                                            var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();

                                            if (tardis == null && lootBox == null)
                                            {
                                                count += 1;
                                                Debug.Log("[OrX Module - Waldo] Punting " + v.Current.vesselName + " ....................");

                                                float speed = 150;
                                                v.Current.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * speed;
                                                Explode();

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            v.Dispose();
                        }
                    }
                }
                yield return new WaitForSeconds(reset);
                attacking = false;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Kerbal

        //////////////////////////////////////////////////////////////////////////////
        // KERBAL UPDATES
        //////////////////////////////////////////////////////////////////////////////

        public KerbalEVA kerbal;
        private KerbalEVA GetKerbal()
        {
            KerbalEVA k = null;

            k = part.FindModuleImplementing<KerbalEVA>();

            return k;
        }

        public void UpgradeStats()
        {
            kerbal = GetKerbal();

            if (walkSpeedUp)
            {
                walkSpeedUp = false;

                kerbal.walkSpeed = _walkSpeed + walkSpeedUpLevel;
                _walkSpeed = kerbal.walkSpeed;
            }

            if (runSpeedUp)
            {
                runSpeedUp = false;
                kerbal.runSpeed = _runSpeed + runSpeedUpLevel;
                _runSpeed = kerbal.runSpeed;
            }

            if (jumpForceUp)
            {
                jumpForceUp = false;

                kerbal.maxJumpForce = _maxJumpForce + jumpForceUpLevel;
                _maxJumpForce = kerbal.maxJumpForce;
            }

            if (swimSpeedUp)
            {
                swimSpeedUp = false;

                kerbal.swimSpeed = _swimSpeed + swimSpeedUpLevel;
                _swimSpeed = kerbal.swimSpeed;
            }
        }

        IEnumerator ResetKerbSize()
        {
            scale = false;
            brute = false;
            stayPunkd = false;
            waldo = false;
            kerbal = GetKerbal();
            this.vessel.vesselName = kerbalName;
            kerbal.massMultiplier = 0.03f;
            kerbal.runSpeed = _runSpeed;
            kerbal.walkSpeed = _walkSpeed;
            kerbal.maxJumpForce = _maxJumpForce;
            yield return new WaitForFixedUpdate();
            kerbalLarge = this.part.gameObject;
            kerbalLarge.transform.localScale += savedTransform.localScale;
        }

        public void ResetKerbalRoutine()
        {
            StartCoroutine(UpdateKerbalRoutine());
            //helmCheck = true;
        }

        IEnumerator UpdateKerbalRoutine()
        {
            pause = true;
            kerbal = GetKerbal();
            kerbal.maxJumpForce = _maxJumpForce;
            kerbal.walkSpeed = _walkSpeed;
            kerbal.runSpeed = _runSpeed;
            kerbal.strafeSpeed = _strafeSpeed;
            kerbal.swimSpeed = _swimSpeed;

            yield return new WaitForSeconds(1);
            pause = false;
            //helmCheck = true;
        }

        //////////////////////////////////////////////////////////////////////////////

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ITEMS - HELMETS, SCUBA KERB ETC...
        //////////////////////////////////////////////////////////////////////////////
        // HELMETS
        //////////////////////////////////////////////////////////////////////////////

        IEnumerator CheckHelm()
        {
            helmCheck = false;

            yield return new WaitForEndOfFrame();

            if (ww2Helm)
            {
                var ww2 = part.vessel.FindPartModuleImplementing<ModuleHelmet_WW2>();
                if (ww2 != null)
                {
                    WW2Helmet();
                }
                else
                {
                    ww2Helm = false;
                    StartCoroutine(UpdateKerbalRoutine());
                }
            }

            if (RaceHelm)
            {
                var race = part.vessel.FindPartModuleImplementing<ModuleHelmet_Race>();
                if (race != null)
                {
                    RaceHelmet();
                }
                else
                {
                    RaceHelm = false;
                    StartCoroutine(UpdateKerbalRoutine());
                }
            }

            if (OrXHelm)
            {
                var orxHelm = part.vessel.FindPartModuleImplementing<ModuleHelmet_OrX>();
                if (orxHelm != null)
                {
                    OrXHelmet();
                }
                else
                {
                    OrXHelm = false;
                    StartCoroutine(UpdateKerbalRoutine());
                }
            }
        }

        private void RemoveHelmet()
        {
            showHelmet = false;
            kerbal.ShowHelmet(showHelmet);
            kerbal.lampOn = false;
            helmetRemoved = true;
        }

        private void ShowHelmet()
        {
            showHelmet = true;
            kerbal.ShowHelmet(showHelmet);
            kerbal.lampOn = false;
            helmetRemoved = false;
        }

        private void OrXHelmet()
        {
            hpCheck = hp;
            ww2Helm = false;
            RaceHelm = false;
            helmCheck = true;
        }

        private void RaceHelmet()
        {
            kerbal = GetKerbal();
            kerbal.maxJumpForce += maxJumpForce_;
            kerbal.walkSpeed += walkSpeed_;
            kerbal.runSpeed += runSpeed_;
            kerbal.strafeSpeed += strafeSpeed_;
            ww2Helm = false;
            OrXHelm = false;
            helmCheck = true;
        }

        private void WW2Helmet()
        {
            kerbal = GetKerbal();
            kerbal.maxJumpForce += maxJumpForce;
            kerbal.walkSpeed += walkSpeed;
            kerbal.runSpeed += runSpeed;
            kerbal.strafeSpeed += strafeSpeed;
            RaceHelm = false;
            OrXHelm = false;
            helmCheck = true;
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

        private void GoBig()
        {
            if (brutify && !stayPunkd && !waldo)
            {
                sizeCheck = true;
                scale = false;
                player = true;
                StartCoroutine(BruteSetup());
            }

            if (punkify && !brute && !waldo)
            {
                sizeCheck = true;
                scale = false;
                player = true;
                StartCoroutine(stayPunkdSetup());
            }

            if (goWaldo && !brute && !stayPunkd)
            {
                sizeCheck = true;
                scale = false;
                player = true;
                StartCoroutine(WaldoSetup());
            }
        }

        private void ChaseCheck()
        {
            if (orx)
            {
                OrXRoutine();
            }

            if (waldo)
            {
                WaldoRoutine();
            }

            if (brute)
            {
                BruteRoutine();
            }

            if (stayPunkd)
            {
                StayPunkdRoutine();
            }
        }

        private void GetHeadBone()
        {
            var count2 = 0;
            headBone = null;

            var skmrs = new List<SkinnedMeshRenderer>(this.part.GetComponentsInChildren<SkinnedMeshRenderer>());
            foreach (SkinnedMeshRenderer skmr in skmrs)
            {
                foreach (Transform bone in skmr.bones)
                {
                    if (count2 == 0)
                    {
                        Debug.Log("[OrX Stay Puffd Head]  ........ Found Transform: " + bone.name);

                        if (bone.name == "bn_helmet01")
                        {
                            count2 = 1;

                            headBone = bone;
                            Debug.Log("[OrX Stay Puffd Head]  ........ Found bn_helmet01 Bone");
                        }
                    }
                }
            }

            if (headBone != null)
            {
                List<Part>.Enumerator head = this.vessel.parts.GetEnumerator();
                while (head.MoveNext())
                {
                    if (head.Current == null) continue;
                    if (head.Current.packed) continue;

                    if (head.Current.name.Contains("Helmet"))
                    {
                        kerbalLarge = head.Current.gameObject;
                        kerbalLarge.transform.parent = headBone;
                    }
                }
                head.Dispose();
            }
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

    }
}


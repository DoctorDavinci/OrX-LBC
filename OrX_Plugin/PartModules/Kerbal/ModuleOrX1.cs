//using BDArmory.Core.Module;
using System.Collections;
using UnityEngine;
//using BDArmory.Modules;
using OrX.chase;
using OrX.missions;
using System.Collections.Generic;
using System.Reflection;
using OrX.spawn;
using System;

namespace OrX.parts
{
    public class ModuleOrX : PartModule, IPartMassModifier
    {
        static ModuleOrX instance;
        public static ModuleOrX Instance => instance;

        #region Fields

        private int orxSalt = 15;// + (Missions.instance.level * 2);
        private int bruteSalt = 40;// + (Missions.instance.level * 5);
        private int stayPunkdSalt = 75;// + (Missions.instance.level * 10);
        private int waldoSalt = 100;// + (Missions.instance.level * 20);

        public bool launchSiteChanged = false;
        public bool beach = false;
        public bool islandBeach = false;
        public bool ironKerbal = false;

        private bool bomberDelay = true;

        // [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "TEAM"),
        //  UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "Player", enabledText = "OrX")]
        public bool team = false;

        //  [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "MODIFY KERBAL SCALE"),
        //   UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "OFF", enabledText = "ON")]
        public bool scale = false;

        //  [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "BRUTIFY"),
        //  UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "OFF", enabledText = "ON")]
        public bool brutify = false;

        // [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "STAY PUNKD"),
        // UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "OFF", enabledText = "ON")]
        public bool punkify = false;

        // [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "GO WALDO"),
        // UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "OFF", enabledText = "ON")]
        public bool goWaldo = false;

        //[KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "RESET HEAD PLACEMENT"),
        // UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "OFF", enabledText = "ON")]
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

        /*
        public HitpointTracker hpTracker;
        private HitpointTracker GetHP()
        {
            HitpointTracker hp = null;

            hp = part.FindModuleImplementing<HitpointTracker>();

            return hp;
        }
        */

        public KerbalEVA kerbal;
        private KerbalEVA GetKerbal()
        {
            KerbalEVA k = null;

            k = part.FindModuleImplementing<KerbalEVA>();

            return k;
        }

        /*
        public BDExplosivePart explosive;
        private BDExplosivePart Getexplosive()
        {
            BDExplosivePart e = null;

            e = part.FindModuleImplementing<BDExplosivePart>();

            return e;
        }
        */

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

        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            try
            {
                if (part == null ||
                    !part.Modules.Contains("HitpointTracker")
                    )
                    return;
            }
            catch (Exception)
            { }

            if (KerbalDamaged())
            {
                hpCheck = hp;
            }
        }

        public void OnDestroy()
        {
            instance = null;
        }

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsEditor && !player)
            {
                //player = true;

                //OrX_KerbalLaunch.instance.StartLaunch();
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    instance = this;

                    part.force_activate();
                    part.maxPressure = 200000;

                    //explosive = Getexplosive();
                    kerbalName = this.vessel.vesselName;
                    kerbal = GetKerbal();

                    hp = part.Damage();
                    hpMax = part.MaxDamage();
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
                    {/*
                        //player = true;
                        team = false;
                        orx = false;
                        stayPunkd = false;
                        brute = false;
                        waldo = false;

                        foreach (var crew in this.part.protoModuleCrew)
                        {
                            this.part.vessel.vesselName = crew.name;
                        }*/
                    }
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
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA && !vessel.HoldPhysics)
                {
                    if (vessel.isActiveVessel)
                    {
                        if (player)
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
                            //OrX_Log.instance.FindPlayerVessel();
                        }
                    }

                    if (setup)
                    {
                        setup = false;
                        Setup();
                    }

                    if (!checkHPstarted && !HPCheckStarted)
                    {
                        HPCheckStarted = true;
                        checkHPstarted = true;
                        StartCoroutine(checkHP());
                    }

                    if (vessel.atmDensity > 0.0007 && FlightGlobals.currentMainBody.atmosphereContainsOxygen)
                    {
                        RemoveHelmet();
                    }

                    if (part.vessel.LandedOrSplashed)
                    {
                        ChaseCheck();
                    }
                }
                else
                {

                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Waldo

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

                SpawnWaldoAttack.instance.sg01 = true;
                SpawnWaldoAttack.instance.CheckSpawnTimer();

                StartCoroutine(WaldoAttackDelay());
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
                            //SpawnWaldoAttack.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            //SpawnWaldoAttack.instance.sg01 = true;
                            //SpawnWaldoAttack.instance.CheckSpawnTimer();
                            StartCoroutine(WaldoAttackDelay());

                        }
                        else
                        {
                            //SpawnWaldoAttack.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            //SpawnWaldoAttack.instance.sg02 = true;
                            //SpawnWaldoAttack.instance.CheckSpawnTimer();
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
                        SpawnWaldoAttack.instance.sg03 = true;
                        SpawnWaldoAttack.instance.CheckSpawnTimer();
                        StartCoroutine(WaldoAttackDelay());


                    }
                    else
                    {
                        SpawnWaldoAttack.instance.sg04 = true;
                        SpawnWaldoAttack.instance.CheckSpawnTimer();
                        StartCoroutine(WaldoAttackDelay());

                    }
                }
            }
        }

        IEnumerator WaldoAttackDelay()
        {
            hp = part.Damage();

            if (hp <= (hpMax / 3) * 2)
            {
                var random = new System.Random().Next(5, 10);
                yield return new WaitForSeconds(random);
                StartCoroutine(WaldoChase());

            }
            else
            {
                if (hp <= hpMax / 3)
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
            DistanceCheck();
            yield return new WaitForSeconds(1);
            level = Missions.instance.level;
            var levelCalc = level * 0.05f;
            var count = 0;
            waldoGroundTarget = false;
            kerbal = GetKerbal();

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (count == 0)
                {
                    if (v.Current == null) continue;
                    if (!v.Current.loaded || v.Current.packed) continue;

                    if (!v.Current.isEVA)
                    {
                        count = 1;
                        waldoGroundTarget = true;
                        SpawnWaldoAttack.instance.SpawnCoords = v.Current.GetWorldPos3D();
                    }
                }
            }
            v.Dispose();

            if (waldoGroundTarget)
            {
                var chase = part.FindModuleImplementing<OrXchaseModule>();
                kerbal.minRunningGee = 1;
                chase.targetCoords = SpawnWaldoAttack.instance.SpawnCoords;
                chase.Patrol();
                chase.PatrolRun();

                yield return new WaitForSeconds(3);

                chase.Stay();
                chase.Wait();

                kerbal.minRunningGee = 10;

                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
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
                        yield return new WaitForSeconds(5);

                        chase.Stay();
                        chase.Wait();

                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
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

                    kerbal.minRunningGee = 1;
                    chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();

                    chase.Patrol();
                    chase.PatrolRun();

                    yield return new WaitForSeconds(3);

                    chase.Stay();
                    chase.Wait();

                    kerbal.minRunningGee = 10;

                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();


                    WaldoAttackRoutine();
                }
            }
        }

        public void WaldoHP()
        {
            stayPunkd = true;
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

        IEnumerator WaldoSetup()
        {
            if (!brute && !stayPunkd)
            {
                Debug.Log("[OrX Module] WALDO KERBAL ........ Starting Waldo Setup");
                yield return new WaitForSeconds(1);
                this.part.AddModule("ModuleOrXMissileFire");

                Debug.Log("[OrX Module] Weapon Manager Added ");

                //AddHelmet(StayPunkdHelmet);

                team = true;
                this.vessel.vesselName = "Waldo";
                Missions.instance.waldo = true;
                kerbalName = this.vessel.vesselName;
                level = Missions.instance.level;
                yield return new WaitForEndOfFrame();

                Debug.Log("[OrX Module] WALDO KERBAL ........Resizing " + part.name);

                // ADD CODE FOR HEAD ATTACH HERE ... BEFORE KERBAL HAS BEEN RESIZED
                //            ModuleBruteHead.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                //            ModuleBruteHead.instance.CheckSpawnTimer();

                yield return new WaitForEndOfFrame();

                List<Part>.Enumerator p = this.vessel.parts.GetEnumerator();
                while (p.MoveNext())
                {
                    if (p.Current == null) continue;
                    if (p.Current.packed) continue;

                    kerbalLarge = p.Current.gameObject;
                    kerbalLarge.transform.localScale += new Vector3(5, 5, 5);

                }
                p.Dispose();

                kerbal = GetKerbal();

                hpMax = 3000 + (1000 * level);
                hp = hpMax;

                kerbal.runSpeed = _runSpeed * 0.9f;
                kerbal.walkSpeed = _walkSpeed * 1.2f;

                //            kerbal.minRunningGee = 10;
                //            kerbal.boundForce = 2;
                //            kerbal.boundSpeed = 0.2f;

                kerbal.massMultiplier = 50000;
                //            kerbal.swimSpeed = 0.4f;

                var _hpDamage = hpDamage * level * 10;
                hpDamage = _hpDamage;

                stayPunkdCheck = true;
                kerbal.minRunningGee = 10;

                //StartCoroutine(WaldoChase());
                //WaldoHPCheck();

                if (!GuiEnabledOrX_WaldoHP && !player)
                {
                    EnableGui();
                }
            }
        }

        IEnumerator WaldoUpdateHP()
        {
            hp = part.Damage();

            if (hp <= hpMax / 3 && finishHim)
            {
                if (Missions.instance.level <= 11)
                {
                    finishHim = false;
                    //OrX_Log.instance.sound_OrXFinishHim.Play();

                }
            }

            if (hp <= hpMax / 10)
            {
                OrX_WaldoHP.instance.waldo = false;
                //explosive = Getexplosive();
                //explosive.tntMass = level;
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

        #region Waldo GUI
        /// <summary>
        /// GUI
        /// </summary>

        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_WaldoHP instanceWHP;
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
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 10, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXWaldoHP);
            GameEvents.onShowUI.Add(GameUiEnableOrXWaldoHP);
            _gameUiToggle = true;
            hp = part.Damage();
            hpMax = part.MaxDamage();
            _hp = hp;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrX_WaldoHP && _gameUiToggle)
            {
                _windowRect = GUI.Window(935506702, _windowRect, GuiWindowOrX_WaldoHP, "");
                DisableControlGUI();
            }
        }

        private void DisableControlGUI()
        {
            OrX_Controls.instance.DisableGUI();
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
            GuiEnabledOrX_WaldoHP = true;
            Debug.Log("[OrX]: Showing Waldo HP GUI");
        }

        private void DisableGui()
        {
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "HP: " + hp + " / " + hpMax, titleStyle);
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Stay Punkd

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

        IEnumerator stayPunkdSetup()
        {
            if (!brute && !waldo)
            {
                yield return new WaitForSeconds(1);

                finishHim = false;
                team = true;

                this.vessel.vesselName = "OrX Stay Punkd";
                kerbalName = this.vessel.vesselName;
                level = Missions.instance.level;
                yield return new WaitForEndOfFrame();

                Debug.Log("[OrX Module] STAY PUFFD MARSHMALLOW KERBAL ........ Starting stayPunkdSetup");
                this.part.AddModule("ModuleOrXMissileFire");

                Debug.Log("[OrX Module] Weapon Manager Added ");

                yield return new WaitForFixedUpdate();
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
                kerbal = GetKerbal();

                hpMax = 50 + (25 * level);
                hp = hpMax;
                kerbal.walkSpeed = _walkSpeed; // * 1.2f;

                //kerbal.boundForce = 2;
                //kerbal.boundSpeed = 0.2f;

                kerbal.massMultiplier = 100000;
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Brute

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

        IEnumerator BruteSetup()
        {
            if (!stayPunkd && !waldo)
            {
                yield return new WaitForSeconds(1);
                finishHim = false;
                team = true;

                this.vessel.vesselName = "Brute";
                kerbalName = this.vessel.vesselName;
                level = Missions.instance.level;
                yield return new WaitForEndOfFrame();

                Debug.Log("[OrX Module] BRUTE KERBAL ........ Starting UpdateBrute");
                this.part.AddModule("ModuleOrXMissileFire");

                Debug.Log("[OrX Module] Weapon Manager Added ");

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

                kerbal = GetKerbal();

                hpMax = 25 + (5 * level);
                hp += hpMax;

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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region OrX

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
                level = Missions.instance.level;
                //var levelCalc = level * 0.05f;
                team = true;

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    if (FlightGlobals.ActiveVessel.vesselName != "PREDATOR")
                    {
                        kerbal = GetKerbal();
                        var chase = part.FindModuleImplementing<OrXchaseModule>();
                        chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        if (!stayPunkd && !brute && !waldo)
                        {
                            kerbal.walkSpeed = (_walkSpeed * 0.8f); // + levelCalc;
                            kerbal.runSpeed = (_runSpeed * 0.8f); // + levelCalc;
                            chase.Patrol();
                            chase.PatrolRun();
                        }
                        else
                        {
                            chase.Patrol();
                            chase.PatrolRun();

                            if (stayPunkdCheck)
                            {
                                chase.Patrol();
                                chase.PatrolRun();
                            }
                        }
                    }
                    else
                    {
                        var chase = part.FindModuleImplementing<OrXchaseModule>();
                        chase.targetCoords = this.vessel.GetWorldPos3D();
                        chase.Patrol();
                        chase.Stay();
                    }
                }
                else
                {
                    var chase = part.FindModuleImplementing<OrXchaseModule>();
                    chase.targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    if (!stayPunkd && !brute)
                    {
                        if (level <= 5)
                        {
                            kerbal.walkSpeed = (_walkSpeed * 0.8f); // + levelCalc;
                            kerbal.runSpeed = (_runSpeed * 0.8f); // + levelCalc;
                        }
                        else
                        {
                            kerbal.runSpeed = _runSpeed + (level * 0.05f);
                        }
                        chase.Patrol();
                        chase.PatrolRun();
                    }
                    else
                    {
                        if (stayPunkdCheck)
                        {
                            if (!brute && !stayPunkd)
                            {
                                kerbal.walkSpeed = (_walkSpeed * 0.8f); // + levelCalc;
                                kerbal.runSpeed = (_runSpeed * 0.8f); // + levelCalc;
                            }
                            else
                            {
                                if (brute)
                                {
                                    kerbal.walkSpeed = (_walkSpeed * 0.8f); // + levelCalc;
                                    kerbal.runSpeed = (_runSpeed * 0.9f); // + levelCalc;
                                }

                                if (stayPunkd)
                                {
                                    kerbal.runSpeed = _runSpeed + (level * 0.1f);
                                    kerbal.walkSpeed = _walkSpeed * 1.5f;
                                }

                                kerbal.minRunningGee = 1;
                                chase.Patrol();
                                chase.PatrolRun();
                            }
                        }
                    }
                }
            }
        }
        /*
        public void CheckGuard()
        {
            var wmPart = this.vessel.FindPartModuleImplementing<MissileFire>();

            if (!guard)
            {
                if (wmPart != null)
                {
                    if (!wmPart.guardMode)
                    {
                        wmPart.guardMode = true;
                    }
                }
                guard = true;
            }
            else
            {
                if (wmPart != null)
                {
                    if (!wmPart.guardMode)
                    {
                        wmPart.guardMode = false;
                    }
                }

                guard = false;
            }
        }*/

        private void SetupBomber()
        {
            var random = new System.Random().Next(0, 100);

            if (random <= 5)
            {
                bomber = true;
            }
        }

        private void CheckBomber()
        {
            if (hp <= hpMax * 0.8)
            {
                StartCoroutine(DetonateMineRoutine());
            }
        }

        IEnumerator ChaseOrX()
        {
            if (HighLogic.LoadedSceneIsFlight && !player)
            {
                chasing = true;
                DistanceCheck();

                yield return new WaitForSeconds(5);

                if (kerbal.isRagdoll)
                {
                    Debug.Log("[OrX Module] " + this.vessel.vesselName + " Recovering from ragdoll .....................");

                    kerbal.RecoverFromRagdoll();
                }

                Debug.Log("[OrX Module] Starting Chase OrX ..................... SEEKING");

                yield return new WaitForEndOfFrame();
                targetOrX = 0.0f;

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    var OrXKerbal = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrX>();

                    if (!OrXKerbal.orx)
                    {
                        var chase = this.vessel.FindPartModuleImplementing<OrXchaseModule>();

                        Debug.Log("[OrX Module] " + FlightGlobals.ActiveVessel.vesselName + " Detected ..................... CHASING");

                        targetOrX += 1;
                        Chase();
                    }
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
                        Debug.Log("[OrX Module] GPS Location of Active Vessel recorded: CHASING " + FlightGlobals.ActiveVessel.vesselName);

                    }
                }

                yield return new WaitForSeconds(10);

                chasing = false;
            }
        }

        IEnumerator UpdateorxRoutine()
        {
            if (!player)
            {
                yield return new WaitForSeconds(1);
                level = Missions.instance.level;
                team = true;
                this.part.AddModule("ModuleOrXMissileFire");

                Debug.Log("[OrX Module] Weapon Manager Added ");

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
                //hpTracker = GetHP();
                hpMax = 10 + (level * 5);
                hp = hpMax;
                //hpTracker.Armor = 1;
                //hpTracker.ArmorThickness = 1;
            }
        }

        IEnumerator DetonateMineRoutine()
        {
            //explosive = Getexplosive();
            //explosive.ArmAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            //explosive.DetonateAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            yield return new WaitForEndOfFrame();
            part.explode();
        }

        IEnumerator BomberDelayRoutine()
        {
            yield return new WaitForSeconds(5);
            bomberDelay = false;
        }

        public void Explode()
        {
            if (!bomberDelay)
            {
                StartCoroutine(DetonateMineRoutine());
            }
            else
            {
                StartCoroutine(BomberDelayRoutine());
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

                            if (targetDistance <= 15)
                            {
                                var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrX>();

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

                                if (targetDistance <= 15)
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
                                var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrX>();

                                if (OrXKerbal.player)
                                {
                                    double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                                    if (targetDistance <= 8)
                                    {
                                        Debug.Log("[OrX Module - Brute] Applying HP Damage to " + v.Current.vesselName + " ....................");

                                        count += 1;
                                        OrXKerbal.hpToRemove -= hpDamage * 2;

                                        Debug.Log("[OrX Module - Brute] Punting " + v.Current.vesselName + " ....................");
                                        float speed = 30;
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

                                    if (targetDistance <= 10)
                                    {
                                        var lootBox = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                                        var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();

                                        if (tardis == null && lootBox == null)
                                        {
                                            count += 1;
                                            Debug.Log("[OrX Module - Brute] Punting " + v.Current.vesselName + " ....................");

                                            float speed = 60;
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
                                            var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrX>();

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

                                                    float speed = 20;
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
                                        var OrXKerbal = v.Current.FindPartModuleImplementing<ModuleOrX>();

                                        if (OrXKerbal != null && !OrXKerbal.orx)
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

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Kerbal

        private void Setup()
        {
            if (!player)
            {
                if (vessel.vesselName == Waldoname || waldo)
                {
                    vessel.vesselName = Waldoname;
                    waldo = true;
                    Missions.instance.saltTotal += waldoSalt;
                    StartCoroutine(WaldoSetup());
                }

                if (vessel.vesselName == Brutename || brute)
                {
                    vessel.vesselName = Brutename;
                    brute = true;
                    Missions.instance.saltTotal += bruteSalt;
                    StartCoroutine(BruteSetup());

                }

                if (vessel.vesselName == StayPunkdname || stayPunkd)
                {
                    vessel.vesselName = StayPunkdname;
                    stayPunkd = true;
                    Missions.instance.saltTotal += stayPunkdSalt;
                    StartCoroutine(stayPunkdSetup());
                }

                if (vessel.vesselName == OrXname || orx)
                {
                    vessel.vesselName = OrXname;
                    orx = true;
                    Missions.instance.saltTotal += orxSalt;
                    StartCoroutine(UpdateorxRoutine());
                }
            }
        }

        public void LevelUP()
        {
            if (level != Missions.instance.level)
            {
                //hpTracker = GetHP();
                //kerbal = GetKerbal();
                level = Missions.instance.level;
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

                hpMax = hpMax + 10;
                hpMax = hpMax;
                hp += 10;
                hp = hp;
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

        private void CheckChute()
        {/*
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                var EVAchute = p.vessel.FindPartModuleImplementing<ModuleEvaChute>();

                if (EVAchute != null)
                {
                    EVAchute.AllowRepack(true);
                    EVAchute.Repack();
                    repack = false;
                }
            }*/
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

            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (!v.HoldPhysics && v.LandedOrSplashed)
                {
                    var OrXKerbal = v.FindPartModuleImplementing<ModuleOrX>();

                    if (OrXKerbal != null && targetOrX == 0)
                    {
                        double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.GetWorldPos3D());

                        if (targetDistance <= 1000)
                        {
                            targetOrX += 1;
                            FlightCamera.SetTarget(v);
                        }
                    }
                }
            }
        }

        public void UpgradeStats()
        {
            kerbal = GetKerbal();
            //hpTracker = GetHP();

            if (medPack)
            {
                medPack = false;

                if (hp <= hpMax - (25 * medPackLevel))
                {
                    hp += 25 * medPackLevel;
                }
                else
                {
                    hp = hpMax;
                }
            }

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

        public bool KerbalDamaged()
        {
            hp = part.Damage();

            if (hpCheck >= hp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Drowning()
        {
            //var _hpTracker = this.part.FindModuleImplementing<HitpointTracker>();
            _hp -= hpDamage;
        }

        private void checkTrim()
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

        private void oxyCheck()
        {
            if (part.vessel.altitude <= -1)
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
                //DrawHP();
                Drowning();

                if (oxygen <= 1)
                {
                    kerbal = GetKerbal();
                    kerbal.isRagdoll = true;
                }
            }
        }

        IEnumerator ToggleJetPackRoutine()
        {
            toggle = true;
            kerbal = GetKerbal();
            kerbal.ToggleJetpack();
            yield return new WaitForSeconds(1);
            toggleJetPack = false;
            toggle = false;
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

        IEnumerator checkHP()
        {
            kerbal = GetKerbal();

            //hpTracker = GetHP();

            var averagedHP = hp / hpMax;

            if (hpToRemove <= 0)
            {
                hp += hpToRemove;
                hpToRemove = 0;
            }

            if (hpToAdd >= 0)
            {
                hp += hpToAdd;
                hpToAdd = 0;
            }

            if (hp <= 0)
            {
                this.part.explode();
            }

            if (averagedHP <= 0.35f)
            {
                if (averagedHP <= 0.2f)
                {
                    kerbal = GetKerbal();

                    kerbal.walkSpeed -= _walkSpeed / 3;
                    kerbal.runSpeed -= _runSpeed / 3;
                }
                else
                {
                    kerbal.walkSpeed -= _walkSpeed / 2;
                    kerbal.runSpeed -= _runSpeed / 2;
                }
            }

            yield return new WaitForSeconds(1);

            if (checkHPstarted)
            {
                StartCoroutine(checkHP());
            }
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

        public static bool UpdateHP(PartModule module)
        {
            if (module == null)
                return false;
            object[] paramList = new object[] { };
            MethodInfo hpMethod = typeof(PartModule).GetMethod("SetDamage", BindingFlags.Instance | BindingFlags.NonPublic);

            if (hpMethod == null)
                return false;
            hpMethod.Invoke(module, paramList);
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

        private void DistanceCheck()
        {
            double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());

            if (targetDistance >= 2000 || this.vessel.heightFromTerrain <= 0 || this.vessel.altitude <= this.vessel.terrainAltitude)
            {
                this.vessel.DestroyVesselComponents();
                this.vessel.Die();
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

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 0.015f, ScreenMessageStyle.UPPER_LEFT));
        }

        private void ScreenMsg2(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 0.015f, ScreenMessageStyle.UPPER_LEFT));
        }

        private void ScreenMsg3(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 3, ScreenMessageStyle.UPPER_CENTER));
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


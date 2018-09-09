using UnityEngine;
using OrXBDAc.spawn;
using System.Collections;
using System.Collections.Generic;
using OrXBDAc.parts;
using System;

namespace OrXBDAc.missions
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
    public class KerbinMissions : MonoBehaviour
    {
        private const float WindowWidth = 420;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 600;
        private Rect _windowRect;

        public bool guiActive;
        public bool ironKerbal;
        private bool launchSiteChanged = false;
        public bool guiOpen = false;
        private static bool tardisLaunch = false;

        private int armedSpawnCount = 0;
        private bool armedSpawn = true;
        private double random = 0;

        public static KerbinMissions instance;

        public bool startMission = true;
        public bool waldo = false;
        public bool debug = true;

        private double latitude = 0;
        private double longitude = 0;
        private double altitude = 0;

        private bool spawning = false;
        private int orxToSpawn = 0;

        public int spawnCount = 0;
        public bool mission = false;
        public bool waldosIsland = false;
        public bool KSC = false;
        public bool Pyramids = false;
        public bool Baikerbanur = false;
        public bool missionSelected = false;
        public bool survivalReset = false;
        public bool survival = false;
        public bool survivalCheck = false;
        private bool keysSet = false;
        private bool pauseCheck = true;

        public double _lat = 0;
        public double _lon = 0;
        public double _alt = 200;

        public double latKSC = -0.093354877488929;
        public double lonKSC = -74.6521799214026;
        public double latWaldosIsland = -1.51812886210386;
        public double lonWaldosIsland = -71.96798623656;
        public double latBaikerbanur = 20.6508271202407;
        public double lonBaikerbanur = -146.425097659734;
        public double latPyramids = -6.49869308429184;
        public double lonPyramids = -141.679184195229;


        private void Awake()
        {
            DontDestroyOnLoad(this);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 2) - (WindowWidth / 2), 160, WindowWidth, _windowHeight);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (GuiEnabledSurvival)
            {
                _windowRect = GUI.Window(90433416, _windowRect, GuiWindowOrXSurvival, "");
            }

            if (GuiEnabledLBC)
            {
                _windowRect = GUI.Window(97596226, _windowRect, GuiWindowOrXLBC, "");
            }

            if (GuiEnabledIWI)
            {
                _windowRect = GUI.Window(424453316, _windowRect, GuiWindowOrXWaldosIsland, "");
            }

            if (GuiEnabledATK)
            {
                _windowRect = GUI.Window(25423316, _windowRect, GuiWindowOrXKillerTomatoes, "");
            }

            if (GuiEnabledTKU)
            {
                _windowRect = GUI.Window(67313416, _windowRect, GuiWindowOrXTKU, "");
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ready)
                {
                    if (pauseCheck)
                    {
                        if (PauseMenu.isOpen || Time.timeScale == 0)
                        {
                            pauseCheck = false;
                            if (guiOpen)
                            {
                                DisableGui();
                            }
                        }
                    }
                    else
                    {
                        if (!PauseMenu.isOpen || Time.timeScale >= 0)
                        {
                            pauseCheck = true;
                        }
                        else
                        {
                            if (guiOpen)
                            {
                                DisableGui();
                            }
                        }
                    }
                }
            }
        }

        private void GameUiEnableOrXMission()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXMisson()
        {
            _gameUiToggle = false;
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

        /// /////////////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////////////////

        #region Level Up

        public int level = 1;
        public bool levelup = false;
        private bool levelChecked = false;

        [KSPField(isPersistant = true)]
        public int saltTotal = 0;
        public int saltGain = 0;
        public int debris = 0;

        private int orxTankSalt = 200;// + (level * 20);
        private int orxAirDroneSalt = 60;// + (level * 3);
        private int orxAirFighterSalt = 75;// + (level * 5);

        public void LevelUp()
        {
            mission = false;
            StartCoroutine(LevelUpRoutine());
        }

        IEnumerator LevelUpRoutine()
        {
            level += 1;
            debris = 0;

            ScreenMsg("<color=#cc4500ff><b>CURRENT LEVEL IS NOW </b></color>" + level);

            if (debug)
            {
                Debug.Log("[OrX Missions] LEVELING UP ... CURRENT LEVEL IS NOW " + level);
            }

            if (waldo)
            {
                waldo = false;
            }

            SpawningSequence();

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;

                if (!v.Current.isEVA)
                {
                    try
                    {
                        if (v.Current.vesselName == "OrX Loot Box")
                        {
                            var lb = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                            lb.CheckLevel();
                        }

                        if (((v.Current.vesselName.Contains("Debris") || v.Current.vesselName.Contains("Rover") || v.Current.vesselName.Contains("Probe"))
                            && v.Current.vesselName.Contains("OrX")))
                        {
                            debris += 1;
                            v.Current.rootPart.AddModule("ModuleOrXDestroyVessel", true);
                        }
                    }
                    catch (InvalidOperationException e)
                    {

                    }
                }
                else
                {
                    try
                    {
                        var OrXkerbal = v.Current.FindPartModuleImplementing<ModuleOrXBDAc>();

                        if (OrXkerbal.player)
                        {
                            if (debug)
                            {
                                Debug.Log("[OrX Missions] Found Kerbal ....................... Leveling up");
                            }
                            OrXkerbal.LevelUP();
                        }
                        else
                        {
                            if (!OrXkerbal.infected)
                            {
                                //v.Current.Die();
                            }
                        }
                    }
                    catch (InvalidOperationException e)
                    {

                    }
                }
            }
            v.Dispose();

            yield return new WaitForEndOfFrame();

            if (debris >= 0)
            {
                if (debug)
                {
                    Debug.Log("ORX REMOVING " + debris + " PIECES OF DEBRIS FROM GAME");
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////////////////

        #region Spawning

        public Vector3d position;

        private Vector3d _SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)_lat, (double)_lon, (double)_alt);
        }

        public void SpawningSequence()
        {
            Debug.Log("[OrX Missions - Spawning Sequence] Spawn Sequence .......................");
            var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

            if (FlightGlobals.ActiveVessel.isEVA)
            {
                if (!survivalReset)
                {
                    survival = false;
                }

                if (survival)
                {
                    StartCoroutine(SpawnSurvival());
                }
                else
                {
                    StartCoroutine(SpawnEVA());
                }
            }
            else
            {
                if (tardis == null)
                {
                    if (survival)
                    {
                        StartCoroutine(SpawnSurvival());
                    }
                    else
                    {
                        if (!FlightGlobals.ActiveVessel.LandedOrSplashed)
                        {
                            StartCoroutine(SpawnAirborne());
                        }

                        if (FlightGlobals.ActiveVessel.Landed)
                        {
                            StartCoroutine(SpawnLanded());
                            StartCoroutine(SpawnEVA());
                        }

                        if (FlightGlobals.ActiveVessel.Splashed)
                        {
                            StartCoroutine(SpawnSplashed());
                        }
                    }
                }
                else
                {
                    if (survival)
                    {
                        StartCoroutine(SpawnSurvival());
                    }
                    else
                    {
                        StartCoroutine(SpawnEVA());
                    }
                }
            }
        }

        IEnumerator SpawnAirborne()
        {
            yield return new WaitForEndOfFrame();

            if (level <= 5)
            {
                SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                SpawnOrXAirborne.instance.SurvivalModeSpawn();
                saltGain += orxAirDroneSalt + (level * 5);
            }
            else
            {
                if (level <= 8)
                {
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.SurvivalModeSpawn();
                    saltGain += orxAirDroneSalt + (level * 5);
                    yield return new WaitForSeconds(10);
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.SurvivalModeSpawn();
                    saltGain += orxAirDroneSalt + (level * 5);
                }
                else
                {
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.SurvivalModeSpawn();
                    saltGain += orxAirDroneSalt + (level * 5);
                    yield return new WaitForSeconds(10);
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.SurvivalModeSpawn();
                    saltGain += orxAirDroneSalt + (level * 5);
                    yield return new WaitForSeconds(10);
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.SurvivalModeSpawn();
                    saltGain += orxAirDroneSalt + (level * 5);
                }
            }
        }

        IEnumerator SpawnSurvival()
        {
            if (mission)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - Update List] SpawnSurvival .......................");
                }

                yield return new WaitForSeconds(15);

                position = FlightGlobals.ActiveVessel.GetWorldPos3D();

                yield return new WaitForSeconds(5);

                if (startMission)
                {
                    startMission = false;
                    spawnCount = 0;
                    SurvivalMode();
                }
                else
                {
                    SurvivalMode();
                }
            }
            else
            {
                Debug.Log("[OrX Missions - SpawnSurvival] Delay Sequence .......................");
                mission = true;
                yield return new WaitForSeconds(5);
                StartCoroutine(SpawnSurvival());
            }
        }

        IEnumerator SpawnSplashed()
        {
            yield return new WaitForEndOfFrame();

            if (level >= 1 && level <= 3)
            {
                if (!FlightGlobals.ActiveVessel.isEVA)
                {
                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.CheckSpawnTimer();
                    saltGain += orxAirDroneSalt + (level * 5);
                }

            }
            else
            {
                if (level == 4)
                {
                    if (!FlightGlobals.ActiveVessel.isEVA)
                    {
                        SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrX_Tank.instance.CheckSpawnTimer();
                        saltGain += orxTankSalt + (level * 25);
                    }
                }
                else
                {

                    if (level == 6)
                    {
                        if (!FlightGlobals.ActiveVessel.isEVA)
                        {
                            SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrX_Tank.instance.CheckSpawnTimer();
                            saltGain += orxTankSalt + (level * 25);
                        }
                    }
                    else
                    {
                        if (level == 7)
                        {
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                            saltGain += orxAirDroneSalt + (level * 5);
                        }
                        else
                        {
                            if (level == 9)
                            {
                                SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                SpawnOrX_Tank.instance.CheckSpawnTimer();
                                saltGain += orxTankSalt + (level * 25);
                            }
                            else
                            {
                                if (level == 11)
                                {
                                    SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                    SpawnOrXAirborne.instance.CheckSpawnTimer();
                                    saltGain += orxAirDroneSalt + (level * 5);
                                    SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                    SpawnOrX_Tank.instance.CheckSpawnTimer();
                                    saltGain += orxTankSalt + (level * 25);
                                }
                                else
                                {
                                    if (level == 13)
                                    {
                                        SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                        SpawnOrXAirborne.instance.CheckSpawnTimer();
                                        saltGain += orxAirDroneSalt + (level * 5);
                                        SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                        SpawnOrX_Tank.instance.CheckSpawnTimer();
                                        saltGain += orxTankSalt + (level * 25);
                                    }
                                    else
                                    {
                                        if (level == 14)
                                        {
                                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                                            saltGain += orxAirDroneSalt + (level * 5);
                                            SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                            SpawnOrX_Tank.instance.CheckSpawnTimer();
                                            saltGain += orxTankSalt + (level * 25);
                                        }
                                        else
                                        {
                                            if (level == 15)
                                            {
                                                SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                                SpawnOrXAirborne.instance.CheckSpawnTimer();
                                                saltGain += orxAirDroneSalt + (level * 5);
                                                SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                                SpawnOrX_Tank.instance.CheckSpawnTimer();
                                                saltGain += orxTankSalt + (level * 25);
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

        IEnumerator SpawnLanded()
        {
            yield return new WaitForEndOfFrame();

            if (level >= 1 && level <= 3)
            {
                if (!FlightGlobals.ActiveVessel.isEVA)
                {
                    //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                    //SpawnOrXAirborne.instance.CheckSpawnTimer();
                    //saltGain += orxAirDroneSalt + (level * 5);
                }
            }
            else
            {
                if (level == 4)
                {
                    if (!FlightGlobals.ActiveVessel.isEVA)
                    {
                        SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrX_Tank.instance.CheckSpawnTimer();
                        saltGain += orxTankSalt + (level * 25);
                    }
                }
                else
                {

                    if (level == 6)
                    {
                        if (!FlightGlobals.ActiveVessel.isEVA)
                        {
                            SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrX_Tank.instance.CheckSpawnTimer();
                            saltGain += orxTankSalt + (level * 25);
                        }
                    }
                    else
                    {
                        if (level == 7)
                        {
                            //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            //SpawnOrXAirborne.instance.CheckSpawnTimer();
                            //saltGain += orxAirDroneSalt + (level * 5);
                        }
                        else
                        {
                            if (level == 9)
                            {
                                SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                SpawnOrX_Tank.instance.CheckSpawnTimer();
                                saltGain += orxTankSalt + (level * 25);
                            }
                            else
                            {
                                if (level == 11)
                                {
                                    //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                    //SpawnOrXAirborne.instance.CheckSpawnTimer();
                                    //saltGain += orxAirDroneSalt + (level * 5);
                                    SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                    SpawnOrX_Tank.instance.CheckSpawnTimer();
                                    saltGain += orxTankSalt + (level * 25);
                                }
                                else
                                {
                                    if (level == 13)
                                    {
                                        //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                        //SpawnOrXAirborne.instance.CheckSpawnTimer();
                                        //saltGain += orxAirDroneSalt + (level * 5);
                                        SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                        SpawnOrX_Tank.instance.CheckSpawnTimer();
                                        saltGain += orxTankSalt + (level * 25);
                                    }
                                    else
                                    {
                                        if (level == 14)
                                        {
                                            //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                            //SpawnOrXAirborne.instance.CheckSpawnTimer();
                                            //saltGain += orxAirDroneSalt + (level * 5);
                                            SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                            SpawnOrX_Tank.instance.CheckSpawnTimer();
                                            saltGain += orxTankSalt + (level * 25);
                                        }
                                        else
                                        {
                                            if (level == 15)
                                            {
                                                //SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                                //SpawnOrXAirborne.instance.CheckSpawnTimer();
                                                //saltGain += orxAirDroneSalt + (level * 5);
                                                SpawnOrX_Tank.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                                                SpawnOrX_Tank.instance.CheckSpawnTimer();
                                                saltGain += orxTankSalt + (level * 25);
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

        IEnumerator SpawnEVA()
        {
            if (mission)
            {
                Debug.Log("[OrX Missions - SpawnEVA] Start Mission .......................");

                if (debug)
                {
                    Debug.Log("[OrX Missions - Update List] SpawnDelay .......................");
                }

                yield return new WaitForSeconds(15);

                if (startMission)
                {
                    startMission = false;
                    spawnCount = 0;
                    StartCoroutine(StartMission());
                }
                else
                {
                    StartCoroutine(StartMission());
                }
            }
            else
            {
                Debug.Log("[OrX Missions - SpawnEVA] Delay Sequence .......................");
                mission = true;
                yield return new WaitForSeconds(5);
                StartCoroutine(SpawnEVA());
            }
        }

        IEnumerator StartMission()
        {
            if (debug)
            {
                Debug.Log("[OrX Missions - StartMission] StartMission ....................... StartMission");
            }

            yield return new WaitForSeconds(5);

            if (spawnHolo)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] HoloCache ..... Level: " + level);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }

                HoloCacheMode();
            }

            if (survival)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Survival ..... Iron Kerbal Level " + level);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }
                SurvivalMode();
            }


            if (KSC)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Loot Box Controversy Level " + level);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }
                LBC();
            }

            if (waldosIsland)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] WaldosIsland Level " + level);
                }

                if (spawnCount == 0)
                {
                    //WaldosIslandSetup.instance.count = 0;
                    //WaldosIslandSetup.instance.CheckSpawnTimer();
                }
                IWI();
            }

            if (Pyramids)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Pyramids Level " + level);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }
                TKU();

            }

            if (Baikerbanur)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Baikerbanur Level " + level);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }
                ATK();
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////////////////

        #region Missions

        private bool addTardisModule = false;

        private void SpawnArmedOrX()
        {
            if (armedSpawn)
            {
                armedSpawn = false;
                armedSpawnCount = 0;

                if (level <= 4)
                {
                    random = new System.Random().Next(1, 3);
                }
                else
                {
                    if (level <= 10)
                    {
                        random = new System.Random().Next(2, 4);
                    }
                    else
                    {
                        if (level <= 15)
                        {
                            random = new System.Random().Next(3, 5);
                        }
                        else
                        {
                            random = new System.Random().Next(4, 8);
                        }
                    }
                }

                StartCoroutine(SpawnArmedOrXRoutine());
            }
        }

        IEnumerator SpawnArmedOrXRoutine()
        {
            armedSpawnCount += 1;
            if (armedSpawnCount <= 2 + level)
            {
                OrXSpawn.instance.SpawnBrute();
                yield return new WaitForSeconds(2);
                SpawnCheckArmedOrX();
            }
            else
            {
                OrXSpawn.instance.SpawnStayPunkd();
                yield return new WaitForSeconds(2);
                SpawnCheckArmedOrX();
            }
        }

        private void SpawnCheckArmedOrX()
        {
            random -= 1;

            if (random >= 0)
            {
                StartCoroutine(SpawnArmedOrXRoutine());
            }
        }

        IEnumerator AddTardisModule()
        {
            FlightGlobals.ActiveVessel.rootPart.AddModule("ModuleOrXTardis");
            yield return new WaitForEndOfFrame();
            var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

            if (KSC)
            {
                if (tardis != null)
                {
                    MissionLauncher.instance.launchSiteChanged = true;
                    MissionLauncher.instance.KSC = true;

                    startMission = true;
                    KSC = true;
                    tardis.Survival = false;
                    tardis.Pyramids = false;
                    tardis.KSC = true;
                    tardis.Baikerbanur = false;
                    tardis.launchSiteChanged = true;
                    tardis.WaldosIsland = false;
                    tardis.triggered = false;
                }
            }

            if (waldosIsland)
            {

            }

            if (Baikerbanur)
            {

            }

            if (Pyramids)
            {

            }
        }

        /// /////////////////////////////////////////////////////////////////////////////

        #region HoloCache Mode

        public bool spawnHolo = false;

        public void HoloCacheMode()
        {
            spawnHolo = true;
            StartCoroutine(HoloCache());
        }

        IEnumerator HoloCache()
        {
            if (FlightGlobals.ready)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    survival = true;

                    if (debug)
                    {
                        Debug.Log("[OrX Missions] HoloCache Mode ...... spawnCount = " + spawnCount);
                    }

                    OrX_Log.instance.sound_SpawnOrXHole.Play();

                    if (!keysSet)
                    {
                        keysSet = true;
                        OrX_Log.instance.SetFocusKeys();
                    }

                    if (!spawning)
                    {
                        spawning = true;

                        if (debug)
                        {
                            Debug.Log("[OrX Missions] HoloCache Mode ................. SPAWNING ");
                        }

                        if (level <= 6)
                        {
                            int _random = new System.Random().Next(4, 10);
                            orxToSpawn = _random;
                        }
                        else
                        {
                            int _random = new System.Random().Next(6, 12);
                            orxToSpawn = _random;
                        }

                        var offset = new System.Random().Next(0, 80);
                        double _lat_ = 0;
                        double _lon_ = 0;

                        if (offset <= 20)
                        {
                            _lat_ = -0.001f;
                            _lon_ = -0.001f;
                        }
                        else
                        {
                            if (offset <= 40)
                            {
                                _lat_ = 0.001f;
                                _lon_ = -0.001f;
                            }
                            else
                            {
                                if (offset <= 60)
                                {
                                    _lat_ = -0.001f;
                                    _lon_ = 0.001f;
                                }
                                else
                                {
                                    if (offset <= 80)
                                    {
                                        _lat_ = 0.001f;
                                        _lon_ = 0.001f;
                                    }
                                }
                            }
                        }

                        SpawnLootBox.instance._lat = FlightGlobals.ActiveVessel.latitude + _lat_;
                        SpawnLootBox.instance._lon = FlightGlobals.ActiveVessel.longitude + _lon_;
                        SpawnLootBox.instance._alt = FlightGlobals.ActiveVessel.altitude;

                        OrXSpawn.instance._lat = FlightGlobals.ActiveVessel.latitude + _lat_;
                        OrXSpawn.instance._lon = FlightGlobals.ActiveVessel.longitude + _lon_;
                        OrXSpawn.instance._alt = FlightGlobals.ActiveVessel.altitude;

                        StartCoroutine(SpawnHoloCacheMode());
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1.5f);
                    StartCoroutine(HoloCache());
                }
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(HoloCache());
            }
        }

        IEnumerator SpawnHoloCacheMode()
        {
            if (!FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    OrXSpawn.instance.SurvivalSpawn();
                    yield return new WaitForSeconds(1.5f);
                    SpawnCheckHoloCacheMode();
                }
                else
                {
                    if (FlightGlobals.ActiveVessel.srfSpeed >= 120)
                    {
                        yield return new WaitForSeconds(30);
                        StartCoroutine(SpawnHoloCacheMode());
                    }
                    else
                    {
                        if (FlightGlobals.ActiveVessel.radarAltitude <= 2000)
                        {
                            OrXSpawn.instance.SurvivalSpawn();
                            yield return new WaitForSeconds(1.5f);
                            SpawnCheckHoloCacheMode();
                        }
                    }
                }
            }
            else
            {
                OrXSpawn.instance.SurvivalSpawn();
                yield return new WaitForSeconds(1.5f);
                SpawnCheckHoloCacheMode();
            }
        }

        private void SpawnCheckHoloCacheMode()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnHoloCacheMode());
            }
            else
            {
                if (!FlightGlobals.ActiveVessel.isEVA)
                {
                    StartCoroutine(SpawnLanded());
                    StartCoroutine(SpawnAirborne());
                }
                else
                {
                    StartCoroutine(SpawnAirborne());
                }

                SpawnLootBox.instance.CheckSpawnTimer();
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Survival Mode

        public void SurvivalMode()
        {
            StartCoroutine(Survival());
        }

        IEnumerator Survival()
        {
            if (FlightGlobals.ready)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    survival = true;

                    if (debug)
                    {
                        Debug.Log("[OrX Missions] Survival Mode ...... spawnCount = " + spawnCount);
                    }

                    OrX_Controls.instance.OpenGUI();
                    OrX_Log.instance.sound_SpawnOrXHole.Play();

                    if (!keysSet)
                    {
                        keysSet = true;
                        OrX_Log.instance.SetFocusKeys();
                    }

                    if (!spawning)
                    {
                        spawning = true;

                        if (debug)
                        {
                            Debug.Log("[OrX Missions] SurvivalMode ................. SPAWNING ");
                        }

                        if (level <= 6)
                        {
                            int _random = new System.Random().Next(4, 10);
                            orxToSpawn = _random;
                        }
                        else
                        {
                            int _random = new System.Random().Next(6, 12);
                            orxToSpawn = _random;
                        }

                        var offset = new System.Random().Next(0, 80);
                        double _lat_ = 0;
                        double _lon_ = 0;

                        if (offset <= 20)
                        {
                            _lat_ = -0.001f;
                            _lon_ = -0.001f;
                        }
                        else
                        {
                            if (offset <= 40)
                            {
                                _lat_ = 0.001f;
                                _lon_ = -0.001f;
                            }
                            else
                            {
                                if (offset <= 60)
                                {
                                    _lat_ = -0.001f;
                                    _lon_ = 0.001f;
                                }
                                else
                                {
                                    if (offset <= 80)
                                    {
                                        _lat_ = 0.001f;
                                        _lon_ = 0.001f;
                                    }
                                }
                            }
                        }

                        SpawnLootBox.instance._lat = FlightGlobals.ActiveVessel.latitude + _lat_;
                        SpawnLootBox.instance._lon = FlightGlobals.ActiveVessel.longitude + _lon_;
                        SpawnLootBox.instance._alt = FlightGlobals.ActiveVessel.altitude;

                        OrXSpawn.instance._lat = FlightGlobals.ActiveVessel.latitude + _lat_;
                        OrXSpawn.instance._lon = FlightGlobals.ActiveVessel.longitude + _lon_;
                        OrXSpawn.instance._alt = FlightGlobals.ActiveVessel.altitude;

                        StartCoroutine(SpawnSurvivalMode());
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1.5f);
                    StartCoroutine(Survival());
                }
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(Survival());
            }
        }

        IEnumerator SpawnSurvivalMode()
        {
            if (!FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    OrXSpawn.instance.SurvivalSpawn();
                    yield return new WaitForSeconds(1.5f);
                    SpawnCheckSurvivalMode();
                }
                else
                {
                    if (FlightGlobals.ActiveVessel.srfSpeed >= 120)
                    {
                        yield return new WaitForSeconds(30);
                        StartCoroutine(SpawnSurvivalMode());
                    }
                    else
                    {
                        if (FlightGlobals.ActiveVessel.radarAltitude <= 2000)
                        {
                            OrXSpawn.instance.SurvivalSpawn();
                            yield return new WaitForSeconds(1.5f);
                            SpawnCheckSurvivalMode();
                        }
                    }
                }
            }
            else
            {
                OrXSpawn.instance.SurvivalSpawn();
                yield return new WaitForSeconds(1.5f);
                SpawnCheckSurvivalMode();
            }
        }

        private void SpawnCheckSurvivalMode()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnSurvivalMode());
            }
            else
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions] SpawnCheckSurvivalMode ................. Loot Box Spawned ");
                }

                if (!FlightGlobals.ActiveVessel.isEVA)
                {
                    StartCoroutine(SpawnLanded());
                    StartCoroutine(SpawnAirborne());
                }
                else
                {
                    StartCoroutine(SpawnAirborne());
                }

                SpawnLootBox.instance.CheckSpawnTimer();
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        #region Survival GUI

        public static bool GuiEnabledSurvival;

        public void EnableSurvivalGui()
        {
            KillCoroutines();
            survival = false;
            KSC = false;
            waldosIsland = false;
            Baikerbanur = false;
            Pyramids = false;
            guiOpen = false;
            _gameUiToggle = true;
            GuiEnabledSurvival = false;
            GuiEnabledTKU = false;
            GuiEnabledLBC = false;
            GuiEnabledATK = false;
            GuiEnabledIWI = false;
            MissionLauncher.instance.DisableGui();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledSurvival = true;
                    Debug.Log("[OrX Mission Survival]: Showing GUI");
                }
                else
                {
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledSurvival = true;
                    Debug.Log("[OrX Mission Survival]: Showing GUI");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    GuiEnabledSurvival = true;
                    Debug.Log("[OrX Mission Survival]: Showing GUI");
                }
                else
                {
                    guiOpen = true;
                    GuiEnabledSurvival = true;
                    Debug.Log("[OrX Mission Survival]: Showing GUI");
                    /*
                    ScreenMsg("<color=#cc4500ff><b>OrX missions are inaccesible in the flight scene if not using the Tardis</b></color>");
                    guiOpen = false;
                    GuiEnabledSurvival = false;
                    GuiEnabledTKU = false;
                    GuiEnabledLBC = false;
                    GuiEnabledATK = false;
                    GuiEnabledIWI = false;*/
                }
            }
        }

        private void GuiWindowOrXSurvival(int OrX_Survival)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXSurvivalTitle(line);
            DrawSurvivalText1(line);
            line++;
            DrawSurvivalText2(line);
            line++;
            DrawSurvivalText3(line);
            line++;
            DrawSurvivalText4(line);
            line++;
            line++;
            DrawSurvivalText13(line);
            line++;
            DrawSurvivalText14(line);
            line++;
            DrawSurvivalText15(line);
            line++;
            DrawSurvivalText16(line);
            line++;
            DrawSurvivalText17(line);
            line++;
            line++;
            DrawStartSurvivalMission(line);
            line++;
            DrawDeclineMission(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawOrXSurvivalTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Iron Kerbal", titleStyle);
        }

        private void DrawSurvivalText1(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Take a stand against TV taxes ... Don't give Waldo",

              titleStyle);
        }

        private void DrawSurvivalText2(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "and his minions the opportunity to tax you for",
              titleStyle);
        }

        private void DrawSurvivalText3(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "what is freely available over the air",

              titleStyle);
        }

        private void DrawSurvivalText4(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "They just keep coming ... Won't leave you alone", titleStyle);
        }

        private void DrawSurvivalText13(float line)
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

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "TH﻿E MISSION",

              titleStyle);
        }

        private void DrawSurvivalText14(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Waldo and his minions are at it again, trying to get",

              titleStyle);
        }

        private void DrawSurvivalText15(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "your hard earned cash by charging for services you",

              titleStyle);
        }

        private void DrawSurvivalText16(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "didn't ask for, don't want and don't need",

              titleStyle);
        }

        private void DrawSurvivalText17(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Hide your antennae .... They'll think you got a TV in there",

              titleStyle);
        }

        private void DrawStartSurvivalMission(float line)
        {
            GUIStyle OrXbuttonStyle = missionSelected ? HighLogic.Skin.box : HighLogic.Skin.button;

            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!missionSelected)
            {
                if (GUI.Button(saveRect, "START MISSION", OrXbuttonStyle))
                {
                    if (!Baikerbanur && !Pyramids && !KSC && !waldosIsland && !survival)
                    {
                        DisableGui();
                        LaunchSurvival();
                    }
                }
            }
        }

        private void LaunchSurvival()
        {
            missionSelected = true;
            survival = true;
            survivalReset = true;

            MissionLauncher.instance.WaldosIsland = false;
            MissionLauncher.instance.Survival = true;
            MissionLauncher.instance.KSC = false;
            MissionLauncher.instance.Pyramids = false;
            MissionLauncher.instance.Baikerbanur = false;
            MissionLauncher.instance.ironKerbal = true;
            OrX_Log.instance.survival = true;
            OrX_Log.instance.ironKerbal = true;

            spawnCount = 0;
            mission = false;
            KSC = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;

            var count = 0;
            Debug.Log("[OrX MISSIONS - SURVIVAL] ........ Iron Kerbal");
            MissionLauncher.instance.launchSiteChanged = true;
            MissionLauncher.instance.Survival = true;

            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null && count == 0)
                    {
                        tardis.Survival = true;
                        count += 1;
                        tardis.Pyramids = false;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.WaldosIsland = false;
                        tardis.launchSiteChanged = true;
                        tardis.triggered = false;
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                    else
                    {
                        SurvivalMode();
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                }
            }
            else
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardis.Survival = true;
                    startMission = true;
                    Pyramids = false;
                    tardis.Pyramids = false;
                    tardis.KSC = false;
                    tardis.Baikerbanur = false;
                    tardis.launchSiteChanged = true;
                    tardis.Baikerbanur = false;
                    tardis.triggered = false;
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Good Luck</b></color>");
                    SurvivalMode();
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Loot Box Controversy

        #region Coords - Loot Box Controversy

        public bool KSCStart = false;
        public double KSCStartlat = -0.093354877488929;
        public double KSCStartlon = -74.6521799214026;
        public double KSCStartalt = 69;

        public bool KSCFinish = false;
        public double KSCFinishlat = -0.093354877488929;
        public double KSCFinishlon = -74.6521799214026;
        public double KSCFinishalt = 69;

        public bool KSCCoord1 = false;
        private double KSCCoord1lat = -0.0993286596882136;
        private double KSCCoord1lon = -74.6452571008766;
        private double KSCCoord1alt = 67;

        public bool KSCCoord2 = false;
        private double KSCCoord2lat = -0.0906792112643335;
        private double KSCCoord2lon = -74.6303881751926;
        private double KSCCoord2alt = 67;

        public bool KSCCoord3 = false;
        private double KSCCoord3lat = -0.108999921969498;
        private double KSCCoord3lon = -74.6276665561495;
        private double KSCCoord3alt = 67;

        public bool KSCCoord4 = false;
        private double KSCCoord4lat = -0.112876682396579;
        private double KSCCoord4lon = -74.6522191740983;
        private double KSCCoord4alt = 67;

        public bool KSCCoord5 = false;
        private double KSCCoord5lat = -0.125929732172947;
        private double KSCCoord5lon = -74.6100402129825;
        private double KSCCoord5alt = 67;

        public bool KSCCoord6 = false;
        private double KSCCoord6lat = -0.123401752691497;
        private double KSCCoord6lon = -74.6344753477158;
        private double KSCCoord6alt = 67;

        public bool KSCCoord7 = false;
        private double KSCCoord7lat = -0.117028421187697;
        private double KSCCoord7lon = -74.6368234014466;
        private double KSCCoord7alt = 67;

        public bool KSCCoord8 = false;
        private double KSCCoord8lat = -0.106978614535463;
        private double KSCCoord8lon = -74.6414465578981;
        private double KSCCoord8alt = 305;

        public bool KSCCoord9 = false;
        private double KSCCoord9lat = -0.0776694056652502;
        private double KSCCoord9lon = -74.644979264208;
        private double KSCCoord9alt = 67;

        public bool KSCCoord10 = false;
        private double KSCCoord10lat = -0.117028421187697;
        private double KSCCoord10lon = -74.6368234014466;
        private double KSCCoord10alt = 67;

        #endregion

        public void LBC()
        {
            KSC = true;
            waldosIsland = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            survival = false;

            StartCoroutine(SpawnLBC());
        }

        IEnumerator SpawnLBC()
        {
            yield return new WaitForSeconds(1);

            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    if (addTardisModule)
                    {
                        addTardisModule = false;
                        StartCoroutine(AddTardisModule());
                    }
                    else
                    {
                        StartCoroutine(LBCDistanceCheck());
                    }
                }
                else
                {
                    StartCoroutine(SpawnLBC());
                }
            }
            else
            {
                StartCoroutine(SpawnLBC());
            }
        }

        IEnumerator LBCDistanceCheck()
        {
            _lat = KSCStartlat;
            _lon = KSCStartlon;
            _alt = KSCStartalt;
            double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

            var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
            if (tardis == null)
            {
                if (targetDistance <= 5000)
                {
                    yield return new WaitForSeconds(5);
                    LootBoxControversy();
                }
                else
                {
                    if (targetDistance >= 25000)
                    {
                        yield return new WaitForSeconds(30);
                        StartCoroutine(LBCDistanceCheck());
                    }
                    else
                    {
                        yield return new WaitForSeconds(5);

                        if (targetDistance <= 15000 && !IWIairSpawn)
                        {
                            IWIairSpawn = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }
                    }
                }
            }
            else
            {
                LootBoxControversy();
            }
        }

        public void LootBoxControversy()
        {
            KSC = true;
            waldosIsland = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            IWIairSpawn2 = false;
            survival = false;

            OrX_Log.instance.sound_SpawnOrXHole.Play();

            if (!keysSet)
            {
                keysSet = true;
                OrX_Log.instance.SetFocusKeys();
            }

            if (debug)
            {
                Debug.Log("[OrX Missions] Loot Box Controversy ...... spawnCount = " + spawnCount);
            }

            if (spawnCount == 0)
            {
                spawnCount += 1;
                KSCCoord1 = true;
                KSCSpawn();
            }
            else
            {
                if (spawnCount == 1)
                {
                    spawnCount += 1;
                    KSCCoord2 = true;
                    KSCSpawn();
                }
                else
                {
                    if (spawnCount == 2)
                    {
                        spawnCount += 1;
                        KSCCoord3 = true;
                        KSCSpawn();
                    }
                    else
                    {
                        if (spawnCount == 3)
                        {
                            spawnCount += 1;
                            KSCCoord4 = true;
                            KSCSpawn();
                        }
                        else
                        {
                            if (spawnCount == 4)
                            {
                                spawnCount += 1;
                                KSCCoord5 = true;
                                KSCSpawn();
                            }
                            else
                            {
                                if (spawnCount == 5)
                                {
                                    spawnCount += 1;
                                    KSCCoord6 = true;
                                    KSCSpawn();
                                }
                                else
                                {
                                    if (spawnCount == 6)
                                    {
                                        spawnCount += 1;
                                        KSCCoord7 = true;
                                        KSCSpawn();
                                    }
                                    else
                                    {
                                        if (spawnCount == 7)
                                        {
                                            spawnCount += 1;
                                            KSCCoord8 = true;
                                            KSCSpawn();
                                        }
                                        else
                                        {
                                            if (spawnCount == 8)
                                            {
                                                spawnCount += 1;
                                                KSCCoord9 = true;
                                                KSCSpawn();
                                            }
                                            else
                                            {
                                                if (spawnCount == 9)
                                                {
                                                    spawnCount += 1;
                                                    KSCCoord10 = true;
                                                    KSCSpawn();
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
        }

        public void KSCSpawn()
        {
            KSC = true;

            if (!spawning)
            {
                spawning = true;

                if (debug)
                {
                    Debug.Log("[OrX Missions] KSCSpawn ................. SPAWNING ");
                }

                if (level <= 6)
                {
                    int _random = new System.Random().Next(4, 10);
                    orxToSpawn = _random;
                }
                else
                {
                    int _random = new System.Random().Next(6, 12);
                    orxToSpawn = _random;
                }

                if (KSCCoord1)
                {
                    latitude = KSCCoord1lat;
                    longitude = KSCCoord1lon;
                    altitude = KSCCoord1alt;
                }

                if (KSCCoord2)
                {
                    latitude = KSCCoord2lat;
                    longitude = KSCCoord2lon;
                    altitude = KSCCoord2alt;
                }

                if (KSCCoord3)
                {
                    latitude = KSCCoord3lat;
                    longitude = KSCCoord3lon;
                    altitude = KSCCoord3alt;
                }

                if (KSCCoord4)
                {
                    latitude = KSCCoord4lat;
                    longitude = KSCCoord4lon;
                    altitude = KSCCoord4alt;
                }

                if (KSCCoord5)
                {
                    latitude = KSCCoord5lat;
                    longitude = KSCCoord5lon;
                    altitude = KSCCoord5alt;
                }

                if (KSCCoord6)
                {
                    latitude = KSCCoord6lat;
                    longitude = KSCCoord6lon;
                    altitude = KSCCoord6alt;
                }

                if (KSCCoord7)
                {
                    latitude = KSCCoord7lat;
                    longitude = KSCCoord7lon;
                    altitude = KSCCoord7alt;
                }

                if (KSCCoord8)
                {
                    latitude = KSCCoord8lat;
                    longitude = KSCCoord8lon;
                    altitude = KSCCoord8alt;
                }

                if (KSCCoord9)
                {
                    latitude = KSCCoord9lat;
                    longitude = KSCCoord9lon;
                    altitude = KSCCoord9alt;
                }

                if (KSCCoord10)
                {
                    latitude = KSCCoord10lat;
                    longitude = KSCCoord10lon;
                    altitude = KSCCoord10alt;
                }
                SpawnLootBox.instance._lat = latitude;
                SpawnLootBox.instance._lon = longitude;
                SpawnLootBox.instance._alt = altitude;

                OrXSpawn.instance._lat = latitude;
                OrXSpawn.instance._lon = longitude;
                OrXSpawn.instance._alt = altitude;

                KSCCoord1 = false;
                KSCCoord2 = false;
                KSCCoord3 = false;
                KSCCoord4 = false;
                KSCCoord5 = false;
                KSCCoord6 = false;
                KSCCoord7 = false;
                KSCCoord8 = false;
                KSCCoord9 = false;
                KSCCoord10 = false;
                StartCoroutine(SpawnKSC());
            }
        }

        IEnumerator SpawnKSC()
        {
            OrXSpawn.instance.SpawnOrX();
            yield return new WaitForSeconds(2);
            SpawnCheckKSC();
        }

        private void SpawnCheckKSC()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnKSC());
            }
            else
            {
                SpawnLootBox.instance.CheckSpawnTimer();

                if (debug)
                {
                    Debug.Log("[OrX Missions] SpawnCheckKSC ................. Loot Box Spawned ");
                }
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        #region Loot Box Controversy GUI

        public static bool GuiEnabledLBC;

        public void EnableLBCGui()
        {
            spawning = false;
            KillCoroutines();
            KSC = false;
            waldosIsland = false;
            Baikerbanur = false;
            Pyramids = false;
            guiOpen = false;
            _gameUiToggle = true;
            MissionLauncher.instance.DisableGui();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledLBC = true;
                    Debug.Log("[OrX Mission LBC]: Showing GUI");
                }
                else
                {
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledLBC = true;
                    Debug.Log("[OrX Mission Launch]: Showing GUI");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    GuiEnabledLBC = true;
                    Debug.Log("[OrX Mission LBC]: Showing GUI");
                }
                else
                {
                    guiOpen = true;
                    GuiEnabledLBC = true;
                    Debug.Log("[OrX Mission LBC]: Showing GUI");
/*
                    ScreenMsg("<color=#cc4500ff><b>OrX missions are inaccesible in the flight scene if not using the Tardis</b></color>");
                    guiOpen = false;
                    GuiEnabledTKU = false;
                    GuiEnabledLBC = false;
                    GuiEnabledATK = false;
                    GuiEnabledIWI = false;*/
                }
            }

            MissionLauncher.instance.DisableGui();
        }

        private void GuiWindowOrXLBC(int OrX_LBC)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXLBCTitle(line);
            DrawOrXLBCText1(line);
            line++;
            DrawOrXLBCText2(line);
            line++;
            DrawOrXLBCText3(line);
            line++;
            DrawOrXLBCText4(line);
            line++;
            DrawOrXLBCText5(line);
            line++;
            line++;
            DrawOrXLBCText13(line);
            line++;
            DrawOrXLBCText14(line);
            line++;
            DrawOrXLBCText15(line);
            line++;
            DrawOrXLBCText16(line);
            line++;
            line++;
            DrawStartLBCMission(line);
            line++;
            DrawDeclineMission(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawOrXLBCTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "The Loot Box Controversy", titleStyle);
        }

        private void DrawOrXLBCText1(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Waldo's AAA Game Studio has made the claim that people do",

              titleStyle);
        }

        private void DrawOrXLBCText2(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "not like linear single player games as much today as they",
              titleStyle);
        }

        private void DrawOrXLBCText3(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "muscle to pushdid five years or 10 years ago and they have",

              titleStyle);
        }

        private void DrawOrXLBCText4(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                " sent their you around for not paying the microtransaction", titleStyle);
        }

        private void DrawOrXLBCText5(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "fee's or buying their DLC",

              titleStyle);
        }

        private void DrawOrXLBCText13(float line)
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

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "TH﻿E MISSION",

              titleStyle);
        }

        private void DrawOrXLBCText14(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Show these brutes who's boss and that you will not just hand",

              titleStyle);
        }

        private void DrawOrXLBCText15(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "hard earned money over that easily ... Go get 'em and take",

              titleStyle);
        }

        private void DrawOrXLBCText16(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "their Loot Boxes, by any means necessary",

              titleStyle);
        }

        private void DrawStartLBCMission(float line)
        {
            GUIStyle OrXbuttonStyle = missionSelected ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (!missionSelected)
            {
                if (!Baikerbanur && !Pyramids && !KSC && !waldosIsland && !survival)
                {
                    if (GUI.Button(saveRect, "START MISSION"))
                    {
                        DisableGui();
                        LaunchLBC();
                    }
                }
            }
        }

        private void LaunchLBC()
        {
            missionSelected = true;
            KSC = true;
            MissionLauncher.instance.KSC = true;
            MissionLauncher.instance.Survival = false;
            MissionLauncher.instance.WaldosIsland = false;
            MissionLauncher.instance.Pyramids = false;
            MissionLauncher.instance.Baikerbanur = false;
            MissionLauncher.instance.ironKerbal = true;
            OrX_Log.instance.survival = false;
            OrX_Log.instance.ironKerbal = true;

            spawnCount = 0;
            mission = false;
            waldosIsland = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            survival = false;
            survivalReset = false;

            var count = 0;
            Debug.Log("[OrX MISSIONS - LOOT BOX CONTROVERSY] ........ Loot Box Controversy");
            _lat = latKSC;
            _lon = lonKSC;
            _alt = 72;

            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    MissionLauncher.instance.launchSiteChanged = true;
                    MissionLauncher.instance.KSC = true;

                    var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null && count == 0)
                    {
                        count += 1;
                        tardis.Survival = false;
                        tardis.Pyramids = false;
                        tardis.KSC = true;
                        tardis.Baikerbanur = false;
                        tardis.WaldosIsland = false;
                        tardis.launchSiteChanged = true;
                        tardis.triggered = false;
                        MissionLauncher.instance.editorLaunch = true;
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                    else
                    {
                        //addTardisModule = true;
                        LBC();
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                }
            }
            else
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

                double targetDistance = Vector3d.Distance(_SpawnCoords(), FlightGlobals.ActiveVessel.GetWorldPos3D());

                if (targetDistance >= 100000)
                {
                    if (tardis != null)
                    {
                        MissionLauncher.instance.launchSiteChanged = true;
                        MissionLauncher.instance.KSC = true;

                        startMission = true;
                        KSC = true;
                        tardis.Survival = false;
                        tardis.Pyramids = false;
                        tardis.KSC = true;
                        tardis.Baikerbanur = false;
                        tardis.launchSiteChanged = true;
                        tardis.WaldosIsland = false;
                        tardis.triggered = false;
                    }
                    else
                    {
                        LBC();
                    }
                }
                else
                {
                    if (tardis != null)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>This is too close to your current position for a Tardis jump</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Please select another destination</b></color>");
                    }
                    else
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Adding to HoloCache database</b></color>");

                        // ADD MISSION GPS TO HOLOCACHE CONTROLLER
                    }
                    //LBC();
                    Debug.Log("[OrX MISSIONS - LOOT BOX CONTROVERSY] ........ Please select another destination");
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Waldo's Island

        private bool IWIairSpawn = false;
        private bool IWIairSpawn2 = false;

        #region Coords - Waldo's Island

        public bool WaldosIslandStart = false;
        public double WaldosIslandStartlat = -1.51812886210386;
        public double WaldosIslandStartlon = -71.96798623656;
        public double WaldosIslandStartalt = 136;

        public bool WaldosIslandFinish = false;
        public double WaldosIslandFinishlat = -1.51900705380425;
        public double WaldosIslandFinishlon = -71.904406638316;
        public double WaldosIslandFinishalt = 136;

        public bool WaldosIslandCoord1 = false;
        private double WaldosIslandCoord1lat = -1.50089462874225;
        private double WaldosIslandCoord1lon = -71.9290019673837;
        private double WaldosIslandCoord1alt = 140;

        public bool WaldosIslandCoord2 = false;
        private double WaldosIslandCoord2lat = -1.52914999123091;
        private double WaldosIslandCoord2lon = -71.9177945145373;
        private double WaldosIslandCoord2alt = 140;

        public bool WaldosIslandCoord3 = false;
        private double WaldosIslandCoord3lat = -1.52850660455987;
        private double WaldosIslandCoord3lon = -71.8996047607408;
        private double WaldosIslandCoord3alt = 140;

        public bool WaldosIslandCoord4 = false;
        private double WaldosIslandCoord4lat = -1.52885277500639;
        private double WaldosIslandCoord4lon = -71.8347598517047;
        private double WaldosIslandCoord4alt = 135;

        public bool WaldosIslandCoord5 = false;
        private double WaldosIslandCoord5lat = -1.58723197147454;
        private double WaldosIslandCoord5lon = -71.8240527826052;
        private double WaldosIslandCoord5alt = 240;

        public bool WaldosIslandCoord6 = false;
        private double WaldosIslandCoord6lat = -1.65315288630501;
        private double WaldosIslandCoord6lon = -71.8616694400322;
        private double WaldosIslandCoord6alt = 420;

        public bool WaldosIslandCoord7 = false;
        private double WaldosIslandCoord7lat = -1.64136983276856;
        private double WaldosIslandCoord7lon = -71.9004716044526;
        private double WaldosIslandCoord7alt = 435;

        public bool WaldosIslandCoord8 = false;
        private double WaldosIslandCoord8lat = -1.67169591632997;
        private double WaldosIslandCoord8lon = -71.7858259103016;
        private double WaldosIslandCoord8alt = 305;

        public bool WaldosIslandCoord9 = false;
        private double WaldosIslandCoord9lat = -1.58972600173454;
        private double WaldosIslandCoord9lon = -71.8115979684141;
        private double WaldosIslandCoord9alt = 235;

        public bool WaldosIslandCoord10 = false;
        private double WaldosIslandCoord10lat = -1.51590761884814;
        private double WaldosIslandCoord10lon = -71.8554121823393;
        private double WaldosIslandCoord10alt = 138;

        #endregion

        public void IWI()
        {
            IWIairSpawn = false;
            IWIairSpawn2 = false;

            _lat = WaldosIslandStartlat;
            _lon = WaldosIslandStartlon;
            _alt = WaldosIslandStartalt;
            StartCoroutine(SpawnIWI());
        }

        IEnumerator SpawnIWI()
        {
            yield return new WaitForSeconds(1);

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    StartCoroutine(IWIDistanceCheck());
                }
                else
                {
                    StartCoroutine(SpawnIWI());
                }
            }
            else
            {
                StartCoroutine(SpawnIWI());
            }
        }

        IEnumerator IWIDistanceCheck()
        {
            _lat = WaldosIslandStartlat;
            _lon = WaldosIslandStartlon;
            _alt = WaldosIslandStartalt;

            double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());
            if (targetDistance <= 7000)
            {
                yield return new WaitForSeconds(5);
                WaldosIslandSetup.instance.count = 0;
                WaldosIslandSetup.instance.CheckSpawnTimer();
                WaldosIsland();
            }
            else
            {
                if (targetDistance >= 25000)
                {
                    yield return new WaitForSeconds(30);
                    StartCoroutine(IWIDistanceCheck());
                }
                else
                {
                    yield return new WaitForSeconds(5);

                    if (targetDistance <= 15000 && !IWIairSpawn)
                    {
                        IWIairSpawn = true;
                        SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrXAirborne.instance.CheckSpawnTimer();
                    }

                    if (targetDistance <= 10000 && !IWIairSpawn2)
                    {
                        IWIairSpawn2 = true;
                        SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrXAirborne.instance.CheckSpawnTimer();
                    }

                    StartCoroutine(IWIDistanceCheck());
                }
            }
        }

        public void WaldosIsland()
        {
            waldosIsland = true;
            KSC = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            IWIairSpawn2 = false;
            survival = false;

            OrX_Log.instance.sound_SpawnOrXHole.Play();

            if (!keysSet)
            {
                keysSet = true;
                OrX_Log.instance.SetFocusKeys();
            }

            if (level <= 6)
            {
                int _random = new System.Random().Next(4, 10);
                orxToSpawn = _random;
            }
            else
            {
                int _random = new System.Random().Next(6, 12);
                orxToSpawn = _random;
            }

            if (spawnCount == 0)
            {
                WaldosIslandCoord1 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 1)
            {
                WaldosIslandCoord2 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 2)
            {
                WaldosIslandCoord3 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 3)
            {
                WaldosIslandCoord4 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 4)
            {
                WaldosIslandCoord5 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 5)
            {
                WaldosIslandCoord6 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 6)
            {
                WaldosIslandCoord7 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 7)
            {
                WaldosIslandCoord8 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 8)
            {
                WaldosIslandCoord9 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 9)
            {
                WaldosIslandCoord10 = true;
                WaldosIslandSpawn();
            }
            spawnCount += 1;
        }

        public void WaldosIslandSpawn()
        {
            if (!spawning)
            {
                Debug.Log("[OrX Missions] WaldosIslandSpawn ................. SPAWNING ");

                spawning = true;
                if (WaldosIslandCoord1)
                {
                    latitude = WaldosIslandCoord1lat;
                    longitude = WaldosIslandCoord1lon;
                    altitude = WaldosIslandCoord1alt;
                }

                if (WaldosIslandCoord2)
                {
                    latitude = WaldosIslandCoord2lat;
                    longitude = WaldosIslandCoord2lon;
                    altitude = WaldosIslandCoord2alt;
                }

                if (WaldosIslandCoord3)
                {
                    latitude = WaldosIslandCoord3lat;
                    longitude = WaldosIslandCoord3lon;
                    altitude = WaldosIslandCoord3alt;
                }

                if (WaldosIslandCoord4)
                {
                    latitude = WaldosIslandCoord4lat;
                    longitude = WaldosIslandCoord4lon;
                    altitude = WaldosIslandCoord4alt;
                }

                if (WaldosIslandCoord5)
                {
                    latitude = WaldosIslandCoord5lat;
                    longitude = WaldosIslandCoord5lon;
                    altitude = WaldosIslandCoord5alt;
                }

                if (WaldosIslandCoord6)
                {
                    latitude = WaldosIslandCoord6lat;
                    longitude = WaldosIslandCoord6lon;
                    altitude = WaldosIslandCoord6alt;
                }

                if (WaldosIslandCoord7)
                {
                    latitude = WaldosIslandCoord7lat;
                    longitude = WaldosIslandCoord7lon;
                    altitude = WaldosIslandCoord7alt;
                }

                if (WaldosIslandCoord8)
                {
                    latitude = WaldosIslandCoord8lat;
                    longitude = WaldosIslandCoord8lon;
                    altitude = WaldosIslandCoord8alt;
                }

                if (WaldosIslandCoord9)
                {
                    latitude = WaldosIslandCoord9lat;
                    longitude = WaldosIslandCoord9lon;
                    altitude = WaldosIslandCoord9alt;
                }

                if (WaldosIslandCoord10)
                {
                    latitude = WaldosIslandCoord10lat;
                    longitude = WaldosIslandCoord10lon;
                    altitude = WaldosIslandCoord10alt;
                }
                SpawnLootBox.instance._lat = latitude;
                SpawnLootBox.instance._lon = longitude;
                SpawnLootBox.instance._alt = altitude;

                OrXSpawn.instance._lat = latitude;
                OrXSpawn.instance._lon = longitude;
                OrXSpawn.instance._alt = altitude;

                WaldosIslandCoord1 = false;
                WaldosIslandCoord2 = false;
                WaldosIslandCoord3 = false;
                WaldosIslandCoord4 = false;
                WaldosIslandCoord5 = false;
                WaldosIslandCoord6 = false;
                WaldosIslandCoord7 = false;
                WaldosIslandCoord8 = false;
                WaldosIslandCoord9 = false;
                WaldosIslandCoord10 = false;
                StartCoroutine(SpawnWaldosIsland());
            }
        }

        IEnumerator SpawnWaldosIsland()
        {
            OrXSpawn.instance.SpawnOrX();
            yield return new WaitForSeconds(1.5f);
            SpawnCheckWaldosIsland();
        }

        private void SpawnCheckWaldosIsland()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnWaldosIsland());
            }
            else
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions] SpawnCheckWaldosIsland ................. Loot Box Spawned ");
                }

                SpawnLootBox.instance.CheckSpawnTimer();
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        #region Waldo's Island GUI

        public static bool GuiEnabledIWI;

        public void EnableIWIGui()
        {
            spawning = false;
            KillCoroutines();
            KSC = false;
            waldosIsland = false;
            Baikerbanur = false;
            Pyramids = false;
            guiOpen = false;
            _gameUiToggle = true;
            MissionLauncher.instance.DisableGui();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledIWI = true;
                    Debug.Log("[OrX Mission IWI]: Showing GUI");
                }
                else
                {
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledIWI = true;
                    Debug.Log("[OrX Mission IWI]: Showing GUI");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    guiOpen = true;
                    GuiEnabledIWI = true;
                    Debug.Log("[OrX Mission IWI]: Showing GUI");
                }
                else
                {
                    guiOpen = true;
                    guiOpen = true;
                    GuiEnabledIWI = true;
                    Debug.Log("[OrX Mission IWI]: Showing GUI");
                    /*
                    ScreenMsg("<color=#cc4500ff><b>OrX missions are inaccesible in the flight scene if not using the Tardis</b></color>");
                    guiOpen = false;
                    GuiEnabledTKU = false;
                    GuiEnabledLBC = false;
                    GuiEnabledATK = false;
                    GuiEnabledIWI = false;*/
                }
            }
        }

        private void GuiWindowOrXWaldosIsland(int OrX_WaldosIsland)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXWaldosIslandTitle(line);
            DrawMissionsText1(line);
            line++;
            DrawMissionsText2(line);
            line++;
            DrawMissionsText3(line);
            line++;
            DrawMissionsText4(line);
            line++;
            DrawMissionsText5(line);
            line++;
            DrawMissionsText6(line);
            line++;
            DrawMissionsText7(line);
            line++;
            DrawMissionsText71(line);
            line++;
            DrawMissionsText8(line);
            line++;
            DrawMissionsText9(line);
            line++;
            DrawMissionsText10(line);
            line++;
            DrawMissionsText11(line);
            line++;
            DrawMissionsText12(line);
            line++;
            line++;
            DrawMissionsText13(line);
            line++;
            DrawMissionsText14(line);
            line++;
            DrawMissionsText15(line);
            line++;
            DrawMissionsText16(line);
            line++;
            DrawMissionsText17(line);
            line++;
            DrawMissionsText18(line);
            line++;
            DrawMissionsText19(line);
            line++;
            line++;
            DrawStartIWIMission(line);
            line++;
            DrawDeclineMission(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawOrXWaldosIslandTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "It's Waldo's Island", titleStyle);
        }

        private void DrawMissionsText1(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Due to the public backlash surrounding the newly designed ",

              titleStyle);
        }

        private void DrawMissionsText2(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "ground launched BGM-86B Cruise Missile, it was decided by",
              titleStyle);
        }

        private void DrawMissionsText3(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "the military branch of the KSC that the prototype would be",

              titleStyle);
        }

        private void DrawMissionsText4(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "destroyed immediately and all data surrounding it wiped", titleStyle);
        }

        private void DrawMissionsText5(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "from the record ... (sometimes you can catch a whisper of",

              titleStyle);
        }

        private void DrawMissionsText6(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "'The Missile That Shall Not Be Named' muttered under hushed",

              titleStyle);
        }

        private void DrawMissionsText7(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "voices in the halls of the KSC ...  talking about it is",

              titleStyle);
        }

        private void DrawMissionsText71(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "punishable by flames and salty tears)",

              titleStyle);
        }

        private void DrawMissionsText8(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "However there was a problem...It hadn't occurred to anyone",

              titleStyle);
        }

        private void DrawMissionsText9(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "that the missile itself needed to be destroyed so they left",

              titleStyle);
        }

        private void DrawMissionsText10(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "it stored at the island airport and when a disgruntled KSC",

              titleStyle);
        }

        private void DrawMissionsText11(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "employee named Waldo went postal and drank the Electric",

              titleStyle);
        }

        private void DrawMissionsText12(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Kewl Aid, management wasn't prepared ......",

              titleStyle);
        }

        private void DrawMissionsText13(float line)
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

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "TH﻿E MISSION",

              titleStyle);
        }

        private void DrawMissionsText14(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "The 'Missile That Shall Not Be Named' prototype is at the",

              titleStyle);
        }

        private void DrawMissionsText15(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Island airport and Waldo Kerman is trying to activate and",

              titleStyle);
        }

        private void DrawMissionsText16(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "launch the missile at the KSC - It must be destroyed before",

              titleStyle);
        }

        private void DrawMissionsText17(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "he can hotwire it (he's looking for a paper clip)",

              titleStyle);
        }

        private void DrawMissionsText18(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Your mission is to obliterate the missile and anything that",

              titleStyle);
        }

        private void DrawMissionsText19(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "stands (or fly's) in your way﻿",

              titleStyle);
        }

        private void DrawStartIWIMission(float line)
        {
            GUIStyle OrXbuttonStyle = missionSelected ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!missionSelected)
            {
                if (!Baikerbanur && !Pyramids && !KSC && !waldosIsland && !survival)
                {
                    if (GUI.Button(saveRect, "START MISSION", OrXbuttonStyle))
                    {
                        DisableGui();
                        LaunchIWI();
                        /*
                        if (saltTotal >= 1000)
                        {
                            saltTotal -= 1000;
                            LaunchIWI();
                        }
                        else
                        {
                            if (OrX_Log.instance.devKitInstalled)
                            {
                                LaunchIWI();
                            }
                            else
                            {
                                ScreenMsg("<color=#cc4500ff><b>Please come back later with more Salt</b></color>");
                                MissionLauncher.instance.EnableGui();
                            }
                        }*/
                    }
                }
            }
        }

        private void LaunchIWI()
        {
            missionSelected = true;
            waldosIsland = true;
            MissionLauncher.instance.WaldosIsland = true;
            MissionLauncher.instance.Survival = false;
            MissionLauncher.instance.KSC = false;
            MissionLauncher.instance.Pyramids = false;
            MissionLauncher.instance.Baikerbanur = false;
            MissionLauncher.instance.ironKerbal = true;
            OrX_Log.instance.survival = false;
            OrX_Log.instance.ironKerbal = true;

            spawnCount = 0;
            mission = false;
            KSC = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = true;
            survival = false;
            survivalReset = false;

            var count = 0;
            Debug.Log("[OrX MISSIONS - WALDOS ISLAND] ........ Waldo's Island");
            _lat = latWaldosIsland;
            _lon = lonWaldosIsland;
            _alt = 138;

            if (HighLogic.LoadedSceneIsEditor)
            {
                MissionLauncher.instance.launchSiteChanged = true;
                MissionLauncher.instance.WaldosIsland = true;

                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null && count == 0)
                    {
                        tardis.Survival = false;
                        count += 1;
                        tardis.Pyramids = false;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.WaldosIsland = true;
                        tardis.launchSiteChanged = true;
                        tardis.triggered = false;
                        MissionLauncher.instance.editorLaunch = true;
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                    else
                    {
                        IWI();
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                }
            }
            else
            {
                MissionLauncher.instance.launchSiteChanged = true;
                MissionLauncher.instance.WaldosIsland = true;
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

                double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

                if (targetDistance >= 100000)
                {
                    if (tardis != null)
                    {
                        startMission = true;
                        waldosIsland = true;
                        tardis.Survival = false;
                        tardis.Pyramids = false;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.launchSiteChanged = true;
                        tardis.WaldosIsland = true;
                        tardis.triggered = false;
                    }
                }
                else
                {
                    if (tardis != null)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>This is too close to your current position for a Tardis jump</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Please select another destination</b></color>");
                    }
                    else
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Adding to HoloCache database</b></color>");

                        // ADD MISSION GPS TO HOLOCACHE CONTROLLER
                    }

                    //IWI();
                    Debug.Log("[OrX MISSIONS - WALDOS ISLAND] ........ Please select another destination");
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Attack of the Killer Tomatoes

        private bool ATKairSpawn = false;
        private bool ATKairSpawn2 = false;
        private bool ATKairSpawn3 = false;

        #region Coords - Killer Tomatoes

        public bool BaikerbanurStart = false;
        public double BaikerbanurStartlat = 20.6508271202407;
        public double BaikerbanurStartlon = -146.425097659734;
        public double BaikerbanurStartalt = 425;

        public bool BaikerbanurFinish = false;
        public double BaikerbanurFinishlat = 20.6508271202407;
        public double BaikerbanurFinishlon = -146.425097659734;
        public double BaikerbanurFinishalt = 425;

        public bool BaikerbanurCoord1 = false;
        private double BaikerbanurCoord1lat = 20.6334588872757;
        private double BaikerbanurCoord1lon = -146.423887752707;
        private double BaikerbanurCoord1alt = 425;

        public bool BaikerbanurCoord2 = false;
        private double BaikerbanurCoord2lat = 20.661327721179;
        private double BaikerbanurCoord2lon = -146.417861143229;
        private double BaikerbanurCoord2alt = 425;

        public bool BaikerbanurCoord3 = false;
        private double BaikerbanurCoord3lat = 20.6761105412511;
        private double BaikerbanurCoord3lon = -146.497256011856;
        private double BaikerbanurCoord3alt = 430;

        public bool BaikerbanurCoord4 = false;
        private double BaikerbanurCoord4lat = 20.6227048006702;
        private double BaikerbanurCoord4lon = -146.372716454763;
        private double BaikerbanurCoord4alt = 467;

        public bool BaikerbanurCoord5 = false;
        private double BaikerbanurCoord5lat = 20.6615824156676;
        private double BaikerbanurCoord5lon = -146.393522415912;
        private double BaikerbanurCoord5alt = 425;

        public bool BaikerbanurCoord6 = false;
        private double BaikerbanurCoord6lat = 20.6653890721477;
        private double BaikerbanurCoord6lon = -146.423299743981;
        private double BaikerbanurCoord6alt = 427;

        public bool BaikerbanurCoord7 = false;
        private double BaikerbanurCoord7lat = 20.5905248408306;
        private double BaikerbanurCoord7lon = -146.408297728023;
        private double BaikerbanurCoord7alt = 420;

        public bool BaikerbanurCoord8 = false;
        private double BaikerbanurCoord8lat = 20.6000033981923;
        private double BaikerbanurCoord8lon = -146.461924003598;
        private double BaikerbanurCoord8alt = 425;

        public bool BaikerbanurCoord9 = false;
        private double BaikerbanurCoord9lat = 20.6768996456568;
        private double BaikerbanurCoord9lon = -146.48288947737;
        private double BaikerbanurCoord9alt = 433;

        public bool BaikerbanurCoord10 = false;
        private double BaikerbanurCoord10lat = 20.630663018562;
        private double BaikerbanurCoord10lon = -146.436930055224;
        private double BaikerbanurCoord10alt = 427;

        #endregion

        public void ATK()
        {
            ATKairSpawn = false;
            ATKairSpawn2 = false;
            ATKairSpawn3 = false;

            _lat = BaikerbanurStartlat;
            _lon = BaikerbanurStartlon;
            _alt = BaikerbanurStartalt;
            StartCoroutine(SpawnATK());
        }

        IEnumerator SpawnATK()
        {
            yield return new WaitForSeconds(1);

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    StartCoroutine(ATKDistanceCheck());
                }
                else
                {
                    StartCoroutine(SpawnATK());
                }
            }
            else
            {
                StartCoroutine(SpawnATK());
            }
        }

        IEnumerator ATKDistanceCheck()
        {
            _lat = BaikerbanurStartlat;
            _lon = BaikerbanurStartlon;
            _alt = BaikerbanurStartalt;

            double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());
            if (targetDistance >=30000)
            {
                yield return new WaitForSeconds(30);
                StartCoroutine(ATKDistanceCheck());
            }
            else
            {
                if (targetDistance >= 25000)
                {
                    yield return new WaitForSeconds(5);
                    StartCoroutine(ATKDistanceCheck());
                }
                else
                {
                    if (targetDistance <= 7000)
                    {
                        SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrXAirborne.instance.CheckSpawnTimer();
                        KillerTomatoes();
                    }
                    else
                    {
                        yield return new WaitForSeconds(15);

                        if (targetDistance <= 10000 && !ATKairSpawn)
                        {
                            ATKairSpawn = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        if (targetDistance <= 15000 && !ATKairSpawn2)
                        {
                            ATKairSpawn2 = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        if (targetDistance <= 20000 && !ATKairSpawn3)
                        {
                            ATKairSpawn3 = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        StartCoroutine(ATKDistanceCheck());
                    }
                }
            }
        }

        IEnumerator PauseATK()
        {
            yield return new WaitForSeconds(60);
            StartCoroutine(ATKDistanceCheck());
        }

        public void KillerTomatoes()
        {
            Baikerbanur = true;
            KSC = false;
            Pyramids = false;
            waldosIsland = false;
            IWIairSpawn = false;
            IWIairSpawn2 = false;
            survival = false;

            OrX_Log.instance.sound_SpawnOrXHole.Play();

            if (!keysSet)
            {
                keysSet = true;
                OrX_Log.instance.SetFocusKeys();
            }

            if (debug)
            {
                Debug.Log("[OrX Missions] Attack of the Killer Tomatoes ...... spawnCount = " + spawnCount);
            }

            if (spawnCount == 0)
            {
                spawnCount += 1;
                BaikerbanurCoord1 = true;
                BaikerbanurSpawn();
            }
            else
            {
                if (spawnCount == 1)
                {
                    spawnCount += 1;
                    BaikerbanurCoord2 = true;
                    BaikerbanurSpawn();
                }
                else
                {
                    if (spawnCount == 2)
                    {
                        spawnCount += 1;
                        BaikerbanurCoord3 = true;
                        BaikerbanurSpawn();
                    }
                    else
                    {
                        if (spawnCount == 3)
                        {
                            spawnCount += 1;
                            BaikerbanurCoord4 = true;
                            BaikerbanurSpawn();
                        }
                        else
                        {
                            if (spawnCount == 4)
                            {
                                spawnCount += 1;
                                BaikerbanurCoord5 = true;
                                BaikerbanurSpawn();
                            }
                            else
                            {
                                if (spawnCount == 5)
                                {
                                    spawnCount += 1;
                                    BaikerbanurCoord6 = true;
                                    BaikerbanurSpawn();
                                }
                                else
                                {
                                    if (spawnCount == 6)
                                    {
                                        spawnCount += 1;
                                        BaikerbanurCoord7 = true;
                                        BaikerbanurSpawn();
                                    }
                                    else
                                    {
                                        if (spawnCount == 7)
                                        {
                                            spawnCount += 1;
                                            BaikerbanurCoord8 = true;
                                            BaikerbanurSpawn();
                                        }
                                        else
                                        {
                                            if (spawnCount == 8)
                                            {
                                                spawnCount += 1;
                                                BaikerbanurCoord9 = true;
                                                BaikerbanurSpawn();
                                            }
                                            else
                                            {
                                                if (spawnCount == 9)
                                                {
                                                    spawnCount += 1;
                                                    BaikerbanurCoord10 = true;
                                                    BaikerbanurSpawn();
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
        }

        public void BaikerbanurSpawn()
        {
            Baikerbanur = true;

            if (!spawning)
            {
                spawning = true;

                if (debug)
                {
                    Debug.Log("[OrX Missions] BaikerbanurSpawn ................. SPAWNING ");
                }

                if (level <= 6)
                {
                    int _random = new System.Random().Next(4, 10);
                    orxToSpawn = _random;
                }
                else
                {
                    int _random = new System.Random().Next(6, 12);
                    orxToSpawn = _random;
                }

                if (BaikerbanurCoord1)
                {
                    latitude = BaikerbanurCoord1lat;
                    longitude = BaikerbanurCoord1lon;
                    altitude = BaikerbanurCoord1alt;
                }

                if (BaikerbanurCoord2)
                {
                    latitude = BaikerbanurCoord2lat;
                    longitude = BaikerbanurCoord2lon;
                    altitude = BaikerbanurCoord2alt;
                }

                if (BaikerbanurCoord3)
                {
                    latitude = BaikerbanurCoord3lat;
                    longitude = BaikerbanurCoord3lon;
                    altitude = BaikerbanurCoord3alt;
                }

                if (BaikerbanurCoord4)
                {
                    latitude = BaikerbanurCoord4lat;
                    longitude = BaikerbanurCoord4lon;
                    altitude = BaikerbanurCoord4alt;
                }

                if (BaikerbanurCoord5)
                {
                    latitude = BaikerbanurCoord5lat;
                    longitude = BaikerbanurCoord5lon;
                    altitude = BaikerbanurCoord5alt;
                }

                if (BaikerbanurCoord6)
                {
                    latitude = BaikerbanurCoord6lat;
                    longitude = BaikerbanurCoord6lon;
                    altitude = BaikerbanurCoord6alt;
                }

                if (BaikerbanurCoord7)
                {
                    latitude = BaikerbanurCoord7lat;
                    longitude = BaikerbanurCoord7lon;
                    altitude = BaikerbanurCoord7alt;
                }

                if (BaikerbanurCoord8)
                {
                    latitude = BaikerbanurCoord8lat;
                    longitude = BaikerbanurCoord8lon;
                    altitude = BaikerbanurCoord8alt;
                }

                if (BaikerbanurCoord9)
                {
                    latitude = BaikerbanurCoord9lat;
                    longitude = BaikerbanurCoord9lon;
                    altitude = BaikerbanurCoord9alt;
                }

                if (BaikerbanurCoord10)
                {
                    latitude = BaikerbanurCoord10lat;
                    longitude = BaikerbanurCoord10lon;
                    altitude = BaikerbanurCoord10alt;
                }
                SpawnLootBox.instance._lat = latitude;
                SpawnLootBox.instance._lon = longitude;
                SpawnLootBox.instance._alt = altitude;

                OrXSpawn.instance._lat = latitude;
                OrXSpawn.instance._lon = longitude;
                OrXSpawn.instance._alt = altitude;

                BaikerbanurCoord1 = false;
                BaikerbanurCoord2 = false;
                BaikerbanurCoord3 = false;
                BaikerbanurCoord4 = false;
                BaikerbanurCoord5 = false;
                BaikerbanurCoord6 = false;
                BaikerbanurCoord7 = false;
                BaikerbanurCoord8 = false;
                BaikerbanurCoord9 = false;
                BaikerbanurCoord10 = false;
                StartCoroutine(SpawnBaikerbanur());
            }
        }

        IEnumerator SpawnBaikerbanur()
        {
            OrXSpawn.instance.SpawnOrX();
            yield return new WaitForSeconds(2);
            SpawnCheckBaikerbanur();
        }

        private void SpawnCheckBaikerbanur()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnBaikerbanur());
            }
            else
            {
                SpawnLootBox.instance.CheckSpawnTimer();

                if (debug)
                {
                    Debug.Log("[OrX Missions] Spawn Check Baikerbanur ................. Loot Box Spawned ");
                }
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        #region Killer Tomatoes GUI

        public static bool GuiEnabledATK;

        public void EnableATKGui()
        {
            spawning = false;
            KillCoroutines();
            KSC = false;
            waldosIsland = false;
            Baikerbanur = false;
            Pyramids = false;
            guiOpen = false;
            _gameUiToggle = true;
            MissionLauncher.instance.DisableGui();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledATK = true;
                    Debug.Log("[OrX Mission ATK]: Showing GUI");
                }
                else
                {
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledATK = true;
                    Debug.Log("[OrX Mission ATK]: Showing GUI");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    guiOpen = true;
                    GuiEnabledATK = true;
                    Debug.Log("[OrX Mission ATK]: Showing GUI");
                }
                else
                {
                    guiOpen = true;
                    guiOpen = true;
                    GuiEnabledATK = true;
                    Debug.Log("[OrX Mission ATK]: Showing GUI");
/*
                    ScreenMsg("<color=#cc4500ff><b>OrX missions are inaccesible in the flight scene if not using the Tardis</b></color>");
                    guiOpen = false;
                    GuiEnabledTKU = false;
                    GuiEnabledLBC = false;
                    GuiEnabledATK = false;
                    GuiEnabledIWI = false;*/
                }
            }
        }

        private void GuiWindowOrXKillerTomatoes(int OrX_KillerTomatoes)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXKillerTomatoesTitle(line);
            DrawKillerTomatoesText1(line);
            line++;
            DrawKillerTomatoesText2(line);
            line++;
            DrawKillerTomatoesText3(line);
            line++;
            DrawKillerTomatoesText4(line);
            line++;
            line++;
            DrawKillerTomatoesText13(line);
            line++;
            DrawKillerTomatoesText14(line);
            line++;
            DrawKillerTomatoesText15(line);
            line++;
            DrawKillerTomatoesText16(line);
            line++;
            line++;
            DrawStartKillerTomatoesMission(line);
            line++;
            DrawDeclineMission(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawOrXKillerTomatoesTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Attack of the Killer Tomatoes", titleStyle);
        }

        private void DrawKillerTomatoesText1(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "We've been recieving odd reports of Tomato attacks believed",

              titleStyle);
        }

        private void DrawKillerTomatoesText2(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "to be to work of Waldo and his minions. We suspect he has",
              titleStyle);
        }

        private void DrawKillerTomatoesText3(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "a new weapon that he has been testing in the mountains",

              titleStyle);
        }

        private void DrawKillerTomatoesText4(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "near Bainkerbanur", titleStyle);
        }

        private void DrawKillerTomatoesText13(float line)
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

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "TH﻿E MISSION",

              titleStyle);
        }

        private void DrawKillerTomatoesText14(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Travel to Baikerbanur and investigate these reports of",

              titleStyle);
        }

        private void DrawKillerTomatoesText15(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Killer Tomatoes ... We do not know what to expect when",

              titleStyle);
        }

        private void DrawKillerTomatoesText16(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "you get there so be prepared for anything",

              titleStyle);
        }

        private void DrawStartKillerTomatoesMission(float line)
        {
            GUIStyle OrXbuttonStyle = missionSelected ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!missionSelected)
            {
                if (!Baikerbanur && !Pyramids && !KSC && !waldosIsland && !survival)
                {
                    if (GUI.Button(saveRect, "START MISSION", OrXbuttonStyle))
                    {
                        DisableGui();
                        LaunchATK();
                        /*
                        if (saltTotal >= 2000)
                        {
                            saltTotal -= 2000;
                            LaunchATK();
                        }
                        else
                        {
                            if (OrX_Log.instance.devKitInstalled)
                            {
                                LaunchATK();
                            }
                            else
                            {
                                ScreenMsg("<color=#cc4500ff><b>Please come back later with more Salt</b></color>");
                                MissionLauncher.instance.EnableGui();
                            }
                        }*/
                    }
                }
            }
        }

        private void LaunchATK()
        {
            missionSelected = true;
            Baikerbanur = true;
            MissionLauncher.instance.Baikerbanur = true;
            MissionLauncher.instance.Survival = false;
            MissionLauncher.instance.WaldosIsland = false;
            MissionLauncher.instance.KSC = false;
            MissionLauncher.instance.Pyramids = false;
            MissionLauncher.instance.ironKerbal = true;
            OrX_Log.instance.survival = false;
            OrX_Log.instance.ironKerbal = true;

            spawnCount = 0;
            mission = false;
            waldosIsland = false;
            KSC = false;
            Pyramids = false;
            IWIairSpawn = false;
            survival = false;
            survivalReset = false;
            var count = 0;
            Debug.Log("[OrX MISSIONS - KILLER TOMATOES] ........ Killer Tomatoes");
            _lat = latBaikerbanur;
            _lon = lonBaikerbanur;
            _alt = 430;

            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    MissionLauncher.instance.launchSiteChanged = true;
                    MissionLauncher.instance.Baikerbanur = true;

                    var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null && count == 0)
                    {
                        tardis.Survival = false;
                        count += 1;
                        tardis.Pyramids = false;
                        tardis.KSC = false;
                        tardis.Baikerbanur = true;
                        tardis.WaldosIsland = false;
                        tardis.launchSiteChanged = true;
                        tardis.triggered = false;
                        MissionLauncher.instance.editorLaunch = true;
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                    else
                    {
                        ATK();
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                }
            }
            else
            {
                MissionLauncher.instance.launchSiteChanged = true;
                MissionLauncher.instance.Baikerbanur = true;
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

                double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

                if (targetDistance >= 100000)
                {
                    if (tardis != null)
                    {
                        tardis.Survival = false;
                        startMission = true;
                        Baikerbanur = true;
                        tardis.Pyramids = false;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.launchSiteChanged = true;
                        tardis.Baikerbanur = true;
                        tardis.triggered = false;
                    }
                }
                else
                {
                    if (tardis != null)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>This is too close to your current position for a Tardis jump</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Please select another destination</b></color>");
                    }
                    else
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Adding to HoloCache database</b></color>");

                        // ADD MISSION GPS TO HOLOCACHE CONTROLLER
                    }

                    //ATK();
                    Debug.Log("[OrX MISSIONS - KILLER TOMATOES] ........ Please select another destination");
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Tuten-Kerman Uldum

        #region Coords - Tuten-Kerman Uldum

        public bool PyramidsStart = false;
        public double PyramidsStartlat = -0.093354877488929;
        public double PyramidsStartlon = -74.6521799214026;
        public double PyramidsStartalt = 1305;

        public bool PyramidsFinish = false;
        public double PyramidsFinishlat = -0.093354877488929;
        public double PyramidsFinishlon = -74.6521799214026;
        public double PyramidsFinishalt = 1305;

        public bool PyramidsCoord1 = false;
        private double PyramidsCoord1lat = -6.50514130245044;
        private double PyramidsCoord1lon = -141.705516691302;
        private double PyramidsCoord1alt = 1305;

        public bool PyramidsCoord2 = false;
        private double PyramidsCoord2lat = -6.52432728274975;
        private double PyramidsCoord2lon = -141.680064736056;
        private double PyramidsCoord2alt = 1305;

        public bool PyramidsCoord3 = false;
        private double PyramidsCoord3lat = -6.51880089428705;
        private double PyramidsCoord3lon = -141.647273825524;
        private double PyramidsCoord3alt = 1305;

        public bool PyramidsCoord4 = false;
        private double PyramidsCoord4lat = -6.47008447521173;
        private double PyramidsCoord4lon = -141.624275369009;
        private double PyramidsCoord4alt = 1305;

        public bool PyramidsCoord5 = false;
        private double PyramidsCoord5lat = -6.45614977358813;
        private double PyramidsCoord5lon = -141.647855669453;
        private double PyramidsCoord5alt = 1305;

        public bool PyramidsCoord6 = false;
        private double PyramidsCoord6lat = -6.44876301914254;
        private double PyramidsCoord6lon = -141.688169875684;
        private double PyramidsCoord6alt = 1305;

        public bool PyramidsCoord7 = false;
        private double PyramidsCoord7lat = -6.46753675591422;
        private double PyramidsCoord7lon = -141.734929050705;
        private double PyramidsCoord7alt = 1360;

        public bool PyramidsCoord8 = false;
        private double PyramidsCoord8lat = -6.50570123468801;
        private double PyramidsCoord8lon = -141.685570777839;
        private double PyramidsCoord8alt = 1380;

        public bool PyramidsCoord9 = false;
        private double PyramidsCoord9lat = -6.5036931911846;
        private double PyramidsCoord9lon = -141.668719543525;
        private double PyramidsCoord9alt = 1350;

        public bool PyramidsCoord10 = false;
        private double PyramidsCoord10lat = -6.48889214168227;
        private double PyramidsCoord10lon = -141.68758022839;
        private double PyramidsCoord10alt = 1350;

        #endregion

        private bool TKUairSpawn = false;
        private bool TKUairSpawn2 = false;
        private bool TKUairSpawn3 = false;

        public void TKU()
        {
            TKUairSpawn = false;
            TKUairSpawn2 = false;
            TKUairSpawn3 = false;
            _lat = PyramidsStartlat;
            _lon = PyramidsStartlon;
            _alt = PyramidsStartalt;
            StartCoroutine(SpawnTKU());
        }

        IEnumerator SpawnTKU()
        {
            yield return new WaitForSeconds(1);

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!FlightGlobals.ActiveVessel.HoldPhysics)
                {
                    StartCoroutine(TKUDistanceCheck());
                }
                else
                {
                    StartCoroutine(SpawnTKU());
                }
            }
            else
            {
                StartCoroutine(SpawnTKU());
            }
        }

        IEnumerator TKUDistanceCheck()
        {
            _lat = PyramidsStartlat;
            _lon = PyramidsStartlon;
            _alt = PyramidsStartalt;

            double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());
            if (targetDistance >= 30000)
            {
                yield return new WaitForSeconds(30);
                StartCoroutine(TKUDistanceCheck());
            }
            else
            {
                if (targetDistance >= 25000)
                {
                    yield return new WaitForSeconds(5);
                    StartCoroutine(TKUDistanceCheck());
                }
                else
                {
                    if (targetDistance <= 7000)
                    {
                        SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                        SpawnOrXAirborne.instance.CheckSpawnTimer();
                        TutenKermanUldum();
                    }
                    else
                    {
                        yield return new WaitForSeconds(30);

                        if (targetDistance <= 10000 && !TKUairSpawn)
                        {
                            TKUairSpawn = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        if (targetDistance <= 15000 && !TKUairSpawn2)
                        {
                            TKUairSpawn2 = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        if (targetDistance <= 20000 && !TKUairSpawn3)
                        {
                            TKUairSpawn3 = true;
                            SpawnOrXAirborne.instance.SpawnCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
                            SpawnOrXAirborne.instance.CheckSpawnTimer();
                        }

                        StartCoroutine(TKUDistanceCheck());
                    }
                }
            }
        }

        public void TutenKermanUldum()
        {
            Pyramids = true;
            KSC = false;
            Baikerbanur = false;
            waldosIsland = false;
            IWIairSpawn = false;
            IWIairSpawn2 = false;
            survival = false;

            OrX_Log.instance.sound_SpawnOrXHole.Play();

            if (!keysSet)
            {
                keysSet = true;
                OrX_Log.instance.SetFocusKeys();
            }

            if (debug)
            {
                Debug.Log("[OrX Missions] Tuten Kerman Uldum ...... spawnCount = " + spawnCount);
            }

            if (spawnCount == 0)
            {
                spawnCount += 1;
                PyramidsCoord1 = true;
                PyramidsSpawn();
            }
            else
            {
                if (spawnCount == 1)
                {
                    spawnCount += 1;
                    PyramidsCoord2 = true;
                    PyramidsSpawn();
                }
                else
                {
                    if (spawnCount == 2)
                    {
                        spawnCount += 1;
                        PyramidsCoord3 = true;
                        PyramidsSpawn();
                    }
                    else
                    {
                        if (spawnCount == 3)
                        {
                            spawnCount += 1;
                            PyramidsCoord4 = true;
                            PyramidsSpawn();
                        }
                        else
                        {
                            if (spawnCount == 4)
                            {
                                spawnCount += 1;
                                PyramidsCoord5 = true;
                                PyramidsSpawn();
                            }
                            else
                            {
                                if (spawnCount == 5)
                                {
                                    spawnCount += 1;
                                    PyramidsCoord6 = true;
                                    PyramidsSpawn();
                                }
                                else
                                {
                                    if (spawnCount == 6)
                                    {
                                        spawnCount += 1;
                                        PyramidsCoord7 = true;
                                        PyramidsSpawn();
                                    }
                                    else
                                    {
                                        if (spawnCount == 7)
                                        {
                                            spawnCount += 1;
                                            PyramidsCoord8 = true;
                                            PyramidsSpawn();
                                        }
                                        else
                                        {
                                            if (spawnCount == 8)
                                            {
                                                spawnCount += 1;
                                                PyramidsCoord9 = true;
                                                PyramidsSpawn();
                                            }
                                            else
                                            {
                                                if (spawnCount == 9)
                                                {
                                                    spawnCount += 1;
                                                    PyramidsCoord10 = true;
                                                    PyramidsSpawn();
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
        }

        public void PyramidsSpawn()
        {
            Pyramids = true;

            if (!spawning)
            {
                spawning = true;

                if (debug)
                {
                    Debug.Log("[OrX Missions] PyramidsSpawn ................. SPAWNING ");
                }

                if (level <= 6)
                {
                    int _random = new System.Random().Next(4, 10);
                    orxToSpawn = _random;
                }
                else
                {
                    int _random = new System.Random().Next(6, 12);
                    orxToSpawn = _random;
                }

                if (PyramidsCoord1)
                {
                    latitude = PyramidsCoord1lat;
                    longitude = PyramidsCoord1lon;
                    altitude = PyramidsCoord1alt;
                }

                if (PyramidsCoord2)
                {
                    latitude = PyramidsCoord2lat;
                    longitude = PyramidsCoord2lon;
                    altitude = PyramidsCoord2alt;
                }

                if (PyramidsCoord3)
                {
                    latitude = PyramidsCoord3lat;
                    longitude = PyramidsCoord3lon;
                    altitude = PyramidsCoord3alt;
                }

                if (PyramidsCoord4)
                {
                    latitude = PyramidsCoord4lat;
                    longitude = PyramidsCoord4lon;
                    altitude = PyramidsCoord4alt;
                }

                if (PyramidsCoord5)
                {
                    latitude = PyramidsCoord5lat;
                    longitude = PyramidsCoord5lon;
                    altitude = PyramidsCoord5alt;
                }

                if (PyramidsCoord6)
                {
                    latitude = PyramidsCoord6lat;
                    longitude = PyramidsCoord6lon;
                    altitude = PyramidsCoord6alt;
                }

                if (PyramidsCoord7)
                {
                    latitude = PyramidsCoord7lat;
                    longitude = PyramidsCoord7lon;
                    altitude = PyramidsCoord7alt;
                }

                if (PyramidsCoord8)
                {
                    latitude = PyramidsCoord8lat;
                    longitude = PyramidsCoord8lon;
                    altitude = PyramidsCoord8alt;
                }

                if (PyramidsCoord9)
                {
                    latitude = PyramidsCoord9lat;
                    longitude = PyramidsCoord9lon;
                    altitude = PyramidsCoord9alt;
                }

                if (PyramidsCoord10)
                {
                    latitude = PyramidsCoord10lat;
                    longitude = PyramidsCoord10lon;
                    altitude = PyramidsCoord10alt;
                }
                SpawnLootBox.instance._lat = latitude;
                SpawnLootBox.instance._lon = longitude;
                SpawnLootBox.instance._alt = altitude;

                OrXSpawn.instance._lat = latitude;
                OrXSpawn.instance._lon = longitude;
                OrXSpawn.instance._alt = altitude;

                PyramidsCoord1 = false;
                PyramidsCoord2 = false;
                PyramidsCoord3 = false;
                PyramidsCoord4 = false;
                PyramidsCoord5 = false;
                PyramidsCoord6 = false;
                PyramidsCoord7 = false;
                PyramidsCoord8 = false;
                PyramidsCoord9 = false;
                PyramidsCoord10 = false;
                StartCoroutine(SpawnPyramids());
            }
        }

        IEnumerator SpawnPyramids()
        {
            OrXSpawn.instance.SpawnOrX();
            yield return new WaitForSeconds(2.5f);
            SpawnCheckPyramids();
        }

        private void SpawnCheckPyramids()
        {
            orxToSpawn -= 1;

            if (orxToSpawn >= 0)
            {
                StartCoroutine(SpawnPyramids());
            }
            else
            {
                SpawnLootBox.instance.CheckSpawnTimer();

                if (debug)
                {
                    Debug.Log("[OrX Missions] Spawn Check Pyramids ................. Loot Box Spawned ");
                }
                spawning = false;
                OrXVesselSwitcher.instance.missionRunning = true;
                OrXVesselSwitcher.instance.missions = true;
                OrXVesselSwitcher.instance.missionPaused = false;
                SpawnArmedOrX();
            }
        }

        #endregion

        #region Tuten Kerman Uldum GUI

        public static bool GuiEnabledTKU;

        public void EnableTKUGui()
        {
            spawning = false;
            KillCoroutines();
            KSC = false;
            waldosIsland = false;
            Baikerbanur = false;
            Pyramids = false;
            guiOpen = false;
            _gameUiToggle = true;
            MissionLauncher.instance.DisableGui();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledTKU = true;
                    Debug.Log("[OrX Mission TKU]: Showing GUI");
                }
                else
                {
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledTKU = true;
                    Debug.Log("[OrX Mission TKU]: Showing GUI");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    GuiEnabledTKU = true;
                    Debug.Log("[OrX Mission TKU]: Showing GUI");
                }
                else
                {
                    guiOpen = true;
                    GuiEnabledTKU = true;
                    Debug.Log("[OrX Mission TKU]: Showing GUI");
/*
                    ScreenMsg("<color=#cc4500ff><b>OrX missions are inaccesible in the flight scene if not using the Tardis</b></color>");
                    guiOpen = false;
                    GuiEnabledTKU = false;
                    GuiEnabledLBC = false;
                    GuiEnabledATK = false;
                    GuiEnabledIWI = false;*/
                }
            }
        }

        private void GuiWindowOrXTKU(int OrX_TKU)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXTKUTitle(line);
            DrawTKUText1(line);
            line++;
            DrawTKUText2(line);
            line++;
            DrawTKUText3(line);
            line++;
            DrawTKUText4(line);
            line++;
            DrawTKUText6(line);
            line++;
            DrawTKUText7(line);
            line++;
            line++;
            DrawTKUText13(line);
            line++;
            DrawTKUText14(line);
            line++;
            DrawTKUText15(line);
            line++;
            DrawTKUText16(line);
            line++;
            DrawTKUText17(line);
            line++;
            DrawTKUText18(line);
            line++;
            DrawTKUText19(line);
            line++;
            line++;
            DrawStartTKUMission(line);
            line++;
            DrawDeclineMission(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawOrXTKUTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Tuten Kerman Uldum", titleStyle);
        }

        private void DrawTKUText1(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "The famous Kerbal archeologist Jebbison Jones has found the",

              titleStyle);
        }

        private void DrawTKUText2(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "lost city of Uldum, an ancient city in the sand.",
              titleStyle);
        }

        private void DrawTKUText3(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Many strange things are said to exist there, but no one has",

              titleStyle);
        }

        private void DrawTKUText4(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "ever returned with any knowledge of this vast expanse…", titleStyle);
        }

        private void DrawTKUText6(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "How Jebbison Jones discovered its location is a closely",

              titleStyle);
        }

        private void DrawTKUText7(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "guarded secret.",

              titleStyle);
        }

        private void DrawTKUText13(float line)
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

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "TH﻿E MISSION",

              titleStyle);
        }

        private void DrawTKUText14(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Jones has gone missing, just after providing you with the",

              titleStyle);
        }

        private void DrawTKUText15(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "coordinates to what he found in the desert",

              titleStyle);
        }

        private void DrawTKUText16(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Cryptic traffic has been intercepted coming from Waldo’s",

              titleStyle);
        }

        private void DrawTKUText17(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "minions mentions a Pyramid by the shade of a mountain ...",

              titleStyle);
        }

        private void DrawTKUText18(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Your orders are to investigate these coordinates and take",

              titleStyle);
        }

        private void DrawTKUText19(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "whatever action is necessary.",

              titleStyle);
        }

        private void DrawStartTKUMission(float line)
        {
            GUIStyle OrXbuttonStyle = missionSelected ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!missionSelected)
            {
                if (!Baikerbanur && !Pyramids && !KSC && !waldosIsland && !survival)
                {
                    if (GUI.Button(saveRect, "START MISSION", OrXbuttonStyle))
                    {
                        DisableGui();
                        LaunchTKU();

                        /*
                        if (saltTotal >= 4000)
                        {
                            saltTotal -= 4000;
                            LaunchTKU();
                        }
                        else
                        {
                            if (OrX_Log.instance.devKitInstalled)
                            {
                                LaunchTKU();
                            }
                            else
                            {
                                ScreenMsg("<color=#cc4500ff><b>Please come back later with more Salt</b></color>");
                                MissionLauncher.instance.EnableGui();
                            }
                        }*/
                    }
                }
            }
        }

        private void LaunchTKU()
        {
            missionSelected = true;
            Pyramids = true;
            MissionLauncher.instance.Pyramids = true;
            MissionLauncher.instance.Survival = false;
            MissionLauncher.instance.WaldosIsland = false;
            MissionLauncher.instance.KSC = false;
            MissionLauncher.instance.Baikerbanur = false;
            MissionLauncher.instance.ironKerbal = true;
            OrX_Log.instance.survival = false;
            OrX_Log.instance.ironKerbal = true;

            spawnCount = 0;
            mission = false;
            waldosIsland = false;
            KSC = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            survival = false;
            survivalReset = false;

            var count = 0;
            Debug.Log("[OrX MISSIONS - TUTEN KERMAN ULDUM] ........ Tuten Kerman Uldum");
            _lat = latPyramids;
            _lon = lonPyramids;
            _alt = 1304;

            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    MissionLauncher.instance.launchSiteChanged = true;
                    MissionLauncher.instance.Pyramids = true;

                    var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null && count == 0)
                    {
                        tardis.Survival = false;
                        count += 1;
                        tardis.Pyramids = true;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.WaldosIsland = false;
                        tardis.launchSiteChanged = true;
                        tardis.triggered = false;
                        MissionLauncher.instance.editorLaunch = true;
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                    else
                    {
                        TKU();
                        EditorLogic.fetch.launchVessel();
                        break;
                    }
                }
            }
            else
            {
                MissionLauncher.instance.launchSiteChanged = true;
                MissionLauncher.instance.Pyramids = true;
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();

                double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

                if (targetDistance >= 100000)
                {
                    if (tardis != null)
                    {
                        tardis.Survival = false;
                        startMission = true;
                        Pyramids = true;
                        tardis.Pyramids = true;
                        tardis.KSC = false;
                        tardis.Baikerbanur = false;
                        tardis.launchSiteChanged = true;
                        tardis.Baikerbanur = false;
                        tardis.triggered = false;
                    }
                }
                else
                {
                    if (tardis != null)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>This is too close to your current position for a Tardis jump</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Please select another destination</b></color>");
                    }
                    else
                    {
                        ScreenMsg("<color=#cc4500ff><b>Selected mission location is " + targetDistance + " meters from your current position</b></color>");
                        ScreenMsg("<color=#cc4500ff><b>Adding to HoloCache database</b></color>");

                        // ADD MISSION GPS TO HOLOCACHE CONTROLLER
                    }

                    //TKU();
                    Debug.Log("[OrX MISSIONS - TUTEN KERMAN ULDUM] ........ Please select another destination");
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        private void DrawDeclineMission(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (!missionSelected)
            {
                if (GUI.Button(saveRect, "DECLINE MISSION"))
                {
                    KillMission();
                    MissionLauncher.instance.EnableGui();
                }
            }
        }

        public void DisableGui()
        {
            guiOpen = false;
            GuiEnabledSurvival = false;
            GuiEnabledLBC = false;
            GuiEnabledATK = false;
            GuiEnabledIWI = false;
            GuiEnabledTKU = false;
            Debug.Log("[OrX Missions - Kerbin]: Hiding GUI");
        }

        public void KillMission()
        {
            DisableGui();
            KillCoroutines();
        }

        public void KillCoroutines()
        {
            ResetKM();
            MissionLauncher.instance.Survival = false;
            MissionLauncher.instance.WaldosIsland = false;
            MissionLauncher.instance.KSC = false;
            MissionLauncher.instance.Pyramids = false;
            MissionLauncher.instance.Baikerbanur = false;
            MissionLauncher.instance.ironKerbal = false;
            OrX_Log.instance.survival = false;
            OrX_Log.instance.ironKerbal = false;
            StopAllCoroutines();
        }

        public void ResetKM()
        {
            spawning = false;
            spawnCount = 0;
            startMission = false;
            missionSelected = false;
            ironKerbal = false;
            mission = false;
            waldosIsland = false;
            KSC = false;
            Pyramids = false;
            Baikerbanur = false;
            IWIairSpawn = false;
            IWIairSpawn2 = false;
            survival = false;
            survivalReset = false;
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////////////////
    }
}


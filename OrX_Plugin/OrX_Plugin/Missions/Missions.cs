using UnityEngine;
using OrX.spawn;
using System.Collections;
using System.Collections.Generic;
using OrX.parts;

namespace OrX.missions
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Missions : MonoBehaviour
    {
        public static Missions instance;

        public bool startMission = true;
        public bool waldo = false;
        public bool debug = true;
        public bool survival = false;

        private void Awake()
        {
            if (instance) Destroy(instance);
            instance = this;
        }

        #region Level Up

        public int level = 0;
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
            if (debug)
            {
                Debug.Log("[OrX Missions - Adding Salt] ADDING " + saltGain + " Salt");
            }

            mission = false;
            StartCoroutine(LevelUpRoutine());
        }

        IEnumerator LevelUpRoutine()
        {
            level += 1;
            debris = 0;

            ScreenMsg("<b>CURRENT LEVEL IS NOW </b>" + level);

            if (debug)
            {
                Debug.Log("[OrX Missions] LEVELING UP ... CURRENT LEVEL IS NOW " + level);
            }

            if (waldo)
            {
                waldo = false;

                if (OrX_WaldoHP.instance.GuiEnabledOrX_WaldoHP)
                {
                    OrX_WaldoHP.instance.GuiEnabledOrX_WaldoHP = false;
                }

                if (!OrX_Controls.instance.guiOpen)
                {
                    OrX_Controls.instance.OpenGUI();
                }
            }

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;

                if (!v.Current.isEVA)
                {
                    if (((v.Current.vesselName.Contains("Debris") || v.Current.vesselName.Contains("Rover")
                        || v.Current.vesselName.Contains("Probe")) && v.Current.vesselName.Contains("OrX")))
                    {
                        debris += 1;
                        v.Current.rootPart.AddModule("ModuleOrXDestroyVessel", true);
                    }

                    if (v.Current.vesselName == "OrX Loot Box")
                    {
                        var lb = v.Current.FindPartModuleImplementing<ModuleOrXLootBox>();
                        lb.CheckLevel();
                    }
                }
                else
                {
                    var OrXkerbal = v.Current.FindPartModuleImplementing<ModuleOrX>();

                    if (!OrXkerbal.team)
                    {
                        if (debug)
                        {
                            Debug.Log("[OrX Missions] Found Kerbal ....................... Leveling up");
                        }
                        OrXkerbal.LevelUP();
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
                ScreenMsg("<b>OrX removing " + debris + " pieces of debris from game</b>");
            }

            SpawningSequence();
        }

        #endregion

        #region Missions

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
        private double KSCCoord1alt = 69;

        public bool KSCCoord2 = false;
        private double KSCCoord2lat = -0.0906792112643335;
        private double KSCCoord2lon = -74.6303881751926;
        private double KSCCoord2alt = 69;

        public bool KSCCoord3 = false;
        private double KSCCoord3lat = -0.108999921969498;
        private double KSCCoord3lon = -74.6276665561495;
        private double KSCCoord3alt = 69;

        public bool KSCCoord4 = false;
        private double KSCCoord4lat = -0.112876682396579;
        private double KSCCoord4lon = -74.6522191740983;
        private double KSCCoord4alt = 69;

        public bool KSCCoord5 = false;
        private double KSCCoord5lat = -0.125929732172947;
        private double KSCCoord5lon = -74.6100402129825;
        private double KSCCoord5alt = 69;

        public bool KSCCoord6 = false;
        private double KSCCoord6lat = -0.123401752691497;
        private double KSCCoord6lon = -74.6344753477158;
        private double KSCCoord6alt = 69;

        public bool KSCCoord7 = false;
        private double KSCCoord7lat = -0.117028421187697;
        private double KSCCoord7lon = -74.6368234014466;
        private double KSCCoord7alt = 69;

        public bool KSCCoord8 = false;
        private double KSCCoord8lat = -0.106978614535463;
        private double KSCCoord8lon = -74.6414465578981;
        private double KSCCoord8alt = 305;

        public bool KSCCoord9 = false;
        private double KSCCoord9lat = -0.0776694056652502;
        private double KSCCoord9lon = -74.644979264208;
        private double KSCCoord9alt = 69;

        public bool KSCCoord10 = false;
        private double KSCCoord10lat = -0.117028421187697;
        private double KSCCoord10lon = -74.6368234014466;
        private double KSCCoord10alt = 69;

        #endregion

        public void LootBoxControversy()
        {
            KSC = true;
            OrX_Log.instance.sound_SpawnOrXHole.Play();

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

                int _random = new System.Random().Next(4, 10);
                orxToSpawn = _random;

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
            }
        }

        #endregion

        #region Waldo's Island

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

        public void WaldosIsland()
        {
            waldosIsland = true;
            OrX_Log.instance.sound_SpawnOrXHole.Play();

            if (spawnCount == 0)
            {
                spawnCount += 1;

                WaldosIslandCoord1 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 1)
            {
                spawnCount += 1;

                WaldosIslandCoord2 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 2)
            {
                spawnCount += 1;

                WaldosIslandCoord3 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 3)
            {
                spawnCount += 1;

                WaldosIslandCoord4 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 4)
            {
                spawnCount += 1;

                WaldosIslandCoord5 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 5)
            {
                spawnCount += 1;

                WaldosIslandCoord6 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 6)
            {
                spawnCount += 1;

                WaldosIslandCoord7 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 7)
            {
                spawnCount += 1;

                WaldosIslandCoord8 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 8)
            {
                spawnCount += 1;

                WaldosIslandCoord9 = true;
                WaldosIslandSpawn();
            }
            if (spawnCount == 9)
            {
                spawnCount += 1;
                WaldosIslandCoord10 = true;
                WaldosIslandSpawn();
            }

            StartCoroutine(StartMissionDelay());
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

                ResetWaldosIslandCoords();

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
            }
        }

        private void ResetWaldosIslandCoords()
        {
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

        #endregion

        #region Attack of the Killer Tomatoes

        public void KillerTomatoes()
        {

        }

        public void BaikerbanurSpawn()
        {
            Baikerbanur = true;

            if (spawnCount == 0)
            {
                spawnCount += 1;

            }

            if (spawnCount == 1)
            {
                spawnCount += 1;

            }
            if (spawnCount == 2)
            {
                spawnCount += 1;

            }
            if (spawnCount == 3)
            {
                spawnCount += 1;

            }
            if (spawnCount == 4)
            {
                spawnCount += 1;

            }
            if (spawnCount == 5)
            {
                spawnCount += 1;

            }
            if (spawnCount == 6)
            {
                spawnCount += 1;

            }
            if (spawnCount == 7)
            {
                spawnCount += 1;

            }
            if (spawnCount == 8)
            {
                spawnCount += 1;

            }
            if (spawnCount == 9)
            {
                spawnCount += 1;

            }
        }


        #endregion

        #region Tuten-Kerman


        public void TutenKerman()
        {

        }

        public void PyramidSpawn()
        {
            Pyramids = true;

            if (spawnCount == 0)
            {
                spawnCount += 1;

            }

            if (spawnCount == 1)
            {
                spawnCount += 1;

            }
            if (spawnCount == 2)
            {
                spawnCount += 1;

            }
            if (spawnCount == 3)
            {
                spawnCount += 1;

            }
            if (spawnCount == 4)
            {
                spawnCount += 1;

            }
            if (spawnCount == 5)
            {
                spawnCount += 1;

            }
            if (spawnCount == 6)
            {
                spawnCount += 1;

            }
            if (spawnCount == 7)
            {
                spawnCount += 1;

            }
            if (spawnCount == 8)
            {
                spawnCount += 1;

            }
            if (spawnCount == 9)
            {
                spawnCount += 1;

            }
        }


        #endregion

        #region Spawning

        public Vector3d SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)latitude, (double)longitude, (double)altitude);
        }

        public void SpawningSequence()
        {
            Debug.Log("[OrX Missions - Spawning Sequence] Spawn Sequence .......................");

            if (FlightGlobals.ActiveVessel.Landed)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    StartCoroutine(SpawnEVA());
                }
                else
                {
                    if (survival)
                    {
                        StartCoroutine(SpawnLanded());
                    }
                    else
                    {
                        StartCoroutine(SpawnEVA());
                    }
                }
            }
            else
            {
                if (FlightGlobals.ActiveVessel.isEVA && !FlightGlobals.ActiveVessel.Splashed)
                {
                    StartCoroutine(SpawnEVA());
                }
                else
                {
                    if (FlightGlobals.ActiveVessel.Splashed)
                    {
                        if (survival)
                        {
                            StartCoroutine(SpawnSplashed());
                        }
                        else
                        {
                            StartCoroutine(SpawnEVA());
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnEVA());
                    }
                }
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

        IEnumerator SpawnEVA()
        {
            if (mission)
            {
                Debug.Log("[OrX Missions - SpawnEVA] Start Mission .......................");
                StartCoroutine(StartMissionDelay());
            }
            else
            {
                Debug.Log("[OrX Missions - SpawnEVA] Delay Sequence .......................");
                mission = true;
                yield return new WaitForSeconds(5);
                StartCoroutine(SpawnEVA());
            }
        }

        IEnumerator StartMissionDelay()
        {
            if (debug)
            {
                Debug.Log("[OrX Missions - Update List] SpawnDelay .......................");
            }

            yield return new WaitForSeconds(20);

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

        IEnumerator StartMission()
        {
            if (debug)
            {
                Debug.Log("[OrX Missions - StartMission] StartMission ....................... StartMission");
            }

            yield return new WaitForSeconds(5);

            if (KSC)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] KSC ..... LootBoxControversy Level " + spawnCount);
                }

                if (spawnCount == 0)
                {
                    //LootBoxContSetup.instance.count = 0;
                    //LootBoxContSetup.instance.CheckSpawnTimer();
                }
                LootBoxControversy();
            }

            if (waldosIsland)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] waldosIsland ....................... WaldosIsland");
                }

                if (spawnCount == 0)
                {
                    WaldosIslandSetup.instance.count = 0;
                    WaldosIslandSetup.instance.CheckSpawnTimer();
                }
                WaldosIsland();
            }

            if (Pyramids)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Pyramids ....................... Pyramids");
                }

                //WaldosIslandSetup.instance.CheckSpawnTimer();
                //PyramidSpawn();

            }

            if (Baikerbanur)
            {
                if (debug)
                {
                    Debug.Log("[OrX Missions - StartMission] Baikerbanur ....................... Baikerbanur");
                }

                //WaldosIslandSetup.instance.CheckSpawnTimer();
                //BaikerbanurSpawn();

            }
        }

        #endregion

        #endregion

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

    }
}


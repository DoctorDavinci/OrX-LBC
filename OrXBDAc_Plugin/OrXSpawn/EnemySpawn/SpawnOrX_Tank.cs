﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSP.UI;
using UnityEngine;
using System.IO;
using System.Reflection;
using BDArmory.Modules;

namespace OrXBDAc.spawn
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SpawnOrX_Tank : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private List<MissileFire> _wmgrsA;
        private List<MissileFire> _wmgrsB;

        private double _lat_ = 2f;
        private double _lon_ = 2f;
        public Vector3d SpawnCoords;
        private bool detecting = false;

        public static SpawnOrX_Tank instance;

        internal static List<ProtoCrewMember> SelectedCrewData;
        private bool loadingCraft = false;

        private string OrX_Tank01 = string.Empty;
        private string flagURL = string.Empty;
        private bool timer = false;
        private bool delay = true;
        private float spawnTimer = 0.0f;
        private double _altitude = 0.0f;
        private float altitude = 0.0f;

        private void Awake()
        {
            var _OrX_Tank01 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            flagURL = "OrX_icon";
            OrX_Tank01 = _OrX_Tank01 + "\\PluginData" + "\\VesselData" + "\\OrX" + "\\OrXLynx.craft";

            if (instance) Destroy(instance);
            instance = this;
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        public void CheckSpawnTimer()
        {
            var count = 0;
            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                if (v.Current.vesselName == "OrX Tank")
                {
                    count += 1;
                }
            }
            v.Dispose();

            if (count == 0)
            {
                Debug.Log("[Spawn OrX Tank] Starting SpawnTimerRoutine ......");
                detecting = false;
                timer = true;
                spawnTimer = new System.Random().Next(10, 15);
                StartCoroutine(SpawnTimerRoutine());
            }
        }

        IEnumerator SpawnTimerRoutine()
        {
            yield return new WaitForEndOfFrame();

            RandomizeGPS();

            yield return new WaitForSeconds(spawnTimer);

            Debug.Log("[Spawn OrX Tank] Starting StartVesselSpawn ......");

            StartCoroutine(SpawnCraftRoutine(OrX_Tank01));
        }

        public void RandomizeGPS()
        {
            _lat_ = 0;
            _lon_ = 0;

            double _random = new System.Random().Next(1, 4);
            double _random2 = new System.Random().Next(0, 4);
            double _random3 = _random + _random2;

            Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ......");

            if (_random3 <= 3)
            {
                double lat = (_random * 2) / 100;
                double lon = (_random2 * 2) / 100;

                if (lat <= 0.005f)
                {
                    lat = 0.005f;
                }

                if (lon <= 0.005f)
                {
                    lon = 0.005f;
                }


                _lat_ += lat;
                _lon_ += lon;
                Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lat_: " + _lat_);
                Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lon_: " + _lon_);

            }
            else
            {
                if (_random3 <= 5)
                {
                    double lat = (_random * 2) / -100;
                    double lon = (_random2 * 2) / 100;

                    if (lat >= -0.005f)
                    {
                        lat = -0.005f;
                    }

                    if (lon <= 0.005f)
                    {
                        lon = 0.005f;
                    }

                    _lat_ += lat;
                    _lon_ += lon;
                    Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lat_: " + _lat_);
                    Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lon_: " + _lon_);

                }
                else
                {
                    if (_random3 <= 7)
                    {
                        double lat = (_random * 2) / 100;
                        double lon = (_random2 * 2) / -100;

                        if (lat <= 0.005f)
                        {
                            lat = 0.005f;
                        }

                        if (lon >= -0.005f)
                        {
                            lon = -0.005f;
                        }

                        _lat_ += lat;
                        _lon_ += lon;
                        Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lat_: " + _lat_);
                        Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lon_: " + _lon_);

                    }
                    else
                    {
                        double lat = (_random * 2) / -100;
                        double lon = (_random2 * 2) / -100;

                        if (lat >= -0.005f)
                        {
                            lat = -0.005f;
                        }

                        if (lon >= -0.005f)
                        {
                            lon = -0.005f;
                        }

                        _lat_ += lat;
                        _lon_ += lon;
                        Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lat_: " + _lat_);
                        Debug.Log("[Spawn OrX Tank] Starting RandomizeGPS ...... _lon_: " + _lon_);

                    }
                }
            }
        }

        private IEnumerator SpawnCraftRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {

            loadingCraft = true;

            yield return new WaitForEndOfFrame();

            Vector3 worldPos = SpawnCoords;

            Vector3 gpsPos = WorldPositionToGeoCoords(worldPos, FlightGlobals.currentMainBody);
            ScreenMsg("<color=#cc4500ff><b>Something isn't Right ............. </b></color>");
            Debug.Log("[Spawn OrX Tank] Starting StartVesselSpawn ......");
            SpawnVesselFromCraftFile(craftUrl, gpsPos, 90, 0, crewData);
        }

        public static Vector3d WorldPositionToGeoCoords(Vector3d worldPosition, CelestialBody body)
        {
            if (!body)
            {
                return Vector3d.zero;
            }

            double lat = body.GetLatitude(worldPosition);
            double longi = body.GetLongitude(worldPosition);
            double alt = body.GetAltitude(worldPosition);
            Debug.Log("[Spawn OrX Tank] Lat: " + lat + " - Lon:" + longi + " - Alt: " + alt);
            return new Vector3d(lat, longi, alt);
        }

        private void SpawnVesselFromCraftFile(string craftURL, Vector3d gpsCoords, float heading, float pitch, List<ProtoCrewMember> crewData = null)
        {
            TankVesselData newData = new TankVesselData();

            newData.craftURL = craftURL;
            newData.latitude = gpsCoords.x + _lat_;
            newData.longitude = gpsCoords.y + _lon_;
            newData.altitude = gpsCoords.z + 5;

            Debug.Log("[Spawn OrX Tank] SpawnVesselFromCraftFile Altitude: " + newData.altitude);

            newData.body = FlightGlobals.currentMainBody;
            newData.heading = heading;
            newData.pitch = pitch;
            newData.orbiting = false;

            newData.flagURL = flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.Unknown;
            newData.crew = new List<CrewData>();

            SpawnVessel(newData, crewData);
        }

        private void SpawnVessel(TankVesselData TankVesselData, List<ProtoCrewMember> crewData = null)
        {
            //      string gameDataDir = KSPUtil.ApplicationRootPath;
            Debug.Log("[Spawn OrX Tank] Spawning Airborne Enemey: " + TankVesselData.name);

            // Set additional info for landed vessels
            bool landed = false;
            if (!landed)
            {
                landed = true;
                if (TankVesselData.altitude == null || TankVesselData.altitude < 0)
                {
                    if (FlightGlobals.ActiveVessel.radarAltitude <= 5000 && FlightGlobals.ActiveVessel.radarAltitude >= 1000)
                    {
                        var _random = new System.Random().Next(0, 2000);
                        var random = _random + FlightGlobals.ActiveVessel.radarAltitude;

                        TankVesselData.altitude = random;//LocationUtil.TerrainHeight(TankVesselData.latitude, TankVesselData.longitude, TankVesselData.body);
                    }
                    else
                    {
                        var _random = new System.Random().Next(0, 2000);

                        if (_random <= 1000)
                        {
                            TankVesselData.altitude = FlightGlobals.ActiveVessel.radarAltitude - _random;//LocationUtil.TerrainHeight(TankVesselData.latitude, TankVesselData.longitude, TankVesselData.body);
                        }
                        else
                        {
                            TankVesselData.altitude = FlightGlobals.ActiveVessel.radarAltitude + _random;//LocationUtil.TerrainHeight(TankVesselData.latitude, TankVesselData.longitude, TankVesselData.body);
                        }
                    }
                }
                Debug.Log("[Spawn OrX Tank] SpawnVessel Altitude: " + TankVesselData.altitude);


                //Vector3d pos = TankVesselData.body.GetWorldSurfacePosition(TankVesselData.latitude, TankVesselData.longitude, TankVesselData.altitude.Value);
                Vector3d pos = TankVesselData.body.GetRelSurfacePosition(TankVesselData.latitude, TankVesselData.longitude, TankVesselData.altitude.Value);



                TankVesselData.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, TankVesselData.body);
                TankVesselData.orbit.UpdateFromStateVectors(pos, TankVesselData.body.getRFrmVel(pos), TankVesselData.body, Planetarium.GetUniversalTime());
            }

            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            bool hasClamp = false;
            float lcHeight = 0;
            ConfigNode craftNode;
            Quaternion craftRotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(TankVesselData.craftURL))
            {
                // Save the current ShipConstruction ship, otherwise the player will see the spawned ship next time they enter the VAB!
                ConfigNode currentShip = ShipConstruction.ShipConfig;

                shipConstruct = ShipConstruction.LoadShip(TankVesselData.craftURL);
                if (shipConstruct == null)
                {
                    Debug.Log("[Spawn OrX Tank] ShipConstruct was null when tried to load '" + TankVesselData.craftURL +
                      "' (usually this means the file could not be found).");
                    return;//continue;
                }

                craftNode = ConfigNode.Load(TankVesselData.craftURL);
                lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
                craftRotation = ConfigNode.ParseQuaternion(craftNode.GetNode("PART").GetValue("rot"));

                // Restore ShipConstruction ship
                ShipConstruction.ShipConfig = currentShip;

                // Set the name
                if (string.IsNullOrEmpty(TankVesselData.name))
                {
                    TankVesselData.name = "OrX Tank";
                }

                // Set some parameters that need to be at the part level
                uint missionID = (uint)Guid.NewGuid().GetHashCode();
                uint launchID = HighLogic.CurrentGame.launchID++;
                foreach (Part p in shipConstruct.parts)
                {
                    p.flightID = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                    p.missionID = missionID;
                    p.launchID = launchID;
                    p.flagURL = flagURL;
                    p.temperature = 1.0;
                }

                ConfigNode empty = new ConfigNode();
                ProtoVessel dummyProto = new ProtoVessel(empty, null);
                Vessel dummyVessel = new Vessel();
                dummyVessel.parts = shipConstruct.parts;
                dummyProto.vesselRef = dummyVessel;

                // Create the ProtoPartSnapshot objects and then initialize them
                foreach (Part p in shipConstruct.parts)
                {
                    dummyProto.protoPartSnapshots.Add(new ProtoPartSnapshot(p, dummyProto));
                }
                foreach (ProtoPartSnapshot p in dummyProto.protoPartSnapshots)
                {
                    p.storePartRefs();
                }

                // Create the ship's parts

                List<ConfigNode> partNodesL = new List<ConfigNode>();
                foreach (ProtoPartSnapshot snapShot in dummyProto.protoPartSnapshots)
                {
                    ConfigNode node = new ConfigNode("PART");
                    snapShot.Save(node);
                    partNodesL.Add(node);
                }
                partNodes = partNodesL.ToArray();
            }
            else
            {

                // Create crew member array
                ProtoCrewMember[] crewArray = new ProtoCrewMember[TankVesselData.crew.Count];
                
                        int i = 0;
                        foreach (CrewData cd in TankVesselData.crew)
                        {
                
                          // Create the ProtoCrewMember
                          ProtoCrewMember crewMember = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
                          if (cd.name != null)
                          {
                            crewMember.KerbalRef.name = cd.name;
                          }

                          crewArray[i++] = crewMember;

                        }
                
                // Create part nodes
                uint flightId = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                partNodes = new ConfigNode[1];
                partNodes[0] = ProtoVessel.CreatePartNode(TankVesselData.craftPart.name, flightId, crewArray);

                // Default the size class
                //sizeClass = UntrackedObjectClass.A;

                // Set the name
                if (string.IsNullOrEmpty(TankVesselData.name))
                {
                    TankVesselData.name = TankVesselData.craftPart.name;
                }
            }

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            //DiscoveryLevels discoveryLevel = TankVesselData.owned ? DiscoveryLevels.Owned : DiscoveryLevels.Unowned;
            //additionalNodes[0] = ProtoVessel.CreateDiscoveryNode(discoveryLevel, sizeClass, contract.TimeDeadline, contract.TimeDeadline);

            // Create the config node representation of the ProtoVessel
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(TankVesselData.name, TankVesselData.vesselType, TankVesselData.orbit, 0, partNodes, additionalNodes);

            // Additional seetings for a landed vessel
            if (!TankVesselData.orbiting)
            {
                Vector3d norm = TankVesselData.body.GetRelSurfaceNVector(TankVesselData.latitude, TankVesselData.longitude);

                double terrainHeight = 0.0;
                if (TankVesselData.body.pqsController != null)
                {
                    terrainHeight = TankVesselData.body.pqsController.GetSurfaceHeight(norm) - TankVesselData.body.pqsController.radius;
                }
                bool splashed = false;// = landed && terrainHeight < 0.001;

                // Create the config node representation of the ProtoVessel
                // Note - flying is experimental, and so far doesn't worx
                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : landed ?
                  Vessel.Situations.LANDED : Vessel.Situations.FLYING).ToString());
                protoVesselNode.SetValue("landed", (landed && !splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", TankVesselData.latitude.ToString());
                protoVesselNode.SetValue("lon", TankVesselData.longitude.ToString());
                protoVesselNode.SetValue("alt", TankVesselData.altitude.ToString());
                protoVesselNode.SetValue("landedAt", TankVesselData.body.name);

                // Figure out the additional height to subtract
                //float lowest = float.MaxValue;
                /*
                if (shipConstruct != null)
                {
                    foreach (Part p in shipConstruct.parts)
                    {
                        foreach (Collider collider in p.GetComponentsInChildren<Collider>())
                        {
                            if (collider.gameObject.layer != 21 && collider.enabled)
                            {
                                lowest = Mathf.Min(lowest, collider.bounds.min.y);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Collider collider in TankVesselData.craftPart.partPrefab.GetComponentsInChildren<Collider>())
                    {
                        if (collider.gameObject.layer != 21 && collider.enabled)
                        {
                            lowest = Mathf.Min(lowest, collider.bounds.min.y);
                        }
                    }
                }

                if (lowest == float.MaxValue)
                {
                    lowest = 0;
                }
                */

                float lowest = 0;

                // Figure out the surface height and rotation
                Quaternion normal = Quaternion.LookRotation((Vector3)norm);// new Vector3((float)norm.x, (float)norm.y, (float)norm.z));
                Quaternion rotation = Quaternion.identity;
                float heading = TankVesselData.heading;
                if (shipConstruct == null)
                {
                    rotation = rotation * Quaternion.FromToRotation(Vector3.up, Vector3.back);
                }
                else if (shipConstruct.shipFacility == EditorFacility.SPH)
                {
                    rotation = rotation * Quaternion.FromToRotation(Vector3.forward, -Vector3.forward);
                    heading += 180.0f;
                }
                else
                {
                    rotation = rotation * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                    rotation = Quaternion.FromToRotation(Vector3.up, -Vector3.up) * rotation;

                    //rotation = craftRotation;


                    TankVesselData.heading = 0;
                    TankVesselData.pitch = 0;
                }

                rotation = rotation * Quaternion.AngleAxis(heading, Vector3.back);
                rotation = rotation * Quaternion.AngleAxis(TankVesselData.roll, Vector3.down);
                rotation = rotation * Quaternion.AngleAxis(TankVesselData.pitch, Vector3.left);

                // Set the height and rotation
                float hgt = (shipConstruct != null ? shipConstruct.parts[0] : TankVesselData.craftPart.partPrefab).localRoot.attPos0.y - lowest;
                hgt += TankVesselData.height;
                protoVesselNode.SetValue("hgt", hgt.ToString(), true);

                Debug.Log("[Spawn OrX Tank] SpawnVessel setting vessel height in protovessel: " + hgt);

                protoVesselNode.SetValue("rot", KSPUtil.WriteQuaternion(normal * rotation), true);

                // Set the normal vector relative to the surface
                Vector3 nrm = (rotation * Vector3.forward);
                protoVesselNode.SetValue("nrm", nrm.x + "," + nrm.y + "," + nrm.z, true);

                protoVesselNode.SetValue("prst", false.ToString(), true);
            }

            // Add vessel to the game
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);

            // Store the id for later use
            TankVesselData.id = protoVessel.vesselRef.id;

            //protoVessel.vesselRef.currentStage = 0;
            hasClamp = false;



            StartCoroutine(PlaceSpawnedVessel(protoVessel.vesselRef, !hasClamp));

            // Associate it so that it can be used in contract parameters
            //ContractVesselTracker.Instance.AssociateVessel(TankVesselData.name, protoVessel.vesselRef);


            //destroy prefabs
            foreach (Part p in FindObjectsOfType<Part>())
            {
                if (!p.vessel)
                {
                    Destroy(p.gameObject);
                }
            }
        }

        private IEnumerator PlaceSpawnedVessel(Vessel v, bool moveVessel)
        {
//            spawnTimer = new System.Random().Next(30, 60);
//            yield return new WaitForSeconds(spawnTimer);
            loadingCraft = true;
            v.isPersistent = true;
            v.Landed = false;
            v.situation = Vessel.Situations.FLYING;
            while (v.packed)
            {
                yield return null;
            }
            v.SetWorldVelocity(Vector3d.zero);
            yield return null;
//            v.Landed = true;
//            v.situation = Vessel.Situations.LANDED;
            v.GoOffRails();
            //            v.IgnoreGForces(240);
            v.UpdateLandedSplashed();
            _altitude = v.altitude;
            
            Debug.Log("[Spawn OrX Tank] Seeking Command Pod for " + v.vesselName);
            var command = v.FindPartModuleImplementing<ModuleCommand>();

            foreach (Part p in v.parts)
            {
                if (p.Modules.Contains("ModuleCommand"))
                {
                    if (command.minimumCrew >= 0 && p.protoModuleCrew.Count <= 1)
                    {
                        ProtoCrewMember crewMember = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
                        crewMember.gender = UnityEngine.Random.Range(0, 100) > 50 ? ProtoCrewMember.Gender.Female : ProtoCrewMember.Gender.Male;
                        p.AddCrewmemberAt(crewMember, p.protoModuleCrew.Count);
                        Debug.Log("[Spawn OrX Tank] Adding OrX to " + v.vesselName + " ...... " + p.protoModuleCrew.Count + " OrX onboard");
                    }
                    else
                    {
                        Debug.Log("[Spawn OrX Tank] Found Probe Core " + p.name + " .......... Skipping");
                    }
                }
            }

            var wmPart = v.FindPartModuleImplementing<MissileFire>();
            if (wmPart != null)
            {
                wmPart.part.force_activate();
                wmPart.guardMode = true;
                wmPart.team = true;
            }

            var PAI = v.FindPartModuleImplementing<BDModulePilotAI>();
            if (PAI != null)
            {
                PAI.part.force_activate();
                if (!PAI.pilotEnabled)
                {
                    PAI.ActivatePilot();
                }
            }

            var SAI = v.FindPartModuleImplementing<BDModulePilotAI>();
            if (SAI != null)
            {
                SAI.part.force_activate();
                if (!SAI.pilotEnabled)
                {
                    SAI.ActivatePilot();
                }
            }

            foreach (Part p in v.parts)
            {
                var engines = p.FindModuleImplementing<ModuleEngines>();
                var enginesFX = p.FindModuleImplementing<ModuleEnginesFX>();

                if (engines != null)
                {
                    p.force_activate();
                    engines.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    engines.Activate();
                }

                if (enginesFX != null)
                {
                    p.force_activate();
                    enginesFX.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesFX.Activate();
                }
            }

            altitude = Convert.ToSingle(_altitude);

            v.KillPermanentGroundContact();
            loadingCraft = false;
            IsMovingVessel = false;

            //            StartMove(v);
//            yield return new WaitForSeconds(1);
//            StartCoroutine(DropMoveRoutine(_vBounds));
//            _debugLr.enabled = false;
//            var setControl = v.FindPartModuleImplementing<ModuleOrXTank>();
//            setControl.s();
        }


        #region Drop

        #region Declarations

        public enum MoveModes { Normal = 0, Slow = 1, Fine = 2, Ludicrous = 3 }

        private MoveModes _moveMode = MoveModes.Normal;
        private bool _moving = false;
        private List<Vessel> _placingVessels = new List<Vessel>();
        private bool _hoverChanged;

        public float MoveHeight = 0;
        private float _hoverAdjust = 0f;
        private readonly float[] _hoverHeights = new float[] { 5000, 5000, 5000, 5000 };

        private float HoverHeight
        {
            get
            {
                return _hoverHeights[(int)_moveMode];
            }
        }

        private readonly float[] _moveSpeeds = new float[] { 50, 50, 50, 50 };

        private float MoveSpeed
        {
            get
            {
                return _moveSpeeds[(int)_moveMode];
            }
        }

        private readonly float[] _moveAccels = new float[] { 100, 100, 100, 100 };

        private float MoveAccel
        {
            get
            {
                return _moveAccels[(int)_moveMode];
            }
        }

        private readonly float[] _rotationSpeeds = new float[] { 50, 50, 50, 50 };

        private float RotationSpeed
        {
            get
            {
                return _rotationSpeeds[(int)_moveMode] * Time.fixedDeltaTime;
            }
        }

        public bool IsMovingVessel = false;
        public Vessel MovingVessel;
        private Quaternion _startRotation;
        private Quaternion _currRotation;
        private float _currMoveSpeed = 0;
        private Vector3 _currMoveVelocity;
        private VesselBounds _vBounds;
        private LineRenderer _debugLr;
        private Vector3 _up;
        private Vector3 _startingUp;
        private readonly float maxPlacementSpeed = 1050;
        private bool _hasRotated = false;
        private float _timeBoundsUpdated = 0;
        private ScreenMessage _moveMessage;

        #endregion

        #region KSP Events

/*
        private void Start()
        {
            _debugLr = new GameObject().AddComponent<LineRenderer>();
            _debugLr.material = new Material(Shader.Find("KSP/Emissive/Diffuse"));
            _debugLr.material.SetColor("_EmissiveColor", Color.green);
            _debugLr.startWidth = 0.15f;
            _debugLr.endWidth = 0.15f;
            _debugLr.enabled = false;
        }
*/
        private void FixedUpdate()
        {
            if (!_moving) return;
//            MovingVessel.IgnoreGForces(240);
//            UpdateMove();

            if (_hasRotated && Time.time - _timeBoundsUpdated > 0.2f)
            {
                UpdateBounds();
            }
        }
/*
        private void LateUpdate()
        {
            if (_moving)
            {
                UpdateDebugLines();
            }
        }
*/
        #endregion

        private void UpdateBounds()
        {
            _hasRotated = false;
            _vBounds.UpdateBounds();
            _timeBoundsUpdated = Time.time;
        }

        private void UpdateMove()
        {
            if (!MovingVessel)
            {
//                EndMove();
                return;
            }
            MovingVessel.IgnoreGForces(240);

            // Lerp is animating move
            if (!_hoverChanged)
                MoveHeight = Mathf.Lerp(MoveHeight, _vBounds.BottomLength + HoverHeight, 10 * Time.fixedDeltaTime);
            else
            {
                double alt = MovingVessel.radarAltitude;
                // sINCE Lerp is animating move from 0 to hoverheight, we do not want this going below current altitude
                if (MoveHeight < alt) MoveHeight = Convert.ToSingle(alt);

                MoveHeight = MovingVessel.Splashed
                  ? Mathf.Lerp(MoveHeight, _vBounds.BottomLength + _hoverAdjust, 10 * Time.fixedDeltaTime)
                  : Mathf.Lerp(MoveHeight, _vBounds.BottomLength + (MoveHeight + _hoverAdjust < 0 ? -MoveHeight : _hoverAdjust), 10 * Time.fixedDeltaTime);
            }
            MovingVessel.ActionGroups.SetGroup(KSPActionGroup.RCS, false);

            _up = (MovingVessel.transform.position - FlightGlobals.currentMainBody.transform.position).normalized;

            Vector3 forward;
            if (MapView.MapIsEnabled)
            {
                forward = North();
            }
            else
            {
                forward = Vector3.ProjectOnPlane(MovingVessel.CoM - FlightCamera.fetch.mainCamera.transform.position, _up).normalized;
                if (Vector3.Dot(-_up, FlightCamera.fetch.mainCamera.transform.up) > 0)
                {
                    forward = Vector3.ProjectOnPlane(FlightCamera.fetch.mainCamera.transform.up, _up).normalized;
                }
            }

            Vector3 right = Vector3.Cross(_up, forward);

            Vector3 offsetDirection = Vector3.zero;
            bool inputting = false;

            //auto level plane
            if (GameSettings.TRANSLATE_FWD.GetKey())
            {
                Quaternion targetRot = Quaternion.LookRotation(-_up, forward);
                _startRotation = Quaternion.RotateTowards(_startRotation, targetRot, RotationSpeed * 2);
                _hasRotated = true;
            }
            else if (GameSettings.TRANSLATE_BACK.GetKey())//auto level rocket
            {
                Quaternion targetRot = Quaternion.LookRotation(forward, _up);
                _startRotation = Quaternion.RotateTowards(_startRotation, targetRot, RotationSpeed * 2);
                _hasRotated = true;
            }

            if (inputting)
            {
                _currMoveSpeed = Mathf.Clamp(Mathf.MoveTowards(_currMoveSpeed, MoveSpeed, MoveAccel * Time.fixedDeltaTime), 0, MoveSpeed);
            }
            else
            {
                _currMoveSpeed = 0;
            }

            Vector3 offset = offsetDirection.normalized * _currMoveSpeed;
            _currMoveVelocity = offset / Time.fixedDeltaTime;
            Vector3 vSrfPt = MovingVessel.CoM - (MoveHeight * _up);
            bool srfBelowWater = false;
            RaycastHit ringHit;

            bool surfaceDetected = CapsuleCast(out ringHit);
            Vector3 finalOffset = Vector3.zero;

            if (surfaceDetected)
            {
                if (FlightGlobals.getAltitudeAtPos(ringHit.point) < 0)
                {
                    srfBelowWater = true;
                }

                Vector3 rOffset = Vector3.Project(ringHit.point - vSrfPt, _up);
                Vector3 mOffset = (vSrfPt + offset) - MovingVessel.CoM;
                finalOffset = rOffset + mOffset + (MoveHeight * _up);
                MovingVessel.Translate(finalOffset);
            }

            PQS bodyPQS = MovingVessel.mainBody.pqsController;

            Vector3d geoCoords = WorldPositionToGeoCoords(MovingVessel.GetWorldPos3D() + (_currMoveVelocity * Time.fixedDeltaTime), MovingVessel.mainBody);
            double lat = geoCoords.x;
            double lng = geoCoords.y;

            Vector3d bodyUpVector = new Vector3d(1, 0, 0);
            bodyUpVector = QuaternionD.AngleAxis(lat, Vector3d.forward/*around Z axis*/) * bodyUpVector;
            bodyUpVector = QuaternionD.AngleAxis(lng, Vector3d.down/*around -Y axis*/) * bodyUpVector;

            double srfHeight = bodyPQS.GetSurfaceHeight(bodyUpVector);

            if (!surfaceDetected || srfBelowWater)
            {
                Vector3 terrainPos = MovingVessel.mainBody.position + (float)srfHeight * _up;
                Vector3 waterSrfPoint = FlightGlobals.currentMainBody.position + ((float)FlightGlobals.currentMainBody.Radius * _up);

                if (!surfaceDetected)
                {
                    MovingVessel.SetPosition(terrainPos + (MoveHeight * _up) + offset);
                }
                else
                {
                    MovingVessel.SetPosition(waterSrfPoint + (MoveHeight * _up) + offset);
                }

                //update vessel situation to splashed down:
                //MovingVessel.UpdateLandedSplashed();
            }

            //fix surface rotation
            Quaternion srfRotFix = Quaternion.FromToRotation(_startingUp, _up);
            _currRotation = srfRotFix * _startRotation;
            MovingVessel.SetRotation(_currRotation);

            if (Vector3.Angle(_startingUp, _up) > 5)
            {
                _startRotation = _currRotation;
                _startingUp = _up;
            }

            MovingVessel.SetWorldVelocity(Vector3d.zero);
            MovingVessel.angularVelocity = Vector3.zero;
            MovingVessel.angularMomentum = Vector3.zero;
        }

        public void StartMove(Vessel v)
        {
            if (!v)
            {
                Debug.Log("[Spawn OrX Tank] SpawnVessel tried to move a null vessel ");
            }

            if (v.packed)
            {
                return;
            }

            if (!_placingVessels.Contains(v))
            {
                Debug.Log("[Spawn OrX Tank] SpawnVessel trying to set altitude for " + v.vesselName);

                MovingVessel = v;
                IsMovingVessel = true;

                _up = (v.transform.position - v.mainBody.transform.position).normalized;
                _startingUp = _up;

                _vBounds = new VesselBounds(MovingVessel);
                _moving = true;
                MoveHeight = _vBounds.BottomLength + 0.5f;

                _startRotation = MovingVessel.transform.rotation;
                _currRotation = _startRotation;

            }
        }

        public void DropMove()
        {
            StartCoroutine(DropMoveRoutine(_vBounds));
            IsMovingVessel = false;
            _debugLr.enabled = false;
        }

        private IEnumerator EndMoveRoutine(VesselBounds vesselBounds)
        {
            Vessel v = vesselBounds.vessel;
            if (!v) yield break;

            yield return new WaitForFixedUpdate();
            vesselBounds.UpdateBounds();

            yield return new WaitForFixedUpdate();
            /*
            while (_moveMode != MoveModes.Normal)
            {
                ToggleMoveMode();
            }
            */
            _moving = false;
            MoveHeight = 0;
            _placingVessels.Add(vesselBounds.vessel);
            float bottomLength = _vBounds.BottomLength;

            //float heightOffset = GetRadarAltitude(movingVessel) - moveHeight;

            float altitude = GetRaycastAltitude(vesselBounds);

            while (v && !v.LandedOrSplashed)
            {
                v.IgnoreGForces(240);
                MovingVessel.IgnoreGForces(240);

                _up = (v.transform.position - FlightGlobals.currentMainBody.transform.position).normalized;
                float placeSpeed = Mathf.Clamp(((altitude - bottomLength) * 2), 0.1f, maxPlacementSpeed);
                if (placeSpeed > 3)
                {
                    v.SetWorldVelocity(Vector3.zero);
                    MovingVessel.angularVelocity = Vector3.zero;
                    MovingVessel.angularMomentum = Vector3.zero;
                    v.Translate(placeSpeed * Time.fixedDeltaTime * -_up);
                }
                else
                {
                    v.SetWorldVelocity(placeSpeed * -_up);
                }
                altitude -= placeSpeed * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _placingVessels.Remove(v);
            _hoverAdjust = 0f;
        }

        private IEnumerator DropMoveRoutine(VesselBounds vesselBounds)
        {
            Vessel v = vesselBounds.vessel;
            if (!v) yield break;

            _moving = false;
            MoveHeight = 0;
            _hoverAdjust = 0f;
            v.UpdateLandedSplashed();
            _placingVessels.Remove(v);
        }
/*
        private void UpdateDebugLines()
        {
            int circleRes = 24;

            Vector3[] positions = new Vector3[circleRes + 3];
            for (int i = 0; i < circleRes; i++)
            {
                positions[i] = GetBoundPoint(i, circleRes, 1);
            }
            positions[circleRes] = GetBoundPoint(0, circleRes, 1);
            positions[circleRes + 1] = MovingVessel.CoM;
            positions[circleRes + 2] = MovingVessel.CoM + (MoveHeight * -_up);

            _debugLr.positionCount = circleRes + 3;
            _debugLr.SetPositions(positions);
        }
*/
        private Vector3 GetBoundPoint(int index, int totalPoints, float radiusFactor)
        {
            float angleIncrement = 360 / (float)totalPoints;

            float angle = index * angleIncrement;

            Vector3 forward = North();//Vector3.ProjectOnPlane((movingVessel.CoM)-FlightCamera.fetch.mainCamera.transform.position, up).normalized;

            float radius = _vBounds.Radius;

            Vector3 offsetVector = (radius * radiusFactor * forward);
            offsetVector = Quaternion.AngleAxis(angle, _up) * offsetVector;

            Vector3 point = MovingVessel.CoM + offsetVector;

            return point;
        }

        private bool CapsuleCast(out RaycastHit rayHit)
        {
            //float radius = (Mathf.Max (Mathf.Max(vesselBounds.size.x, vesselBounds.size.y), vesselBounds.size.z)) + (currMoveSpeed*2);
            float radius = _vBounds.Radius + Mathf.Clamp(_currMoveSpeed, 0, 200);

            return Physics.CapsuleCast(MovingVessel.CoM + (250 * _up), MovingVessel.CoM + (249 * _up), radius, -_up, out rayHit, 2000, 1 << 15);
        }

        private float GetRadarAltitude(Vessel vessel)
        {
            //Not needed anymore - RadarAlt is part of vessel now 
            float radarAlt = Mathf.Clamp((float)(vessel.mainBody.GetAltitude(vessel.CoM) - vessel.terrainAltitude), 0, (float)vessel.altitude);
            return radarAlt;
        }

        private float GetRaycastAltitude(VesselBounds vesselBounds) //TODO do the raycast from the bottom point of the ship, and include vessels in layer mask, so you can safely place on top of vessel
        {
            RaycastHit hit;

            //test
            if (Physics.Raycast(vesselBounds.vessel.CoM - (vesselBounds.BottomLength * _up), -_up, out hit, (float)vesselBounds.vessel.altitude, (1 << 15) | (1 << 0)))
            {
                return Vector3.Project(hit.point - vesselBounds.vessel.CoM, _up).magnitude;
            }

            /*
                  if(Physics.Raycast(vesselBounds.vessel.CoM, -up, out hit, (float)vesselBounds.vessel.altitude, (1<<15)))
                  {
                      return hit.distance;
                  }*/

            else
            {
                //return GetRadarAltitude(vesselBounds.vessel);
                return (float)vesselBounds.vessel.radarAltitude;
            }
        }

        private Vector3 GetRaycastPosition(VesselBounds vesselBounds)
        {
            Vector3 ZeroVector = new Vector3(0, 0, 0);
            RaycastHit hit;
            if (Physics.Raycast(vesselBounds.vessel.CoM - (vesselBounds.BottomLength * _up), -_up, out hit, (float)vesselBounds.vessel.altitude, (1 << 15) | (1 << 0)))
            {
                return Vector3.Project(hit.point - vesselBounds.vessel.CoM, _up);
            }
            else
            {
                return ZeroVector;
            }
        }
        /*
                private void ToggleMoveMode()
                {
                    _moveMode = (MoveModes)(int)Mathf.Repeat((float)_moveMode + 1, 4);
        //            ShowModeMessage();

                    switch (_moveMode)
                    {
                        case MoveModes.Normal:
                            _debugLr.material.SetColor("_EmissiveColor", Color.green);
                            break;
                        case MoveModes.Slow:
                            _debugLr.material.SetColor("_EmissiveColor", XKCDColors.Orange);
                            break;
                        case MoveModes.Fine:
                            _debugLr.material.SetColor("_EmissiveColor", XKCDColors.BrightRed);
                            break;
                        case MoveModes.Ludicrous:
                            _debugLr.material.SetColor("_EmissiveColor", XKCDColors.PurpleishBlue);
                            break;
                    }
                }

                private void ShowModeMessage()
                {
                    if (_moveMessage != null)
                    {
                        ScreenMessages.RemoveMessage(_moveMessage);
                    }
                    _moveMessage = ScreenMessages.PostScreenMessage("Mode : " + _moveMode.ToString(), 3, ScreenMessageStyle.UPPER_CENTER);
                }
                */
        private Vector3 North()
        {
            Vector3 n = MovingVessel.mainBody.GetWorldSurfacePosition(MovingVessel.latitude + 1, MovingVessel.longitude, MovingVessel.altitude) - MovingVessel.GetWorldPos3D();
            n = Vector3.ProjectOnPlane(n, _up);
            return n.normalized;
        }

        public struct VesselBounds
        {
            public Vessel vessel;
            public float BottomLength;
            public float Radius;

            private Vector3 _localBottomPoint;
            public Vector3 BottomPoint
            {
                get
                {
                    return vessel.transform.TransformPoint(_localBottomPoint);
                }
            }

            public VesselBounds(Vessel v)
            {
                vessel = v;
                BottomLength = 0;
                Radius = 0;
                _localBottomPoint = Vector3.zero;
                UpdateBounds();
            }

            public void UpdateBounds()
            {
                Vector3 up = (vessel.CoM - vessel.mainBody.transform.position).normalized;
                Vector3 forward = Vector3.ProjectOnPlane(vessel.CoM - FlightCamera.fetch.mainCamera.transform.position, up).normalized;
                Vector3 right = Vector3.Cross(up, forward);

                float maxSqrDist = 0;
                Part furthestPart = null;

                //bottom check
                Vector3 downPoint = vessel.CoM - (2000 * up);
                Vector3 closestVert = vessel.CoM;
                float closestSqrDist = Mathf.Infinity;

                //radius check
                Vector3 furthestVert = vessel.CoM;
                float furthestSqrDist = -1;

                foreach (Part p in vessel.parts)
                {
                    if (p.Modules.Contains("Tailhook")) return;
                    if (p.Modules.Contains("Arrestwire")) return;
                    if (p.Modules.Contains("Catapult")) return;
                    if (p.Modules.Contains("CLLSalt")) return;
                    if (p.Modules.Contains("OLSalt")) return;

                    float sqrDist = Vector3.ProjectOnPlane((p.transform.position - vessel.CoM), up).sqrMagnitude;
                    if (sqrDist > maxSqrDist)
                    {
                        maxSqrDist = sqrDist;
                        furthestPart = p;
                    }

                    //if(Vector3.Dot(up, p.transform.position-vessel.CoM) < 0)
                    //{

                    foreach (MeshFilter mf in p.GetComponentsInChildren<MeshFilter>())
                    {
                        //Mesh mesh = mf.mesh;
                        foreach (Vector3 vert in mf.mesh.vertices)
                        {
                            //bottom check
                            Vector3 worldVertPoint = mf.transform.TransformPoint(vert);
                            float bSqrDist = (downPoint - worldVertPoint).sqrMagnitude;
                            if (bSqrDist < closestSqrDist)
                            {
                                closestSqrDist = bSqrDist;
                                closestVert = worldVertPoint;
                            }

                            //radius check
                            //float sqrDist = (vessel.CoM-worldVertPoint).sqrMagnitude;
                            float hSqrDist = Vector3.ProjectOnPlane(vessel.CoM - worldVertPoint, up).sqrMagnitude;
                            if (!(hSqrDist > furthestSqrDist)) continue;
                            furthestSqrDist = hSqrDist;
                            furthestVert = worldVertPoint;
                        }
                    }

                    //}
                }

                Vector3 radVector = Vector3.ProjectOnPlane(furthestVert - vessel.CoM, up);
                Radius = radVector.magnitude;

                BottomLength = Vector3.Project(closestVert - vessel.CoM, up).magnitude;
                _localBottomPoint = vessel.transform.InverseTransformPoint(closestVert);

                //Debug.Log ("Vessel bottom length: "+bottomLength);
                /*
                        if(furthestPart!=null)
                        {
                            Debug.Log ("Furthest Part: "+furthestPart.partInfo.title);

                            Vector3 furthestVert = vessel.CoM;
                            float furthestSqrDist = -1;

                            foreach(var mf in furthestPart.GetComponentsInChildren<MeshFilter>())
                            {
                                Mesh mesh = mf.mesh;
                                foreach(var vert in mesh.vertices)
                                {
                                    Vector3 worldVertPoint = mf.transform.TransformPoint(vert);
                                    float sqrDist = (vessel.CoM-worldVertPoint).sqrMagnitude;
                                    if(sqrDist > furthestSqrDist)
                                    {
                                        furthestSqrDist = sqrDist;
                                        furthestVert = worldVertPoint;
                                    }
                                }
                            }

                            Vector3 radVector = Vector3.ProjectOnPlane(furthestVert-vessel.CoM, up);
                            radius = radVector.magnitude;
                            Debug.Log ("Vert test found radius to be "+radius);
                        }
                        */
                //radius *= 1.75f;
                //radius += 5;//15;
                Radius += Mathf.Clamp(Radius, 2, 10);
            }


        }

        public static List<string> partIgnoreModules = new List<string>(9)
        {
            "Tailhook",
            "Arrestwire",
            "Catapult",
            "CLLSalt",
            "OLSalt"
        };

        private static bool IsPartModuleIgnored(string ModuleName)
        {
            return true;
        }




        #endregion


        #region Data
        internal class CrewData
        {
            public string name = null;
            public ProtoCrewMember.Gender? gender = null;
            public bool addToRoster = true;

            public CrewData() { }
            public CrewData(CrewData cd)
            {
                name = cd.name;
                gender = cd.gender;
                addToRoster = cd.addToRoster;
            }
        }

        private class TankVesselData
        {
            public string name = null;
            public Guid? id = null;
            public string craftURL = null;
            public AvailablePart craftPart = null;
            public string flagURL = null;
            public VesselType vesselType = VesselType.Ship;
            public CelestialBody body = null;
            public Orbit orbit = null;
            public double latitude = 0.0;
            public double longitude = 0.0;
            public double? altitude = null;
            public float height = 0.0f;
            public bool orbiting = false;
            public bool owned = false;
            public List<CrewData> crew = new List<CrewData>();
            public PQSCity pqsCity = null;
            public Vector3d pqsOffset;
            public float heading;
            public float pitch;
            public float roll;

            public TankVesselData() { }
            public TankVesselData(TankVesselData vd)
            {
                name = vd.name;
                id = vd.id;
                craftURL = vd.craftURL;
                craftPart = vd.craftPart;
                flagURL = vd.flagURL;
                vesselType = vd.vesselType;
                body = vd.body;
                orbit = vd.orbit;
                latitude = vd.latitude;
                longitude = vd.longitude;
                altitude = vd.altitude;
                height = vd.height;
                orbiting = vd.orbiting;
                owned = vd.owned;
                pqsCity = vd.pqsCity;
                pqsOffset = vd.pqsOffset;
                heading = vd.heading;
                pitch = vd.pitch;
                roll = vd.roll;

                foreach (CrewData cd in vd.crew)
                {
                    crew.Add(new CrewData(cd));
                }
            }
        }
        #endregion
    }

}


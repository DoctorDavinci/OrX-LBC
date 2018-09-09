using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSP.UI;
using UnityEngine;
using KSP.UI.Screens;
using System.IO;
using System.Reflection;
using OrXBDAc.missions;

namespace OrXBDAc.spawn
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SpawnOrX_Fighter : MonoBehaviour
    {


        ////////////////////////////

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static SpawnOrX_Fighter instance;

        internal static List<ProtoCrewMember> SelectedCrewData;

        private string orxfighter = string.Empty;
        private string flagURL = string.Empty;
        public bool delay = false;
        private float spawnTimer = 0.0f;

        private double _lat_ = 0.0f;
        private double _lon_ = 0.0f;
        private bool checking = false;
        private double altitude = 0;

        private void Awake()
        {
            var _orxfighter = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            flagURL = _orxfighter + "\\OrX_icon.png";
            orxfighter = _orxfighter + "\\PluginData" + "\\VesselData" + "\\OrX" + "\\OrXFighter01.craft";

            if (instance) Destroy(instance);
            instance = this;
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.radarAltitude >= 1000)
                {
                    if (delay && OrX_Log.instance.ironKerbal)
                    {
                        delay = false;
                        StartFighterSpawn();
                    }
                }
            }
        }


        private bool loadingCraft = false;

        IEnumerator ResetDelay()
        {
            yield return new WaitForSeconds(30);

            StartFighterSpawn();
        }


        public void StartFighterSpawn()
        {
            if (OrX_Log.instance.modeReset)
            {
                StartCoroutine(ResetDelay());
            }
            else
            {
                if (OrX_Log.instance.ironKerbal)
                {
                    if (KerbinMissions.instance.level <= 3)
                    {
                        RandomizeGPS();
                    }
                    else
                    {
                        if (KerbinMissions.instance.level >= 2 && KerbinMissions.instance.level <= 5)
                        {
                            RandomizeGPS();
                        }
                        else
                        {
                            if (KerbinMissions.instance.level >= 5 && KerbinMissions.instance.level <= 11)
                            {
                                RandomizeGPS();
                            }
                            else
                            {
                                if (KerbinMissions.instance.level >= 10)
                                {
                                    RandomizeGPS();
                                }
                            }
                        }
                    }
                }
            }
        }


        public void RandomizeGPS()
        {

            if (FlightGlobals.ActiveVessel.radarAltitude <= 3000)
            {
                altitude = FlightGlobals.ActiveVessel.radarAltitude + 3000;
            }
            else
            {
                altitude = FlightGlobals.ActiveVessel.radarAltitude + 1000;
            }

            var previousLat = _lat_;
            var previousLon = _lon_;

            _lat_ = 0;
            _lon_ = 0;

            double _random = new System.Random().Next(1, 4);
            double _random2 = new System.Random().Next(0, 4);
            double _random3 = _random + _random2;

            Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ......");

            if (_random3 <= 3)
            {
                double lat = (_random * 4) / 100;
                double lon = (_random2 * 4) / 100;

                if (lat <= 0.07f)
                {
                    lat = 0.07f;
                }

                if (lon <= 0.07f)
                {
                    lon = 0.07f;
                }


                _lat_ += lat;
                _lon_ += lon;
                Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lat_: " + _lat_);
                Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lon_: " + _lon_);

            }
            else
            {
                if (_random3 <= 5)
                {
                    double lat = (_random * 4) / -100;
                    double lon = (_random2 * 4) / 100;

                    if (lat >= -0.07f)
                    {
                        lat = -0.07f;
                    }

                    if (lon <= 0.07f)
                    {
                        lon = 0.07f;
                    }

                    _lat_ += lat;
                    _lon_ += lon;
                    Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lat_: " + _lat_);
                    Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lon_: " + _lon_);

                }
                else
                {
                    if (_random3 <= 7)
                    {
                        double lat = (_random * 4) / 100;
                        double lon = (_random2 * 4) / -100;

                        if (lat <= 0.07f)
                        {
                            lat = 0.07f;
                        }

                        if (lon >= -0.07f)
                        {
                            lon = -0.07f;
                        }

                        _lat_ += lat;
                        _lon_ += lon;
                        Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lat_: " + _lat_);
                        Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lon_: " + _lon_);

                    }
                    else
                    {
                        double lat = (_random * 4) / -100;
                        double lon = (_random2 * 4) / -100;

                        if (lat >= -0.07f)
                        {
                            lat = -0.07f;
                        }

                        if (lon >= -0.07f)
                        {
                            lon = -0.07f;
                        }

                        _lat_ += lat;
                        _lon_ += lon;
                        Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lat_: " + _lat_);
                        Debug.Log("[Spawn OrX Fighter] Starting RandomizeGPS ...... _lon_: " + _lon_);
                    }
                }
            }

            if (previousLat == _lat_ && previousLon == _lon_)
            {
                _lat_ = previousLon + (_random * 0.001);
            }
            else
            {
                if (previousLon == _lon_)
                {
                    _lon_ = previousLat + (_random * 0.001);
                }
            }

            StartCoroutine(DelayRoutine());
        }

        IEnumerator DelayRoutine()
        {
            delay = false;
            spawnTimer = new System.Random().Next(20, 30);
            yield return new WaitForSeconds(spawnTimer);
            OrX_Log.instance.sound_SpawnOrXHole.Play();
            ScreenMsg("<color=#cc4500ff><b>Something isn't right .................</b></color>");
            CheckSpawnTimer();
        }

        public void CheckSpawnTimer()
        {
            Debug.Log("[Spawn OrX Fighter] Starting SpawnTimerRoutine ......");
            loadingCraft = false;
            StartCoroutine(SpawnTimerRoutine());
        }


        IEnumerator SpawnTimerRoutine()
        {
            yield return new WaitForSeconds(5);

            if (!FlightGlobals.ActiveVessel.Landed)
            {
                Debug.Log("[Spawn OrX Fighter] Starting StartVesselSpawn ......");

                StartCoroutine(SpawnCraftRoutine(orxfighter));
            }
            else
            {
                CheckSpawnTimer();
            }
        }

        public Vector3d SpawnCoords;

        public bool startup = true;

        private IEnumerator SpawnCraftRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {
            Debug.Log("[Spawn OrX Fighter] Seeking Player Vessel ......");

            Vector3 worldPos = FlightGlobals.ActiveVessel.GetWorldPos3D();

            Debug.Log("[Spawn OrX Fighter] Found " + FlightGlobals.ActiveVessel.vesselName + " ..............");

            Vector3 gpsPos = WorldPositionToGeoCoords(worldPos, FlightGlobals.currentMainBody);
            spawnTimer = new System.Random().Next(15, 30);
            yield return new WaitForSeconds(spawnTimer);

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
            Debug.Log("[Spawn OrX Fighter] Lat: " + lat + " - Lon:" + longi + " - Alt: " + alt);
            return new Vector3d(lat, longi, alt);
        }


        private void SpawnVesselFromCraftFile(string craftURL, Vector3d gpsCoords, float heading, float pitch, List<ProtoCrewMember> crewData = null)
        {

            OrXorxfighterVesselData newData = new OrXorxfighterVesselData();

            newData.craftURL = craftURL;
            newData.latitude = gpsCoords.x + _lat_;
            newData.longitude = gpsCoords.y + _lon_;
            newData.altitude = gpsCoords.z + altitude;

            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile gpsCoords.x: " + gpsCoords.x);
            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile gpsCoords.y: " + gpsCoords.y);

            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile _lat_: " + _lat_);
            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile _lon_: " + _lon_);

            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile newData.latitude: " + newData.latitude);
            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile newData.longitude: " + newData.longitude);

            Debug.Log("[Spawn OrX Fighter] SpawnVesselFromCraftFile newData.altitude: " + newData.altitude);

            newData.body = FlightGlobals.currentMainBody;
            newData.heading = heading;
            newData.pitch = pitch;
            newData.orbiting = false;

            newData.flagURL = flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.Plane;

            SpawnVessel(newData, crewData);
        }

        private void SpawnVessel(OrXorxfighterVesselData OrXorxfighterVesselData, List<ProtoCrewMember> crewData = null)
        {
            //      string gameDataDir = KSPUtil.ApplicationRootPath;
            Debug.Log("[Spawn OrX Fighter] Spawning " + OrXorxfighterVesselData.name);

            // Set additional info for landed vessels
            bool landed = false;
            if (!landed)
            {
                landed = true;
                if (OrXorxfighterVesselData.altitude == null || OrXorxfighterVesselData.altitude < 0)
                {
                    OrXorxfighterVesselData.altitude = 1;//LocationUtil.TerrainHeight(OrXorxfighterVesselData.latitude, OrXorxfighterVesselData.longitude, OrXorxfighterVesselData.body);
                }
                Debug.Log("[Spawn OrX Fighter] SpawnVessel Altitude: " + OrXorxfighterVesselData.altitude);

                //Vector3d pos = OrXorxfighterVesselData.body.GetWorldSurfacePosition(OrXorxfighterVesselData.latitude, OrXorxfighterVesselData.longitude, OrXorxfighterVesselData.altitude.Value);
                Vector3d pos = OrXorxfighterVesselData.body.GetRelSurfacePosition(OrXorxfighterVesselData.latitude, OrXorxfighterVesselData.longitude, OrXorxfighterVesselData.altitude.Value);

                OrXorxfighterVesselData.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, OrXorxfighterVesselData.body);
                OrXorxfighterVesselData.orbit.UpdateFromStateVectors(pos, OrXorxfighterVesselData.body.getRFrmVel(pos), OrXorxfighterVesselData.body, Planetarium.GetUniversalTime());
            }

            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            bool hasClamp = false;
            float lcHeight = 0;
            ConfigNode craftNode;
            Quaternion craftRotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(OrXorxfighterVesselData.craftURL))
            {
                // Save the current ShipConstruction ship, otherwise the player will see the spawned ship next time they enter the VAB!
                ConfigNode currentShip = ShipConstruction.ShipConfig;

                shipConstruct = ShipConstruction.LoadShip(OrXorxfighterVesselData.craftURL);
                if (shipConstruct == null)
                {
                    Debug.Log("[Spawn OrX Fighter] ShipConstruct was null when tried to load '" + OrXorxfighterVesselData.craftURL +
                      "' (usually this means the file could not be found).");
                    return;//continue;
                }

                craftNode = ConfigNode.Load(OrXorxfighterVesselData.craftURL);
                lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
                craftRotation = ConfigNode.ParseQuaternion(craftNode.GetNode("PART").GetValue("rot"));

                // Restore ShipConstruction ship
                ShipConstruction.ShipConfig = currentShip;

                // Set the name
                if (string.IsNullOrEmpty(OrXorxfighterVesselData.name))
                {
                    OrXorxfighterVesselData.name = "OrX Fighter";
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

                    // Had some issues with this being set to -1 for some ships - can't figure out
                    // why.  End result is the vessel exploding, so let's just set it to a positive
                    // value.
                    p.temperature = 1.0;
                }

                // Create a dummy ProtoVessel, we will use this to dump the parts to a config node.
                // We can't use the config nodes from the .craft file, because they are in a
                // slightly different format than those required for a ProtoVessel (seriously
                // Squad?!?).
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Create crew member array
                ProtoCrewMember[] crewArray = new ProtoCrewMember[OrXorxfighterVesselData.crew.Count];


                int i = 0;
                foreach (CrewData cd in OrXorxfighterVesselData.crew)
                {

                    // Create the ProtoCrewMember
                    ProtoCrewMember crewMember = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
                    if (cd.name != null)
                    {
                        crewMember.KerbalRef.name = cd.name;
                    }

                    crewArray[i++] = crewMember;

                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Create part nodes
                uint flightId = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                partNodes = new ConfigNode[1];
                partNodes[0] = ProtoVessel.CreatePartNode(OrXorxfighterVesselData.craftPart.name, flightId, crewArray);

                // Default the size class
                //sizeClass = UntrackedObjectClass.A;

                // Set the name
                if (string.IsNullOrEmpty(OrXorxfighterVesselData.name))
                {
                    OrXorxfighterVesselData.name = OrXorxfighterVesselData.craftPart.name;
                }
            }

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            //DiscoveryLevels discoveryLevel = OrXorxfighterVesselData.owned ? DiscoveryLevels.Owned : DiscoveryLevels.Unowned;
            //additionalNodes[0] = ProtoVessel.CreateDiscoveryNode(discoveryLevel, sizeClass, contract.TimeDeadline, contract.TimeDeadline);

            // Create the config node representation of the ProtoVessel
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(OrXorxfighterVesselData.name, OrXorxfighterVesselData.vesselType, OrXorxfighterVesselData.orbit, 0, partNodes, additionalNodes);

            // Additional seetings for a landed vessel
            if (!OrXorxfighterVesselData.orbiting)
            {
                Vector3d norm = OrXorxfighterVesselData.body.GetRelSurfaceNVector(OrXorxfighterVesselData.latitude, OrXorxfighterVesselData.longitude);

                double terrainHeight = 0.0;
                if (OrXorxfighterVesselData.body.pqsController != null)
                {
                    terrainHeight = OrXorxfighterVesselData.body.pqsController.GetSurfaceHeight(norm) - OrXorxfighterVesselData.body.pqsController.radius;
                }
                bool splashed = false;// = landed && terrainHeight < 0.001;

                // Create the config node representation of the ProtoVessel
                // Note - flying is experimental, and so far doesn't worx
                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : landed ?
                  Vessel.Situations.LANDED : Vessel.Situations.FLYING).ToString());
                protoVesselNode.SetValue("landed", (landed && !splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", OrXorxfighterVesselData.latitude.ToString());
                protoVesselNode.SetValue("lon", OrXorxfighterVesselData.longitude.ToString());
                protoVesselNode.SetValue("alt", OrXorxfighterVesselData.altitude.ToString());
                protoVesselNode.SetValue("landedAt", OrXorxfighterVesselData.body.name);

                // Figure out the additional height to subtract
                float lowest = float.MaxValue;
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
                    foreach (Collider collider in OrXorxfighterVesselData.craftPart.partPrefab.GetComponentsInChildren<Collider>())
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

                // Figure out the surface height and rotation
                Quaternion normal = Quaternion.LookRotation((Vector3)norm);// new Vector3((float)norm.x, (float)norm.y, (float)norm.z));
                Quaternion rotation = Quaternion.identity;
                float heading = OrXorxfighterVesselData.heading;
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


                    OrXorxfighterVesselData.heading = 0;
                    OrXorxfighterVesselData.pitch = 0;
                }

                rotation = rotation * Quaternion.AngleAxis(heading, Vector3.back);
                rotation = rotation * Quaternion.AngleAxis(OrXorxfighterVesselData.roll, Vector3.down);
                rotation = rotation * Quaternion.AngleAxis(OrXorxfighterVesselData.pitch, Vector3.left);

                // Set the height and rotation
                if (landed || splashed)
                {
                    float hgt = (shipConstruct != null ? shipConstruct.parts[0] : OrXorxfighterVesselData.craftPart.partPrefab).localRoot.attPos0.y - lowest;
                    hgt += OrXorxfighterVesselData.height + 2;
                    protoVesselNode.SetValue("hgt", hgt.ToString(), true);
                }
                protoVesselNode.SetValue("rot", KSPUtil.WriteQuaternion(normal * rotation), true);

                // Set the normal vector relative to the surface
                Vector3 nrm = (rotation * Vector3.forward);
                protoVesselNode.SetValue("nrm", nrm.x + "," + nrm.y + "," + nrm.z, true);

                protoVesselNode.SetValue("prst", false.ToString(), true);
            }

            // Add vessel to the game
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            delay = true;

            // Store the id for later use
            OrXorxfighterVesselData.id = protoVessel.vesselRef.id;

            //protoVessel.vesselRef.currentStage = 0;
            hasClamp = false;

            StartCoroutine(PlaceSpawnedVessel(protoVessel.vesselRef, !hasClamp));

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
            loadingCraft = true;
            v.isPersistent = true;
            v.Landed = false;
            v.situation = Vessel.Situations.FLYING;
            while (v.packed)
            {
                yield return null;
            }
            v.SetWorldVelocity(Vector3d.zero);

            //      yield return null;
            //      FlightGlobals.ForceSetActiveVessel(v);
            yield return null;
            v.Landed = true;
            v.situation = Vessel.Situations.PRELAUNCH;
            v.GoOffRails();
            v.IgnoreGForces(240);

            //Staging.beginFlight();
            StageManager.BeginFlight();
            /*
                  if (moveVessel)
                  {
                    MoveOrXorxfighter.Instance.StartMove(v, false);
                    MoveOrXorxfighter.Instance.MoveHeight = 2;

                    yield return null;
                    if (MoveOrXorxfighter.Instance.MovingVessel == v)
                    {
                      v.Landed = false;
                    }
                  }*/
            loadingCraft = false;
        }
        /*
            public static class LocationUtil
            {
              public static float TerrainHeight(double lat, double lon, CelestialBody body)
              {
                return 0;
              }
            }
            */

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

        private class OrXorxfighterVesselData
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
            
            public OrXorxfighterVesselData() { }
            public OrXorxfighterVesselData(OrXorxfighterVesselData vd)
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
    }

}


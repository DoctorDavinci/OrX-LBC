using System;
using System.Collections;
using System.Collections.Generic;
using OrXBDAc.parts;
using UnityEngine;
using KSP.UI.Screens;

namespace OrXBDAc.spawn
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SpawnOrX_MissionHC : MonoBehaviour
    {
        ////////////////////////////

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static SpawnOrX_MissionHC instance;

        internal static List<ProtoCrewMember> SelectedCrewData;

        public string HoloCacheName = string.Empty;

        public string craftFile = string.Empty;
        private string flagURL = string.Empty;
        private bool timer = false;
        private bool delay = true;
        private float spawnTimer = 0.0f;
        private bool loadingCraft = false;

        private double _lat_ = 0.0f;
        private double _lon_ = 0.0f;

        public double _lat = 0;
        public double _lon = 0;
        public double _alt = 0;

        public Vector3d SpawnCoords;

        public bool holo = true;
        public bool IWI = false;
        public bool ATK = false;
        public bool TKU = false;

        private string craftFileLoc = string.Empty;
        private string WaldosIslandHolo = string.Empty;
        private string KillerTomatoesHolo = string.Empty;
        private string TutenKermanUldumHolo = string.Empty;

        private void Awake()
        {
            WaldosIslandHolo = UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/VesselData/HoloCache/WaldosIsland.craft";
            KillerTomatoesHolo = UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/VesselData/HoloCache/KillerTomatoes.craft";
            TutenKermanUldumHolo = UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/VesselData/HoloCache/TutenKermanUldum.craft";

            if (instance) Destroy(instance);
            instance = this;
        }


        public Vector3d _SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)_lat, (double)_lon, (double)_alt);
        }

        public void SpawnMissionHoloCache()
        {
            if (IWI)
            {
                IWI = false;
                craftFileLoc = WaldosIslandHolo;
            }

            if (ATK)
            {
                ATK = false;
                craftFileLoc = KillerTomatoesHolo;
            }

            if (TKU)
            {
                TKU = false;
                craftFileLoc = TutenKermanUldumHolo;
            }

            Debug.Log("[Spawn OrX HoloCache] Spawning Empty HoloCache ...... " + craftFile);
            loadingCraft = false;
            timer = true;

            var randomizeLoc = new System.Random().Next(1, 100);

            if (randomizeLoc <= 26)
            {
                _lat_ = 0.001f;
                _lon_ = 0.001f;
            }
            else
            {
                if (randomizeLoc <= 51)
                {
                    _lat_ = -0.001f;
                    _lon_ = 0.001f;
                }
                else
                {
                    if (randomizeLoc <= 76)
                    {
                        _lat_ = 0.001f;
                        _lon_ = -0.001f;
                    }
                    else
                    {
                        if (randomizeLoc <= 101)
                        {
                            _lat_ = -0.001f;
                            _lon_ = -0.001;
                        }
                    }
                }
            }


            _lat = FlightGlobals.ActiveVessel.latitude + _lat_;
            _lon = FlightGlobals.ActiveVessel.longitude + _lon_;
            _alt = FlightGlobals.ActiveVessel.altitude + 2;

            //OrX_Log.instance.sound_OrXBoomstick.Play();
            StartCoroutine(SpawnEmptyHoloRoutine(craftFileLoc));

        }

        private IEnumerator SpawnEmptyHoloRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {
            loadingCraft = true;

            Vector3 worldPos = _SpawnCoords();
            Vector3 gpsPos = WorldPositionToGeoCoords(worldPos, FlightGlobals.currentMainBody);
            yield return new WaitForFixedUpdate();
            SpawnVesselFromCraftFile(craftUrl, gpsPos, 90, 0, crewData);
        }

        public void CheckSpawnTimer()
        {
            string craftFileLoc = UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName + "/" + craftFile + ".craft";

            Debug.Log("[Spawn OrX HoloCache] Spawning: " + craftFile);
            loadingCraft = false;
            timer = true;
            //OrX_Log.instance.sound_OrXBoomstick.Play();
            StartCoroutine(SpawnCraftRoutine(craftFileLoc));
        }

        private IEnumerator SpawnCraftRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {
            loadingCraft = true;

            Vector3 worldPos = _SpawnCoords();
            Vector3 gpsPos = WorldPositionToGeoCoords(worldPos, FlightGlobals.currentMainBody);
            yield return new WaitForFixedUpdate();
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
            Debug.Log("[Spawn OrX HoloCache] Lat: " + lat + " - Lon:" + longi + " - Alt: " + alt);
            return new Vector3d(lat, longi, alt);
        }

        private void SpawnVesselFromCraftFile(string craftURL, Vector3d gpsCoords, float heading, float pitch, List<ProtoCrewMember> crewData = null)
        {
            HoloCacheData newData = new HoloCacheData();

            newData.craftURL = craftURL;
            newData.latitude = gpsCoords.x + _lat_;
            newData.longitude = gpsCoords.y + _lon_;
            newData.altitude = gpsCoords.z + 5;

            Debug.Log("[Spawn OrX HoloCache] SpawnVesselFromCraftFile Altitude: " + newData.altitude);

            newData.body = FlightGlobals.currentMainBody;
            newData.heading = heading;
            newData.pitch = pitch;
            newData.orbiting = false;

            newData.flagURL = flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.Unknown;

            SpawnVessel(newData, crewData);
        }

        private void SpawnVessel(HoloCacheData HoloCacheData, List<ProtoCrewMember> crewData = null)
        {
            //      string gameDataDir = KSPUtil.ApplicationRootPath;
            Debug.Log("[Spawn OrX HoloCache] Spawning " + HoloCacheData.name);

            // Set additional info for landed vessels
            bool landed = false;
            if (!landed)
            {
                landed = true;
                if (HoloCacheData.altitude == null || HoloCacheData.altitude < 0)
                {
                    HoloCacheData.altitude = 5;//LocationUtil.TerrainHeight(HoloCacheData.latitude, HoloCacheData.longitude, HoloCacheData.body);
                }
                Debug.Log("[Spawn OrX HoloCache] SpawnVessel Altitude: " + HoloCacheData.altitude);

                //Vector3d pos = HoloCacheData.body.GetWorldSurfacePosition(HoloCacheData.latitude, HoloCacheData.longitude, HoloCacheData.altitude.Value);
                Vector3d pos = HoloCacheData.body.GetRelSurfacePosition(HoloCacheData.latitude, HoloCacheData.longitude, HoloCacheData.altitude.Value);

                HoloCacheData.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, HoloCacheData.body);
                HoloCacheData.orbit.UpdateFromStateVectors(pos, HoloCacheData.body.getRFrmVel(pos), HoloCacheData.body, Planetarium.GetUniversalTime());
            }

            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            bool hasClamp = false;
            float lcHeight = 0;
            ConfigNode craftNode;
            Quaternion craftRotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(HoloCacheData.craftURL))
            {
                // Save the current ShipConstruction ship, otherwise the player will see the spawned ship next time they enter the VAB!
                ConfigNode currentShip = ShipConstruction.ShipConfig;

                shipConstruct = ShipConstruction.LoadShip(HoloCacheData.craftURL);
                if (shipConstruct == null)
                {
                    Debug.Log("[Spawn OrX HoloCache] ShipConstruct was null when tried to load '" + HoloCacheData.craftURL +
                      "' (usually this means the file could not be found).");
                    return;//continue;
                }

                craftNode = ConfigNode.Load(HoloCacheData.craftURL);
                lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
                craftRotation = ConfigNode.ParseQuaternion(craftNode.GetNode("PART").GetValue("rot"));

                // Restore ShipConstruction ship
                ShipConstruction.ShipConfig = currentShip;

                // Set the name
                if (string.IsNullOrEmpty(HoloCacheData.name))
                {
                    HoloCacheData.name = craftFile;
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

                // Create crew member array
                ProtoCrewMember[] crewArray = new ProtoCrewMember[HoloCacheData.crew.Count];
                /*
                        int i = 0;
                        foreach (CrewData cd in HoloCacheData.crew)
                        {
                /*
                          // Create the ProtoCrewMember
                          ProtoCrewMember crewMember = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
                          if (cd.name != null)
                          {
                            crewMember.KerbalRef.name = cd.name;
                          }

                          crewArray[i++] = crewMember;

                        }
                */
                // Create part nodes
                uint flightId = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                partNodes = new ConfigNode[1];
                partNodes[0] = ProtoVessel.CreatePartNode(HoloCacheData.craftPart.name, flightId, crewArray);

                // Default the size class
                //sizeClass = UntrackedObjectClass.A;

                // Set the name
                if (string.IsNullOrEmpty(HoloCacheData.name))
                {
                    HoloCacheData.name = HoloCacheData.craftPart.name;
                }
            }

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            //DiscoveryLevels discoveryLevel = HoloCacheData.owned ? DiscoveryLevels.Owned : DiscoveryLevels.Unowned;
            //additionalNodes[0] = ProtoVessel.CreateDiscoveryNode(discoveryLevel, sizeClass, contract.TimeDeadline, contract.TimeDeadline);

            // Create the config node representation of the ProtoVessel
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(HoloCacheData.name, HoloCacheData.vesselType, HoloCacheData.orbit, 0, partNodes, additionalNodes);

            // Additional seetings for a landed vessel
            if (!HoloCacheData.orbiting)
            {
                Vector3d norm = HoloCacheData.body.GetRelSurfaceNVector(HoloCacheData.latitude, HoloCacheData.longitude);

                double terrainHeight = 0.0;
                if (HoloCacheData.body.pqsController != null)
                {
                    terrainHeight = HoloCacheData.body.pqsController.GetSurfaceHeight(norm) - HoloCacheData.body.pqsController.radius;
                }
                bool splashed = false;// = landed && terrainHeight < 0.001;

                // Create the config node representation of the ProtoVessel
                // Note - flying is experimental, and so far doesn't worx
                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : landed ?
                  Vessel.Situations.LANDED : Vessel.Situations.FLYING).ToString());
                protoVesselNode.SetValue("landed", (landed && !splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", HoloCacheData.latitude.ToString());
                protoVesselNode.SetValue("lon", HoloCacheData.longitude.ToString());
                protoVesselNode.SetValue("alt", HoloCacheData.altitude.ToString());
                protoVesselNode.SetValue("landedAt", HoloCacheData.body.name);

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
                    foreach (Collider collider in HoloCacheData.craftPart.partPrefab.GetComponentsInChildren<Collider>())
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
                float heading = HoloCacheData.heading;
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


                    HoloCacheData.heading = 0;
                    HoloCacheData.pitch = 0;
                }

                rotation = rotation * Quaternion.AngleAxis(heading, Vector3.back);
                rotation = rotation * Quaternion.AngleAxis(HoloCacheData.roll, Vector3.down);
                rotation = rotation * Quaternion.AngleAxis(HoloCacheData.pitch, Vector3.left);

                // Set the height and rotation
                if (landed || splashed)
                {
                    float hgt = (shipConstruct != null ? shipConstruct.parts[0] : HoloCacheData.craftPart.partPrefab).localRoot.attPos0.y - lowest;
                    hgt += HoloCacheData.height;
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
            //protoVessel.vesselRef.transform.rotation = protoVessel.rotation;


            // Store the id for later use
            HoloCacheData.id = protoVessel.vesselRef.id;

            //protoVessel.vesselRef.currentStage = 0;
            hasClamp = false;

            StartCoroutine(PlaceSpawnedVessel(protoVessel.vesselRef, !hasClamp));

            // Associate it so that it can be used in contract parameters
            //ContractVesselTracker.Instance.AssociateVessel(HoloCacheData.name, protoVessel.vesselRef);


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
            v.situation = Vessel.Situations.LANDED;
            v.GoOffRails();
            v.IgnoreGForces(240);
            var hc = v.FindPartModuleImplementing<ModuleOrXMissionHolo>();
            if (hc != null)
            {
                hc.powerDown = false;

                if (!hc.getGPS)
                {
                    hc.spawned = true;
                }

                if (hc.sth)
                {
                    hc.spawned = true;
                    hc.spawn = true;
                }
            }
            else
            {
                v.rootPart.AddModule("ModuleOrXEnemySetup", true);
            }
            StageManager.BeginFlight();
            loadingCraft = false;
            holo = false;
        }

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

        private class HoloCacheData
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

            public HoloCacheData() { }
            public HoloCacheData(HoloCacheData vd)
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


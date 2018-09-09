using System;
using System.Collections;
using System.Collections.Generic;
using OrXBDAc.missions;
using UnityEngine;
using KSP.UI.Screens;
using System.IO;
using System.Reflection;

namespace OrXBDAc.spawn
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SpawnOrX_DCKFT : MonoBehaviour
    {
        ////////////////////////////

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static SpawnOrX_DCKFT instance;

        internal static List<ProtoCrewMember> SelectedCrewData;

        private string OrXLootBox = string.Empty;
        private string flagURL = string.Empty;
        private bool timer = false;
        private bool delay = true;
        private float spawnTimer = 0.0f;

        public Vector3d SpawnCoords;

        private void Start()
        {
            if (instance)
                Destroy(instance);

            instance = this;
        }

        private string DCKFTVesselName = string.Empty;

        private string OrX_DCKFT01 = string.Empty;
        private string OrX_DCKFT02 = string.Empty;
        private string OrX_DCKFT03 = string.Empty;
        private string OrX_DCKFT04 = string.Empty;
        private string OrX_DCKFT05 = string.Empty;
        private string OrX_DCKFT06 = string.Empty;
        private string OrX_DCKFT07 = string.Empty;
        private string OrX_DCKFT08 = string.Empty;
        private string OrX_DCKFT09 = string.Empty;
        private string OrX_DCKFT10 = string.Empty;

        public bool sg01 = false;
        public bool sg02 = false;
        public bool sg03 = false;
        public bool sg04 = false;
        public bool sg05 = false;
        public bool sg06 = false;
        public bool sg07 = false;
        public bool sg08 = false;
        public bool sg09 = false;
        public bool sg10 = false;

        private void Awake()
        {
            var _OrX_DCKFT01 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT01 = _OrX_DCKFT01 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT01.craft";

            var _OrX_DCKFT02 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            OrX_DCKFT02 = _OrX_DCKFT02 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT02.craft";

            var _OrX_DCKFT03 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            OrX_DCKFT03 = _OrX_DCKFT03 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT03.craft";

            var _OrX_DCKFT04 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            OrX_DCKFT04 = _OrX_DCKFT04 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT04.craft";
            var _OrX_DCKFT05 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT05 = _OrX_DCKFT05 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT05.craft";
            var _OrX_DCKFT06 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT06 = _OrX_DCKFT06 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT06.craft";
            var _OrX_DCKFT07 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT07 = _OrX_DCKFT07 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT07.craft";
            var _OrX_DCKFT08 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT08 = _OrX_DCKFT08 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT08.craft";
            var _OrX_DCKFT09 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT09 = _OrX_DCKFT09 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT09.craft";
            var _OrX_DCKFT10 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            OrX_DCKFT10 = _OrX_DCKFT10 + "\\PluginData" + "\\VesselData" + "\\SportingGoodsDepartment" + "\\DCKFT" + "OrXDCKFT10.craft";

            if (instance) Destroy(instance);
            instance = this;
        }

        private bool loadingCraft = false;

        private double _lat_ = 0.0f;
        private double _lon_ = 0.0f;

        public void CheckSpawnTimer()
        {
            if (sg01)
            {
                Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                loadingCraft = false;
                timer = true;
                DCKFTVesselName = "";
                StartCoroutine(SpawnCraftRoutine(OrX_DCKFT01));
            }
            else
            {
                if (sg02)
                {
                    Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                    loadingCraft = false;
                    timer = true;
                    DCKFTVesselName = "";
                    StartCoroutine(SpawnCraftRoutine(OrX_DCKFT02));
                }
                else
                {
                    if (sg03)
                    {
                        Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                        loadingCraft = false;
                        timer = true;
                        DCKFTVesselName = "";
                        StartCoroutine(SpawnCraftRoutine(OrX_DCKFT03));
                    }
                    else
                    {
                        if (sg04)
                        {
                            Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                            loadingCraft = false;
                            timer = true;
                            DCKFTVesselName = "";
                            StartCoroutine(SpawnCraftRoutine(OrX_DCKFT04));
                        }
                        else
                        {
                            if (sg05)
                            {
                                Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                loadingCraft = false;
                                timer = true;
                                DCKFTVesselName = "";
                                StartCoroutine(SpawnCraftRoutine(OrX_DCKFT05));
                            }
                            else
                            {
                                if (sg06)
                                {
                                    Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                    loadingCraft = false;
                                    timer = true;
                                    DCKFTVesselName = "";
                                    StartCoroutine(SpawnCraftRoutine(OrX_DCKFT06));
                                }
                                else
                                {
                                    if (sg07)
                                    {
                                        Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                        loadingCraft = false;
                                        timer = true;
                                        DCKFTVesselName = "";
                                        StartCoroutine(SpawnCraftRoutine(OrX_DCKFT07));
                                    }
                                    else
                                    {
                                        if (sg08)
                                        {
                                            Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                            loadingCraft = false;
                                            timer = true;
                                            DCKFTVesselName = "";
                                            StartCoroutine(SpawnCraftRoutine(OrX_DCKFT08));
                                        }
                                        else
                                        {
                                            if (sg09)
                                            {
                                                Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                                loadingCraft = false;
                                                timer = true;
                                                DCKFTVesselName = "";
                                                StartCoroutine(SpawnCraftRoutine(OrX_DCKFT09));
                                            }
                                            else
                                            {
                                                if (sg10)
                                                {
                                                    Debug.Log("[Spawn OrX S Mart] Spawning Vessel ......");
                                                    loadingCraft = false;
                                                    timer = true;
                                                    DCKFTVesselName = "";
                                                    StartCoroutine(SpawnCraftRoutine(OrX_DCKFT10));
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

            sg01 = false;
            sg02 = false;
            sg03 = false;
            sg04 = false;
            sg05 = false;
            sg06 = false;
            sg07 = false;
            sg08 = false;
            sg09 = false;
            sg10 = false;
        }

        private IEnumerator SpawnCraftRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {
            loadingCraft = true;

            Vector3 worldPos = FlightGlobals.ActiveVessel.GetWorldPos3D();
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
            Debug.Log("[Spawn OrX S Mart] Lat: " + lat + " - Lon:" + longi + " - Alt: " + alt);
            return new Vector3d(lat, longi, alt);
        }


        private void SpawnVesselFromCraftFile(string craftURL, Vector3d gpsCoords, float heading, float pitch, List<ProtoCrewMember> crewData = null)
        {
            LootBoxVesselData newData = new LootBoxVesselData();

            newData.craftURL = craftURL;
            newData.latitude = gpsCoords.x + 0.0006f;    //+ _lat_;
            newData.longitude = gpsCoords.y + 0.0006f;
            newData.altitude = gpsCoords.z + 5;

            Debug.Log("[Spawn OrX S Mart] SpawnVesselFromCraftFile Altitude: " + newData.altitude);

            newData.body = FlightGlobals.currentMainBody;
            newData.heading = heading;
            newData.pitch = pitch;
            newData.orbiting = false;

            newData.flagURL = flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.SpaceObject;

            SpawnVessel(newData, crewData);
        }

        private void SpawnVessel(LootBoxVesselData LootBoxVesselData, List<ProtoCrewMember> crewData = null)
        {
            //      string gameDataDir = KSPUtil.ApplicationRootPath;
            Debug.Log("[Spawn OrX S Mart] Spawning " + LootBoxVesselData.name);

            // Set additional info for landed vessels
            bool landed = false;
            if (!landed)
            {
                landed = true;
                if (LootBoxVesselData.altitude == null || LootBoxVesselData.altitude < 0)
                {
                    LootBoxVesselData.altitude = 5;//LocationUtil.TerrainHeight(LootBoxVesselData.latitude, LootBoxVesselData.longitude, LootBoxVesselData.body);
                }
                Debug.Log("[Spawn OrX S Mart] SpawnVessel Altitude: " + LootBoxVesselData.altitude);

                //Vector3d pos = LootBoxVesselData.body.GetWorldSurfacePosition(LootBoxVesselData.latitude, LootBoxVesselData.longitude, LootBoxVesselData.altitude.Value);
                Vector3d pos = LootBoxVesselData.body.GetRelSurfacePosition(LootBoxVesselData.latitude, LootBoxVesselData.longitude, LootBoxVesselData.altitude.Value);

                LootBoxVesselData.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, LootBoxVesselData.body);
                LootBoxVesselData.orbit.UpdateFromStateVectors(pos, LootBoxVesselData.body.getRFrmVel(pos), LootBoxVesselData.body, Planetarium.GetUniversalTime());
            }

            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            bool hasClamp = false;
            float lcHeight = 0;
            ConfigNode craftNode;
            Quaternion craftRotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(LootBoxVesselData.craftURL))
            {
                // Save the current ShipConstruction ship, otherwise the player will see the spawned ship next time they enter the VAB!
                ConfigNode currentShip = ShipConstruction.ShipConfig;

                shipConstruct = ShipConstruction.LoadShip(LootBoxVesselData.craftURL);
                if (shipConstruct == null)
                {
                    Debug.Log("[Spawn OrX S Mart] ShipConstruct was null when tried to load '" + LootBoxVesselData.craftURL +
                      "' (usually this means the file could not be found).");
                    return;//continue;
                }

                craftNode = ConfigNode.Load(LootBoxVesselData.craftURL);
                lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
                craftRotation = ConfigNode.ParseQuaternion(craftNode.GetNode("PART").GetValue("rot"));

                // Restore ShipConstruction ship
                ShipConstruction.ShipConfig = currentShip;

                // Set the name
                if (string.IsNullOrEmpty(LootBoxVesselData.name))
                {
                    LootBoxVesselData.name = DCKFTVesselName;
                    ;
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
                ProtoCrewMember[] crewArray = new ProtoCrewMember[LootBoxVesselData.crew.Count];
                /*
                        int i = 0;
                        foreach (CrewData cd in LootBoxVesselData.crew)
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
                partNodes[0] = ProtoVessel.CreatePartNode(LootBoxVesselData.craftPart.name, flightId, crewArray);

                // Default the size class
                //sizeClass = UntrackedObjectClass.A;

                // Set the name
                if (string.IsNullOrEmpty(LootBoxVesselData.name))
                {
                    LootBoxVesselData.name = LootBoxVesselData.craftPart.name;
                }
            }

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            //DiscoveryLevels discoveryLevel = LootBoxVesselData.owned ? DiscoveryLevels.Owned : DiscoveryLevels.Unowned;
            //additionalNodes[0] = ProtoVessel.CreateDiscoveryNode(discoveryLevel, sizeClass, contract.TimeDeadline, contract.TimeDeadline);

            // Create the config node representation of the ProtoVessel
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(LootBoxVesselData.name, LootBoxVesselData.vesselType, LootBoxVesselData.orbit, 0, partNodes, additionalNodes);

            // Additional seetings for a landed vessel
            if (!LootBoxVesselData.orbiting)
            {
                Vector3d norm = LootBoxVesselData.body.GetRelSurfaceNVector(LootBoxVesselData.latitude, LootBoxVesselData.longitude);

                double terrainHeight = 0.0;
                if (LootBoxVesselData.body.pqsController != null)
                {
                    terrainHeight = LootBoxVesselData.body.pqsController.GetSurfaceHeight(norm) - LootBoxVesselData.body.pqsController.radius;
                }
                bool splashed = false;// = landed && terrainHeight < 0.001;

                // Create the config node representation of the ProtoVessel
                // Note - flying is experimental, and so far doesn't worx
                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : landed ?
                  Vessel.Situations.LANDED : Vessel.Situations.FLYING).ToString());
                protoVesselNode.SetValue("landed", (landed && !splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", LootBoxVesselData.latitude.ToString());
                protoVesselNode.SetValue("lon", LootBoxVesselData.longitude.ToString());
                protoVesselNode.SetValue("alt", LootBoxVesselData.altitude.ToString());
                protoVesselNode.SetValue("landedAt", LootBoxVesselData.body.name);

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
                    foreach (Collider collider in LootBoxVesselData.craftPart.partPrefab.GetComponentsInChildren<Collider>())
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
                float heading = LootBoxVesselData.heading;
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


                    LootBoxVesselData.heading = 0;
                    LootBoxVesselData.pitch = 0;
                }

                rotation = rotation * Quaternion.AngleAxis(heading, Vector3.back);
                rotation = rotation * Quaternion.AngleAxis(LootBoxVesselData.roll, Vector3.down);
                rotation = rotation * Quaternion.AngleAxis(LootBoxVesselData.pitch, Vector3.left);

                // Set the height and rotation
                if (landed || splashed)
                {
                    float hgt = (shipConstruct != null ? shipConstruct.parts[0] : LootBoxVesselData.craftPart.partPrefab).localRoot.attPos0.y - lowest;
                    hgt += LootBoxVesselData.height;
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
            LootBoxVesselData.id = protoVessel.vesselRef.id;

            //protoVessel.vesselRef.currentStage = 0;
            hasClamp = false;

            StartCoroutine(PlaceSpawnedVessel(protoVessel.vesselRef, !hasClamp));

            // Associate it so that it can be used in contract parameters
            //ContractVesselTracker.Instance.AssociateVessel(LootBoxVesselData.name, protoVessel.vesselRef);


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

            //Staging.beginFlight();
            StageManager.BeginFlight();
            /*
                  if (moveVessel)
                  {
                    MoveOrXHole.Instance.StartMove(v, false);
                    MoveOrXHole.Instance.MoveHeight = 2;

                    yield return null;
                    if (MoveOrXHole.Instance.MovingVessel == v)
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

        private class LootBoxVesselData
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

            public LootBoxVesselData() { }
            public LootBoxVesselData(LootBoxVesselData vd)
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


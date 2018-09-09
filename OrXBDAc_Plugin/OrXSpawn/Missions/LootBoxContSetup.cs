using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSP.UI;
using UnityEngine;
using KSP.UI.Screens;
using System.IO;
using System.Reflection;
using OrXBDAc.parts;

namespace OrXBDAc.spawn
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class LootBoxContSetup : MonoBehaviour
    {
        public static LootBoxContSetup instance;

        private string OrXMissileTurret = string.Empty;
        private string OrXCIWS = string.Empty;
        private string OrXTMTSNBN = string.Empty;
        private string flagURL = string.Empty;

        public Vector3d SpawnCoords;

        private void Start()
        {
            if (instance)
                Destroy(instance);

            instance = this;
        }

        private void Awake()
        {
            var _hole = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            flagURL = _hole + "\\OrX_icon.png";
            OrXMissileTurret = _hole + "\\PluginData" + "\\VesselData" + "\\OrX" + "\\OrXMissileTurret.craft";
            OrXCIWS = _hole + "\\PluginData" + "\\VesselData" + "\\OrX" + "\\OrXCIWS.craft";
            OrXTMTSNBN = _hole + "\\PluginData" + "\\VesselData" + "\\OrX" + "\\OrXTMTSNBN.craft";

            if (instance) Destroy(instance);
            instance = this;
        }

        private bool loadingCraft = false;

        private double _lat_ = 0.0f;
        private double _lon_ = 0.0f;

        public void CheckSpawnTimer()
        {
            Debug.Log("[Waldo's Island Setup] Spawning Waldo's Island ......");
            loadingCraft = false;

            if (count == 0)
            {
                count += 1;
                Debug.Log("[Waldo's Island Setup] Spawning OrX Fighter ......");

                SpawnOrX_Fighter.instance.delay = true;
                Debug.Log("[Waldo's Island Setup] Spawning CIWS 1 ......");

                SpawnCoords = FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)CIWSlat1, (double)CIWSlon1, (double)CIWSalt1);
                StartCoroutine(SpawnCraftRoutine(OrXCIWS));

            }
            else
            {
                if (count == 1)
                {
                    count += 1;
                    Debug.Log("[Waldo's Island Setup] Spawning CIWS 2 ......");

                    SpawnCoords = FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)CIWSlat2, (double)CIWSlon2, (double)CIWSalt2);
                    StartCoroutine(SpawnCraftRoutine(OrXCIWS));

                }
                else
                {
                    if (count == 2)
                    {
                        count += 1;
                        Debug.Log("[Waldo's Island Setup] Spawning Missile Emplacement ......");

                        SpawnCoords = FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)AIM120lat2, (double)AIM120lon2, (double)AIM120alt2);
                        StartCoroutine(SpawnCraftRoutine(OrXMissileTurret));

                    }
                    else
                    {
                        if (count == 3)
                        {
                            count += 1;
                            Debug.Log("[Waldo's Island Setup] Spawning TMTSNBN ......");

                            SpawnCoords = FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)TMTSNBNlat1, (double)TMTSNBNlon1, (double)TMTSNBNalt1);
                            StartCoroutine(SpawnCraftRoutine(OrXTMTSNBN));

                        }
                    }
                }
            }
        }

        internal static List<ProtoCrewMember> SelectedCrewData;

        public int count = 0;

        private double CIWSlat1 = -1.52048993708087;
        private double CIWSlon1 = -71.9109985778636;
        private double CIWSalt1 = 133.496322012157;

        private double CIWSlat2 = -1.52727406678969;
        private double CIWSlon2 = -71.8786227708736;
        private double CIWSalt2 = 133.552046996192;

        private double AIM120lat2 = -1.56605689728948;
        private double AIM120lon2 = -71.9195373112996;
        private double AIM120alt2 = 296.179936145898;

        private double TMTSNBNlat1 = -1.52333073539685;
        private double TMTSNBNlon1 = -71.9033109035629;
        private double TMTSNBNalt1 = 133.477209510864;

        private IEnumerator SpawnCraftRoutine(string craftUrl, List<ProtoCrewMember> crewData = null)
        {
            loadingCraft = true;

            yield return new WaitForFixedUpdate();
            Debug.Log("[Waldo's Island Setup] SpawnCraftRoutine SpawnCoords......");

            Vector3 gpsPos = WorldPositionToGeoCoords(SpawnCoords, FlightGlobals.currentMainBody);

            yield return new WaitForFixedUpdate();
            Debug.Log("[Waldo's Island Setup] SpawnCraftRoutine SpawnVesselFromCraftFile ......");

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
            Debug.Log("[Wlados Island Setup] Lat: " + lat + " - Lon:" + longi + " - Alt: " + alt);
            return new Vector3d(lat, longi, alt);
        }


        private void SpawnVesselFromCraftFile(string craftURL, Vector3d gpsCoords, float heading, float pitch, List<ProtoCrewMember> crewData = null)
        {
            WaldosIslandVesselData newData = new WaldosIslandVesselData();

            newData.craftURL = craftURL;
            newData.latitude = gpsCoords.x;
            newData.longitude = gpsCoords.y;   
            newData.altitude = gpsCoords.z + 2;

            Debug.Log("[Wlados Island Setup] SpawnVesselFromCraftFile Altitude: " + newData.altitude);

            newData.body = FlightGlobals.currentMainBody;
            newData.heading = heading;
            newData.pitch = pitch;
            newData.orbiting = false;

            newData.flagURL = flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.Unknown;

            SpawnVessel(newData, crewData);
        }

        private void SpawnVessel(WaldosIslandVesselData WaldosIslandVesselData, List<ProtoCrewMember> crewData = null)
        {
            //      string gameDataDir = KSPUtil.ApplicationRootPath;
            Debug.Log("[Wlados Island Setup] Spawning " + WaldosIslandVesselData.name);

            // Set additional info for landed vessels
            bool landed = false;
            if (!landed)
            {
                landed = true;
                if (WaldosIslandVesselData.altitude == null || WaldosIslandVesselData.altitude < 0)
                {
                    WaldosIslandVesselData.altitude = 5;//LocationUtil.TerrainHeight(WaldosIslandVesselData.latitude, WaldosIslandVesselData.longitude, WaldosIslandVesselData.body);
                }
                Debug.Log("[Wlados Island Setup] SpawnVessel Altitude: " + WaldosIslandVesselData.altitude);

                //Vector3d pos = WaldosIslandVesselData.body.GetWorldSurfacePosition(WaldosIslandVesselData.latitude, WaldosIslandVesselData.longitude, WaldosIslandVesselData.altitude.Value);
                Vector3d pos = WaldosIslandVesselData.body.GetRelSurfacePosition(WaldosIslandVesselData.latitude, WaldosIslandVesselData.longitude, WaldosIslandVesselData.altitude.Value);

                WaldosIslandVesselData.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, WaldosIslandVesselData.body);
                WaldosIslandVesselData.orbit.UpdateFromStateVectors(pos, WaldosIslandVesselData.body.getRFrmVel(pos), WaldosIslandVesselData.body, Planetarium.GetUniversalTime());
            }

            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            bool hasClamp = false;
            float lcHeight = 0;
            ConfigNode craftNode;
            Quaternion craftRotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(WaldosIslandVesselData.craftURL))
            {
                // Save the current ShipConstruction ship, otherwise the player will see the spawned ship next time they enter the VAB!
                ConfigNode currentShip = ShipConstruction.ShipConfig;

                shipConstruct = ShipConstruction.LoadShip(WaldosIslandVesselData.craftURL);
                if (shipConstruct == null)
                {
                    Debug.Log("[Wlados Island Setup] ShipConstruct was null when tried to load '" + WaldosIslandVesselData.craftURL +
                      "' (usually this means the file could not be found).");
                    return;//continue;
                }

                craftNode = ConfigNode.Load(WaldosIslandVesselData.craftURL);
                lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
                craftRotation = ConfigNode.ParseQuaternion(craftNode.GetNode("PART").GetValue("rot"));

                // Restore ShipConstruction ship
                ShipConstruction.ShipConfig = currentShip;

                // Set the name
                if (string.IsNullOrEmpty(WaldosIslandVesselData.name))
                {
                    WaldosIslandVesselData.name = "OrX Waldo's Island";
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
                ProtoCrewMember[] crewArray = new ProtoCrewMember[WaldosIslandVesselData.crew.Count];
                /*
                        int i = 0;
                        foreach (CrewData cd in WaldosIslandVesselData.crew)
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
                partNodes[0] = ProtoVessel.CreatePartNode(WaldosIslandVesselData.craftPart.name, flightId, crewArray);

                // Default the size class
                //sizeClass = UntrackedObjectClass.A;

                // Set the name
                if (string.IsNullOrEmpty(WaldosIslandVesselData.name))
                {
                    WaldosIslandVesselData.name = WaldosIslandVesselData.craftPart.name;
                }
            }

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            //DiscoveryLevels discoveryLevel = WaldosIslandVesselData.owned ? DiscoveryLevels.Owned : DiscoveryLevels.Unowned;
            //additionalNodes[0] = ProtoVessel.CreateDiscoveryNode(discoveryLevel, sizeClass, contract.TimeDeadline, contract.TimeDeadline);

            // Create the config node representation of the ProtoVessel
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(WaldosIslandVesselData.name, WaldosIslandVesselData.vesselType, WaldosIslandVesselData.orbit, 0, partNodes, additionalNodes);

            // Additional seetings for a landed vessel
            if (!WaldosIslandVesselData.orbiting)
            {
                Vector3d norm = WaldosIslandVesselData.body.GetRelSurfaceNVector(WaldosIslandVesselData.latitude, WaldosIslandVesselData.longitude);

                double terrainHeight = 0.0;
                if (WaldosIslandVesselData.body.pqsController != null)
                {
                    terrainHeight = WaldosIslandVesselData.body.pqsController.GetSurfaceHeight(norm) - WaldosIslandVesselData.body.pqsController.radius;
                }
                bool splashed = false;// = landed && terrainHeight < 0.001;

                // Create the config node representation of the ProtoVessel
                // Note - flying is experimental, and so far doesn't worx
                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : landed ?
                  Vessel.Situations.LANDED : Vessel.Situations.FLYING).ToString());
                protoVesselNode.SetValue("landed", (landed && !splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", WaldosIslandVesselData.latitude.ToString());
                protoVesselNode.SetValue("lon", WaldosIslandVesselData.longitude.ToString());
                protoVesselNode.SetValue("alt", WaldosIslandVesselData.altitude.ToString());
                protoVesselNode.SetValue("landedAt", WaldosIslandVesselData.body.name);

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
                    foreach (Collider collider in WaldosIslandVesselData.craftPart.partPrefab.GetComponentsInChildren<Collider>())
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
                float heading = WaldosIslandVesselData.heading;
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


                    WaldosIslandVesselData.heading = 0;
                    WaldosIslandVesselData.pitch = 0;
                }

                rotation = rotation * Quaternion.AngleAxis(heading, Vector3.back);
                rotation = rotation * Quaternion.AngleAxis(WaldosIslandVesselData.roll, Vector3.down);
                rotation = rotation * Quaternion.AngleAxis(WaldosIslandVesselData.pitch, Vector3.left);

                // Set the height and rotation
                if (landed || splashed)
                {
                    float hgt = (shipConstruct != null ? shipConstruct.parts[0] : WaldosIslandVesselData.craftPart.partPrefab).localRoot.attPos0.y - lowest;
                    hgt += WaldosIslandVesselData.height;
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
            WaldosIslandVesselData.id = protoVessel.vesselRef.id;

            //protoVessel.vesselRef.currentStage = 0;
            hasClamp = false;

            StartCoroutine(PlaceSpawnedVessel(protoVessel.vesselRef, !hasClamp));

            // Associate it so that it can be used in contract parameters
            //ContractVesselTracker.Instance.AssociateVessel(WaldosIslandVesselData.name, protoVessel.vesselRef);


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
            loadingCraft = false;
            if (count <= 4)
            {
                CheckSpawnTimer(); /// loop back to beginning
            }
            else
            {
                count = 0;
            }

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

        private class WaldosIslandVesselData
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

            public WaldosIslandVesselData() { }
            public WaldosIslandVesselData(WaldosIslandVesselData vd)
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


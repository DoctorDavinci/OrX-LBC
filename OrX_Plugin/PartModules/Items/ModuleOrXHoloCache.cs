using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrX.spawn;
using System.IO;

namespace OrX.parts
{
    public class ModuleOrXHoloCache : PartModule
    {
        [KSPField(isPersistant = true)]
        public bool getGPS = true;
        [KSPField(isPersistant = true)]
        public bool setup = false;
        [KSPField(isPersistant = true)]
        public bool isSetup = false;
        [KSPField(isPersistant = true)]
        public bool save = false;

        [KSPField(isPersistant = true)]
        public bool sth = false;
        [KSPField(isPersistant = true)]
        public string _sth = string.Empty;

        [KSPField(isPersistant = true)]
        double lat = 0f;
        [KSPField(isPersistant = true)]
        double lon = 0f;
        [KSPField(isPersistant = true)]
        double alt = 0f;

        [KSPField(isPersistant = true)]
        public string celestialBody = string.Empty;
        [KSPField(isPersistant = true)]
        private string HoloCacheName =  "HoloCache";
        [KSPField(isPersistant = true)]
        private string text1 = string.Empty;
        [KSPField(isPersistant = true)]
        private string text2 = string.Empty;
        [KSPField(isPersistant = true)]
        private string text3 = string.Empty;

        [KSPField(isPersistant = true)]
        public bool spawned = false;
        [KSPField(isPersistant = true)]
        public bool spawn = true;
        [KSPField(isPersistant = true)]
        public bool openHolo = false;

        private bool spawning = false;
        public bool powerDown = false;
        private bool velMatch = false;
        private int count = 0;


        IEnumerator VelocityMatch()
        {
            var point1 = FlightGlobals.ActiveVessel.GetWorldPos3D();
            yield return new WaitForSeconds(1);
            var point2 = FlightGlobals.ActiveVessel.GetWorldPos3D();
            var heading = (point1 - point2).normalized;
            this.part.GetComponent<Rigidbody>().velocity = heading * FlightGlobals.ActiveVessel.srfSpeed;
        }


        #region Coords

        [KSPField(isPersistant = true)] public string HoloCraft1 = "HoloCraft1";
        [KSPField(isPersistant = true)] public bool HoloCoord1 = false;
        [KSPField(isPersistant = true)] public double HoloCoord1lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord1lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord1alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft2 = "HoloCraft2";
        [KSPField(isPersistant = true)] public bool HoloCoord2 = false;
        [KSPField(isPersistant = true)] public double HoloCoord2lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord2lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord2alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft3 = "HoloCraft3";
        [KSPField(isPersistant = true)] public bool HoloCoord3 = false;
        [KSPField(isPersistant = true)] public double HoloCoord3lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord3lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord3alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft4 = "HoloCraft4";
        [KSPField(isPersistant = true)] public bool HoloCoord4 = false;
        [KSPField(isPersistant = true)] public double HoloCoord4lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord4lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord4alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft5 = "HoloCraft5";
        [KSPField(isPersistant = true)] public bool HoloCoord5 = false;
        [KSPField(isPersistant = true)] public double HoloCoord5lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord5lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord5alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft6 = "HoloCraft6";
        [KSPField(isPersistant = true)] public bool HoloCoord6 = false;
        [KSPField(isPersistant = true)] public double HoloCoord6lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord6lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord6alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft7 = "HoloCraft7";
        [KSPField(isPersistant = true)] public bool HoloCoord7 = false;
        [KSPField(isPersistant = true)] public double HoloCoord7lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord7lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord7alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft8 = "HoloCraft8";
        [KSPField(isPersistant = true)] public bool HoloCoord8 = false;
        [KSPField(isPersistant = true)] public double HoloCoord8lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord8lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord8alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft9 = "HoloCraft9";
        [KSPField(isPersistant = true)] public bool HoloCoord9 = false;
        [KSPField(isPersistant = true)] public double HoloCoord9lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord9lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord9alt = 0;

        [KSPField(isPersistant = true)] public string HoloCraft10 = "HoloCraft10";
        [KSPField(isPersistant = true)] public bool HoloCoord10 = false;
        [KSPField(isPersistant = true)] public double HoloCoord10lat = 0;
        [KSPField(isPersistant = true)] public double HoloCoord10lon = 0;
        [KSPField(isPersistant = true)] public double HoloCoord10alt = 0;

        #endregion

        public override void OnStart(StartState state)
        {
            part.force_activate();
            powerDown = false;

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (sth)
                {
                    _sth = "True";
                }
                else
                {
                    _sth = "False";
                }
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (!powerDown)
                {
                    if (!isSetup)
                    {
                        if (!setup)
                        {
                            setup = true;
                            StartCoroutine(CountDownRoutine());
                        }
                    }
                    else
                    {
                        if (!this.vessel.HoldPhysics)
                        {
                            if (spawn)
                            {
                                spawn = false;
                                spawned = true;
                                spawning = true;
                                StartCoroutine(Spawn());
                                Debug.Log("[OrX HoloCache] Spawning Enemy Vessels ...................");
                            }
                            else
                            {
                                if (spawned)
                                {
                                    if (save && !spawning)
                                    {
                                        save = false;
                                        StartCoroutine(CountDownRoutine());
                                    }
                                }
                                else
                                {
                                    if (save)
                                    {
                                        save = false;
                                        openHolo = true;
                                    }

                                    StartCoroutine(CountDownRoutine());
                                }
                            }
                        }
                    }
                }
            }
        }

        private Vector3d _SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)lat, (double)lon, (double)alt);
        }

        IEnumerator CountDownRoutine()
        {
            StartCoroutine(VelocityMatch());
            yield return new WaitForSeconds(2);
            EnableGui();
        }

        private string HoloCacheListToString()
        {
            string finalString = string.Empty;
            string aString = string.Empty;
            aString += FlightGlobals.currentMainBody.name;
            aString += ",";
            aString += HoloCacheName;
            aString += ",";
            aString += sth;
            aString += ",";

            aString += this.vessel.latitude;
            aString += ",";
            aString += this.vessel.longitude;
            aString += ",";
            aString += this.vessel.altitude;
            aString += ";";

            aString += text1;
            aString += ";";
            aString += text2;
            aString += ";";
            aString += text3;
            aString += ";";

            finalString += aString;
            finalString += ":";

            string bString = string.Empty;
            bString += FlightGlobals.currentMainBody.name;
            bString += ",";
            bString += HoloCacheName;
            bString += ",";
            bString += sth;
            bString += ",";

            bString += this.vessel.latitude;
            bString += ",";
            bString += this.vessel.longitude;
            bString += ",";
            bString += this.vessel.altitude;
            bString += ";";

            bString += text1;
            bString += ";";
            bString += text2;
            bString += ";";
            bString += text3;
            bString += ";";

            finalString += bString;

            return finalString;
        }

        IEnumerator SaveSTHTargets()
        {
            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName))
            {
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName);
            }

            int vesselCount = 0;

            HoloCoord1 = false;
            HoloCoord2 = false;
            HoloCoord3 = false;
            HoloCoord4 = false;
            HoloCoord5 = false;
            HoloCoord6 = false;
            HoloCoord7 = false;
            HoloCoord8 = false;
            HoloCoord9 = false;
            HoloCoord10 = false;
            sth = true;
            spawn = true;

            if (sth)
            {
                _sth = "True";
            }
            else
            {
                _sth = "False";
            }

            if (getGPS)
            {
                getGPS = false;
            }

            yield return new WaitForSeconds(2);

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                if (v.Current != this.vessel && v.Current.parts.Count >= 1)
                {
                    if (!HoloCoord10)
                    {
                        if (vesselCount <= 10)
                        {
                            vesselCount += 1;
                            yield return new WaitForSeconds(3);
                            Vector3d vLoc = v.Current.GetWorldPos3D();
                            double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), vLoc);

                            if (targetDistance <= 2000)
                            {
                                ScreenMsg("<color=#cfc100ff><b>Adding Vessel #" + vesselCount + " to HoloCache: " + v.Current.vesselName + "</b></color>");
                                OrX_ShipSave.instance.HoloCacheName = HoloCacheName;

                                if (!HoloCoord1)
                                {
                                    HoloCoord1 = true;
                                    HoloCoord1lat = v.Current.latitude;
                                    HoloCoord1lon = v.Current.longitude;
                                    HoloCoord1alt = v.Current.altitude;
                                    OrX_ShipSave.instance.saveShip = true;

                                    OrX_ShipSave.instance.holo = false;
                                    OrX_ShipSave.instance.VesselToSave = v.Current;
                                    OrX_ShipSave.instance.ShipName = HoloCraft1;
                                    OrX_ShipSave.instance.Save();
                                    yield return new WaitForSeconds(2);

                                }
                                else
                                {
                                    if (!HoloCoord2)
                                    {
                                        HoloCoord2 = true;
                                        HoloCoord2lat = v.Current.latitude;
                                        HoloCoord2lon = v.Current.longitude;
                                        HoloCoord2alt = v.Current.altitude;
                                        OrX_ShipSave.instance.saveShip = true;

                                        OrX_ShipSave.instance.holo = false;
                                        OrX_ShipSave.instance.VesselToSave = v.Current;
                                        OrX_ShipSave.instance.ShipName = HoloCraft2;
                                        OrX_ShipSave.instance.Save();
                                        yield return new WaitForSeconds(2);

                                    }
                                    else
                                    {
                                        if (!HoloCoord3)
                                        {
                                            HoloCoord3 = true;
                                            HoloCoord3lat = v.Current.latitude;
                                            HoloCoord3lon = v.Current.longitude;
                                            HoloCoord3alt = v.Current.altitude;
                                            OrX_ShipSave.instance.saveShip = true;

                                            OrX_ShipSave.instance.holo = false;
                                            OrX_ShipSave.instance.VesselToSave = v.Current;
                                            OrX_ShipSave.instance.ShipName = HoloCraft3;
                                            OrX_ShipSave.instance.Save();
                                            yield return new WaitForSeconds(2);

                                        }
                                        else
                                        {
                                            if (!HoloCoord4)
                                            {
                                                HoloCoord4 = true;
                                                HoloCoord4lat = v.Current.latitude;
                                                HoloCoord4lon = v.Current.longitude;
                                                HoloCoord4alt = v.Current.altitude;
                                                OrX_ShipSave.instance.saveShip = true;

                                                OrX_ShipSave.instance.holo = false;
                                                OrX_ShipSave.instance.VesselToSave = v.Current;
                                                OrX_ShipSave.instance.ShipName = HoloCraft4;
                                                OrX_ShipSave.instance.Save();
                                                yield return new WaitForSeconds(2);

                                            }
                                            else
                                            {
                                                if (!HoloCoord5)
                                                {
                                                    HoloCoord5 = true;
                                                    HoloCoord5lat = v.Current.latitude;
                                                    HoloCoord5lon = v.Current.longitude;
                                                    HoloCoord5alt = v.Current.altitude;
                                                    OrX_ShipSave.instance.saveShip = true;

                                                    OrX_ShipSave.instance.holo = false;
                                                    OrX_ShipSave.instance.VesselToSave = v.Current;
                                                    OrX_ShipSave.instance.ShipName = HoloCraft5;
                                                    OrX_ShipSave.instance.Save();
                                                    yield return new WaitForSeconds(2);

                                                }
                                                else
                                                {
                                                    if (!HoloCoord6)
                                                    {
                                                        HoloCoord6 = true;
                                                        HoloCoord6lat = v.Current.latitude;
                                                        HoloCoord6lon = v.Current.longitude;
                                                        HoloCoord6alt = v.Current.altitude;
                                                        OrX_ShipSave.instance.saveShip = true;

                                                        OrX_ShipSave.instance.holo = false;
                                                        OrX_ShipSave.instance.VesselToSave = v.Current;
                                                        OrX_ShipSave.instance.ShipName = HoloCraft6;
                                                        OrX_ShipSave.instance.Save();
                                                        yield return new WaitForSeconds(2);

                                                    }
                                                    else
                                                    {
                                                        if (!HoloCoord7)
                                                        {
                                                            HoloCoord7 = true;
                                                            HoloCoord7lat = v.Current.latitude;
                                                            HoloCoord7lon = v.Current.longitude;
                                                            HoloCoord7alt = v.Current.altitude;
                                                            OrX_ShipSave.instance.saveShip = true;

                                                            OrX_ShipSave.instance.holo = false;
                                                            OrX_ShipSave.instance.VesselToSave = v.Current;
                                                            OrX_ShipSave.instance.ShipName = HoloCraft7;
                                                            OrX_ShipSave.instance.Save();
                                                            yield return new WaitForSeconds(2);

                                                        }
                                                        else
                                                        {
                                                            if (!HoloCoord8)
                                                            {
                                                                HoloCoord8 = true;
                                                                HoloCoord8lat = v.Current.latitude;
                                                                HoloCoord8lon = v.Current.longitude;
                                                                HoloCoord8alt = v.Current.altitude;
                                                                OrX_ShipSave.instance.saveShip = true;

                                                                OrX_ShipSave.instance.holo = false;
                                                                OrX_ShipSave.instance.VesselToSave = v.Current;
                                                                OrX_ShipSave.instance.ShipName = HoloCraft8;
                                                                OrX_ShipSave.instance.Save();
                                                                yield return new WaitForSeconds(2);

                                                            }
                                                            else
                                                            {
                                                                if (!HoloCoord9)
                                                                {
                                                                    HoloCoord9 = true;
                                                                    HoloCoord9lat = v.Current.latitude;
                                                                    HoloCoord9lon = v.Current.longitude;
                                                                    HoloCoord9alt = v.Current.altitude;
                                                                    OrX_ShipSave.instance.saveShip = true;

                                                                    OrX_ShipSave.instance.holo = false;
                                                                    OrX_ShipSave.instance.VesselToSave = v.Current;
                                                                    OrX_ShipSave.instance.ShipName = HoloCraft9;
                                                                    OrX_ShipSave.instance.Save();
                                                                    yield return new WaitForSeconds(2);

                                                                }
                                                                else
                                                                {
                                                                    HoloCoord10 = true;
                                                                    HoloCoord10lat = v.Current.latitude;
                                                                    HoloCoord10lon = v.Current.longitude;
                                                                    HoloCoord10alt = v.Current.altitude;
                                                                    OrX_ShipSave.instance.saveShip = true;

                                                                    OrX_ShipSave.instance.holo = false;
                                                                    OrX_ShipSave.instance.VesselToSave = v.Current;
                                                                    OrX_ShipSave.instance.ShipName = HoloCraft10;
                                                                    OrX_ShipSave.instance.Save();
                                                                    yield return new WaitForSeconds(2);

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
                    }
                }
            }
            v.Dispose();

            yield return new WaitForSeconds(1);
            sth = true;
            Debug.Log("[OrX HoloCache] Saved OrX HoloCache Targets");
            GetGPS();
        }

        private void GetGPS()
        {
            openHolo = true;
            this.vessel.vesselName = HoloCacheName;
            celestialBody = this.vessel.mainBody.name;
            lat = this.vessel.latitude;
            lon = this.vessel.longitude;
            alt = this.vessel.altitude;
            string soi = FlightGlobals.currentMainBody.name;

            if (sth)
            {
                _sth = "True";
            }
            else
            {
                _sth = "False";
            }

            if (getGPS)
            {
                getGPS = false;
            }

            isSetup = true;
            setup = false;

            ScreenMsg("<color=#cfc100ff><b>Creating HoloCache: " + HoloCacheName + "</b></color>");
            Debug.Log("[OrX HoloCache JDI] Creating HoloCache: " + HoloCacheName);
            Debug.Log("[OrX HoloCache JDI] CelestialBody: " + celestialBody);
            Debug.Log("[OrX HoloCache JDI] GPS Latitude: " + lat);
            Debug.Log("[OrX HoloCache JDI] GPS Longitude: " + lon);
            Debug.Log("[OrX HoloCache JDI] GPS Altitude: " + alt);
            Debug.Log("[OrX HoloCache JDI] STH: " + _sth);

            OrXTargetManager.instance.HoloCacheName = HoloCacheName;
            //OrXTargetManager.instance.Vreport(this.vessel);
            OrXTargetManager.instance.resetHoloCache = true;
            OrXTargetManager.instance.holoCache = this.vessel;
            OrXTargetManager.instance.craftToSpawn = HoloCacheName;
            OrXTargetManager.instance._lat = lat;
            OrXTargetManager.instance._lon = lon;
            OrXTargetManager.instance._alt = alt;
            OrXTargetManager.instance.sth = _sth;
            SaveHoloCache();
        }

        private void AddCoords()
        {
            if (sth)
            {
                _sth = "True";
            }
            else
            {
                _sth = "False";
            }

            ScreenMsg("<color=#cfc100ff><b>Adding " + HoloCacheName + " HoloCache coordinates to list</b></color>");
            Debug.Log("[OrX HoloCache JDI] Adding HoloCache: " + HoloCacheName);
            Debug.Log("[OrX HoloCache JDI] CelestialBody: " + celestialBody);
            Debug.Log("[OrX HoloCache JDI] GPS Latitude: " + lat);
            Debug.Log("[OrX HoloCache JDI] GPS Longitude: " + lon);
            Debug.Log("[OrX HoloCache JDI] GPS Altitude: " + alt);
            Debug.Log("[OrX HoloCache JDI] STH: " + _sth);

            OrXTargetManager.instance.HoloCacheName = HoloCacheName;
            //OrXTargetManager.instance.Vreport(this.vessel);
            OrXTargetManager.instance.resetHoloCache = false;
            OrXTargetManager.instance.holoCache = this.vessel;
            OrXTargetManager.instance.craftToSpawn = HoloCacheName;
            OrXTargetManager.instance._lat = lat;
            OrXTargetManager.instance._lon = lon;
            OrXTargetManager.instance._alt = alt;
            OrXTargetManager.instance.sth = _sth;
            OrXTargetManager.HoloCacheTargets[OrXHoloCache.OrXCoords.Kerbin].Add(new OrXHoloCacheinfo(_SpawnCoords(), HoloCacheName));
        }

        private void SaveHoloCache()
        {
            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName))
            {
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName);
            }
            ConfigNode fileNode = ConfigNode.Load("GameData/OrXHoloCache/" + HoloCacheName + "/" + HoloCacheName + ".cfg");
            if (fileNode == null)
            {
                fileNode = new ConfigNode();
                fileNode.AddNode("OrX");
                fileNode.Save("GameData/OrXHoloCache/" + HoloCacheName + "/" + HoloCacheName + ".cfg");
            }

            if (fileNode != null && fileNode.HasNode("OrX"))
            {
                ConfigNode node = fileNode.GetNode("OrX");
                ConfigNode HoloCacheNode = null;

                if (node.HasNode("OrXHoloCacheCoords"))
                {
                    foreach (ConfigNode n in node.GetNodes("OrXHoloCacheCoords"))
                    {
                        if (n.GetValue("SOI") == this.vessel.mainBody.name)
                        {
                            HoloCacheNode = n;
                            break;
                        }
                    }

                    if (HoloCacheNode == null)
                    {
                        HoloCacheNode = node.AddNode("OrXHoloCacheCoords");
                        HoloCacheNode.AddValue("SOI", this.vessel.mainBody.name);
                    }
                }
                else
                {
                    HoloCacheNode = node.AddNode("OrXHoloCacheCoords");
                    HoloCacheNode.AddValue("SOI", this.vessel.mainBody.name);
                }

                string targetString = HoloCacheListToString();
                HoloCacheNode.SetValue("Targets", targetString, true);
                fileNode.Save("GameData/OrXHoloCache/" + HoloCacheName + "/" + HoloCacheName + ".cfg");
                Debug.Log("[OrX HoloCache] Saved OrX HoloCache Targets");
                OrX_ShipSave.instance.HoloCacheName = HoloCacheName;
                OrX_ShipSave.instance.saveShip = true;
                OrX_ShipSave.instance.holo = true;
                OrX_ShipSave.instance.VesselToSave = this.vessel;
                OrX_ShipSave.instance.Save();
            }
        }

        IEnumerator Spawn()
        {
            spawning = true;
            SpawnOrX_HoloCache.instance.HoloCacheName = HoloCacheName;

            if (HoloCoord1)
            {
                HoloCoord1 = false;
                SpawnOrX_HoloCache.instance.craftFile = HoloCraft1;
                SpawnOrX_HoloCache.instance._lat = HoloCoord1lat;
                SpawnOrX_HoloCache.instance._lon = HoloCoord1lon;
                SpawnOrX_HoloCache.instance._alt = HoloCoord1alt;
                SpawnOrX_HoloCache.instance.holo = false;
                SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                StartCoroutine(Spawn());
            }
            else
            {
                yield return new WaitForSeconds(5);

                if (HoloCoord2)
                {
                    HoloCoord2 = false;
                    SpawnOrX_HoloCache.instance.craftFile = HoloCraft2;
                    SpawnOrX_HoloCache.instance._lat = HoloCoord2lat;
                    SpawnOrX_HoloCache.instance._lon = HoloCoord2lon;
                    SpawnOrX_HoloCache.instance._alt = HoloCoord2alt;
                    SpawnOrX_HoloCache.instance.holo = false;
                    SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                    StartCoroutine(Spawn());
                }
                else
                {
                    if (HoloCoord3)
                    {
                        HoloCoord3 = false;
                        SpawnOrX_HoloCache.instance.craftFile = HoloCraft3;
                        SpawnOrX_HoloCache.instance._lat = HoloCoord3lat;
                        SpawnOrX_HoloCache.instance._lon = HoloCoord3lon;
                        SpawnOrX_HoloCache.instance._alt = HoloCoord3alt;
                        SpawnOrX_HoloCache.instance.holo = false;
                        SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                        StartCoroutine(Spawn());
                    }
                    else
                    {
                        if (HoloCoord4)
                        {
                            HoloCoord4 = false;
                            SpawnOrX_HoloCache.instance.craftFile = HoloCraft4;
                            SpawnOrX_HoloCache.instance._lat = HoloCoord4lat;
                            SpawnOrX_HoloCache.instance._lon = HoloCoord4lon;
                            SpawnOrX_HoloCache.instance._alt = HoloCoord4alt;
                            SpawnOrX_HoloCache.instance.holo = false;
                            SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                            StartCoroutine(Spawn());
                        }
                        else
                        {
                            if (HoloCoord5)
                            {
                                HoloCoord5 = false;
                                SpawnOrX_HoloCache.instance.craftFile = HoloCraft5;
                                SpawnOrX_HoloCache.instance._lat = HoloCoord5lat;
                                SpawnOrX_HoloCache.instance._lon = HoloCoord5lon;
                                SpawnOrX_HoloCache.instance._alt = HoloCoord5alt;
                                SpawnOrX_HoloCache.instance.holo = false;
                                SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                StartCoroutine(Spawn());
                            }
                            else
                            {
                                if (HoloCoord6)
                                {
                                    HoloCoord6 = false;
                                    SpawnOrX_HoloCache.instance.craftFile = HoloCraft6;
                                    SpawnOrX_HoloCache.instance._lat = HoloCoord6lat;
                                    SpawnOrX_HoloCache.instance._lon = HoloCoord6lon;
                                    SpawnOrX_HoloCache.instance._alt = HoloCoord6alt;
                                    SpawnOrX_HoloCache.instance.holo = false;
                                    SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                    StartCoroutine(Spawn());
                                }
                                else
                                {
                                    if (HoloCoord7)
                                    {
                                        HoloCoord7 = false;
                                        SpawnOrX_HoloCache.instance.craftFile = HoloCraft7;
                                        SpawnOrX_HoloCache.instance._lat = HoloCoord7lat;
                                        SpawnOrX_HoloCache.instance._lon = HoloCoord7lon;
                                        SpawnOrX_HoloCache.instance._alt = HoloCoord7alt;
                                        SpawnOrX_HoloCache.instance.holo = false;
                                        SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                        StartCoroutine(Spawn());
                                    }
                                    else
                                    {
                                        if (HoloCoord8)
                                        {
                                            HoloCoord8 = false;
                                            SpawnOrX_HoloCache.instance.craftFile = HoloCraft8;
                                            SpawnOrX_HoloCache.instance._lat = HoloCoord8lat;
                                            SpawnOrX_HoloCache.instance._lon = HoloCoord8lon;
                                            SpawnOrX_HoloCache.instance._alt = HoloCoord8alt;
                                            SpawnOrX_HoloCache.instance.holo = false;
                                            SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                            StartCoroutine(Spawn());
                                        }
                                        else
                                        {
                                            if (HoloCoord9)
                                            {
                                                HoloCoord9 = false;
                                                SpawnOrX_HoloCache.instance.craftFile = HoloCraft9;
                                                SpawnOrX_HoloCache.instance._lat = HoloCoord9lat;
                                                SpawnOrX_HoloCache.instance._lon = HoloCoord9lon;
                                                SpawnOrX_HoloCache.instance._alt = HoloCoord9alt;
                                                SpawnOrX_HoloCache.instance.holo = false;
                                                SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                                StartCoroutine(Spawn());
                                            }
                                            else
                                            {
                                                if (HoloCoord10)
                                                {
                                                    HoloCoord10 = false;
                                                    SpawnOrX_HoloCache.instance.craftFile = HoloCraft10;
                                                    SpawnOrX_HoloCache.instance._lat = HoloCoord10lat;
                                                    SpawnOrX_HoloCache.instance._lon = HoloCoord10lon;
                                                    SpawnOrX_HoloCache.instance._alt = HoloCoord10alt;
                                                    SpawnOrX_HoloCache.instance.holo = false;
                                                    SpawnOrX_HoloCache.instance.CheckSpawnTimer();
                                                }
                                                else
                                                {
                                                    spawning = false;
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

        IEnumerator DestroyHoloCache()
        {
            if (!spawning)
            {
                // Play mission impossible "this message will self destruct in 10 seconds
                ScreenMsg("<color=#cfc100ff><b>" + HoloCacheName + " will self destruct in 10 seconds</b></color>");
                yield return new WaitForSeconds(10);
                this.part.explode();
            }
            else
            {
                ScreenMsg("<color=#cfc100ff><b>" + HoloCacheName + " Spawning Vessels ... Please Stand By</b></color>");
                yield return new WaitForSeconds(5);
                StartCoroutine(DestroyHoloCache());
            }
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        #region HoloCache JDI GUI
        /// <summary>
        /// GUI
        /// </summary>

        private const float WindowWidth = 250;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public bool GuiEnabledOrX_HoloCache = false;
        public bool GuiEnabledOrX_HoloCache2 = false;
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
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 200, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXHoloCache);
            GameEvents.onShowUI.Add(GameUiEnableOrXHoloCache);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrX_HoloCache && _gameUiToggle)
            {
                _windowRect = GUI.Window(384766702, _windowRect, GuiWindowOrX_HoloCache, "");
            }

            if (GuiEnabledOrX_HoloCache2 && _gameUiToggle)
            {
                GuiEnabledOrX_HoloCache = false;
                _windowRect = GUI.Window(38572892, _windowRect, GuiWindowOrX_HoloCache2, "");
            }
        }

        private void GuiWindowOrX_HoloCache(int OrX_HoloCache)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle(line);
            line++;
            DrawHoloCacheName(line);
            line++;
            DrawText01(line);
            line++;
            DrawText02(line);
            line++;
            DrawText03(line);
            line++;
            line++;
            DrawToggleMode(line);
            line++;
            DrawSave(line);
            line++;
            DrawCloseGui(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void GuiWindowOrX_HoloCache2(int OrX_HoloCache2)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle(line);
            line++;
            line++;
            DrawHoloCacheName1(line);
            line++;
            DrawText011(line);
            line++;
            DrawText022(line);
            line++;
            DrawText033(line);
            line++;
            line++;
            DrawCloseGui(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void EnableGui()
        {
            if (!isSetup)
            {
                GuiEnabledOrX_HoloCache = true;
                GuiEnabledOrX_HoloCache2 = false;
                Debug.Log("[OrX]: Showing HoloCache JDI GUI");
            }
            else
            {
                GuiEnabledOrX_HoloCache2 = true;
                GuiEnabledOrX_HoloCache = false;
                Debug.Log("[OrX]: Showing HoloCache JDI GUI 2");
            }
        }

        private void DisableGui()
        {
            if (GuiEnabledOrX_HoloCache)
            {
                GuiEnabledOrX_HoloCache = false;
                Debug.Log("[OrX]: Hiding HoloCache JDI GUI");
            }

            if (GuiEnabledOrX_HoloCache2)
            {
                GuiEnabledOrX_HoloCache2 = false;
                Debug.Log("[OrX]: Hiding HoloCache JDI GUI 2");
            }
        }

        private void GameUiEnableOrXHoloCache()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXHoloCache()
        {
            _gameUiToggle = false;
        }

        private void DrawTitle(float line)
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
                "OrX HoloCache",
                titleStyle);
        }

        private void DrawHoloCacheName(float line)
        {
            var leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            GUI.Label(new Rect(LeftIndent, ContentTop + line * entryHeight, 60, entryHeight), "HoloCache Name",
                leftLabel);
            float textFieldWidth = 100;
            var fwdFieldRect = new Rect(LeftIndent + contentWidth - textFieldWidth,
                ContentTop + line * entryHeight, textFieldWidth, entryHeight);
            HoloCacheName = GUI.TextField(fwdFieldRect, HoloCacheName);
        }

        private void DrawText01(float line)
        {
            var leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            GUI.Label(new Rect(LeftIndent, ContentTop + line * entryHeight, 60, entryHeight), "1",
                leftLabel);
            float textFieldWidth = 200;
            var fwdFieldRect = new Rect(LeftIndent + contentWidth - textFieldWidth,
                ContentTop + line * entryHeight, textFieldWidth, entryHeight);
            text1 = GUI.TextField(fwdFieldRect, text1);
        }

        private void DrawText02(float line)
        {
            var leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            GUI.Label(new Rect(LeftIndent, ContentTop + line * entryHeight, 60, entryHeight), "2",
                leftLabel);
            float textFieldWidth = 200;
            var fwdFieldRect = new Rect(LeftIndent + contentWidth - textFieldWidth,
                ContentTop + line * entryHeight, textFieldWidth, entryHeight);
            text2 = GUI.TextField(fwdFieldRect, text2);
        }

        private void DrawText03(float line)
        {
            var leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            GUI.Label(new Rect(LeftIndent, ContentTop + line * entryHeight, 60, entryHeight), "3",
                leftLabel);
            float textFieldWidth = 200;
            var fwdFieldRect = new Rect(LeftIndent + contentWidth - textFieldWidth,
                ContentTop + line * entryHeight, textFieldWidth, entryHeight);
            text3 = GUI.TextField(fwdFieldRect, text3);
        }

        private void DrawHoloCacheName1(float line)
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
                HoloCacheName,
              titleStyle);

        }

        private void DrawText011(float line)
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
                text1,
              titleStyle);

        }

        private void DrawText022(float line)
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
                text2,
              titleStyle);

        }

        private void DrawText033(float line)
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
                text3,
              titleStyle);

        }

        private void DrawToggleMode(float line)
        {
            GUIStyle OrXbuttonStyle = sth ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (!sth)
            {
                if (GUI.Button(saveRect, "SAVE VESSELS", OrXbuttonStyle))
                {
                    sth = true;
                }
            }
            else
            {
                if (GUI.Button(saveRect, "SAVE VESSELS", OrXbuttonStyle))
                {
                    sth = false;
                }
            }
        }

        private void DrawSave(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (!save)
            {
                if (GUI.Button(saveRect, "Save HoloCache"))
                {
                    if (Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName))
                    {
                        ScreenMsg("<color=#cfc100ff><b>" + HoloCacheName + " already exists ... Please use a different name</b></color>");
                    }
                    else
                    {
                        powerDown = true;
                        save = true;
                        openHolo = true;

                        if (sth)
                        {
                            StartCoroutine(SaveSTHTargets());
                            DisableGui();
                        }
                        else
                        {
                            GetGPS();
                            DisableGui();
                        }
                    }
                }
            }
        }

        private void DrawCloseGui(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (!openHolo)
            {
                if (GUI.Button(saveRect, "Reset HoloCache"))
                {
                    openHolo = false;
                    powerDown = true;
                    isSetup = false;
                    setup = false;
                    DisableGui();
                    ScreenMsg("<color=#cfc100ff><b>" + HoloCacheName + " needs to be put in a container before re-use</b></color>");
                }
            }
            else
            {
                if (GUI.Button(saveRect, "Close Window"))
                {
                    DisableGui();
                    StartCoroutine(DestroyHoloCache());
                }
            }
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using OrX.parts;
using KSP.UI.Screens;
using UnityEngine;
using System.Text;
using System;
using System.IO;

namespace OrX
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class OrXTargetManager : MonoBehaviour
    {
        public static Dictionary<OrXHoloCache.OrXCoords, List<OrXTargetInfo>> TargetDatabase;
		public static Dictionary<OrXHoloCache.OrXCoords, List<OrXHoloCacheinfo>> HoloCacheTargets;

		public static OrXTargetManager instance;

        public bool resetHoloCache = false;

        private StringBuilder debugString = new StringBuilder();
        public string craftFile = string.Empty;
        public string craftToSpawn = string.Empty;
        public string sth = string.Empty;
        public string cfgToLoad = string.Empty;
        public string HoloCacheName = string.Empty;

        private float updateTimer = 0;

        public Vessel holoCache;

        public double _lat = 0f;
        public double _lon = 0f;
        public double _alt = 0f;

		void Awake()
		{
            if (instance)
            {
                Destroy(instance);
            }
            instance = this;
            holoCache = null;
            resetHoloCache = false;
        }

        void Start()
		{
			//legacy targetDatabase
			TargetDatabase = new Dictionary<OrXHoloCache.OrXCoords, List<OrXTargetInfo>>();
			TargetDatabase.Add(OrXHoloCache.OrXCoords.Kerbin, new List<OrXTargetInfo>());
			TargetDatabase.Add(OrXHoloCache.OrXCoords.Mun, new List<OrXTargetInfo>());

			if(HoloCacheTargets == null)
			{
				HoloCacheTargets = new Dictionary<OrXHoloCache.OrXCoords, List<OrXHoloCacheinfo>>();
				HoloCacheTargets.Add(OrXHoloCache.OrXCoords.Kerbin, new List<OrXHoloCacheinfo>());
				HoloCacheTargets.Add(OrXHoloCache.OrXCoords.Mun, new List<OrXHoloCacheinfo>());
			}

            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/"))
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/");

            //StartCoroutine(loadHolo());
        }

        void OnDestroy()
        {
            HoloCacheTargets = new Dictionary<OrXHoloCache.OrXCoords, List<OrXHoloCacheinfo>>();
            TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].RemoveAll(target => target == null);
            TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].RemoveAll(target => target.team == OrXHoloCache.OrXCoords.Kerbin);
            TargetDatabase[OrXHoloCache.OrXCoords.Mun].RemoveAll(target => target == null);
            TargetDatabase[OrXHoloCache.OrXCoords.Mun].RemoveAll(target => target.team == OrXHoloCache.OrXCoords.Mun);
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 5, ScreenMessageStyle.UPPER_CENTER));
        }

        IEnumerator loadHolo()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                LoadHoloCacheTargets();
            }
            else
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(loadHolo());
            }
        }

        public void SaveHoloCacheTargets() 
		{
            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/"))
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/");

            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/OrX/"))
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/OrX/");

            string soi = FlightGlobals.currentMainBody.name;
			Debug.Log("[OrX HoloCache] SOI: " + soi);
			ConfigNode fileNode = ConfigNode.Load("GameData/OrX/HoloCache/" + cfgToLoad + "/" + cfgToLoad + ".cfg");
			if(fileNode == null)
			{
				fileNode = new ConfigNode();
				fileNode.AddNode("OrX");
				fileNode.Save("GameData/OrX/HoloCache/" + cfgToLoad + "/" + cfgToLoad + ".cfg");
			}
		
			if(fileNode!=null && fileNode.HasNode("OrX"))
			{
				ConfigNode node = fileNode.GetNode("OrX");
                
				if(!FlightGlobals.ready)
				{
					return;
				}
                
				ConfigNode HoloCacheNode = null;
				if(node.HasNode("OrXHoloCacheCoords"))
				{
					foreach(ConfigNode n in node.GetNodes("OrXHoloCacheCoords"))
					{
						if(n.GetValue("SOI") == soi)
						{
							HoloCacheNode = n;
							break;
						}
					}

					if(HoloCacheNode == null)
					{
						HoloCacheNode = node.AddNode("OrXHoloCacheCoords");
						HoloCacheNode.AddValue("SOI", soi);
					}
				}
				else
				{
					HoloCacheNode = node.AddNode("OrXHoloCacheCoords");
					HoloCacheNode.AddValue("SOI", soi);
				}

                craftFile = craftToSpawn;
				string targetString = HoloCacheListToString();
				HoloCacheNode.SetValue("Targets", targetString, true);
                fileNode.Save("GameData/OrX/HoloCache/" + cfgToLoad + "/" + cfgToLoad + ".cfg");
                Debug.Log("[OrX HoloCache] Saved OrX HoloCache Targets");
                OrX_ShipSave.instance.saveShip = true;
            }
		}

        public void LoadHoloCacheTargets()
        {
            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/"))
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/");

            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/OrX/"))
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrX/Plugin/PluginData/OrX/");

            if (HoloCacheTargets == null)
            {
                HoloCacheTargets = new Dictionary<OrXHoloCache.OrXCoords, List<OrXHoloCacheinfo>>();
            }
            HoloCacheTargets.Clear();
            HoloCacheTargets.Add(OrXHoloCache.OrXCoords.Kerbin, new List<OrXHoloCacheinfo>());
            HoloCacheTargets.Add(OrXHoloCache.OrXCoords.Mun, new List<OrXHoloCacheinfo>());

            string soi = FlightGlobals.currentMainBody.name;
            string holoCacheLoc = UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/";
            var files = new List<string>(Directory.GetFiles(holoCacheLoc, "*.cfg", SearchOption.AllDirectories));
            
            if (files != null)
            {
                List<string>.Enumerator cfgsToAdd = files.GetEnumerator();
                while (cfgsToAdd.MoveNext())
                {
                    try
                    {
                        ConfigNode fileNode = ConfigNode.Load(cfgsToAdd.Current);

                        if (fileNode != null && fileNode.HasNode("OrX"))
                        {
                            ConfigNode node = fileNode.GetNode("OrX");

                            foreach (ConfigNode HoloCacheNode in node.GetNodes("OrXHoloCacheCoords"))
                            {
                                if (HoloCacheNode.HasValue("SOI") && HoloCacheNode.GetValue("SOI") == soi)
                                {
                                    if (HoloCacheNode.HasValue("Targets"))
                                    {
                                        string targetString = HoloCacheNode.GetValue("Targets");
                                        if (targetString == string.Empty)
                                        {
                                            Debug.Log("[OrX HoloCache] OrX HoloCache Target string was empty!");
                                            return;
                                        }
                                        StringToHoloCacheList(targetString);
                                        Debug.Log("[OrX HoloCache] Loaded OrX HoloCache Targets");
                                    }
                                    else
                                    {
                                        Debug.Log("[OrX HoloCache] No OrX HoloCache Targets value found!");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[OrX Target Manager] LoadHoloCacheTargets Out Of Range ...... Continuing");
                    }
                }
                cfgsToAdd.Dispose();
            }
            else
            {
                Debug.Log("[OrX HoloCache] === HoloCache List is empty ===");
            }
            OrXHoloCache.instance.reload = false;
        }

        IEnumerator ResetHoloCache()
        {
            yield return new WaitForSeconds(5);
            if (holoCache != null)
            {
                if (resetHoloCache)
                {
                    resetHoloCache = false;
                    holoCache.rootPart.explode();
                }
                holoCache = null;
            }
        }

        //format: Current Main Body Name, Craft File, lat, long, alt;
        private string HoloCacheListToString()
		{
			string finalString = string.Empty;
			string aString = string.Empty;
			foreach(OrXHoloCacheinfo HoloCacheInfo in HoloCacheTargets[OrXHoloCache.OrXCoords.Kerbin])
			{
				aString += FlightGlobals.currentMainBody.name;
				aString += ",";
                aString += craftFile;
                aString += ",";
                aString += sth;
                aString += ",";
                aString += _lat;
				aString += ",";
				aString += _lon;
				aString += ",";
				aString += _alt;
				aString += ";";
			}
			if(aString == string.Empty)
			{
				aString = "null";
			}
			finalString += aString;
			finalString += ":";
            
			string bString = string.Empty;
			foreach(OrXHoloCacheinfo HoloCacheInfo in HoloCacheTargets[OrXHoloCache.OrXCoords.Mun])
			{
				bString += FlightGlobals.currentMainBody.name;
                bString += ",";
				bString += craftFile;
                bString += ",";
                bString += sth;
                bString += ",";
                bString += _lat;
                bString += ",";
				bString += _lon;
				bString += ",";
				bString += _alt;
				bString += ";";
			}
			if(bString == string.Empty)
			{
				bString = "null";
			}
			finalString += bString;
            
			return finalString;
		}

		private void StringToHoloCacheList(string listString)
		{
			if(listString == null || listString == string.Empty)
			{
				Debug.Log("[OrX Target Manager] === HoloCache List string was empty or null ===");
				return;
			}

			string[] teams = listString.Split(new char[]{ ':' });

			Debug.Log("[OrX Target Manager] Loading HoloCache Targets ..........");

            try
            {
                if (teams[0] != null && teams[0].Length > 0 && teams[0] != "null")
                {
                    string[] teamACoords = teams[0].Split(new char[] { ';' });
                    for (int i = 0; i < teamACoords.Length; i++)
                    {
                        if (teamACoords[i] != null && teamACoords[i].Length > 0)
                        {
                            string[] data = teamACoords[i].Split(new char[] { ',' });
                            string name = data[0];
                            craftToSpawn = data[1];
                            sth = data[1];
                            double lat = double.Parse(data[3]);
                            double longi = double.Parse(data[4]);
                            double alt = double.Parse(data[5]);
                            OrXHoloCacheinfo newInfo = new OrXHoloCacheinfo(new Vector3d(lat, longi, alt), craftToSpawn);
                            HoloCacheTargets[OrXHoloCache.OrXCoords.Kerbin].Add(newInfo);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("[OrX Target Manager] HoloCache config file processed ...... ");
            }
        }

		IEnumerator CleanDatabaseRoutine()
		{
			while(enabled)
			{
				yield return new WaitForSeconds(5);
			
				TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].RemoveAll(target => target == null);
				TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].RemoveAll(target => target.team == OrXHoloCache.OrXCoords.Kerbin);
			}
		}

		void RemoveTarget(OrXTargetInfo target, OrXHoloCache.OrXCoords team)
		{
			TargetDatabase[team].Remove(target);
		}

        public void Vreport(Vessel v)
        {
            ReportVessel(v);
        }

        public static void ReportVessel(Vessel v)
        {
            if (!v) return;

            OrXTargetInfo info = v.gameObject.GetComponent<OrXTargetInfo>();
            if (!info)
            {
                List<ModuleOrXHoloCache>.Enumerator jdi = v.FindPartModulesImplementing<ModuleOrXHoloCache>().GetEnumerator();
                while (jdi.MoveNext())
                {
                    if (jdi.Current == null) continue;
                    if (jdi.Current.getGPS)
                    {
                        info = v.gameObject.AddComponent<OrXTargetInfo>();
                        break;
                    }

                }
                jdi.Dispose();
            }

            // add target to database
            if (info)
            {
                AddTarget(info);
                info.detectedTime = Time.time;
            }
        }

        public static void AddTarget(OrXTargetInfo target)
        {
            TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].Add(target);
        }

		public static void ClearDatabase()
		{
			foreach(OrXHoloCache.OrXCoords t in TargetDatabase.Keys)
			{
				foreach(OrXTargetInfo target in TargetDatabase[t])
				{
					target.detectedTime = 0;
				}
			}

			TargetDatabase[OrXHoloCache.OrXCoords.Kerbin].Clear();
            TargetDatabase[OrXHoloCache.OrXCoords.Mun].Clear();
        }
    }
}


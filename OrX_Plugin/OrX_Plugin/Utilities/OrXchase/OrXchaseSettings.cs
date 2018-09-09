﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace OrX.chase
{
    class OrXchaseSettings
    {
        public static bool targetVesselBySelection = false;
        public static bool displayDebugLines = false;
        public static bool displayDebugLinesSetting = false;
        public static bool displayToggleHelmet = true;
        public static bool displayLoadingKerbals = true;

        public static int selectMouseButton = 0;
        public static int dispatchMouseButton = 2;

        public static string selectKeyButton = "o";
        public static string dispatchKeyButton = "p";


        private static Dictionary<Guid, string> collection = new Dictionary<Guid, string>();

        private static bool isLoaded = false;

        public static void LoadConfiguration()
        {
            if (FileExcist("Config.cfg"))
            {
                KSP.IO.TextReader tr = KSP.IO.TextReader.CreateForType<OrXchaseSettings>("Config.cfg");
                string[] lines = tr.ReadToEnd().Split('\n');

                foreach (var line in lines)
                {
                    string[] parts = line.Split('=');

                    try
                    {
                        if (parts.Length > 1)
                        {
                            string name = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (name)
                            {
                                case "ShowDebugLineSalt": { displayDebugLinesSetting = bool.Parse(value); } break;
                                case "ShowLoadingKerbalSalt": { displayLoadingKerbals = bool.Parse(value); } break;
                                case "EnableHelmetToggle": { displayToggleHelmet = bool.Parse(value); } break;
                                case "SelectMouseButton": { selectMouseButton = int.Parse(value); } break;
                                case "DispatchMouseButton": { dispatchMouseButton = int.Parse(value); } break;
                                case "SelectKey": { selectKeyButton = value; } break;
                                case "DispatchKey": { dispatchKeyButton = value; } break;
                                case "TargetVesselBySelection": { targetVesselBySelection = bool.Parse(value); } break;
                            }
                        }
                    }
                    catch
                    {
                       OrXchaseDebug.DebugWarning("[OrX Chase] Config loading error ");
                    }
                }
                displayDebugLines = displayDebugLinesSetting;
            }
        }

        public static void SaveConfiguration()
        {
            KSP.IO.TextWriter tr = KSP.IO.TextWriter.CreateForType<OrXchaseSettings>("Config.cfg");
            tr.Write("ShowDebugLines = false");
            tr.Write("# 0 = left, 1 = right, 2 = middle mouse button.");
            tr.Write("SelectMouseButton = 0");
            tr.Write("DispatchMouseButton = 2");
            tr.Write("# Lookup Unity Keybinding for different optionSalt");
            tr.Write("# use lower case or eat exception sandwich. ");
            tr.Write("SelectKey = o");
            tr.Write("DispatchKey = p");
            tr.Write("");
            tr.Write("ShowLoadingKerbals = false");
            tr.Write("EnableHelmetToggle = true");
            tr.Close();

        }

        public static bool FileExcist(string name)
        {
           return KSP.IO.File.Exists<OrXchaseSettings>(name);
        }

        public static void Load()
        {
            OrXchaseDebug.DebugWarning("OnLoad()");
			if (displayLoadingKerbals) {
				ScreenMessages.PostScreenMessage ("Loading Kerbals...", 3, ScreenMessageStyle.LOWER_CENTER);
			}

            LoadFunction();
        }

        public static void LoadFunction()
        {
            OrXchaseDebug.ProfileStart();
            LoadFile();
            OrXchaseDebug.ProfileEnd("OrXchaseSettings.Load()");
            isLoaded = true;
        }

        public static void Save()
        {
            if (isLoaded)
            {
                OrXchaseDebug.DebugWarning("OnSave()");

				if (displayLoadingKerbals) {
					ScreenMessages.PostScreenMessage ("Saving Kerbals...", 3, ScreenMessageStyle.LOWER_CENTER);
				}

                SaveFunction();

                isLoaded = false;
            }
        }

        public static void SaveFunction()
        {
            OrXchaseDebug.ProfileStart();
            SaveFile();
            OrXchaseDebug.ProfileEnd("OrXchaseSettings.Save()");
        }

        public static void LoadEva(OrXchaseContainer container)
        {

            OrXchaseDebug.DebugWarning("OrXchaseSettings.LoadEva(" + container.Name + ")");

            //The eva was already has a old save.
            //Load it.
            if (collection.ContainsKey(container.flightID))
            {
                //string evaString = collection[container.flightID];
                //OrXchaseDebug.DebugWarning(evaString);

                container.FromSave(collection[container.flightID]);
            }
            else
            {
                //No save yet.
            }
        }
        public static void SaveEva(OrXchaseContainer container){

            OrXchaseDebug.DebugWarning("OrXchaseSettings.SaveEva(" + container.Name + ")");

            if (container.status == Status.Removed)
            {
                if (collection.ContainsKey(container.flightID))
                {
                    collection.Remove(container.flightID);
                }
            }
            else
            {
                //The eva was already has a old save.
                if (collection.ContainsKey(container.flightID))
                {
                    //Replace the old save.
                    collection[container.flightID] = container.ToSave();
                }
                else
                {
                    //No save yet. Add it now.
                    collection.Add(container.flightID, container.ToSave());
                }
            }
        }

        private static void LoadFile()
        {
            string fileName  = String.Format("Evas-{0}.txt", HighLogic.CurrentGame.Title);
            if (FileExcist(fileName))
            {
                KSP.IO.TextReader tr = KSP.IO.TextReader.CreateForType<OrXchaseSettings>(fileName);

                string file = tr.ReadToEnd();
                tr.Close();

                OrXchaseTokenReader reader = new OrXchaseTokenReader(file);

                OrXchaseDebug.DebugLog("Size KeySize: " + collection.Count);

                //read every eva.
                while (!reader.EOF)
                {
                    //Load all the eva's in the list.
                    LoadEva(reader.NextToken('[', ']'));
                }
            }
        }

        private static void LoadEva(string eva)
        {
            Guid flightID = GetFlightIDFromEvaString(eva);
            if (!collection.ContainsKey(flightID))
            {
                collection.Add(flightID, eva);
            }
        }


        private static Guid GetFlightIDFromEvaString(string evaString)
        {
            OrXchaseTokenReader reader = new OrXchaseTokenReader(evaString);

            string sflightID = reader.NextTokenEnd(',');

            //Load the eva
            Guid flightID = new Guid(sflightID);
            return flightID;
        }


        private static void SaveFile()
        {
            KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<OrXchaseSettings>(String.Format("Evas-{0}.txt", HighLogic.CurrentGame.Title));

            foreach (var item in collection)
            {
                tw.Write("[" + item.Value + "]");
            }

            tw.Close();

            collection.Clear();
        }
    }
}

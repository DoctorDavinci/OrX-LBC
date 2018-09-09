/*
 * This module written by Claw. For more details please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules 
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.90.0
 *
 * InflightShipSave v0.1.0
 * 
 * This plugin allows the user to save the active, in-flight vessel back out to a .craft file.
 * 
 * Things to fix:
 * - Make the save key bindable (so that it can be changed in a .cfg file)
 * - Adjust the rotation/position of the root part, so that the craft isn't sideways in the ground.
 * 
 * 
 * Change Log:
 * 
 * v0.1.0 (13 Jan 15) - Initial release
 * 
 */
using System.Collections;
using UnityEngine;
using KSP;
using OrX.parts;
using System.IO;

namespace OrX
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_ShipSave : MonoBehaviour
    {
        public static OrX_ShipSave instance;

        private KeyCode BoundKey = KeyCode.F6;

        public bool saveShip = false;
        public string ShipName = string.Empty;
        public Vessel VesselToSave;
        public bool holo = true;
        public string shipDescription = string.Empty;
        public string HoloCacheName = string.Empty;
        public bool sthTargets = false;
        private bool OrXDevKitIsInstalled = false;

        public void Awake()
        {
            if (instance)
                Destroy(instance);
            instance = this;
        }

        public void Save()
        {
            if (!Directory.Exists(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName))
            {
                Directory.CreateDirectory(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName);
            }

            ShipName = VesselToSave.vesselName;

            if (holo)
            {
                Debug.Log("[OrX Ship Save] HoloCache Identified .......................");
                shipDescription = "HoloCache";
            }
            else
            {
                Debug.Log("[OrX Ship Save] Vessel Identified .......................");

                shipDescription = "";
            }
            ShipConstruct ConstructToSave = new ShipConstruct(ShipName, shipDescription, VesselToSave.parts[0]);

            Debug.Log("[OrX Ship Save] Saving: " + ShipName + " ............");

            ScreenMsg("<color=#cfc100ff><b>Saving: " + ShipName + "</b></color>");

            Quaternion OriginalRotation = VesselToSave.vesselTransform.rotation;
            Vector3 OriginalPosition = VesselToSave.vesselTransform.position;

            VesselToSave.SetRotation(new Quaternion(0, 0, 0, 1));
            Vector3 ShipSize = ShipConstruction.CalculateCraftSize(ConstructToSave);
            VesselToSave.SetPosition(new Vector3(0, ShipSize.y + 2, 0));

            ConfigNode CN = new ConfigNode("ShipConstruct");
            CN = ConstructToSave.SaveShip();
            CleanEditorNodes(CN);

            VesselToSave.SetRotation(OriginalRotation);
            VesselToSave.SetPosition(OriginalPosition);

            Debug.Log("Facility: " + ConstructToSave.shipFacility);

            Debug.Log("[OrX Ship Save] Saving: " + UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName + "/" + ShipName + ".craft");
            CN.Save(UrlDir.ApplicationRootPath + "GameData/OrXHoloCache/" + HoloCacheName + "/" + ShipName + ".craft");

            ScreenMsg("<color=#cfc100ff><b>" + ShipName + " Saved to GameData/OrXHoloCache/" + HoloCacheName + "</b></color>");
            if (sthTargets)
            {
                sthTargets = false;
            }
            saveShip = false;
            holo = true;
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.HoldPhysics)
            {
                if (Input.GetKeyDown(BoundKey) && BDArmoryExtensions.BDArmoryIsInstalled())
                {
                    StartCoroutine(SaveDelayRoutine());
                }
            }
        }

        IEnumerator SaveDelayRoutine()
        {
            yield return new WaitForSeconds(2);
            if (HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.HoldPhysics)
            {
                Debug.Log("[OrX Ship Save] Active Vessel Identified .......................");

                VesselToSave = FlightGlobals.ActiveVessel;
                ShipName = FlightGlobals.ActiveVessel.vesselName;
                shipDescription = "_Rescued";

                ShipConstruct ConstructToSave = new ShipConstruct(ShipName, shipDescription, VesselToSave.parts[0]);

                Debug.Log("[OrX Ship Save] Saving: " + ShipName + " ............");

                ScreenMsg("<color=#cfc100ff><b>Saving: " + ShipName + "</b></color>");

                Quaternion OriginalRotation = VesselToSave.vesselTransform.rotation;
                Vector3 OriginalPosition = VesselToSave.vesselTransform.position;

                VesselToSave.SetRotation(new Quaternion(0, 0, 0, 1));
                Vector3 ShipSize = ShipConstruction.CalculateCraftSize(ConstructToSave);
                VesselToSave.SetPosition(new Vector3(0, ShipSize.y + 2, 0));

                ConfigNode CN = new ConfigNode("ShipConstruct");
                CN = ConstructToSave.SaveShip();
                CleanEditorNodes(CN);

                VesselToSave.SetRotation(OriginalRotation);
                VesselToSave.SetPosition(OriginalPosition);

                Debug.Log("Facility: " + ConstructToSave.shipFacility);

                if (ConstructToSave.shipFacility == EditorFacility.SPH)
                {
                    Debug.Log("Ship Saved: " + UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/SPH/" + ShipName + "_Rescued.craft");
                    CN.Save(UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/SPH/" + ShipName + "_Rescued.craft");
                }
                else
                {
                    Debug.Log("Ship Saved: " + UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/VAB/" + ShipName + "_Rescued.craft");
                    CN.Save(UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/VAB/" + ShipName + "_Rescued.craft");
                }
            }
            else
            {
                //Save();
            }
        }

        private void CleanEditorNodes (ConfigNode CN)
        {

            CN.SetValue("EngineIgnited", "False");
            CN.SetValue("currentThrottle", "0");
            CN.SetValue("Staged", "False");
            CN.SetValue("sensorActive", "False");
            CN.SetValue("throttle", "0");
            CN.SetValue("generatorIsActive", "False");
            CN.SetValue("persistentState", "STOWED");

            string ModuleName = CN.GetValue("name");

            // Turn off or remove specific things
            if ("ModuleScienceExperiment" == ModuleName)
            {
                CN.RemoveNodes("ScienceData");
            }
            else if ("ModuleScienceExperiment" == ModuleName)
            {
                CN.SetValue("Inoperable", "False");
                CN.RemoveNodes("ScienceData");
            }
            else if ("Log" == ModuleName)
            {
                CN.ClearValues();
            }


            for (int IndexNodes = 0; IndexNodes < CN.nodes.Count; IndexNodes++)
            {
                CleanEditorNodes (CN.nodes[IndexNodes]);
            }
        }

        private void PristineNodes (ConfigNode CN)
        {
            if (null == CN) { return; }

            if ("PART" == CN.name)
            {
                string PartName = ((CN.GetValue("part")).Split('_'))[0];

                Debug.Log("PART: " + PartName);
                
                Part NewPart = PartLoader.getPartInfoByName(PartName).partPrefab;
                ConfigNode NewPartCN = new ConfigNode();
                Debug.Log("New Part: " + NewPart.name);

                NewPart.InitializeModules();

                CN.ClearNodes();

                // EVENTS, ACTIONS, PARTDATA, MODULE, RESOURCE

                Debug.Log("EVENTS");
                NewPart.Events.OnSave(CN.AddNode("EVENTS"));
                Debug.Log("ACTIONS");
                NewPart.Actions.OnSave(CN.AddNode("ACTIONS"));
                Debug.Log("PARTDATA");
                NewPart.OnSave(CN.AddNode("PARTDATA"));
                Debug.Log("MODULE");
                for (int IndexModules = 0; IndexModules < NewPart.Modules.Count; IndexModules++)
                {
                    NewPart.Modules[IndexModules].Save(CN.AddNode("MODULE"));
                }
                Debug.Log("RESOURCE");
                for (int IndexResources = 0; IndexResources < NewPart.Resources.Count; IndexResources++)
                {
                    NewPart.Resources[IndexResources].Save(CN.AddNode("RESOURCE"));
                }

                //CN.AddNode(CompiledNodes);

                return;
            }
            for (int IndexNodes = 0; IndexNodes < CN.nodes.Count; IndexNodes++)
            {
                PristineNodes(CN.nodes[IndexNodes]);
            }
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrXBDAc.chase
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class OrXchaseController : MonoBehaviour
    {
        public static OrXchaseController instance;
		public string debug = "";
        public List<OrXchaseContainer> collection = new List<OrXchaseContainer>();
        
        public void Start()
        {

            OrXchaseDebug.DebugWarning("OrXchaseController.Start()");
            //initialize the singleton.
            instance = this;
                     
            GameEvents.onPartPack.Add(OnPartPack);
            GameEvents.onPartUnpack.Add(OnPartUnpack);
            
            GameEvents.onCrewOnEva.Add(OnCrewOnEva);
            GameEvents.onCrewBoardVessel.Add(OnCrewBoardVessel);
//            GameEvents.onCrewKilled.Add(OnCrewKilled);
//            GameEvents.onVesselWillDestroy.Add(VesselDestroyed);

            GameEvents.onGameStateSave.Add(OnSave);
            GameEvents.onFlightReady.Add(onFlightReadyCallback);
        }

        public void OnDestroy()
        {
            OrXchaseDebug.DebugWarning("OrXchaseController.OnDestroy()");
        
            
            GameEvents.onPartPack.Remove(OnPartPack);
            GameEvents.onPartUnpack.Remove(OnPartUnpack);
        
            GameEvents.onCrewOnEva.Remove(OnCrewOnEva);
            GameEvents.onCrewBoardVessel.Remove(OnCrewBoardVessel);
//            GameEvents.onCrewKilled.Remove(OnCrewKilled);
//            GameEvents.onVesselWillDestroy.Add(VesselDestroyed);

            GameEvents.onGameStateSave.Remove(OnSave);
            GameEvents.onFlightReady.Remove(onFlightReadyCallback);
        }
           
        /// <summary>
        /// Load the list 
        /// </summary>
        private void onFlightReadyCallback()
        {
            //Load the eva list.
            OrXchaseDebug.DebugLog("onFlightReadyCallback()");
            OrXchaseSettings.Load();
        }

        public void OnSave(ConfigNode node)
        {
            //Save the eva list.
            // Might be double.
            foreach (var item in collection)
            {
                OrXchaseSettings.SaveEva(item);
            }

            OrXchaseSettings.Save();
        }

        public void OnPartPack(Part part)
        {
            if (part.vessel.isEVA)
            {
               //save before pack
                OrXchaseDebug.DebugWarning("Pack: " + part.vessel.name);
                                
                Unload(part.vessel, false);
            }
        }

        public void OnPartUnpack(Part part)
        {
            if (part.vessel.isEVA)
            {               
                //save before pack
                OrXchaseDebug.DebugWarning("Unpack: " + part.vessel.name);

                Load(part.vessel);
            }
        }

        /// <summary>
        /// Runs when the kerbal goes on EVA.
        /// </summary>
        /// <param name="e"></param>
        public void OnCrewOnEva(GameEvents.FromToAction<Part, Part> e)
        {
            //add new kerbal
            OrXchaseDebug.DebugLog("OnCrewOnEva()");
            Load(e.to.vessel);
        }

        /// <summary>
        /// Runs when the EVA goes onboard a vessel.
        /// </summary>
        /// <param name="e"></param>
        public void OnCrewBoardVessel(GameEvents.FromToAction<Part, Part> e)
        {
            //remove kerbal
            OrXchaseDebug.DebugLog("OnCrewBoardVessel()");
            Unload(e.from.vessel, true);
        }

        /// <summary>
        /// Runs when the EVA is killed.
        /// </summary>
        /// <param name="report"></param>

        public void OnCrewKilled(EventReport report)
        {
            OrXchaseDebug.DebugLog("OnCrewKilled()");
		KerbalRoster boboo = new KerbalRoster(Game.Modes.SANDBOX);	
		print(boboo[report.sender].name);
		//MonoBehaviour.print(report.origin);
		//MonoBehaviour.print(report.origin.vessel);
            Unload(report.origin.vessel, true);
        }

        public void VesselDestroyed(Vessel report) {
            OrXchaseDebug.DebugLog("VesselDestroyed()");
		if (report.isEVA) Unload(report, true);
        }

        public void Load(Vessel vessel)
        {
            
            if (!vessel.isEVA)
            {
                OrXchaseDebug.DebugWarning("Tried loading a non eva.");
                return;
            }

            KerbalEVA OrXEVA = vessel.GetComponent<KerbalEVA>();
            var currentEVA = vessel.FindPartModuleImplementing<KerbalEVA>();

            if (!Contains(vessel.id))
            {
                OrXchaseContainer container = new OrXchaseContainer(vessel.id);

                //load the vessel here.
                container.Load(currentEVA);
                OrXchaseSettings.LoadEva(container);

                collection.Add(container);
            }
            else
            {
                //Reload
                OrXchaseContainer container = GetEva(vessel.id);

                container.Load(currentEVA);
                OrXchaseSettings.LoadEva(container);
            }
        }

        public void Unload(Vessel vessel, bool delete)
        {
            
            if (!vessel.isEVA)
            {
                OrXchaseDebug.DebugWarning("Tried unloading a non eva.");
                return;
            }
            
            OrXchaseDebug.DebugLog("Unload(" + vessel.name + ")");

            foreach (var item in collection)
            {
                if(item.flightID == vessel.id)
                {
                    if (delete)
                    {
                       item.status = Status.Removed;
                    }

                    //unload the vessel here. 
                    item.Unload();
                    OrXchaseSettings.SaveEva(item);


                    OrXchaseDebug.DebugLog("Remove EVA: (" + vessel.name + ")");
                    collection.Remove(item);
                    break;
                }
            }     
        }

        public bool Contains(Guid id)
        {
            OrXchaseDebug.DebugLog("Contains()");

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].flightID == id)
                    return true;
            }

            return false;
        }

        
        public OrXchaseContainer GetEva(Guid flightID)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].flightID == flightID)
                    return collection[i];
            }

            return null;
        }
    }
}

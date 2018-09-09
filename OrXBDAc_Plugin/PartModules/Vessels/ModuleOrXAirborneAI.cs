
using BDArmory.Modules;

namespace OrXBDAc.parts
{
    public class ModuleOrXAirborne : BDModulePilotAI
    {
        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>


        private bool set = false;

        public bool holeSpawned = false;
        public bool enginesStarted = false;
        public bool denied = false;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!enginesStarted)
                {
                    StartEngines();
                }
                else
                {
                    if (this.vessel.parts.Count <= 10 || this.vessel.Splashed)
                    {
                        this.vessel.DestroyVesselComponents();
                        this.vessel.Die();
                    }
                }
            }
        }

        private void StartEngines()
        {           
            foreach (Part p in vessel.parts)
            {
                var engines = p.FindModuleImplementing<ModuleEngines>();
                var enginesFX = p.FindModuleImplementing<ModuleEnginesFX>();

                if (engines != null)
                {

                    engines.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesStarted = true;
                    engines.Activate();
                }

                if (enginesFX != null)
                {
                    enginesFX.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
                    enginesStarted = true;
                    enginesFX.Activate();
                }
            }

            if (!pilotOn)
            {
                ActivatePilot();
                Play();
            }
        }


        public void Play()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!set)
                {
                    var _random = new System.Random().Next(0, 40);

                    if (_random < 20)
                    {
                        //OrX_Log.instance.sound_SpawnOrXRevenge.Play();
                    }
                    else
                    {
                        //OrX_Log.instance.sound_SpawnOrXThing.Play();
                    }
                    
                    set = true;
                }
            }
        }
    }
}
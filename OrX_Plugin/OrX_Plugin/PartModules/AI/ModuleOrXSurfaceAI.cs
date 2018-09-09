
using BDArmory.Modules;
using System.Collections;
using UnityEngine;

namespace OrX.parts
{
    public class ModuleOrXSurface : BDModuleSurfaceAI
    {
        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>


        private bool set = false;

        public bool enginesStarted = false;

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
                    if (part.vessel.parts.Count <= 5)
                    {
                        part.explode();
                    }
                }
            }
        }

        private void StartEngines()
        {
            enginesStarted = true;

            foreach (Part p in vessel.parts)
            {
                var engines = p.FindModuleImplementing<ModuleEngines>();
                var enginesFX = p.FindModuleImplementing<ModuleEnginesFX>();

                if (engines != null)
                {
                    engines.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));

                    engines.Activate();
                }

                if (enginesFX != null)
                {
                    enginesFX.ActivateAction(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));

                    enginesFX.Activate();
                }
            }

            if (!pilotOn)
            {
                ActivatePilot();
                StartCoroutine(Play());

            }

            enginesStarted = true;

        }



        IEnumerator Play()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!set)
                {
                    set = true;

                    yield return new WaitForSeconds(2);

                    //OrX_Log.instance.sound_SpawnOrXRevenge.Play();

                }
            }
        }
    }
}
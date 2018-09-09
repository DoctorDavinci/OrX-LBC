using OrXBDAc.spawn;
using System.Collections;
using UnityEngine;
using BDArmory.Modules;

namespace OrXBDAc.parts
{
    public class ModuleOrXODST : PartModule
    {
        private bool set = false;

        private bool chutesDeployed = false;
        private bool setup = true;
        public bool holeSpawned = false;
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
                if (this.part.vessel.radarAltitude <= 40)
                {
                    if (setup)
                    {
                        setup = false;
                        Setup();
                    }
                }

                if (this.part.vessel.radarAltitude <= 8000 && !chutesDeployed)
                {
                    Debug.Log("[OrX ODST] Altitude: " + this.part.vessel.radarAltitude);

                    DeployChutes();
                }

                if (this.part.vessel.radarAltitude <= 10)
                {
                    Debug.Log("[OrX ODST] Altitude: " + this.part.vessel.radarAltitude);

                    if (!holeSpawned)
                    {
                        holeSpawned = true;
                        //SpawnOrXHole.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                        //SpawnOrXHole.instance.CheckSpawnTimer();

                        foreach (Part p in this.vessel.parts)
                        {
                            part.explode();
                        }
                    }
                }

                if (vessel.Splashed)
                {
                    SpawnOrXAirborne.instance.SpawnCoords = this.vessel.GetWorldPos3D();
                    SpawnOrXAirborne.instance.CheckSpawnTimer();

                    foreach (Part p in this.vessel.parts)
                    {
                        part.explode();
                    }
                }
            }
        }

        private void DeployChutes()
        {
            chutesDeployed = true;
            Set();
            foreach (Part p in this.vessel.parts)
            {
                var chute = p.FindModuleImplementing<ModuleParachute>();
                if (chute != null)
                {
                    chute.deployAltitude = 8000;
                    chute.spreadAngle = 10;
                    chute.Deploy();
                }
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////
        /// </summary>

        public void Setup()
        {
//            var _hole = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//            flagURL = _hole + "\\OrX_icon.png";
//            hole = _hole + "\\OrXhole.craft";
//            Debug.Log("[OrX ODST - Spawn OrX Hole] craftURL: " + hole);
            setup = false;
        }


        /// <summary>
        /// /////////////////////////
        /// </summary>

        public void Set()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!set)
                {
                    StartCoroutine(SetControl());
                    set = true;
                }

            }
        }

        IEnumerator SetControl()
        {
            var _random = new System.Random().Next(0, 20);

            if (_random < 10)
            {
                //sound_SpawnOrXWhatsThat.Play();
            }

            if (_random <= 20 && _random >= 10)
            {
                //sound_SpawnOrXHolyHole.Play();
            }

            if (_random <= 30 && _random >= 20)
            {
                //sound_SpawnOrXOrders.Play();
            }

            if (_random <= 40 && _random >= 30)
            {
                //sound_SpawnOrXThing.Play();
            }

            if (_random <= 50 && _random >= 40)
            {
                //sound_SpawnOrXNeeds.Play();
            }

            if (_random >= 50)
            {
                //sound_SpawnOrXRevenge.Play();
            }

            yield return new WaitForSeconds(2);

            var wmPart = this.vessel.FindPartModuleImplementing<MissileFire>();

            Debug.Log("[OrX ODST - Craft Control] Found Weapon Manager on " + this.vessel.name + " .......... Setting Team");

            foreach (Part avp in FlightGlobals.ActiveVessel.parts)
            {
                var activeVesselWM = avp.FindModuleImplementing<MissileFire>();

                if (activeVesselWM != null)
                {
                    if (activeVesselWM.team)
                    {
                        if (wmPart.team)
                        {
                            wmPart.ToggleTeam();
                            wmPart.team = false;
                        }
                    }
                    else
                    {
                        if (!wmPart.team)
                        {
                            wmPart.ToggleTeam();
                            wmPart.team = true;
                        }
                    }
                }
                else
                {
                    if (!wmPart.team)
                    {
                        wmPart.ToggleTeam();
                        wmPart.team = true;
                    }
                }
            }

            yield return new WaitForFixedUpdate();

            if (!wmPart.guardMode)
            {
                Debug.Log("[OrX ODST - Craft Control] Turning on Guard mode: " + this.vessel.name + " .......... ");
                wmPart.ToggleGuardMode();
            }
            else
            {
                Debug.Log("[OrX ODST - Craft Control] Guard mode already active on " + this.vessel.name + " .......... Cycling");

                wmPart.ToggleGuardMode();
                yield return new WaitForEndOfFrame();
                wmPart.ToggleGuardMode();
            }
        }
    }
}
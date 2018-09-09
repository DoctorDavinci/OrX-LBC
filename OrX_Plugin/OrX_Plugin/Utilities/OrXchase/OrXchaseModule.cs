using System;
using System.Collections.Generic;
using System.Text;

namespace OrX.chase
{
    /// <summary>
    /// Keep track of the Context Menu.
    /// </summary>
    public class OrXchaseModule : PartModule
    {
        private OrXchaseContainer currentContainer;
        private bool chasing = false;

        public Vector3d target;

        public void Update()
        {
            if (!FlightGlobals.ready || PauseMenu.isOpen)
                return;
            /*
            if (currentContainer == null)
                return;
*/
                //ResetEvents();
                //SetEvents();
            if (part.vessel.speed == 0)
            {
                chasing = false;
            }
        }

        public void Load(OrXchaseContainer current)
        {
            this.currentContainer = current;
        }

        /// <summary>
        /// The default events based on the kerbal status.
        /// </summary>
        public void ResetEvents()
        {
            Events["Follow"].active = false;
            Events["Stay"].active = false;
            Events["SetPoint"].active = false;
            Events["Wait"].active = false;
            Events["Patrol"].active = false;
            Events["EndPatrol"].active = false;
            Events["PatrolRun"].active = false;
            Events["PatrolWalk"].active = false;
			Events["ToggleHelmet"].active = false;
			Events["StartWanderer"].active = false;
        }

        /// <summary>
        /// Set events based on the kerbal status.
        /// </summary>
        public void SetEvents()
        {
            /*
            if (!currentContainer.Loaded)
                return;

			if (!currentContainer.EVA.vessel.Landed) {
				return; 
			}

            if (currentContainer.mode == Mode.None)
            {
                Events["Follow"].active = true;
                Events["Stay"].active = false;
				//Events["StartWanderer"].active = true;
            }
            else if (currentContainer.mode == Mode.Follow)
            {
                Events["Follow"].active = false;
                Events["Stay"].active = true;
            }
			else if (currentContainer.mode == Mode.Patrol)
            {
                if (currentContainer.AllowRunning)
                {
                    Events["PatrolWalk"].active = true;
                }
                else
                {
                    Events["PatrolRun"].active = true;
                }

                Events["Patrol"].active = false;
                Events["EndPatrol"].active = true;
            }
            else if (currentContainer.mode == Mode.Order)
            {
                Events["Stay"].active = true;
                Events["Follow"].active = true;
            }
            
            if (currentContainer.CanTakeHelmetOff)
            {
                Events["ToggleHelmet"].active = true;
            }
            
            if (currentContainer.IsActive)
            {
                Events["Follow"].active = false;
                Events["Stay"].active = false;
                Events["SetPoint"].active = true;
                Events["Wait"].active = true;

                if (currentContainer.mode != Mode.Patrol)
                {
                    if (currentContainer.AllowPatrol)
                    {
                        Events["Patrol"].active = true;
                    }
                }
                else
                {
                    Events["SetPoint"].active = false;
                    Events["Wait"].active = false;
                }
            }*/
        }


        [KSPEvent(guiActive = false, active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Follow()
        {
            targetCoords = FlightGlobals.ActiveVessel.GetWorldPos3D();
            currentContainer.Follow();
        }

        [KSPEvent(guiActive = false, active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Stay()
        {
            currentContainer.Stay();
        }

        [KSPEvent(guiActive = false, active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void SetPoint()
        {
            currentContainer.SetWaypoint();
        }

        [KSPEvent(guiActive = false, active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Wait()
        {
            currentContainer.Wait();
        }
        /// <summary>
        /// ///////////////////////////
        /// 
        /// </summary>
        /// 

        public bool AllowRunning { get; set; }
        public List<PatrolAction> actions = new List<PatrolAction>();
        public int currentPatrolPoint = 0;
        public string referenceBody = "None";
        public Vector3d targetCoords;

        [KSPEvent(guiActive = false, guiName = "Patrol", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Patrol()
        {
            currentContainer.SetWaypoint();
            currentContainer.StartPatrol();
            if (this.referenceBody == "None")
            {
                this.referenceBody = FlightGlobals.ActiveVessel.mainBody.bodyName;
            }
            
            actions.Add(new PatrolAction(PatrolActionType.Move, 0, targetCoords));
        }


        /// <summary>
        /// //////////////////////////////////////////////////
        /// </summary>
        /// 

        [KSPEvent(guiActive = false, guiName = "End Patrol", active = true, guiActiveUnfocused = false, unfocusedRange = 8)]
        public void EndPatrol()
        {
            currentContainer.EndPatrol();
        }

        [KSPEvent(guiActive = false, guiName = "Walk", active = true, guiActiveUnfocused = false, unfocusedRange = 8)]
        public void PatrolWalk()
        {
            currentContainer.SetWalkPatrolMode();
        }

        [KSPEvent(guiActive = false, guiName = "Run", active = true, guiActiveUnfocused = false, unfocusedRange = 8)]
        public void PatrolRun()
        {
            currentContainer.SetRunPatrolMode();
        }

        [KSPEvent(guiActive = false, guiName = "Toggle Helmet", active = true, guiActiveUnfocused = false, unfocusedRange = 8)]
        public void ToggleHelmet()
        {
            currentContainer.ToggleHelmet();
        }

		[KSPEvent(guiActive = false, guiName = "Wander", active = true, guiActiveUnfocused = false, unfocusedRange = 8)]
		public void StartWanderer()
		{
			currentContainer.StartWanderer();
		}

        /*
        [KSPEvent(guiActive = true, guiName = "Debug", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Debug()
        {
            foreach (var item in OrXchaseController.fetch.collection)
            {
                OrXchaseDebug.DebugLog("Item: " + item.flightID);
                OrXchaseDebug.DebugLog("leader: " + item.formation.GetLeader());
                OrXchaseDebug.DebugLog("patrol: " + item.patrol.ToString());
                OrXchaseDebug.DebugLog("order: " + item.order.ToString());
                OrXchaseDebug.DebugLog("patrol: " + item.patrol);
            }

            currentContainer.EVA.headLamp.light.intensity += 100;
        }


        [KSPEvent(guiActive = true, guiName = "Save", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void ClearSave()
        {
            OrXchaseSettings.SaveFunction();
        }


        [KSPEvent(guiActive = true, guiName = "Load", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Load()
        {
            OrXchaseSettings.LoadFunction();
        }
        */

    }

    public class PatrolAction
    {
        public Vector3d position;
        public PatrolActionType type;
        public int delay = 0;

        public PatrolAction()
        {
            this.type = PatrolActionType.Move;
            this.delay = 10;
            this.position = new Vector3d();
        }

        public PatrolAction(PatrolActionType type, int delay, Vector3d position)
        {
            this.type = type;
            this.delay = delay;
            this.position = position;
        }

        public string ToSave()
        {
            return "(" + type.ToString() + "," + delay.ToString() + "," + position.ToString() + ")";
        }

        public void FromSave(string action)
        {
            OrXchaseTokenReader reader = new OrXchaseTokenReader(action);

            string sType = reader.NextTokenEnd(',');
            string sDelay = reader.NextTokenEnd(',');
            string sPosition = reader.NextToken('[', ']');

            type = (PatrolActionType)Enum.Parse(typeof(PatrolActionType), sType);
            delay = int.Parse(sDelay);
            position = Util.ParseVector3d(sPosition, false);
        }

        public override string ToString()
        {
            return "position = " + position.ToString() + ", delay = " + delay + ", type = " + type.ToString();
        }
    }

    [Flags]
    public enum PatrolActionType
    {
        Move,
        Wait,
    }

}

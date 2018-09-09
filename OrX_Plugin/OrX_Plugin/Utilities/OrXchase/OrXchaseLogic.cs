using UnityEngine;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace OrX.chase
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrXchaseLogic : MonoBehaviour
    {
		//List<IDetection> detectionSystems = new List<IDetection>();

		public OrXchaseLogic(){

			//detectionSystems.Add (new DeadSpaceDetection ());
		}

        public void Start()
        {
            OrXchaseDebug.DebugWarning("OrXchaseLogic.Start()");

        }
        public void OnDestroy()
        {
            OrXchaseDebug.DebugWarning("OrXchaseLogic.OnDestroy()");
        }

		public void FixedUpdate(){
			// Update detection systems.
			//foreach (var detection in detectionSystems) {
			//	detection.UpdateMap (OrXchaseController.instance.collection);
			//}
		}

        public void Update()
        {
            if (!FlightGlobals.ready || PauseMenu.isOpen)
                return;

            // Replace this with a check to see if GUI is hidden
            if (Input.GetKeyDown(KeyCode.F2) && OrXchaseSettings.displayDebugLinesSetting) {
                OrXchaseSettings.displayDebugLines = !OrXchaseSettings.displayDebugLines;
                foreach (OrXchaseContainer container in OrXchaseController.instance.collection) {
                    container.togglePatrolLines();
                }
            }

			if (Input.GetKeyDown (KeyCode.B)) {
				foreach (OrXchaseContainer container in OrXchaseController.instance.collection) {
					container.EVA.PackToggle ();
				}
			}
				
            try
            {
                foreach (OrXchaseContainer eva in OrXchaseController.instance.collection.ToArray())
                {
                    if (eva == null)
                    {
                        //is this possible ?
                        OrXchaseDebug.DebugWarning("eva == null");
                        continue;
                    }

                    //skip unloaded vessels
                    if (!eva.Loaded)
                    {
                        continue;
                    }

                    //Turn the lights on when dark.
                    //Skip for now, too buggy..
                    //eva.UpdateLamps();

                    if (eva.mode == Mode.None)
                    {
                        //Nothing to do here.
                        continue;
                    }

                    //Recover from ragdoll, if possible.
                    if (eva.IsRagDoll)
                    {
                        //eva.RecoverFromRagdoll();
                        continue;
                    }

                    Vector3d move = -eva.Position;

                    //Get next Action, Formation or Patrol
                    Vector3d target = eva.GetNextTarget();

                    // Path Finding
                    //todo: check if the target is occopied.
                    move += target;

                    double sqrDist = move.sqrMagnitude;
                    float speed = TimeWarp.deltaTime;

                    if (eva.OnALadder)
                    {
                        eva.ReleaseLadder();
                    }

                    #region Break Free Code

                    if (eva.IsActive)
                    {
                        Mode mode = eva.mode;
                        
                        if (Input.GetKeyDown(KeyCode.W))
                            mode = OrX.chase.Mode.None;
                        if (Input.GetKeyDown(KeyCode.S))
                            mode = OrX.chase.Mode.None;
                        if (Input.GetKeyDown(KeyCode.A))
                            mode = OrX.chase.Mode.None;
                        if (Input.GetKeyDown(KeyCode.D))
                            mode = OrX.chase.Mode.None;
                        if (Input.GetKeyDown(KeyCode.Q))
                            mode = OrX.chase.Mode.None;
                        if (Input.GetKeyDown(KeyCode.E))
                            mode = OrX.chase.Mode.None;

                        if (mode == Mode.None)
                        {
                            //break free!
                            eva.mode = mode;
                            continue;
                        }
                    }
                    #endregion

                    //Animation Logic
                    eva.UpdateAnimations(sqrDist, ref speed);

                    move.Normalize();

                    //Distance Logic
                    eva.CheckDistance(move, speed, sqrDist);

                    //Reset Animation Mode Events
                    eva.CheckModeIsNone();

                }
            }
            catch (Exception exp)
            {
                OrXchaseDebug.DebugWarning("[OrX Chase] OrXchaseLogic: " + exp.Message + ":" + exp.ToString());
            }
        }
    }
}

using System;
using UnityEngine;

namespace OrXBDAc.chase
{
    public class OrXchaseContainer
    {
        public Guid flightID;
        public Mode mode = Mode.None;
        public Status status = Status.None;

        private bool selected = false;
        private bool loaded = false;
        private bool showHelmet = true;

        private KerbalEVA eva;

        public OrXchaseFormation formation = new OrXchaseFormation();
        public OrXchasePatrol patrol = new OrXchasePatrol();
		public OrXchaseOrder order = new OrXchaseOrder();
		public OrXchaseWanderer wanderer = new OrXchaseWanderer();

		const float RunMultiplier = 1.75f;
		const float BoundSpeedMultiplier = 1.25f;

        public void togglePatrolLines() {
			if (OrXchaseSettings.displayDebugLines) {
				patrol.GenerateLine ();
			} else {
//				patrol.Hide ();
			}
        }

        public bool IsActive
        {
            get { return (eva.vessel == FlightGlobals.ActiveVessel); }
        }

        public bool IsRagDoll
        {
            get { return eva.isRagdoll; }
        }

        public bool AllowPatrol
        {
            get { return (patrol.actions.Count >= 1);  }
        }

        public bool AllowRunning
        {
            get {

                if (mode == Mode.Patrol)
                {
                    return patrol.AllowRunning;
                }
                else if (mode == Mode.Order)
                {
                    return order.AllowRunning;
                }

                return false;
            }
        }

        public bool CanTakeHelmetOff
        {
            get { return (FlightGlobals.ActiveVessel.mainBody.bodyName == "Kerbin") && OrXchaseSettings.displayToggleHelmet; }
        }

        public KerbalEVA EVA
        {
            get { return eva; }
        }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Get the world position of the kerbal.
        /// </summary>
        public Vector3d Position
        {
            get { return eva.vessel.GetWorldPos3D(); }
        }

        public bool Loaded
        {
            get {
                bool isLoaded = loaded;

				if (loaded) {
					isLoaded |= eva.isEnabled;
				}

                return isLoaded;
            }
        }

        public string Name { get; set; }

        public bool OnALadder { get { return eva.OnALadder; } }


        public OrXchaseContainer(Guid flightID)
        {
            this.flightID = flightID;
            this.loaded = false;
        }

        public void Load(KerbalEVA eva)
        {
            //Load KerbalEVA.
            this.eva = eva;
            loaded = true;

            //Set Name
            this.Name = eva.name;

            //module on last.
            OrXchaseModule module = (OrXchaseModule)eva.GetComponent(typeof(OrXchaseModule));
            module.Load(this);

            OrXchaseDebug.DebugWarning("OrXchaseContainer.Load("+eva.name+")");
        }

        public void Unload()
        {
            OrXchaseDebug.DebugWarning("OrXchaseContainer.Unload(" + eva.name + ")");
            loaded = false;
        }

        public string ToSave()
        {
            return (flightID + "," + Name + "," + mode + "," + status + "," + selected + "," + showHelmet + ","
                + this.formation.ToSave() + ","
                + this.patrol.ToSave() + ","
				+ this.order.ToSave()  + ","
				+ this.wanderer.ToSave()			
			);
        }

        public void FromSave(string OrXchaseSettings)
        {
            OrXchaseTokenReader reader = new OrXchaseTokenReader(OrXchaseSettings);

            try
            {
                string sflightID = reader.NextTokenEnd(',');
                string sName = reader.NextTokenEnd(',');
                string mode = reader.NextTokenEnd(',');
                string status = reader.NextTokenEnd(',');
                string selected = reader.NextTokenEnd(',');
                string showHelmet = reader.NextTokenEnd(',');

                string formation = reader.NextToken('(', ')'); reader.Consume();
                string patrol = reader.NextToken('(', ')'); reader.Consume();
				string order = reader.NextToken('(', ')'); reader.Consume();
				string wanderer = reader.NextToken('(', ')');

                this.Name = sName;
                this.mode = (Mode)Enum.Parse(typeof(Mode), mode);
                this.status = (Status)Enum.Parse(typeof(Status), status);
                this.selected = bool.Parse(selected);
                this.showHelmet = bool.Parse(showHelmet);


                this.formation.FromSave(formation);
                this.patrol.FromSave(patrol);
                this.order.FromSave(order);
				this.wanderer.FromSave(wanderer);

                OrXchaseDebug.DebugLog("Loaded: " + mode);
                OrXchaseDebug.DebugLog("name: " + sName);
                OrXchaseDebug.DebugLog("status: " + status);
                OrXchaseDebug.DebugLog("selected: " + selected);

                if (this.showHelmet == false)
                {
                    eva.ShowHelmet(this.showHelmet);
                }
            }
            catch
            {
                throw new Exception("[OrX Chase] FromSave Failed.");
            }
        }


        public void Follow()
        {
            Guid flightID = (FlightGlobals.fetch.activeVessel).id;
            OrXchaseContainer leader = OrXchaseController.instance.GetEva(flightID);

            selected = false;
            mode = Mode.Follow;
            formation.SetLeader(leader);
        }

        public void Stay()
        {
            mode = Mode.None;
        }

        public void SetWaypoint()
        {
            patrol.Move(FlightGlobals.ActiveVessel);
        }

        public void Wait()
        {
            patrol.Wait(eva.vessel);
        }

        public void StartPatrol()
        {
            
            if (patrol.AllowRunning)
            {
                mode = Mode.Patrol;
            }
            else
            {
                patrol.AllowRunning = true;
                mode = Mode.Patrol;
            }
        }

        public void EndPatrol()
        {
            mode = Mode.None;
            patrol.Clear();
            eva.Animate(AnimationState.Idle);
        }

        public void SetRunPatrolMode()
        {
            patrol.AllowRunning = true;
        }

        public void SetWalkPatrolMode()
        {
            patrol.AllowRunning = false;
        }

		public void StartWanderer()
		{
			mode = Mode.Wander;
			//wanderer.SetEva (eva);
		}

        public void ToggleHelmet()
        {
//            this.showHelmet = !showHelmet;
//            eva.ShowHelmet(this.showHelmet);
        }

        public void UpdateLamps()
        {
            bool lampOn = Util.IsDark(eva.transform);

            if (showHelmet)
            {
                eva.TurnLamp(lampOn);
            }

			if (!showHelmet && eva.lampOn)
            {               
               eva.TurnLamp(false);
            }
        }

        public void RecoverFromRagdoll()
        {
            if (eva != null)
            {
                eva.RecoverFromRagdoll();
            }
        }

        public Vector3d GetNextTarget()
        {
            if (mode == Mode.Follow)
            {
				var target = formation.GetNextTarget();

				if (target == Vector2d.zero) {
					mode = Mode.None;
				}

				return target;
            }
            else if (mode == Mode.Patrol)
            {
                return patrol.GetNextTarget();
            }
            else if (mode == Mode.Order)
            {
                return order.GetNextTarget();
            }
			else if (mode == Mode.Wander)
			{
				return wanderer.GetNextTarget();
			}

            //Error
            throw new Exception("[OrX Chase] New Mode Introduced");
        }

        public void ReleaseLadder()
        {
            eva.ReleaseLadder();
        }

        public void UpdateAnimations(double sqrDist, ref float speed)
        {
            if (eva.part != null)
            {
                double geeForce = FlightGlobals.currentMainBody.GeeASL;

                if (eva.part.WaterContact)
                {
                    speed *= eva.swimSpeed;
                    eva.Animate(AnimationState.Swim);
                }
                else if (eva.JetpackDeployed)
                {
                    speed *= 1f;
                    eva.Animate(AnimationState.Idle);
                }
                else if (geeForce >= eva.minRunningGee)//sqrDist > 5f &&
                {
                    if (AllowRunning)
                    {
                        speed *= eva.runSpeed;
                        eva.Animate(AnimationState.Run);
                    }
                    else if (sqrDist > 4 && mode == Mode.Follow)
                    {
                        speed *= eva.runSpeed * RunMultiplier;
                        eva.Animate(AnimationState.Run);
                    }
                    else if (sqrDist > 8f && mode == Mode.Follow)
                    {
                        speed *= eva.runSpeed * RunMultiplier;
                        eva.Animate(AnimationState.Run);
                    }
                    else
                    {
                        speed *= eva.walkSpeed;
                        eva.Animate(AnimationState.Walk);
                    }
                }
                else if (geeForce >= eva.minWalkingGee)
                {
                    speed *= eva.walkSpeed;
                    eva.Animate(AnimationState.Walk);
                }
                else
                {
                    speed *= eva.boundSpeed * BoundSpeedMultiplier; //speedup
                    eva.Animate(AnimationState.BoundSpeed);
                }
            }
        }

        public void CheckDistance(Vector3d move, float speed, double sqrDist)
        {
            if (eva.part != null)
            {
                IOrXchaseControlType controlType = null;

                if (mode == Mode.Follow) { controlType = formation; }
                else if (mode == Mode.Patrol) { controlType = patrol; }
                else if (mode == Mode.Order) { controlType = order; }
                else if (mode == Mode.Wander) { controlType = wanderer; }

                if (controlType.CheckDistance(sqrDist))
                {
                    eva.Animate(AnimationState.Idle);

                    if (controlType is OrXchaseOrder)
                    {
                        mode = Mode.None;
                    }
                }
                else
                {
                    if (AbleToMove())
                    {
                        Move(move, speed);
                    }
                }
            }
/*            else
            {
                IOrXchaseControlType controlType = null;

                if (mode == Mode.Follow) { controlType = formation; }
                else if (mode == Mode.Patrol) { controlType = patrol; }
                else if (mode == Mode.Order) { controlType = order; }
                else if (mode == Mode.Wander) { controlType = wanderer; }

                if (controlType.CheckDistance(sqrDist))
                {
                    eva.Animate(AnimationState.Idle);

                    if (controlType is OrXchaseOrder)
                    {
                        mode = Mode.None;
                    }
                }
                else
                {
                    if (AbleToMove())
                    {
                        Move(move, speed);
                    }
                }
            }*/
        }

        private bool AbleToMove()
        {
			Rigidbody rigidbody = null;
			eva.GetComponentCached<Rigidbody>(ref rigidbody);

            if (rigidbody != null)
            {
                return (!eva.isEnabled) | (!eva.isRagdoll) | (!rigidbody.isKinematic);
            }
            else
            {
                Debug.LogError("[OrX Chase Container] RigidBody not found .......... ERROR");
                return (false) | (true) | (true);
            }
        }

        /// <summary>
        /// Move the current kerbal to target.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="speed"></param>
        public void Move(Vector3d move, float speed)
        {
            #region Move & Rotate Kerbal

            //speed values
            move *= speed;

            //rotate
            if (move != Vector3d.zero)
            {
                if (eva.JetpackDeployed)
                {
                    eva.PackToggle();
                }
                else
                {
                    //rotation
                    Quaternion from = eva.part.vessel.transform.rotation;
                    Quaternion to = Quaternion.LookRotation(move, eva.fUp);
                    Quaternion result = Quaternion.RotateTowards(from, to, eva.turnRate);

                    eva.part.vessel.SetRotation(result);

					Rigidbody rigidbody = null;
					eva.GetComponentCached<Rigidbody>(ref rigidbody);

                    //move
					if(rigidbody != null){
						rigidbody.MovePosition(rigidbody.position + move);
					}
                }
            }

            #endregion
        }

        public void CheckModeIsNone()
        {
            if(mode == Mode.None)
                eva.Animate(AnimationState.Idle);
        }

        public void Order(Vector3d position, Vector3d vector3d)
        {
            order.Move(position, vector3d);
        }




    }
}

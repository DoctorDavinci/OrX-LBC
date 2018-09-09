using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Xml;
using OrX;
using BDArmory;

namespace OrX.chase
{
    /// <summary>
    /// The object responsible for Patroling the kerbal.
    /// </summary>
    public class OrXchasePatrol : IOrXchaseControlType
    {
        public bool AllowRunning { get; set; }
        public List<PatrolAction> actions = new List<PatrolAction>();
        public int currentPatrolPoint = 0;
        public string referenceBody = "None";
        public Vector3d target;
        private float delta = 0;

        public bool CheckDistance(double sqrDistance)
        {
            bool complete = (sqrDistance < 1);

            if (complete)
            {
                Move(FlightGlobals.ActiveVessel);
                SetNextPoint();
                /*
                PatrolAction currentPoint = actions[currentPatrolPoint];

                if (currentPoint.type == PatrolActionType.Wait)
                {
                    delta += Time.deltaTime;

                    if (delta > currentPoint.delay)
                    {
                        SetNextPoint();
                        delta = 0;
                    }
                }
                else //move
                {
                    SetNextPoint();
                }*/
            }

            return complete;
        }

        private void SetNextPoint()
        {
            ++currentPatrolPoint;

            if (currentPatrolPoint >= actions.Count)
                currentPatrolPoint = 0;
        }

        public Vector3d GetNextTarget()
        {
            Vector3d position = FlightGlobals.ActiveVessel.GetWorldPos3D();
            return position;
        }
        
        public void Move(Vessel vessel)
        {
            if (this.referenceBody == "None")
            {
                this.referenceBody = vessel.mainBody.bodyName;
            }

            Vector3d position = Util.GetWorldPos3DSave(vessel);
            actions.Add(new PatrolAction(PatrolActionType.Move, 1, position));

        }

        public void Wait(Vessel vessel)
        {
            SetReferenceBody();

            Vector3d position = vessel.GetWorldPos3D(); //Util.GetWorldPos3DSave(vessel);
            actions.Add(new PatrolAction(PatrolActionType.Wait, 1, position));
        }

        private void SetReferenceBody()
        {
            if (this.referenceBody == "None")
            {
                this.referenceBody = FlightGlobals.ActiveVessel.mainBody.bodyName;
            }
        }

        public void Clear()
        {
            referenceBody = "None";

            currentPatrolPoint = 0;
            actions.Clear();

//            if (OrXchaseSettings.displayDebugLines)
//                lineRenderer.SetVertexCount(0);

        }
        /*
        public void Hide() {
            lineRenderer.SetVertexCount(0);
        }*/


		public string ToSave()
        {
            string actionList = "{";
            for (int i = 0; i < actions.Count; i++)
			{
                actionList += actions[i].ToSave();
			}
            actionList += "}";

            string[] args = new string[] {
                AllowRunning.ToString(),
                currentPatrolPoint.ToString(),
                referenceBody.ToString(),
                actionList
            };

            return string.Format("({0}, {1}, {2}, {3})", args);
        }

		public void FromSave(string patrol)
        {
            try
            {
                //OrXchaseDebug.DebugWarning("Patrol.FromSave()");
                OrXchaseTokenReader reader = new OrXchaseTokenReader(patrol);

                string sAllowRunning = reader.NextTokenEnd(',');
                string sCurrentPatrolPoint = reader.NextTokenEnd(',');
                string sReferenceBody = reader.NextTokenEnd(',');
                string sPointlist = reader.NextToken('{', '}');

                AllowRunning = bool.Parse(sAllowRunning);
                currentPatrolPoint = int.Parse(sCurrentPatrolPoint);
                referenceBody = sReferenceBody;

                actions.Clear();

                if (!string.IsNullOrEmpty(sPointlist))
                {
                    reader = new OrXchaseTokenReader(sPointlist);

                    while (!reader.EOF)
                    {
                        PatrolAction action = new PatrolAction();

                        string token = reader.NextToken('(', ')');
                        action.FromSave(token);

                        actions.Add(action);
                    }


                    if (OrXchaseSettings.displayDebugLines)
                        GenerateLine();
                }
            }
            catch
            {
                throw new Exception("[OrX Chase] Patrol.FromSave Failed.");
            }
        }


        public void GenerateLine()
        {
//            lineRenderer.SetVertexCount(actions.Count+1);

            for (int i = 0; i < actions.Count; i++)
            {
                lineRenderer.SetPosition(i, Util.GetWorldPos3DLoad(actions[i].position));
            }
            lineRenderer.SetPosition(actions.Count, Util.GetWorldPos3DLoad(actions[0].position));
        }


        LineRenderer lineRenderer;

        private void setLine(Vector3d position)
        {
//            lineRenderer.SetVertexCount(actions.Count);
            lineRenderer.SetPosition(actions.Count - 1, Util.GetWorldPos3DLoad(position));
        }

        public OrXchasePatrol()
        {
            if (OrXchaseSettings.displayDebugLines)
            {
                lineRenderer = new GameObject().AddComponent<LineRenderer>();

                lineRenderer.useWorldSpace = false;
                lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
//                lineRenderer.SetWidth(0.05f, 0.05f);
//                lineRenderer.SetColors(Color.green, Color.red);

				Renderer _renderer = null;
				lineRenderer.GetComponentCached<Renderer> (ref _renderer);

				if (_renderer != null) {
					_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					_renderer.receiveShadows = false;
					_renderer.enabled = true;
				}
//                lineRenderer.SetVertexCount(0);
            }
        }
    }
    /*
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
            return "(" + type.ToString() + "," + delay.ToString() + "," + position.ToString() +  ")";
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
    */
}

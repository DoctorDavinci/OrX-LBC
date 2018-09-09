using System;
using System.Xml;

namespace OrXBDAc.chase
{
    /// <summary>
    /// The object responsible for Formations.
    /// </summary>
    public class OrXchaseFormation : IOrXchaseControlType
    {
        private OrXchaseContainer leader;

        /// <summary>
        /// Get the next position to walk to. 
        /// Formation should handle differents positions.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public Vector3d GetNextTarget()
        {           
            if (leader == null)
            {
                return Vector3d.zero;
            }
            
            //get the leader. 
            var target = FlightGlobals.ActiveVessel.GetWorldPos3D();        
            
            //update move vector.
            return target;
        }

        public void SetLeader(OrXchaseContainer leader)
        {
            this.leader = leader;
        }

        public string GetLeader()
        {
            if (leader != null)
            {
                if (leader.Loaded)
                {
                    return FlightGlobals.ActiveVessel.vesselName;
                }
            }

            return "None";
        }

        /// <summary>
        /// Check if the distance to the target is reached.
        /// </summary>
        /// <param name="sqrDistance"></param>
        /// <returns></returns>
        public bool CheckDistance(double sqrDistance)
        {
            if (sqrDistance < 1)
            {
                return true;
            }
            return false;
        }

        
        public string ToSave()
        {
            string leaderID = "null";
            if(leader != null)
            {
                leaderID = leader.flightID.ToString();
            }
            return "(Leader:" + leaderID + ")";
        }

		public void FromSave(string formation)
        {
            try
            {
                //OrXchaseDebug.DebugWarning("Formation.FromSave()");
                formation = formation.Remove(0, 7); //Leader:
                
                if (formation != "null")
                {
                    Guid flightID = new Guid(formation);
                    OrXchaseContainer container = OrXchaseController.instance.GetEva(flightID);

                    if (container != null)
                    {
                        leader = container;
                    }
                }
            }
            catch
            {
                throw new Exception("[OrX Chase] Formation.FromSave Failed.");
            }  
        }
    }
}

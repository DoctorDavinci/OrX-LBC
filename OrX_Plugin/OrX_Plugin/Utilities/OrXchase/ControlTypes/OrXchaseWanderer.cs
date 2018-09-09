using System;
using OrX;
using OrX.parts;

namespace OrX.chase
{
	public class OrXchaseWanderer : IOrXchaseControlType
	{     
		static Random random = new Random();

		public Vector3d Position;
		public KerbalEVA eva;
		public float elapsed = 0;
		public string referenceBody;

		bool busy = false;

		public OrXchaseWanderer ()
		{
		}

		public void SetEva (KerbalEVA eva)
		{
			this.eva = eva;
			GenerateNewPosition ();
		}
			
		public bool CheckDistance (double sqrDistance)
		{
			if (sqrDistance < 1)
			{
				busy = false;
				return true;
			}
			return false;
		}

		public Vector3d GetNextTarget ()
		{
			if (!busy) {
				GenerateNewPosition ();
			}

			return Position;
		}

		public string ToSave()
		{
			return "(" + ")";
		}

		private void GenerateNewPosition(){
			Vector3d position = eva.vessel.CoMD;

            //Vector3d eastUnit = eva.vessel.mainBody.getRFrmVel(position).normalized; //uses the rotation of the body's frame to determine "east"
            //Vector3d upUnit = (eva.vessel - eva.vessel.mainBody.position).normalized;
            //Vector3d northUnit = Vector3d.Cross(upUnit, eastUnit); //north = up cross east

            var count = 0.0f;
            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (!v.HoldPhysics)
                {
                    var enemy = v.FindPartModuleImplementing<ModuleOrX>();

                    if (!enemy.orx)
                    {
                        double targetDistance = Vector3d.Distance(position, v.GetWorldPos3D());

                        if (targetDistance <= 2500 && count <= 1)
                        {
                            count += 1;
                            x = v.latitude;
                            y = v.longitude;
                            z = v.altitude;
                        }
                    }
                }
            }

            if (count >= 0)
            {
                var offset = new Vector3d(x, y, z);
                var str = Environment.NewLine + eva.vessel.transform.up.ToString();
                str += Environment.NewLine + eva.vessel.transform.forward.ToString();
                str += Environment.NewLine + eva.vessel.transform.right.ToString();

                //OrXchaseController.instance.debug = str;

                Position = position;
                Position += offset;
            }
        }

		private void SetReferenceBody()
		{
			if (this.referenceBody == "None")
			{
				this.referenceBody = FlightGlobals.ActiveVessel.mainBody.bodyName;
			}
		}

		public void FromSave(string action){

		}
	}
}


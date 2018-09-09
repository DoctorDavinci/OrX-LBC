﻿using System.Xml;
using System;

namespace OrX.chase
{
    /// <summary>
    /// The object responsible for ordering the kerbal around. 
    /// </summary>
    public class OrXchaseOrder : IOrXchaseControlType
    {
        public Vector3d Offset = -Vector3d.one;
        public Vector3d Position;

        public bool AllowRunning { get; set; }

        public OrXchaseOrder()
        {
            AllowRunning = true;
        }

        public bool CheckDistance(double sqrDistance)
        {
            bool complete = (sqrDistance < 1);            
            return complete;
        }

        public Vector3d GetNextTarget()
        {
            return Position;
        }

        public void Move(Vector3d pos, Vector3d off)
        {
            this.Offset = off;
            this.Position = pos;
        }

        public override string ToString()
        {
            return Position + ": offset(" + Offset + ")";
        }

		public string ToSave()
        {
            return "(" + AllowRunning.ToString() + "," + Position + "," + Offset + ")";
        }

		public void FromSave(string order)
        {
            //OrXchaseDebug.DebugWarning("Order.FromSave()");
            OrXchaseTokenReader reader = new OrXchaseTokenReader(order);

            string sAllowRunning = reader.NextTokenEnd(',');
            string sPosition = reader.NextToken('[', ']'); reader.Consume(); // , 
            string sOffset = reader.NextToken('[', ']');

            AllowRunning = bool.Parse(sAllowRunning);
            Position = Util.ParseVector3d(sPosition, false);
            Offset = Util.ParseVector3d(sOffset, false);                        
        }
    }
}


using UnityEngine;

namespace OrX
{
    public struct OrXHoloCacheinfo
    {
        public Vector3d gpsCoordinates;
        public string name;
        public Vessel gpsVessel;

        public Vector3d worldPos
        {
            get
            {
                if (!FlightGlobals.currentMainBody)
                    return Vector3d.zero;

                return GetWorldSurfacePostion(gpsCoordinates, FlightGlobals.currentMainBody);
            }
        }

        public static Vector3 GetWorldSurfacePostion(Vector3d geoPosition, CelestialBody body)
        {
            if (!body)
            {
                return Vector3.zero;
            }
            return body.GetWorldSurfacePosition(geoPosition.x, geoPosition.y, geoPosition.z);
        }


        public OrXHoloCacheinfo(Vector3d coords, string name, Vessel vessel = null)
        {
            gpsVessel = vessel;
            gpsCoordinates = coords;
            this.name = name;
        }


        public bool EqualsTarget(OrXHoloCacheinfo other)
        {
            return name == other.name && gpsCoordinates == other.gpsCoordinates && gpsVessel == other.gpsVessel;
        }
    }
}
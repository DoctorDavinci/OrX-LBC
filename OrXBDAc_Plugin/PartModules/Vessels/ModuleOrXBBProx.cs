﻿using BDArmory.Modules;
using System.Collections;
using UnityEngine;
using OrXBDAc.missions;
using System.Collections.Generic;

namespace OrXBDAc.parts
{
    public class ModuleOrXBBProx : MissileFire
    {
        private bool detecting = false;
        private bool start = false;
        private bool chasing = false;
        private double speed = 0;

        private float proximity = 15;
        private int count = 0;
        private bool detonating = false;

        public BDExplosivePart mine;
        private BDExplosivePart GetMine()
        {
            BDExplosivePart m = null;

            m = part.FindModuleImplementing<BDExplosivePart>();

            return m;
        }

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
                mine = GetMine();
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (this.vessel.radarAltitude <= 20)
                {
                    if (!chasing)
                    {
                        chasing = true;
                        Chase();
                    }

                    if (!detonating && !detecting)
                    {
                        detecting = true;
                        StartCoroutine(ProxDetect());
                    }
                }
            }
        }

        private void Chase()
        {
            if (FlightGlobals.ActiveVessel.isEVA)
            {
                speed = 3 + KerbinMissions.instance.level * 1;
                mine.tntMass = 1;
                var heading = (FlightGlobals.ActiveVessel.GetWorldPos3D() - this.part.vessel.GetWorldPos3D()).normalized;
                this.part.GetComponent<Rigidbody>().velocity = heading * speed;
            }
            else
            {
                if (FlightGlobals.ActiveVessel.srfSpeed >= 5)
                {
                    speed = (FlightGlobals.ActiveVessel.srfSpeed * 0.7f) + (KerbinMissions.instance.level * 0.1f);
                    mine.tntMass = 5 + KerbinMissions.instance.level;
                    var heading = (FlightGlobals.ActiveVessel.GetWorldPos3D() - this.part.vessel.GetWorldPos3D()).normalized;
                    this.part.GetComponent<Rigidbody>().velocity = heading * speed;
                }
                else
                {
                    speed = 4 + KerbinMissions.instance.level * 0.1f;
                    mine.tntMass = 5 + KerbinMissions.instance.level;
                    var heading = (FlightGlobals.ActiveVessel.GetWorldPos3D() - this.part.vessel.GetWorldPos3D()).normalized;
                    this.part.GetComponent<Rigidbody>().velocity = heading * speed;
                }
            }

            chasing = false;
        }

        IEnumerator ProxDetect()
        {
            double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());

            if (targetDistance <= 10)
            {
                mine = GetMine();

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    speed = 1.5f + KerbinMissions.instance.level * 0.1;
                    mine.tntMass = 1;
                    StartCoroutine(DetonateMineRoutine());
                    Debug.Log("[OrX Bonus Ball] Waldo Attack " + this.vessel.vesselName + " Found Kerbal ... Detonating");
                }
                else
                {
                    if (FlightGlobals.ActiveVessel.srfSpeed >= 5)
                    {
                        speed = (FlightGlobals.ActiveVessel.srfSpeed * 0.9f) + (KerbinMissions.instance.level * 0.1f);
                        mine.tntMass = 5 + (KerbinMissions.instance.level * 0.1f);
                        StartCoroutine(DetonateMineRoutine());
                        Debug.Log("[OrX Bonus Ball] Waldo Attack " + this.vessel.vesselName + " Found Vessel ... Detonating");
                    }
                    else
                    {
                        speed = 4 + KerbinMissions.instance.level * 0.1f;
                        mine.tntMass = 5 + (KerbinMissions.instance.level * 0.1f);
                        StartCoroutine(DetonateMineRoutine());
                        Debug.Log("[OrX Bonus Ball] Waldo Attack " + this.vessel.vesselName + " Found Vessel ... Detonating");
                    }
                }
            }

            yield return new WaitForSeconds(2);
            detecting = false;
        }

        IEnumerator DetonateMineRoutine()
        {
            detonating = true;
            mine = GetMine();
            mine.ArmAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            AddForce();
            yield return new WaitForFixedUpdate();
            mine.DetonateAG(new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate));
            part.explode();
        }

        private void AddForce()
        {
            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                double targetDistance = Vector3d.Distance(this.vessel.GetWorldPos3D(), v.Current.GetWorldPos3D());

                if (targetDistance <= 25)
                {
                    var heading = (v.Current.GetWorldPos3D() - this.part.vessel.GetWorldPos3D()).normalized;
                    v.Current.GetComponent<Rigidbody>().velocity = heading * (targetDistance / speed * v.Current.totalMass);
                }
            }
            v.Dispose();
        }
    }
}

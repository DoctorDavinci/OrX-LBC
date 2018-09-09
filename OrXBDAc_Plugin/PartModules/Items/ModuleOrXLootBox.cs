
using System.Collections;
using UnityEngine;
using OrXBDAc.spawn;
using OrXBDAc.missions;

namespace OrXBDAc.parts
{
    public class ModuleOrXLootBox : PartModule
    {
        public float level = 0;
        public bool orxSpawned = false;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                this.part.force_activate();
                level = KerbinMissions.instance.level;
                this.vessel.name = "OrX Loot Box";
                LevelCheck();
            }
        }

        public void CheckLevel()
        {
            if (level <= KerbinMissions.instance.level - 3)
            {
                Explode();
            }
            else
            {
                StartCoroutine(LevelCheck());
            }
        }

        public void Explode()
        {
            if (level <= KerbinMissions.instance.level - 3)
            {
                StartCoroutine(ExplodeRoutine());
            }
        }

        IEnumerator LevelCheck()
        {
            yield return new WaitForSeconds(2);
            if (!orxSpawned)
            {
                orxSpawned = true;
                OrXSpawn.instance._lat = this.vessel.latitude;
                OrXSpawn.instance._lon = this.vessel.longitude;
                OrXSpawn.instance._alt = this.vessel.altitude;

                if (KerbinMissions.instance.level == 3)
                {
                    OrXSpawn.instance.SpawnBrute();
                }

                if (KerbinMissions.instance.level == 5)
                {
                    OrXSpawn.instance.SpawnWaldo();
                }

                if (KerbinMissions.instance.level == 7)
                {
                    StartCoroutine(SpawnBruteRoutine());
                }

                if (KerbinMissions.instance.level == 9)
                {
                    OrXSpawn.instance.SpawnBrute();
                    StartCoroutine(SpawnBruteRoutine());
                }

                if (KerbinMissions.instance.level == 10)
                {
                    OrXSpawn.instance.SpawnWaldo();
                    StartCoroutine(SpawnWaldoRoutine1());
                }

                if (KerbinMissions.instance.level == 12)
                {
                    OrXSpawn.instance.SpawnStayPunkd();
                    StartCoroutine(SpawnBruteRoutine());
                }

                if (KerbinMissions.instance.level == 14)
                {
                    OrXSpawn.instance.SpawnStayPunkd();
                    StartCoroutine(SpawnBruteRoutine());
                }

                if (KerbinMissions.instance.level == 15)
                {
                    OrXSpawn.instance.SpawnWaldo();
                    StartCoroutine(SpawnWaldoRoutine2());
                }

                if (KerbinMissions.instance.level == 18)
                {
                    OrXSpawn.instance.SpawnWaldo();
                    StartCoroutine(SpawnWaldoRoutine3());
                }

                if (KerbinMissions.instance.level == 20)
                {
                    OrXSpawn.instance.SpawnWaldo();
                    StartCoroutine(SpawnWaldoRoutine4());
                }
            }
            else
            {
                Explode();
            }
        }

        IEnumerator ExplodeRoutine()
        {
            int _random = new System.Random().Next(5, 8);
            yield return new WaitForSeconds(_random);
            this.part.explode();
        }

        IEnumerator SpawnBruteRoutine()
        {
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
        }

        IEnumerator SpawnWaldoRoutine1()
        {
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnStayPunkd();
        }

        IEnumerator SpawnWaldoRoutine2()
        {
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
        }

        IEnumerator SpawnWaldoRoutine3()
        {
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
        }

        IEnumerator SpawnWaldoRoutine4()
        {
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(3);
            OrXSpawn.instance.SpawnStayPunkd();
        }
    }
}
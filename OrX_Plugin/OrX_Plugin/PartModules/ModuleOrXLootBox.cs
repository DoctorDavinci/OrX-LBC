
using System.Collections;
using UnityEngine;
using OrX.spawn;
using OrX.missions;

namespace OrX.parts
{
    public class ModuleOrXLootBox : PartModule
    {
        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>

        public float level = 0;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                this.part.force_activate();
                level = Missions.instance.level;
                this.vessel.name = "OrX Loot Box";
                LevelCheck();
            }
        }

        public void CheckLevel()
        {
            LevelCheck();
        }

        public void Explode()
        {
            StartCoroutine(ExplodeRoutine());
        }

        IEnumerator ExplodeRoutine()
        {
            int _random = new System.Random().Next(5, 8);
            yield return new WaitForSeconds(_random);
            this.part.explode();
        }

        IEnumerator SpawnBruteRoutine()
        {
            yield return new WaitForSeconds(5);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(5);
            OrXSpawn.instance.SpawnBrute();
        }

        IEnumerator SpawnWaldoRoutine1()
        {
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnStayPunkd();
        }

        IEnumerator SpawnWaldoRoutine2()
        {
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
        }

        IEnumerator SpawnWaldoRoutine3()
        {
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();

        }

        IEnumerator SpawnWaldoRoutine4()
        {
            OrXSpawn.instance.SpawnStayPunkd();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnBrute();
            yield return new WaitForSeconds(2);
            OrXSpawn.instance.SpawnStayPunkd();

        }

        private void LevelCheck()
        {
            if (Missions.instance.level == 3)
            {
                OrXSpawn.instance.SpawnBrute();
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }

            if (Missions.instance.level == 5)
            {
                OrXSpawn.instance.SpawnWaldo();
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 7)
            {
                StartCoroutine(SpawnBruteRoutine());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 9)
            {
                OrXSpawn.instance.SpawnBrute();
                StartCoroutine(SpawnBruteRoutine());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 10)
            {
                OrXSpawn.instance.SpawnWaldo();
                StartCoroutine(SpawnWaldoRoutine1());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 12)
            {
                OrXSpawn.instance.SpawnStayPunkd();
                StartCoroutine(SpawnBruteRoutine());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 14)
            {
                OrXSpawn.instance.SpawnStayPunkd();
                StartCoroutine(SpawnBruteRoutine());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 15)
            {
                OrXSpawn.instance.SpawnWaldo();
                StartCoroutine(SpawnWaldoRoutine2());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 18)
            {
                OrXSpawn.instance.SpawnWaldo();
                StartCoroutine(SpawnWaldoRoutine3());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }


            if (Missions.instance.level == 20)
            {
                OrXSpawn.instance.SpawnWaldo();
                StartCoroutine(SpawnWaldoRoutine4());
            }
            else
            {
                if (level <= Missions.instance.level - 3)
                {
                    Explode();
                }
            }

        }
    }
}
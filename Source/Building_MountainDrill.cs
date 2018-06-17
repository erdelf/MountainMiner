using RimWorld;
using Verse;

namespace MountainMiner
{
    internal class Building_MountainDrill : Building
    {
        private CompPowerTrader powerComp;

        private float progress;

        public float Progress
        {
            get => this.progress;
            set => this.progress = value;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map: map, respawningAfterLoad: respawningAfterLoad);
            this.powerComp = this.GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(value: ref this.progress, label: "MountainProgress");
        }

        public void Drill(float miningPoints)
        {
            this.progress += miningPoints;
            if (UnityEngine.Random.Range(min: 0, max: 1000) == 0)
            {
                this.ProduceLump();
            }
        }

        public void DrillWorkDone(Pawn driller)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map: this.Map))
                {
                    this.Map.roofGrid.SetRoof(c: intVec, def: RoofDefOf.RoofRockThin);
                }
            }
            this.Progress = 0f;
        }

        private void ProduceLump()
        {
            if (!this.TryGetNextResource(resDef: out ThingDef resDef, cell: out IntVec3 _)) return;
            Thing thing = ThingMaker.MakeThing(def: resDef);
            GenPlace.TryPlaceThing(thing: thing, center: this.InteractionCell, map: this.Map, mode: ThingPlaceMode.Near);
        }

        public bool TryGetNextResource(out ThingDef resDef, out IntVec3 cell)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (!intVec.InBounds(map: this.Map)) continue;
                ThingDef thingDef = DefDatabase<ThingDef>.GetNamed(defName: "Chunk" + this.Map.terrainGrid.TerrainAt(c: intVec).defName.Split('_')[0], errorOnFail: false);
                //GenStep_RocksFromGrid.RockDefAt(intVec);
                if (thingDef == null)
                {
                    if (!Find.World.NaturalRockTypesIn(tile: this.Map.areaManager.Home.ID).TryRandomElement(result: out thingDef))
                        thingDef = ThingDef.Named(defName: "Sandstone");
                    thingDef = ThingDef.Named(defName: "Chunk" + thingDef);
                }
                //Log.Message(GenStep_RocksFromGrid.RockDefAt(intVec).defName);

                resDef = thingDef;
                cell   = intVec;
                return true;
            }
            resDef = null;
            cell = IntVec3.Invalid;
            return false;
        }

        public bool CanDrillNow() => (this.powerComp == null || this.powerComp.PowerOn) && this.RoofPresent();

        public bool RoofPresent()
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map: this.Map) && this.Map.roofGrid.RoofAt(c: intVec) != null && this.Map.roofGrid.RoofAt(c: intVec).isThickRoof)
                    return true;
            }
            return false;
        }

        public override string GetInspectString() => string.Concat(values: new[]
            {
                base.GetInspectString(),
                "Progress"/*.Translate()*/,
                ": ",
                this.Progress.ToStringPercent()
            });
    }
}
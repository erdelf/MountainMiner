using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MountainMiner
{
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class WorkGiver_UpDrill : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(singleDef: ThingDef.Named(defName: "ManualMountainMiner"));

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.listerBuildings.AllBuildingsColonistOfDef(def: ThingDef.Named(defName: "ManualMountainMiner")).Cast<Thing>();

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                if (allBuildingsColonist[index: i].def == ThingDef.Named(defName: "ManualMountainMiner"))
                {
                    CompPowerTrader comp = allBuildingsColonist[index: i].GetComp<CompPowerTrader>();
                    if (comp == null || comp.PowerOn)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction)
            {
                return false;
            }

            if (!(t is Building building)) return false;
            if (building.IsForbidden(pawn: pawn)) return false;
            if (!pawn.CanReserve(target: building)) return false;
            Building_MountainDrill mountainDrill = (Building_MountainDrill) building;
            return mountainDrill.CanDrillNow() && !building.IsBurning();
        }
        
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) => new Job(def: JobDefOf_MM.OperateHighDrill, targetA: t, expiryInterval: 1500, checkOverrideOnExpiry: true);
    }
}

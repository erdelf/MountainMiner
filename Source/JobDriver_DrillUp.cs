using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;

namespace MountainMiner
{
    class JobDriver_DrillUp : JobDriver
    {
        const int ticks = GenDate.TicksPerDay;
        Building_MountainDrill Comp => (Building_MountainDrill)this.TargetA.Thing;

        public override bool TryMakePreToilReservations() => this.pawn.Reserve(this.TargetA, this.job);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => {
                if (this.Comp.CanDrillNow()) return false;
                return true;
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);

            Toil mine = new Toil();
            mine.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
            mine.WithProgressBar(TargetIndex.A, () => this.Comp.Progress);
            mine.tickAction = delegate
            {
                Pawn pawn = mine.actor;
                this.Comp.Drill(pawn.GetStatValue(StatDefOf.MiningSpeed) / ticks);
                pawn.skills.Learn(SkillDefOf.Mining, 0.125f);
                if (this.Comp.Progress>=1)
                {
                    this.Comp.DrillWorkDone(pawn);
                    EndJobWith(JobCondition.Succeeded);
                    pawn.records.Increment(RecordDefOf.CellsMined);
                }
            };
            mine.WithEffect(this.TargetThingA.def.repairEffect, TargetIndex.A);
            mine.defaultCompleteMode = ToilCompleteMode.Never;
            yield return mine;
        }
    }
}

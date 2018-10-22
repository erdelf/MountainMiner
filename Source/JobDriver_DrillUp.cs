using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace MountainMiner
{
    using JetBrains.Annotations;

    [UsedImplicitly]
    internal class JobDriver_DrillUp : JobDriver
    {
        private const int TICKS = GenDate.TicksPerDay;
        private Building_MountainDrill Comp => (Building_MountainDrill)this.TargetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => this.pawn.Reserve(target: this.TargetA, job: this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(condition: () => !this.Comp.CanDrillNow());
            yield return Toils_Goto.GotoThing(ind: TargetIndex.A, peMode: PathEndMode.Touch).FailOnDespawnedNullOrForbidden(ind: TargetIndex.A);

            Toil mine = new Toil();
            mine.WithEffect(effectDef: EffecterDefOf.Drill, ind: TargetIndex.A);
            mine.WithProgressBar(ind: TargetIndex.A, progressGetter: () => this.Comp.Progress);
            mine.tickAction = delegate
            {
                Pawn minePawn = mine.actor;
                this.Comp.Drill(miningPoints: minePawn.GetStatValue(stat: StatDefOf.MiningSpeed) / TICKS);
                minePawn.skills.Learn(sDef: SkillDefOf.Mining, xp: 0.125f);
                if (!(this.Comp.Progress >= 1)) return;
                this.Comp.DrillWorkDone(driller: minePawn);
                this.EndJobWith(condition: JobCondition.Succeeded);
                minePawn.records.Increment(def: RecordDefOf.CellsMined);
            };
            mine.WithEffect(effectDef: this.TargetThingA.def.repairEffect, ind: TargetIndex.A);
            mine.defaultCompleteMode = ToilCompleteMode.Never;
            yield return mine;
        }
    }
}

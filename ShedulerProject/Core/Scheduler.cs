using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    static class Scheduler
    {
        static int ANTS_NUMBER = 10;
        static int DEFAULT_MAX_STEPS = 300;
        static double EVAPORATION = 0.05;
        static double MIN_PHERAMONE = 0.3;

        static public TimeTable Schedule(TimeTableData problemData)
        {
            problemData.PrepareHelpers();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Solution firstWeekSolution = Schedule(problemData, 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            var firstWeekAssignments = firstWeekSolution.ScheduledWeeklyAssignments;
            
            DEFAULT_MAX_STEPS = 600;

            sw.Start();
            // try to partially apply the first week solution to the second one
            Solution secondWeekSolution = Schedule(problemData, 2, firstWeekAssignments);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            var secondWeekAssignments = secondWeekSolution.ScheduledWeeklyAssignments;

            var result = new TimeTable(problemData);

            foreach (var assignment in firstWeekAssignments)
                result.AddAssignment(assignment);

            foreach (var assignment in secondWeekAssignments)
                result.AddAssignment(assignment);

            return result;
        }

        static Solution Schedule(TimeTableData problemData, int week, 
                                WeeklyEventAssignment[] guidingAssignments = null)
        {
            TimeTableData timeTable = problemData;
            problemData.PrepareSuitableTimeSlots(false);

            MMASData mmasData = new MMASData(timeTable, week, EVAPORATION, MIN_PHERAMONE);
            bool secondWeek = guidingAssignments != null;

            if (secondWeek)
                mmasData.SetPheromoneFromExistingAssignments(guidingAssignments);
            else 
                mmasData.ResetPheromone();
            
            Solution bestSoFarSolution = new Solution(problemData, week);

            bestSoFarSolution.RandomInitialSolution();
            bestSoFarSolution.ComputeFeasibility();
            bestSoFarSolution.ComputeHcv();

            int currIter = 0;
            int lastImprIter = 0;
            while (currIter - lastImprIter < 200)
            {
                Solution bestIterSolution = Enumerable.Range(0, ANTS_NUMBER)
                                                      .AsParallel()
                                                      .Select(_ => 
                                                      {
                                                          var ant = new Ant(timeTable, mmasData, week);
                                                          return !secondWeek ? 
                                                              ant.GetSolution() :
                                                              ant.GetSolution(guidingAssignments);
                                                      })
                                                      .Min();

                // apply local search until local optimum is reached or a steps limit reached
                if (secondWeek)
                {
                    DEFAULT_MAX_STEPS = Math.Min(DEFAULT_MAX_STEPS + 50, 5000);
                    bestIterSolution.ResolveOnlyWeekSpecificConflicts = true;
                }
                bestIterSolution.LocalSearch(bestIterSolution.IsFeasible ? 3000 : DEFAULT_MAX_STEPS);

                // output the new best solution, if found
                if (bestIterSolution.CompareTo(bestSoFarSolution) < 0)
                {
                    bestIterSolution.CopyTo(bestSoFarSolution);
                    lastImprIter = currIter;
                }

                // update pheromones
                mmasData.EvaporatePheromone();
                mmasData.SetPheromoneLimits();
                if (bestIterSolution.ResolveOnlyWeekSpecificConflicts)
                    mmasData.DepositPheromone(bestIterSolution);
                else
                    mmasData.DepositPheromone(bestSoFarSolution);

                currIter++;
                Console.WriteLine("iter: {0}, HCV: {1}, SCV: {2}", currIter, bestSoFarSolution.Hcv, bestSoFarSolution.Scv);
            }

            bestSoFarSolution.ComputeHcv();
            bestSoFarSolution.ComputeScv();
            Console.WriteLine("RAW: HCV: {0}, SCV: {1}", bestSoFarSolution.Hcv, bestSoFarSolution.Scv);

            problemData.PrepareSuitableTimeSlots(true);

            bestSoFarSolution.TryResolveHcv();
            bestSoFarSolution.ComputeHcv();
            bestSoFarSolution.ComputeScv();
            Console.WriteLine("RESOLVE: HCV: {0}, SCV: {1}", bestSoFarSolution.Hcv, bestSoFarSolution.Scv);

            bestSoFarSolution.LocalSearch(10000, 1, 1); //try to resolve scv
            bestSoFarSolution.ComputeHcv();
            bestSoFarSolution.ComputeScv();
            Console.WriteLine("RESULT: HCV: {0}, SCV: {1}", bestSoFarSolution.Hcv, bestSoFarSolution.Scv);

            return bestSoFarSolution;
        }
    }
}
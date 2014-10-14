using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    class MMASData
    {
        public double EvaporationRate;
        public double MaxPherLevel;
        public double MinPherLevel;
        //public double alpha;
        public double[,] Pheromones;	// matrix keeping pheromone between events and timeslots
        public int[] SortedEventList;			// vector keeping sorted lists of events

        TimeTableData data;
        Event[] events;
        int eventesNumber;
        int timeslotsNumber;

        public MMASData(TimeTableData problemData, int week, double evaporation, double minPher)
        {
            //member variables initialization
            data = problemData;
            events = data.GetWeekEvents(week);
            eventesNumber = events.Length;
            timeslotsNumber = problemData.TotalTimeSlots;
            Pheromones = new double[eventesNumber, timeslotsNumber];
            EvaporationRate = evaporation;
            MinPherLevel = minPher;

            if (EvaporationRate < 1.0)
            {
                MaxPherLevel = 10.0 / (1.0 - EvaporationRate);
            }
            else
            {
                MaxPherLevel = int.MaxValue;
            }

            // creating a set of pre-sorted event lists
            // sorting events based on level of correlations
            int[] event_correlation = new int[eventesNumber];
            for (int i = 0; i < eventesNumber; i++)
            {
                // summing up the correlations for each event
                event_correlation[i] = 0;
                for (int j = 0; j < eventesNumber; j++)
                {
                    if (problemData.ConflictingEvents(events[i].Id, events[j].Id))
                        event_correlation[i] += 1;
                }
            }

            SortedEventList = new int[eventesNumber];
            for (int i = 0; i < eventesNumber; i++)
            {
                // sorting the list
                int max_correlation = -1;
                int event_index = -1;
                for (int j = 0; j < eventesNumber; j++)
                {
                    if (event_correlation[j] > max_correlation)
                    {
                        max_correlation = event_correlation[j];
                        event_index = j;
                    }
                }

                event_correlation[event_index] = -2;
                SortedEventList[i] = event_index;
            }
        }

        public void SetPheromoneFromExistingAssignments(WeeklyEventAssignment[] assignments)
        {
            SetPheromone(MinPherLevel);
            foreach (var assignment in assignments)
            {
                var eventIndex = Array.IndexOf(events, assignment.Event);
                if (eventIndex != -1)
                {
                    var slotId = TimeSlot.ToId(assignment.TimeSlot, data.Days, data.SlotsPerDay);
                    Pheromones[eventIndex, slotId] = MaxPherLevel;
                }
            }
        }

        public void ResetPheromone()
        {
            // initialize pheromon levels between events and timeslots to the maximal values
            SetPheromone(MaxPherLevel);
        }

        void SetPheromone(double value)
        {
            for (int i = 0; i < eventesNumber; i++)
            {
                for (int j = 0; j < timeslotsNumber; j++)
                {
                    Pheromones[i, j] = value;
                }
            }
        }

        public void EvaporatePheromone()
        {
            // evaporate some pheromone
            for (int i = 0; i < eventesNumber; i++)
            {
                for (int j = 0; j < timeslotsNumber; j++)
                {
                    Pheromones[i, j] *= (1 - EvaporationRate);
                }
            }
        }

        public void SetPheromoneLimits()
        {
            // limit pheromone values according to MAX-MIN
            for (int i = 0; i < eventesNumber; i++)
            {
                for (int j = 0; j < timeslotsNumber; j++)
                {
                    if (Pheromones[i, j] < MinPherLevel)
                    {
                        Pheromones[i, j] = MinPherLevel;
                    }

                    if (Pheromones[i, j] > MaxPherLevel)
                    {
                        Pheromones[i, j] = MaxPherLevel;
                    }
                }
            }
        }

        public void DepositPheromone(Solution solution)
        {
            // calculate pheromone update
            for (int i = 0; i < solution.Result.Length; i++)
            {
                int timeslot = solution.Result[i].TimeSlotId;
                Pheromones[i, timeslot] += 1.0;
            }
        }
    }
}

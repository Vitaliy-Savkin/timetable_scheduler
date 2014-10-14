using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SchedulerProject.Core
{
    class Ant
    {
        public Ant(TimeTableData problemData, MMASData mmasData, int week)
        {
            // memeber variables initialization
            data = problemData;
            solution = new Solution(problemData, week);
            events = data.GetWeekEvents(week);
            eventsCount = events.Length;
            this.mmasData = mmasData;
            totalTimeSlots = data.TotalTimeSlots;
        }

        Solution solution;
        TimeTableData data;
        MMASData mmasData;
        Event[] events;
        int totalTimeSlots;
        int eventsCount;

        public Solution GetSolution()
        {
            // itarate through all the events to complete the path
            for (var i = 0; i < eventsCount; i++)
            {
                // chose next event from the list
                var e = mmasData.SortedEventList[i];
                var timeslot = GetTimeSlotId(e);
                // put an event i into timeslot t
                solution.Result[e].TimeSlotId = timeslot;
                solution.TimeslotEvents[timeslot].Add(e);
            }

            solution.AssignRoomForEachTimeSlot();

            return solution;
        }

        public Solution GetSolution(WeeklyEventAssignment[] existingAssignments)
        {
            foreach (var assignment in existingAssignments)
            {
                var eventIndex = Array.IndexOf(events, assignment.Event);
                if (eventIndex != -1)
                {
                    var slotId = TimeSlot.ToId(assignment.TimeSlot, data.Days, data.SlotsPerDay);
                    solution.Result[eventIndex].TimeSlotId = slotId;
                    solution.Result[eventIndex].RoomId = assignment.RoomId;
                    solution.TimeslotEvents[slotId].Add(eventIndex);
                }
            }

            // itarate through all the events to complete the path
            for (var i = 0; i < eventsCount; i++)
            {
                // chose next event from the list
                var e = mmasData.SortedEventList[i];
                if (solution.Result[e].TimeSlotId == -1)
                {
                    var timeslot = GetTimeSlotId(e);
                    solution.Result[e].TimeSlotId = timeslot;
                    int[] assignedRooms = existingAssignments
                        .Where(a => TimeSlot.ToId(a.TimeSlot, data.Days, data.SlotsPerDay) == timeslot)                                                             
                        .Select(a => a.RoomId)
                        .ToArray();
                    solution.Result[e].RoomId = data.Rooms.Where(r => data.SuitableRoom(events[e].Id, r.Id) &&
                                                                        !assignedRooms.Contains(r.Id))
                                                            .Select(r => r.Id)
                                                            .Shuffle(solution.rg)
                                                            .DefaultIfEmpty(data.Rooms[solution.rg.Next(data.Rooms.Length)].Id)
                                                            .First(); 
                    solution.TimeslotEvents[timeslot].Add(e);
                }
            }

            return solution;
        }

        // finding the range for normalization
        double GetNormalizationRange(int eventIndex)
        {
            double range = 0.0;
            for (int j = 0; j < data.TotalTimeSlots; j++)
                range += mmasData.Pheromones[eventIndex, j];
            return range;
        }

        // choose a timeslot for the event based on the pheromone table and the random number
        int GetTimeSlotId(int eventIndex)
        {
            // choose a random number between 0.0 and sum of the pheromone level
            // for this event and current sum of heuristic information
            var limit = solution.rg.NextDouble() * GetNormalizationRange(eventIndex);

            double total = 0.0;
            int timeslot = -1;
            for (int j = 0; j < totalTimeSlots; j++)
            {
                // check the pheromone
                total += mmasData.Pheromones[eventIndex, j];
                if (total >= limit)
                {
                    while (!data.SuitableTimeSlot(events[eventIndex].Id, j))
                        j = (j + 1) % totalTimeSlots;
                    timeslot = j;
                    break;
                }
            }
            return timeslot;
        }
    }
}

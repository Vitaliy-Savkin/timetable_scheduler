using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SchedulerProject.Core
{   
    struct InternalEventAssignment
    {
        public int TimeSlotId;
        public int RoomId;
    }

    class Solution : IComparable<Solution>
    {
        //result index = week events index
        public InternalEventAssignment[] Result; // vector of (timeslot, room) assigned for each event (index is an event number)

        public Dictionary<int, List<int>> TimeslotEvents = new Dictionary<int, List<int>>(); // for each timeslot a vector of events taking place in it
        public TimeTableData data;
        public Random rg = new Random();

        public bool IsFeasible;
        public int Scv;   // keeps the number of soft constraint violations (ComputeScv() has to be called)
        public int Hcv;   // keeps the number of hard constraint violations (ComputeHcv() has to be called)

        public bool ResolveOnlyWeekSpecificConflicts = false;

        Event[] events;
        int eventsCount;
        int week;

        // CHECK CORRECTNESS AND PERFORMANCE !!!
        // 
        public void CopyTo(Solution dest)
        {
            dest.data = this.data;
            dest.IsFeasible = this.IsFeasible;
            dest.Scv = this.Scv;
            dest.Hcv = this.Hcv;
            dest.events = this.events;
            dest.eventsCount = this.eventsCount;
            dest.week = week;
            dest.Result = this.Result.AsEnumerable().ToArray();
            dest.ResolveOnlyWeekSpecificConflicts = this.ResolveOnlyWeekSpecificConflicts;
            dest.TimeslotEvents = this.TimeslotEvents
                                    .AsEnumerable()
                                    .ToDictionary(kvp => kvp.Key,
                                                kvp => kvp.Value.AsEnumerable().ToList());
        }

        public int CompareTo(Solution other)
        {
            this.ComputeHcv();
            this.ComputeScv();
            other.ComputeHcv();
            other.ComputeScv();
            int result = this.Hcv.CompareTo(other.Hcv);
            return result == 0 ? this.Scv.CompareTo(other.Scv) : result;
        }

        private Solution() { }

        public Solution(TimeTableData problemData, int week)
        {
            data = problemData;
            rg = new Random();
            events = data.GetWeekEvents(week);
            eventsCount = events.Length;
            this.week = week;
            Result = new InternalEventAssignment[eventsCount];
            for (int t = 0; t < data.TotalTimeSlots; t++)
                TimeslotEvents.Add(t, new List<int>());
            for (int i = 0; i < Result.Length; i++)
            {
                Result[i].RoomId = Result[i].TimeSlotId = -1;
            }
        }

        public WeeklyEventAssignment[] ScheduledWeeklyAssignments
        {
            get
            {
                return Result.Select((a, i) =>
                {
                    Event ev = data.Events.First(e => e.Id == data.GetWeekEvents(week)[i].Id);
                    Room room = data.Rooms.FirstOrDefault(r => r.Id == a.RoomId);
                    TimeSlot slot = TimeSlot.FromId(a.TimeSlotId,
                                                    data.Days, data.SlotsPerDay);

                    return new WeeklyEventAssignment(ev, room, slot, week) { Conflicts = EventHcv(i) + (data.SuitableRoom(ev.Id, room.Id) ? 0 : 1) };
                }).ToArray();
            }
        }

        /// <summary>
        /// Produce a random initial solution.
        /// </summary>
        public void RandomInitialSolution()
        {
            // assign a random timeslot to each event
            for (int e = 0; e < eventsCount; e++)
            {
                int t = (int)(rg.NextDouble() * data.TotalTimeSlots);
                while (!data.SuitableTimeSlot(events[e].Id, t))
                    t = (t + 1) % data.TotalTimeSlots;

                Result[e].TimeSlotId = t;
                TimeslotEvents[t].Add(e);
            }

            var notEmptySlots = from p in TimeslotEvents
                                where p.Value.Count != 0
                                select p.Key;

            // and assign rooms to events in each non-empty timeslot
            foreach (int timeSlot in notEmptySlots)
            {
                AssignRooms(timeSlot);
            }
        }

        #region Hcv
        public bool ComputeFeasibility()
        {
            Hcv = GetHcv(true);
            IsFeasible = Hcv == 0;
            return IsFeasible;
        }

        /// <summary>
        /// Check feasibility.
        /// </summary>
        int GetHcv(bool stopOnFirst)
        {
            int hcv = 0;
            for (int i = 0; i < eventsCount; i++)
            {
                for (int j = i + 1; j < eventsCount; j++)
                {
                    if (Result[i].TimeSlotId == Result[j].TimeSlotId &&
                        Result[i].RoomId == Result[j].RoomId)
                    {
                        if (stopOnFirst) return 1;
                        hcv++; // only one class can be in each room at any timeslot
                    }

                    if (data.ConflictingEvents(events[i].Id, events[j].Id) &&
                        Result[i].TimeSlotId == Result[j].TimeSlotId)
                    {
                        if (stopOnFirst) return 1;
                        hcv++; // two events sharing groups cannot be in the same timeslot
                    }
                }

                if(!data.SuitableRoom(events[i].Id, Result[i].RoomId))
                {
                    if (stopOnFirst) return 1;
                    hcv++; // each event should take place in a suitable room
                }
            }

            // if none of the previous hard constraint violations occurs the timetable is feasible
            return hcv;
        }

        /// <summary>
        /// Compute hard constraint violations.
        /// </summary>
        public int ComputeHcv()
        {
            Hcv = GetHcv(false);
            return Hcv;
        }

        /// <summary>
        /// Evaluate number of hcv caused by event e.
        /// </summary>
        int EventHcv(int e)
        {
            int eHcv = 0;			// set to zero hard constraint violations for event e
            int t = Result[e].TimeSlotId;	// note the timeslot in which event e is
            for (int i = 0; i < TimeslotEvents[t].Count; i++)
            {
                if (TimeslotEvents[t][i] != e)
                {
                    if (Result[e].RoomId == Result[TimeslotEvents[t][i]].RoomId)
                    {
                        // adds up number of events sharing room and timeslot with the given one
                        eHcv = eHcv + 1;
                    }

                    if (data.ConflictingEvents(events[e].Id, events[TimeslotEvents[t][i]].Id))
                    {
                        // adds up number of incompatible( because of students in common) events in the same timeslot                    
                        eHcv = eHcv + 1;
                    }
                }
            }

            // the suitable room hard constraint is taken care of by the assignroom routine
            return eHcv;
        }

        /// <summary>
        /// Evaluate the hcv that might be affected when event e is moved from its timeslot.
        /// </summary>
        int EventAffectedHcv(int e)
        {
            int aHcv = 0;					// set to zero the affected hard constraint violations for event e
            int t = Result[e].TimeSlotId;			// t timeslot where event e is
            for (int i = 0; i < TimeslotEvents[t].Count; i++)
            {
                for (int j = i + 1; j < TimeslotEvents[t].Count; j++)
                {
                    if (Result[TimeslotEvents[t][i]].RoomId == Result[TimeslotEvents[t][j]].RoomId)
                    {
                        // adds up number of room clashes in the timeslot of the given event
                        // (rooms assignement are affected by move for the whole timeslot)
                        aHcv = aHcv + 1;
                    }
                }

                if (TimeslotEvents[t][i] != e)
                {
                    if (data.ConflictingEvents(events[e].Id, events[TimeslotEvents[t][i]].Id))
                    {
                        // adds up number of incompatible (because of students in common) events in the same timeslot
                        // the only hcv of this type affected when e is moved are the ones involving e
                        aHcv = aHcv + 1;
                    }
                }
            }

            // the suitable room hard constraint is taken care of by the assignroom routine
            return aHcv;
        }

        /// <summary>
        /// Evaluate all the room hcv as above for all the events in timeslot t.
        /// </summary>
        int AffectedRoomInTimeslotHcv(int t)
        {
            int roomHcv = 0;
            for (int i = 0; i < TimeslotEvents[t].Count; i++)
            {
                for (int j = i + 1; j < TimeslotEvents[t].Count; j++)
                {
                    if (Result[TimeslotEvents[t][i]].RoomId ==
                        Result[TimeslotEvents[t][j]].RoomId)
                    {
                        roomHcv += 1;
                    }
                }
            }

            return roomHcv;
        }   
        
        bool SingleEventLocalSearchHcv(int[] eventList, int eventIndex, double prob1, double prob2, ref int stepCount, bool copyIfHcvCountEquals)
        {
            if (prob1 != 0)
            {
                var ev = eventList[eventIndex];
                var t_start = (int)(rg.NextDouble() * data.TotalTimeSlots);	// try moves of type 1
                var t_orig = Result[ev].TimeSlotId;
                for (int h = 0, t = t_start; h < data.TotalTimeSlots; t = (t + 1) % data.TotalTimeSlots, h++)
                {
                    if (rg.NextDouble() < prob1)
                    {
                        Solution neighbourSolution = new Solution();
                        this.CopyTo(neighbourSolution);

                        if (neighbourSolution.Move1(ev, t))
                        {
                            stepCount++;
                            var neighbourAffectedHcv = neighbourSolution.EventAffectedHcv(ev) +
                                                       neighbourSolution.AffectedRoomInTimeslotHcv(t_orig);
                            var currentAffectedHcv = EventAffectedHcv(ev) + AffectedRoomInTimeslotHcv(t);
                            if (neighbourAffectedHcv < currentAffectedHcv || 
                                copyIfHcvCountEquals && neighbourAffectedHcv == currentAffectedHcv)
                            {
                                neighbourSolution.CopyTo(this);
                                return true;
                            }
                        }
                    }
                }
            }

            if (prob2 != 0)
            {
                for (var j = (eventIndex + 1) % eventsCount; j != eventIndex; j = (j + 1) % eventsCount)
                {
                    if (rg.NextDouble() < prob2)
                    {
                        Solution neighbourSolution = new Solution();
                        this.CopyTo(neighbourSolution);
                        if (neighbourSolution.Move2(eventList[eventIndex], eventList[j]))
                        {
                            stepCount++;
                            var neighbourAffectedHcv = neighbourSolution.EventAffectedHcv(eventList[eventIndex]) +
                                                       neighbourSolution.EventAffectedHcv(eventList[j]);
                            var currentAffectedHcv = EventAffectedHcv(eventList[eventIndex]) + EventAffectedHcv(eventList[j]);
                            if (neighbourAffectedHcv < currentAffectedHcv || 
                                copyIfHcvCountEquals && neighbourAffectedHcv == currentAffectedHcv)
                            {
                                neighbourSolution.CopyTo(this);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Scv
        int GetGaps(IEnumerable<TimeSlot> weekSlots)
        {
            return weekSlots.GroupBy(ts => ts.Day).Count(daySlots =>
            {
                var max = daySlots.Max(s => s.Slot);
                var min = daySlots.Min(s => s.Slot);
                return max - min != daySlots.Count() - 1;
            });
        }

        /// <summary>
        /// Compute soft constraint violations.
        /// </summary>
        public int ComputeScv()
        {
            Scv = GetScv();
            return Scv;
        }

        /// <summary>
        /// Compute soft constraint violations.
        /// </summary>
        int GetScv()
        {
            var groupsTimeSlots = new Dictionary<int, List<TimeSlot>>();
            var lecturersTimeSlots = new Dictionary<int, List<TimeSlot>>();
            var lastSlotsCount = 0;
            for (var i = 0; i < Result.Length; i++)
            {
                var ts = TimeSlot.FromId(Result[i].TimeSlotId, data.Days, data.SlotsPerDay);
                if (ts.Slot == data.SlotsPerDay)
                    lastSlotsCount++;

                foreach (var gId in events[i].Groups)
                {
                    if (!groupsTimeSlots.ContainsKey(gId))
                        groupsTimeSlots.Add(gId, new List<TimeSlot>());
                    groupsTimeSlots[gId].Add(ts);
                }

                var lId = events[i].LecturerId;
                if (!lecturersTimeSlots.ContainsKey(lId))
                    lecturersTimeSlots.Add(lId, new List<TimeSlot>());
                lecturersTimeSlots[lId].Add(ts);
            }

            var groupsGaps = groupsTimeSlots.Values.Sum(timeSlots => GetGaps(timeSlots));
            var lecturerGaps = lecturersTimeSlots.Values.Sum(timeSlots => GetGaps(timeSlots));
            //TODO: var singleEventsDays
            return //lastSlotsCount + 
                groupsGaps + lecturerGaps;
            //return groupsGaps;
        }

        /// <summary>
        /// Evaluate number of scv caused by event e
        /// </summary>
        int EventScv(int e)
        {
            return 0;
        }

        /// <summary>
        /// Evaluate the number of single classes that event e actually solves in the day (or created if e leaves its timeslot).
        /// NOTE: Should be called only if the solution is feasible.
        /// </summary>
        int SingleClassesScv(int e)
        {
            int t = Result[e].TimeSlotId;
            int classes;
            int singleClasses = 0;
            for (int i = 0; i < data.Groups.Length; i++)
            {
                if (data.GroupHasEvent(data.Groups[i].Id, events[e].Id))
                {
                    classes = 0;

                    //check 8 nearest timeslosts?
                    for (int s = t - (t % 9); s < t - (t % 9) + 9; s++)
                    {
                        if (classes > 1)
                        {
                            break;
                        }

                        if (s != t)
                        {	
                            // we are in the feasible region so there are not events sharing students in the same timeslot
                            for (int j = 0; j < TimeslotEvents[s].Count; j++)
                            {
                                if (data.GroupHasEvent(data.Groups[i].Id, events[TimeslotEvents[s][j]].Id))
                                {
                                    classes += 1;
                                    break;
                                }
                            }
                        }
                    }

                    // classes = 0 means that the group under consideration has a single class in the day (for event e) but that W
                    // but we are not interested in that here (it is counted in eventScv(e))
                    if (classes == 1)
                    {
                        singleClasses += 1;
                    }
                }
            }

            return singleClasses;
        }

        // TODO: Should be refactored! (see SingleEventLocalSearchHcv)
        bool SingleEventLocalSearchScv(int[] eventList, int eventIndex, int currentScv, double prob1, double prob2, ref int stepCount)
        {
            if (prob1 != 0)
            {
                var ev = eventList[eventIndex];
                var t_start = (int)(rg.NextDouble() * data.TotalTimeSlots);	// try moves of type 1
                var t_orig = Result[ev].TimeSlotId;
                for (int h = 0, t = t_start; h < data.TotalTimeSlots; t = (t + 1) % data.TotalTimeSlots, h++)
                {
                    if (rg.NextDouble() < prob1)
                    {
                        Solution neighbourSolution = new Solution();
                        this.CopyTo(neighbourSolution);

                        if (neighbourSolution.Move1(ev, t))
                        {
                            stepCount++;
                            var neighbourFeasible = neighbourSolution.ComputeFeasibility();
                            if (neighbourFeasible)
                            {
                                var neighbourScv = neighbourSolution.ComputeScv();

                                if (neighbourScv < currentScv)
                                {
                                    neighbourSolution.CopyTo(this);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            if (prob2 != 0)
            {
                for (var j = (eventIndex + 1) % eventsCount; j != eventIndex; j = (j + 1) % eventsCount)
                {
                    if (rg.NextDouble() < prob2)
                    {
                        Solution neighbourSolution = new Solution();
                        this.CopyTo(neighbourSolution);
                        if (neighbourSolution.Move2(eventList[eventIndex], eventList[j]))
                        {
                            stepCount++;
                            var neighbourFeasible = neighbourSolution.ComputeFeasibility();
                            if (neighbourFeasible)
                            {
                                var neighbourScv = neighbourSolution.ComputeScv();

                                if (neighbourScv < currentScv)
                                {
                                    neighbourSolution.CopyTo(this);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Local search
        /// <summary>
        /// Move event e to timeslot t (type 1 move).
        /// </summary>
        bool Move1(int e, int t)
        {
            if (ResolveOnlyWeekSpecificConflicts && data.IsEveryWeekEvent(e))
                return false;

            if (!data.SuitableTimeSlot(events[e].Id, t))
                return false;

	        //move event e to timeslot t
	        int prevSlot = Result[e].TimeSlotId;
	        Result[e].TimeSlotId = t;

            TimeslotEvents[prevSlot].Remove(e); // remove event e from the original timeslot 
	        TimeslotEvents[t].Add(e);	      // and place it in timeslot t

	        // reorder in label order events in timeslot t
            //timeslot_events[t].Sort(); // TODO: try to replace insertion and sort with Binary search

	        // reassign rooms to events in timeslot t
	        AssignRooms(t);

	        // do the same for the original timeslot of event e if it is not empty
	        if (TimeslotEvents[prevSlot].Count != 0)
	        {
		        AssignRooms(prevSlot);
	        }

            return true;
        }

        /// <summary>
        /// Swaps events e1 and e2 (type 2 move).
        /// </summary>
        bool Move2(int e1, int e2)
        {
            if (ResolveOnlyWeekSpecificConflicts && (data.IsEveryWeekEvent(e1) || data.IsEveryWeekEvent(e2)))
                return false;

	        //swap timeslots between event e1 and event e2
            int t1 = Result[e1].TimeSlotId,
                t2 = Result[e2].TimeSlotId;

            if (!data.SuitableTimeSlot(events[e1].Id, t2) ||
                !data.SuitableTimeSlot(events[e2].Id, t1))
                return false;

	        Result[e1].TimeSlotId = t2;
	        Result[e2].TimeSlotId = t1;

            ReplaceEvent(t1, e1, e2);
            ReplaceEvent(t2, e2, e1);

	        AssignRooms(Result[e1].TimeSlotId);
	        AssignRooms(Result[e2].TimeSlotId);

            return true;
        }

        /// <summary>
        /// 3-cycle permutation of events e1, e2 and e3 (type 3 move).
        /// </summary>
        bool Move3(int e1, int e2, int e3)
        {
	        // permute event e1, e2, and e3 in a 3-cycle
            
            if (ResolveOnlyWeekSpecificConflicts && (data.IsEveryWeekEvent(e1) || data.IsEveryWeekEvent(e2) || data.IsEveryWeekEvent(e3)))
                return false;

            int t1 = Result[e1].TimeSlotId,
                t2 = Result[e2].TimeSlotId,
                t3 = Result[e3].TimeSlotId;

            if (!data.SuitableTimeSlot(events[e1].Id, t2) ||
                !data.SuitableTimeSlot(events[e2].Id, t3) ||
                !data.SuitableTimeSlot(events[e3].Id, t1))
                return false;

	        Result[e1].TimeSlotId = t2;
	        Result[e2].TimeSlotId = t3;
	        Result[e3].TimeSlotId = t1;

            ReplaceEvent(t1, e1, e2);
            ReplaceEvent(t2, e2, e3);
            ReplaceEvent(t3, e3, e1);

	        AssignRooms(Result[e1].TimeSlotId);
	        AssignRooms(Result[e2].TimeSlotId);
	        AssignRooms(Result[e3].TimeSlotId);

            return true;
        }

        void ReplaceEvent(int timeSlot, int currEvent, int newEvent)
        {
            TimeslotEvents[timeSlot].Remove(currEvent);
            TimeslotEvents[timeSlot].Add(newEvent);
            //timeslot_events[timeSlot].Sort();
        }
     
        /// <summary>
        /// Apply local search with the given parameters.
        /// </summary>
        public void LocalSearch(int maxSteps, double prob1 = 1.0, double prob2 = 0.9, double prob3 = 0.0)
        {
            // perform local search with given time limit and probabilities for each type of move
            var eventList = new int[eventsCount];
            for (var e = 0; e < eventsCount; e++)
            {
                eventList[e] = e;
            }

            for (var e = 0; e < eventsCount; e++)
            {	
                // scramble the list of events to obtain a random order
                var j = (int)(rg.NextDouble() * eventsCount);
                var tmp = eventList[e];
                eventList[e] = eventList[j];
                eventList[j] = tmp;
            }
            
            var stepCount = 0;				// set step counter to zero
            var checkedEvents = 0;          // counter of events considered
            var foundBetter = false;
            ComputeFeasibility();
            var i = 0;
            while (checkedEvents < eventsCount && stepCount < maxSteps)
            {
                //Console.WriteLine("checked: {0}, steps: {1}", checkedEvents, stepCount);
                var currentHcv = EventHcv(eventList[i]);
                if (currentHcv != 0)
                {
                    foundBetter = SingleEventLocalSearchHcv(eventList, i, prob1, prob2, ref stepCount, false);
                }
                ComputeFeasibility();
                if (IsFeasible)
                {
                    var currentScv = ComputeScv();
                    foundBetter = SingleEventLocalSearchScv(eventList, i, currentScv, prob1, prob2, ref stepCount);
                }

                if (foundBetter)
                    checkedEvents = 0;
                else
                    checkedEvents++;
                i = (i + 1) % eventsCount;
                foundBetter = false;
            }
        }
        #endregion

        int FindSuitableRoomId(int eventId, int timeSlotId)
        {
            var suitable = data.Rooms.Select(r => r.Id).Where(id => data.SuitableRoom(eventId, id)).ToArray();
            var busy = TimeslotEvents[timeSlotId].Select(e => Result[e].RoomId).ToArray();
            var x = suitable.Except(busy).ToArray();
            if (x.Any())
            {
                return x.First();
            }
            return -1;
        }

        bool SlotWithConflictingEvents(int eventId, int timeSlotId)
        {
            return TimeslotEvents[timeSlotId].FindIndex(e => data.ConflictingEvents(eventId, events[e].Id)) != -1;
        }

        bool TryMoveToSuitableTimeSlot(int eventIndex)
        {
            var evId = events[eventIndex].Id;
            for (var t = 0; t < data.TotalTimeSlots; t++)
            {
                if (t != Result[eventIndex].TimeSlotId &&
                    data.SuitableTimeSlot(evId, t) && !SlotWithConflictingEvents(evId, t))
                {
                    var roomId = FindSuitableRoomId(evId, t);
                    if (roomId != -1)
                    {
                        TimeslotEvents[t].Add(eventIndex);
                        Result[eventIndex].RoomId = roomId;
                        Result[eventIndex].TimeSlotId = t;
                        return true;
                    }
                }
            }
            return false;
        }

        bool TrySwapToSuitableTimeSlot(int eventIndex)
        {
            var evTimeSlot = Result[eventIndex].TimeSlotId;
            TimeslotEvents[evTimeSlot].Remove(eventIndex);

            var evId = events[eventIndex].Id;
            for (var t = 0; t < data.TotalTimeSlots; t++)
            {
                if (t != evTimeSlot && data.SuitableTimeSlot(evId, t))
                {
                    for(int i = 0; i < TimeslotEvents[t].Count; i++)
                    {
                        var e = TimeslotEvents[t][i];
                        if (data.SuitableTimeSlot(events[e].Id, t))
                        {
                            TimeslotEvents[t].RemoveAt(i);
                            var conflicting1 = SlotWithConflictingEvents(evId, t);
                            var conflicting2 = SlotWithConflictingEvents(events[e].Id, evTimeSlot);
                            var room1 = FindSuitableRoomId(evId, t);
                            var room2 = FindSuitableRoomId(events[e].Id, evTimeSlot);
                            if (!conflicting1 && !conflicting2 &&
                                room1 != -1 && room2 != -1)
                            {
                                TimeslotEvents[t].Add(eventIndex);
                                Result[eventIndex].RoomId = room2;
                                Result[eventIndex].TimeSlotId = t;

                                TimeslotEvents[evTimeSlot].Add(e);
                                Result[e].RoomId = room1;
                                Result[e].TimeSlotId = evTimeSlot;
                                return true;
                            }
                            else
                            {
                                TimeslotEvents[t].Insert(i, e);
                            }
                        }
                    }
                }
            }
            TimeslotEvents[evTimeSlot].Add(eventIndex);
            return false;
        }

        bool TrySwapRooms(int eventIndex)
        {
            var t = Result[eventIndex].TimeSlotId;
            var evId = events[eventIndex].Id;
            foreach (var e in TimeslotEvents[t])
            {
                if (e != eventIndex)
                {
                    var room1 = Result[eventIndex].RoomId;
                    var room2 = Result[e].RoomId;
                    if (room1 != room2 && data.SuitableRoom(evId, room2) && data.SuitableRoom(events[e].Id, room1))
                    {
                        Result[e].RoomId = room1;
                        Result[eventIndex].RoomId = room2;
                        return true;
                    }
                }
            }
            return false;
        }

        public void TryResolveHcv()
        {
            ResolveOnlyWeekSpecificConflicts = true;
            var eventList = new int[eventsCount];
            for (var e = 0; e < eventsCount; e++)
            {
                eventList[e] = e;
            }

            ComputeFeasibility();
            // Should not be forever because we have reserved slots
            // TODO: add stop another stop condition
            //while (!feasible) 
            for (int tryNumber = 0; tryNumber < 10 && !IsFeasible; tryNumber++)
            {
                // collect conflicting events
                var groupConflictingEvents = new List<Tuple<int, int>>();
                var roomConflictingEvents = new List<int>();
                for (var i = 0; i < eventsCount; i++)
                {
                    var useFirstForLocalSearch = !data.IsEveryWeekEvent(events[i].Id);
                    for (var j = i + 1; j < eventsCount; j++)
                    {
                        if (Result[i].TimeSlotId == Result[j].TimeSlotId)
                        {
                            var doRemove = false;
                            var indexToRemove = useFirstForLocalSearch ? i : j;
                            if (data.ConflictingEvents(events[i].Id, events[j].Id))
                            {
                                doRemove = true;
                                groupConflictingEvents.Add(Tuple.Create(indexToRemove, useFirstForLocalSearch ? j : i));
                            }
                            if (Result[i].RoomId == Result[j].RoomId)
                            {
                                doRemove = true;
                                roomConflictingEvents.Add(indexToRemove);
                            }

                            if(doRemove)
                                TimeslotEvents[Result[i].TimeSlotId].Remove(indexToRemove);
                        }
                    }
                }

                // try to resolve event conflicts
                foreach (var conflictingEvents in groupConflictingEvents)
                {
                    if (Result[conflictingEvents.Item1].TimeSlotId != Result[conflictingEvents.Item2].TimeSlotId)
                        continue;

                    var tmp = GetHcv(false);
                    Console.WriteLine("FIX! {0} - {1}", 
                                        events[conflictingEvents.Item1].Id, events[conflictingEvents.Item2].Id);
                    if (!TryMoveToSuitableTimeSlot(conflictingEvents.Item1))
                    {
                        if (!TrySwapToSuitableTimeSlot(conflictingEvents.Item1))
                        {
                            if (!TryMoveToSuitableTimeSlot(conflictingEvents.Item2))
                            {
                                if (!TrySwapToSuitableTimeSlot(conflictingEvents.Item2))
                                {
                                    Console.WriteLine("no event fix");
                                }
                                else
                                    Console.WriteLine("swap item2");
                            }
                            else
                                Console.WriteLine("move item2");
                        }
                        else
                            Console.WriteLine("swap item1");
                    }
                    else
                        Console.WriteLine("move item1");
                    Console.WriteLine("fixed: " + (tmp - GetHcv(false)));
                }

                ComputeHcv();
                Console.WriteLine("H: " + Hcv);

                // try to resolve room conflicts
                for (var index = 0; index < eventsCount; index++)
                {
                    if (!data.SuitableRoom(events[index].Id, Result[index].RoomId) || roomConflictingEvents.Contains(index))
                    {
                        var tmp = GetHcv(false);
                        Console.WriteLine("FIX! ARR: " + roomConflictingEvents.Contains(index));
                        var suitable = FindSuitableRoomId(events[index].Id, Result[index].TimeSlotId);
                        if (suitable != -1)
                        {
                            Result[index].RoomId = suitable;
                            Console.WriteLine("room found");
                        }
                        else
                        {
                            if (!TrySwapRooms(index))
                            {
                                if (!TryMoveToSuitableTimeSlot(index))
                                {
                                    if(!TrySwapToSuitableTimeSlot(index))
                                        Console.WriteLine("no room fix! " + events[index].Id);
                                }
                                else
                                    Console.WriteLine("move event");
                            }
                            else
                                Console.WriteLine("swap event");
                        }
                        Console.WriteLine("fixed: " + (tmp - GetHcv(false)));
                    }
                }

                ComputeHcv();
                Console.WriteLine("H: " + Hcv);

                int rHcv = 0;
                for (var index = 0; index < eventsCount; index++)
                {
                    if (!data.SuitableRoom(events[index].Id, Result[index].RoomId))
                        rHcv++;
                }

                ComputeFeasibility();
            }
        }

        public void AssignRoomForEachTimeSlot()
        {
            // assign rooms to events in each non-empty timeslot
            for (var i = 0; i < data.TotalTimeSlots; i++)
                if (TimeslotEvents[i].Count != 0)
                    AssignRooms(i);
        }

        /// <summary>
        /// Assign rooms to events for timeslot t (first apply matching algorithm and then assign unplaced rooms).
        /// </summary>
        void AssignRooms(int t)
        {
            if (ResolveOnlyWeekSpecificConflicts && AffectedRoomInTimeslotHcv(t) == 0)
                return;

            var eventsCount = TimeslotEvents[t].Count;
            var x = HopcroftKarpMatching(ResolveOnlyWeekSpecificConflicts ? MakeWeekSpecificGraph(t) : MakeGraph(t), eventsCount);

            var roomsAssignmentCounters = new int[data.Rooms.Length];

            for (var e = 0; e < eventsCount; e++)
            {
                if (x[e] != NIL_VERT_ID)
                {
                    var roomId = x[e] - eventsCount;
                    Result[TimeslotEvents[t][e]].RoomId = data.Rooms[roomId].Id;
                    roomsAssignmentCounters[roomId]++;
                }
            }

            for (var e = 0; e < eventsCount; e++)
            {
                if (x[e] == NIL_VERT_ID)
                {
                    var lessBusyRoom = FindLessBusyRoom(roomsAssignmentCounters);
                    Result[TimeslotEvents[t][e]].RoomId = data.Rooms[lessBusyRoom].Id;
                    roomsAssignmentCounters[lessBusyRoom]++;
                }
            }
        }

        int FindLessBusyRoom(int[] roomsAssignments)
        {
            var id = -1;
            var min = int.MaxValue;
            for (var i = 0; i < roomsAssignments.Length; i++)
            {
                if (data.Rooms[i].Type != RoomType.Assigned && roomsAssignments[i] < min)
                {
                    min = roomsAssignments[i];
                    id = i;
                }
            }
            return id;
        }

        #region Mathing algorithm

        int[] Pair_G1, Pair_G2, Dist;
        int NIL_VERT_ID;
        bool[,] Graph;
        int FIRST_PART_COUNT;
        int N;

        //result: event -> room
        int[] HopcroftKarpMatching(bool[,] g, int fBipartCount)
        {
            Graph = g;
            FIRST_PART_COUNT = fBipartCount;
            N = Graph.GetLength(0);
            NIL_VERT_ID = N - 1;
            Pair_G1 = Enumerable.Repeat(NIL_VERT_ID, N).ToArray();
            Pair_G2 = Enumerable.Repeat(NIL_VERT_ID, N).ToArray();
            Dist = new int[N];
            while (BFS())
            {
                for (int v = 0; v < FIRST_PART_COUNT; v++)
                {
                    if (Pair_G1[v] == NIL_VERT_ID)
                        DFS(v);
                }
            }
            return Pair_G1.Take(FIRST_PART_COUNT).ToArray();
        }

        bool BFS()
        {
            Queue<int> q = new Queue<int>();
            for (int v = 0; v < FIRST_PART_COUNT; v++)
            {
                if (Pair_G1[v] == NIL_VERT_ID)
                {
                    Dist[v] = 0;
                    q.Enqueue(v);
                }
                else
                    Dist[v] = int.MaxValue;
            }

            Dist[NIL_VERT_ID] = int.MaxValue;

            while (q.Count != 0)
            {
                int v = q.Dequeue();
                for (int u = FIRST_PART_COUNT; u < N; u++)
                {
                    if (Graph[u, v] && Dist[Pair_G2[u]] == int.MaxValue)
                    {
                        Dist[Pair_G2[u]] = Dist[v] + 1;
                        q.Enqueue(Pair_G2[u]);
                    }
                }
            }

            return Dist[NIL_VERT_ID] != int.MaxValue; 
        }

        bool DFS(int v) 
        {
            if (v != NIL_VERT_ID)
            {
                for (int u = FIRST_PART_COUNT; u < N; u++)
                {
                    if (Graph[u, v] && Dist[Pair_G2[u]] == Dist[v] + 1)
                        if (DFS(Pair_G2[u]))
                        {
                            Pair_G2[u] = v;
                            Pair_G1[v] = u;
                            return true;
                        }
                }
                Dist[v] = int.MaxValue;
                return false;
            }
            return true;
        }

        bool[,] MakeGraph(int timeslot)
        {
            int eventsCount = TimeslotEvents[timeslot].Count;
            //verts 0..evenstCount - 1 - events verts ids
            //verts eventsCount..eventsCount + data.Rooms.Count - 1 - rooms verts ids
            //vert eventsCount + data.Rooms.Count - NULL vert
            var res = new bool[eventsCount + data.Rooms.Length + 1, eventsCount + data.Rooms.Length + 1];
            for (int e = 0; e < eventsCount; e++)
            {
                Event ev = events[TimeslotEvents[timeslot][e]];
                for (int r = 0; r < data.Rooms.Length; r++)
                {
                    res[e, eventsCount + r] = res[eventsCount + r, e] = data.SuitableRoom(ev.Id, data.Rooms[r].Id);
                }
            }
            return res;
        }

        bool[,] MakeWeekSpecificGraph(int timeslot)
        {
            int eventsCount = TimeslotEvents[timeslot].Count;
            //verts 0..evenstCount - 1 - events verts ids
            //verts eventsCount..eventsCount + data.Rooms.Count - 1 - rooms verts ids
            //vert eventsCount + data.Rooms.Count - NULL vert
            var res = new bool[eventsCount + data.Rooms.Length + 1, eventsCount + data.Rooms.Length + 1];
            List<int> busyRooms = new List<int>();
            for (int e = 0; e < eventsCount; e++)
            {
                Event ev = events[TimeslotEvents[timeslot][e]];
                if (data.IsEveryWeekEvent(ev.Id))
                {
                    int suitableRoom = Result[TimeslotEvents[timeslot][e]].RoomId;
                    int roomIndex = Array.FindIndex(data.Rooms, r => r.Id == suitableRoom);
                    res[e, eventsCount + roomIndex] = res[eventsCount + roomIndex, e] = true;
                    busyRooms.Add(suitableRoom);
                }
            }
            
            for (int e = 0; e < eventsCount; e++)
            {
                Event ev = events[TimeslotEvents[timeslot][e]];
                if (!data.IsEveryWeekEvent(ev.Id))
                {
                    for (int r = 0; r < data.Rooms.Length; r++)
                    {
                        res[e, eventsCount + r] = res[eventsCount + r, e] =
                            data.SuitableRoom(ev.Id, data.Rooms[r].Id) && !busyRooms.Contains(data.Rooms[r].Id);
                    }
                }
            }
            return res;
        } 

        #endregion
    }
}

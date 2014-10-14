using System;
using System.Linq;
using System.Collections.Generic;

namespace SchedulerProject.Core
{
    public class TimeSlot : IComparable<TimeSlot>, IEquatable<TimeSlot>
    {
        public static IEnumerable<TimeSlot> EnumerateAll(int days, int slots)
        {
            for (var day = 1; day <= days; day++)
            {
                for (var slot = 1; slot <= slots; slot++)
                {
                    yield return new TimeSlot(day, slot);
                }
            }
        }

        public static int ToId(TimeSlot timeSlot, int daysCount, int slotsCount)
        {
            if (daysCount <= 0)
                throw new ArgumentException("daysCount");
            if (slotsCount <= 0)
                throw new ArgumentException("slotsCount");

            if (timeSlot.Day > daysCount || timeSlot.Slot > slotsCount)
                throw new ArgumentException("timeSlot");

            return (timeSlot.Day - 1) * slotsCount + (timeSlot.Slot - 1);
        }


        public static TimeSlot FromId(int id, int daysCount, int slotsCount)
        {
            if (daysCount <= 0)
                throw new ArgumentException("daysCount");
            if (slotsCount <= 0)
                throw new ArgumentException("slotsCount");

            int day = 1 + id / slotsCount;
            int slot = 1 + id % slotsCount;

            if (day > daysCount || slot > slotsCount)
                throw new ArgumentException("id");

            return new TimeSlot(day, slot);
        }

        public TimeSlot(int day, int slot)
        {
            Day = day;
            Slot = slot;
        }

        // 1..
        public int Day, Slot;

        public override string ToString()
        {
            return string.Format("({0} - {1})", Day, Slot);
        }

        public static TimeSlot Parse(string str)
        {
            var tmp = str.Substring(1, str.Length - 2).Split('-').Select(s => int.Parse(s.Trim())).ToArray();
            return new TimeSlot(tmp[0], tmp[1]);
        }

        public int CompareTo(TimeSlot other)
        {
            int d = Day.CompareTo(other.Day);
            return d == 0 ? Slot.CompareTo(other.Slot) : d;
        }

        public override int GetHashCode()
        {
            return Day * 100 + Slot;
        }

        public bool Equals(TimeSlot other)
        {
            return CompareTo(other) == 0;
        }
    }

    public enum TimeConstraintType
    {
        Desirible,  //soft constraint
        Undesirible,//soft constraint
        Necessary,  //hard constraint
        Impossible, //hard constraint
    }

    public class TimeConstraintsSet : IEnumerable<TimeSlot>
    {
        public TimeConstraintsSet(TimeConstraintType type)
        {
            Type = type;
        }

        public TimeConstraintType Type { get; set; }

        List<TimeSlot> constraints = new List<TimeSlot>();

        public void AddConstraint(int day, int slot)
        {
            constraints.Add(new TimeSlot(day, slot));
        }

        public void AddConstraint(TimeSlot slot)
        {
            if (!constraints.Contains(slot))
                constraints.Add(slot);
        }

        public void AddConstraintsRange(params TimeSlot[] slots)
        {
            foreach (var slot in slots)
                AddConstraint(slot);
        }

        IEnumerator<TimeSlot> IEnumerable<TimeSlot>.GetEnumerator()
        {
            return constraints.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return constraints.GetEnumerator();
        }

        public override string ToString()
        {
            return Type.ToString() + " [" + string.Join(",", constraints) + "]";
        }

        public static TimeConstraintsSet Parse(string str)
        {
            int typeLength = str.IndexOf(' ');
            string dataStr = str.Substring(typeLength).Trim();
            TimeConstraintType type = ParseHelper.ParseEnum<TimeConstraintType>(str.Substring(0, typeLength));
            return new TimeConstraintsSet(type)
            {
                constraints = dataStr.Substring(1, dataStr.Length - 2)
                                     .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => TimeSlot.Parse(s.Trim()))
                                     .ToList()
            };
        }
    }

    public class TimeConstraints
    {
        // TODO: if there are NecessaryTimeSlots, do not take into account other constraints

        //public TimeConstraintsSet DesiribleTimeSlots { get; private set; }
        //public TimeConstraintsSet UndesiribleTimeSlots { get; private set; }
        //public TimeConstraintsSet ImpossibleTimeSlots { get; private set; }
        //public TimeConstraintsSet NecessaryTimeSlots { get; private set; }

        TimeConstraintsSet desiribleTimeSlots = new TimeConstraintsSet(TimeConstraintType.Desirible);
        TimeConstraintsSet undesiribleTimeSlots = new TimeConstraintsSet(TimeConstraintType.Undesirible);
        TimeConstraintsSet impossibleTimeSlots = new TimeConstraintsSet(TimeConstraintType.Impossible);
        TimeConstraintsSet necessaryTimeSlots = new TimeConstraintsSet(TimeConstraintType.Necessary);

        public override string ToString()
        {
            return string.Format("[\n{0};\n{1};\n{2};\n{3};\n]", 
                                 desiribleTimeSlots, undesiribleTimeSlots, impossibleTimeSlots, necessaryTimeSlots);
        }

        public bool IsEmpty
        {
            get { return EnumerateConstraintsSets().All(s => !s.Any()); }
        }

        public static TimeConstraints Parse(string str)
        {
            var tmp = str.Substring(1, str.Length - 2)
                         .Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)                         
                         .Select(s => TimeConstraintsSet.Parse(s.Trim()))
                         .ToArray();

            return new TimeConstraints()
            {
                desiribleTimeSlots = tmp[0],
                undesiribleTimeSlots = tmp[1],
                impossibleTimeSlots = tmp[2],
                necessaryTimeSlots = tmp[3]
            };
        }

        public static TimeConstraints DeepCopy(TimeConstraints value)
        {
            // quick and dirty
            return value == null ? null : TimeConstraints.Parse(value.ToString());
        }

        public IEnumerable<TimeConstraintsSet> EnumerateConstraintsSets()
        {
            if (desiribleTimeSlots != null)
                yield return desiribleTimeSlots;
            if (undesiribleTimeSlots != null)
                yield return undesiribleTimeSlots;
            if (impossibleTimeSlots != null)
                yield return impossibleTimeSlots;
            if (necessaryTimeSlots != null)
                yield return necessaryTimeSlots;
        }

        public TimeConstraintsSet this[TimeConstraintType type]
        {
            get
            {
                switch (type)
                {
                    case TimeConstraintType.Desirible:
                        return desiribleTimeSlots;
                    case TimeConstraintType.Impossible:
                        return impossibleTimeSlots;
                    case TimeConstraintType.Necessary:
                        return necessaryTimeSlots;
                    case TimeConstraintType.Undesirible:
                        return undesiribleTimeSlots;
                    default:
                        return null;
                }
            }
        }
    }
}
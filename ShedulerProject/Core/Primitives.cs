using System;
using System.Linq;
using System.Collections.Generic;

namespace SchedulerProject.Core
{
    public static class ParseHelper
    {  
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }

    public abstract class AbstractPrimitive<T> : IEquatable<AbstractPrimitive<T>>
    {
        public int Id = -1;

        public virtual bool IsEmpty
        {
            get { return Id == -1; }
        }

        public abstract void CopyTo(T destination);

        // UID of the primitive, Id field presents just for performance
        protected abstract string StringRepresentation();

        public override string ToString()
        {
            return StringRepresentation();
        }

        public bool Equals(AbstractPrimitive<T> other)
        {
            return ReferenceEquals(this, other) || 
                   this.StringRepresentation() == other.StringRepresentation();
        }
    }

    public interface ITimeConstrainedPrimitive
    {
        TimeConstraints TimeConstraints { get; }
    }

    public enum RoomType
    {
        Assigned,
        Laboratory,
        Lecture,
        Practice
    }

    public class Subject : AbstractPrimitive<Subject>
    {
        public string Name = string.Empty;
        public int Course = 1;
        public int Difficulty = 1;
        public int? LecturerId;

        public override bool IsEmpty
        {
            get { return base.IsEmpty || string.IsNullOrWhiteSpace(Name); }
        }
        
        public override void CopyTo(Subject destination)
        {
            destination.Id = Id;
            destination.Course = Course;
            destination.Name = Name;
            destination.Difficulty = Difficulty;
            if (LecturerId.HasValue)
                destination.LecturerId = LecturerId.Value;
        }

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    public class Lecturer : AbstractPrimitive<Lecturer>, ITimeConstrainedPrimitive
    {
        public string Name = string.Empty;
        public TimeConstraints TimeConstraints { get; set; }

        public override bool IsEmpty
        {
            get { return base.IsEmpty || string.IsNullOrWhiteSpace(Name); }
        }

        public override void CopyTo(Lecturer destination)
        {
            destination.Id = Id;
            destination.Name = Name;
            destination.TimeConstraints = TimeConstraints.DeepCopy(TimeConstraints);
        }

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    public class Group : AbstractPrimitive<Group>
    {
        public int Course = 1;
        public string Name = string.Empty;

        public override bool IsEmpty
        {
            get { return base.IsEmpty || string.IsNullOrWhiteSpace(Name); }
        }

        public override void CopyTo(Group destination)
        {
            destination.Id = Id;
            destination.Course = Course;
            destination.Name = Name;
        }

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    //public class TimeConstraints
    //{
    //    public int[] HardTimeSlotsConstraints;
    //    public int[] SoftTimeSlotsConstraints;
    //}

    public class Room : AbstractPrimitive<Room>, ITimeConstrainedPrimitive
    {
        public string Housing = string.Empty;
        public string RoomNumber = string.Empty;
        public RoomType Type;
        public TimeConstraints TimeConstraints { get; set; }

        public override bool IsEmpty
        {
            get { return base.IsEmpty || string.IsNullOrWhiteSpace(RoomNumber) || string.IsNullOrWhiteSpace(Housing); } 
        }

        public override void CopyTo(Room destination)
        {
            destination.Id = Id;
            destination.Housing = Housing;
            destination.RoomNumber = RoomNumber;
            destination.Type = Type;
            destination.TimeConstraints = TimeConstraints.DeepCopy(TimeConstraints);
        }

        protected override string StringRepresentation()
        {
            return string.Format("{0} - {1} ({2})", RoomNumber, Housing, Type);
        }
    }

    public class Event : AbstractPrimitive<Event>
    {
        public bool OnceInTwoWeeks;
        public int LecturerId;
        public int SubjectId;
        public int[] Groups;
        public RoomType RoomType;
        public int HardAssignedRoom = -1;

        public static int[] ParseGroups(string str)
        {
            return str.Split(',').Select(int.Parse).ToArray();
        }

        public static string GroupsToString(int[] groupIds)
        {
            return string.Join(",", groupIds);
        }

        public override bool IsEmpty
        {
            get { return base.IsEmpty || LecturerId < 0 || SubjectId < 0 || !Groups.Any(); }
        }

        public override void CopyTo(Event destination)
        {
            destination.Id = Id;
            destination.LecturerId = LecturerId;
            destination.SubjectId = SubjectId;
            destination.RoomType = RoomType;
            destination.Groups = Groups.AsEnumerable().ToArray();
            destination.OnceInTwoWeeks = OnceInTwoWeeks;
            destination.HardAssignedRoom = HardAssignedRoom;
        }

        protected override string StringRepresentation()
        {
            return string.Format("{0} - {1} ({2})", SubjectId, LecturerId, RoomType);
        }
        
        // should be set by the sheduler
        //public int AssignedTimeSlotId;
        //public int AssignedRoomId;
    }
}
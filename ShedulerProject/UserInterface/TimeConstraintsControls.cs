using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public class SingleTimeSlotConstraintsController : TimeSlotControl<SingleTimeSlotConstraintsController>
    {
        public SingleTimeSlotConstraintsController()
        {
            RefreshBackColor();
        }

        TimeConstraintType?[] availableConstraints = new TimeConstraintType?[]{ null,
                                                                                TimeConstraintType.Desirible, 
                                                                                TimeConstraintType.Undesirible, 
                                                                                TimeConstraintType.Impossible, 
                                                                                TimeConstraintType.Necessary};

        Color[] constrainsColors = new Color[] { Color.White, Color.LightGreen, Color.Orange, Color.Red, Color.DarkBlue };

        int currIndex = 0;

        public TimeConstraintType? SelectedConstraint
        {
            get { return availableConstraints[currIndex]; }
            set
            {
                currIndex = Array.IndexOf(availableConstraints, value);
                if (currIndex == -1) currIndex = 0;
                RefreshBackColor();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                currIndex = (currIndex + 1) % availableConstraints.Length;
            else if (e.Button == MouseButtons.Right)
            {
                if (currIndex == 0)
                    currIndex = availableConstraints.Length - 1;
                else
                    currIndex--;
            }
            RefreshBackColor();
            base.OnMouseClick(e);
        }

        void RefreshBackColor()
        {
            BackColor = constrainsColors[currIndex];
        }
    }

    public class TimeSlotsConstraintsEditControl : TimeSlotsGrid<SingleTimeSlotConstraintsController>
    {
        static int DAYS_COUNT = 6, SLOTS_COUNT = 5;
        public TimeSlotsConstraintsEditControl(Size slotSize)
            : base(DAYS_COUNT, SLOTS_COUNT)
        {
            TimeSlotControlSize = slotSize;
            foreach (var slot in TimeSlot.EnumerateAll(DAYS_COUNT, SLOTS_COUNT))
                AddControlToSlot(new SingleTimeSlotConstraintsController()
                {
                    TimeSlot = slot,
                    Size = slotSize
                });
        }

        public TimeConstraints SelectedConstraints
        {
            get { return CollectTimeConstaints(); }
            set { FillTimeConstraints(value); }
        }

        TimeConstraints CollectTimeConstaints()
        {
            var result = new TimeConstraints();
            FillConstraintSet(result, TimeConstraintType.Impossible);
            FillConstraintSet(result, TimeConstraintType.Undesirible);
            FillConstraintSet(result, TimeConstraintType.Necessary);
            FillConstraintSet(result, TimeConstraintType.Desirible);
            return result;
        }

        void FillConstraintSet(TimeConstraints constraints, TimeConstraintType type)
        {
            var slots = from c in EnumerateTimeSlotsControls()
                        where c.SelectedConstraint == type
                        select c.TimeSlot;
            constraints[type].AddConstraintsRange(slots.ToArray());
        }

        void FillTimeConstraints(TimeConstraints constraints)
        {
            foreach (var constraintsSet in constraints.EnumerateConstraintsSets())
                foreach(var timeSlot in constraintsSet)
                {
                    GetTimeSlotControl(timeSlot).SelectedConstraint = constraintsSet.Type;
                }
        }
    }
}

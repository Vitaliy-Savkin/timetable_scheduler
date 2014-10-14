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
    public delegate void PropertyChangedEventHandler<T, C>(C sender, T oldValue, T newValue)
        where C : TimeSlotControl<C>;

    public class TimeSlotControl<T> : Control
        where T : TimeSlotControl<T>
    {
        protected TimeSlot timeSlot;
        public TimeSlot TimeSlot
        {
            get { return timeSlot; }
            set
            {
                var owningGrid = OwningGrid;
                if (owningGrid != null)
                {
                    owningGrid.RemoveControlFromSlot(timeSlot);
                }
                TimeSlot prevValue = timeSlot;
                timeSlot = value;
                if (owningGrid != null)
                {
                    owningGrid.AddControlToSlot(this as T);
                }
                OnTimeSlotChanged(prevValue, value);
            }
        }

        public TimeSlotsGrid<T> OwningGrid
        {
            get { return Parent as TimeSlotsGrid<T>; }
            set
            {
                value.AddControlToSlot(this as T);
            }
        }

        protected virtual void OnTimeSlotChanged(TimeSlot oldValue, TimeSlot newValue)
        {
            if (TimeSlotChanged != null)
                TimeSlotChanged(this as T, oldValue, newValue);
        }

        public event PropertyChangedEventHandler<TimeSlot, T> TimeSlotChanged;
    }

    public class TimeSlotsGrid<ControlType> : UserControl
        where ControlType : TimeSlotControl<ControlType>
    {
        string[] dayNames = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        string[] slotNames = { "Первая пара", "Вторая пара", "Третья пара", "Четвертая пара", "Пятая пара" };

        protected int slotsCount = 5, daysCount = 6;

        public TimeSlotsGrid(int days, int slots)
        {
            if (days <= 0 || days > dayNames.Length)
            {
                throw new ArgumentOutOfRangeException("days");
            }
            if (slots <= 0 || slots > slotNames.Length)
            {
                throw new ArgumentOutOfRangeException("slots");
            }
            daysCount = days;
            slotsCount = slots;
            minTimeSlot = new TimeSlot(1, 1);
            maxTimeSlot = new TimeSlot(daysCount, slotsCount);
            gridPen = Pens.Black;
        }

        TimeSlot minTimeSlot, maxTimeSlot;
        Size timeSlotControlSize;
        Pen gridPen;

        int slotOffset;
        int dayOffset;
        int minTimeSlotControlWidth;
        int minTimeSlotControlHeight;
        int penWidth;

        void SetControlLocation(TimeSlot timeSlot, TimeSlotControl<ControlType> control)
        {
            var x = slotOffset + (timeSlot.Day - 1) * (TimeSlotControlSize.Width + penWidth) + penWidth;
            var y = dayOffset + (timeSlot.Slot - 1) * (TimeSlotControlSize.Height + penWidth) + penWidth;
            control.Location = new Point(x, y);
        }

        void CalculateControlParams()
        {
            using (Graphics g = CreateGraphics())
            {
                SizeF[] slotRectangles = slotNames.Select(s => g.MeasureString(s, Font)).ToArray();
                SizeF[] dayRectangles = dayNames.Select(s => g.MeasureString(s, Font)).ToArray();
                slotOffset = (int)slotRectangles.Max(r => r.Width);
                dayOffset = (int)dayRectangles.Max(r => r.Height);
                minTimeSlotControlWidth = (int)dayRectangles.Max(r => r.Width) + 4;
                minTimeSlotControlHeight = (int)slotRectangles.Max(r => r.Height) + 4;
                timeSlotControlSize = new Size(Math.Max(minTimeSlotControlWidth, timeSlotControlSize.Width),
                                               Math.Max(minTimeSlotControlHeight, timeSlotControlSize.Height));
                penWidth = (int)gridPen.Width;
                Size = MaximumSize = new Size(
                    slotOffset + daysCount * timeSlotControlSize.Width + penWidth * (daysCount + 1),                                              
                    dayOffset + slotsCount * timeSlotControlSize.Height + penWidth * (slotsCount + 1));

                foreach (var pair in timeSlotControls)
                    SetControlLocation(pair.Key, pair.Value);
            }
        }

        public Size TimeSlotControlSize
        {
            get { return timeSlotControlSize; }
            set
            {
                timeSlotControlSize = value;
                CalculateControlParams();
                //OnTimeSlotSizeChanged();
            }
        }

        public Pen GridPen
        {
            get { return gridPen; }
            set
            {
                gridPen = value;
                CalculateControlParams();
            }
        }

        Dictionary<TimeSlot, ControlType> timeSlotControls = new Dictionary<TimeSlot, ControlType>();

        public void AddControlToSlot(ControlType control)
        {
            TimeSlot timeSlot = control.TimeSlot;
            if (timeSlot.CompareTo(minTimeSlot) >= 0 && 
                timeSlot.CompareTo(maxTimeSlot) <= 0 &&
                !Controls.Contains(control))
            {
                RemoveControlFromSlot(timeSlot);
                timeSlotControls[timeSlot] = control;
                Controls.Add(control);
                SetControlLocation(timeSlot, control);
            }
        }

        public void RemoveControlFromSlot(TimeSlot slot)
        {
            TimeSlotControl<ControlType> control = GetTimeSlotControl(slot);
            if (control != null)
            {
                Controls.Remove(control);
                timeSlotControls.Remove(slot);
            }
        }

        public void ClearAllTimeSlots()
        {
            var timeSlots = timeSlotControls.Keys.ToArray();
            foreach (var timeSlot in timeSlots)
            {
                RemoveControlFromSlot(timeSlot);
            }
        }

        public ControlType GetTimeSlotControl(TimeSlot slot)
        {
            ControlType control;
            if (timeSlotControls.TryGetValue(slot, out control))
            {
                return control;
            }
            return null;
        }

        public IEnumerable<ControlType> EnumerateTimeSlotsControls()
        {
            return timeSlotControls.Values.AsEnumerable();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            CalculateControlParams();
            base.OnFontChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var x = slotOffset;
            e.Graphics.DrawLine(gridPen, x, dayOffset, x, Height);
            for (var day = 0; day < daysCount; day++)
            {
                e.Graphics.DrawString(dayNames[day], Font, new SolidBrush(ForeColor),
                    new RectangleF(x, 0, TimeSlotControlSize.Width, minTimeSlotControlHeight),
                    new StringFormat() { Alignment = StringAlignment.Center });
                x += TimeSlotControlSize.Width + penWidth;
                e.Graphics.DrawLine(gridPen, x, dayOffset, x, Height);
            }

            var y = dayOffset;
            e.Graphics.DrawLine(gridPen, slotOffset, y, Width, y);
            for (var slot = 0; slot < slotsCount; slot++)
            {
                e.Graphics.DrawString(slotNames[slot], Font, new SolidBrush(ForeColor),
                    new RectangleF(0, y, minTimeSlotControlWidth, TimeSlotControlSize.Height),
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                y += TimeSlotControlSize.Height + penWidth;
                e.Graphics.DrawLine(gridPen, slotOffset, y, Width, y);
            }

            base.OnPaint(e);
        }
    }
}

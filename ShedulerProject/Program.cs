using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using SchedulerProject.Core;
using SchedulerProject.UserInterface;

namespace SchedulerProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TimeTable timeTable = TimeTable.LoadFromXml("..\\..\\..\\time_table_input.xml");
            //var sw = System.Diagnostics.Stopwatch.StartNew();
            //EventAssignment[] result = Sheduler.Shedule(timeTable);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            //for (int i = 0; i < result.Length; i++)
            //{
            //    //Console.WriteLine("Event {0} -> TimeSlot: {1}, Room: {2}", i, result[i].TimeSlotId, result[i].RoomId);
            //    Console.WriteLine("{0}\t{1}\t{2}", i, result[i].TimeSlotId, result[i].RoomId);            
            //}
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm f = new MainForm();
            Application.Run(f);
        }
    }
}

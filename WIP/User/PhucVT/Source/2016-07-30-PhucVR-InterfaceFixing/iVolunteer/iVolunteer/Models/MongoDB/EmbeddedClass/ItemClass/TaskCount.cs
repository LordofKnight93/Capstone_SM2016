using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    public class TaskCount
    {
        public int AllTask { get; set; }
        public int Done { get; set; }
        public int Doing { get; set; }
        public int Pending { get; set; }
        public int Rework { get; set; }
        public TaskCount()
        {
            this.AllTask = 0;
            this.Done = 0;
            this.Doing = 0;
            this.Pending = 0;
            this.Rework = 0;
        }
        public TaskCount(int all, int done, int doing, int pending, int rework)
        {
            this.AllTask = all;
            this.Done = done;
            this.Doing = doing;
            this.Pending = pending;
            this.Rework = rework;
        }
    }
}
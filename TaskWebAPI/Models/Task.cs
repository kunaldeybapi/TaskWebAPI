using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskWebAPI.Models
{
    public class Task
    {
        public Int32 taskID { get; set; }        
        public string parentTask { get; set; }
        public string task { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int priority { get; set; }
        public bool isTaskComplete { get; set; }
    }
}
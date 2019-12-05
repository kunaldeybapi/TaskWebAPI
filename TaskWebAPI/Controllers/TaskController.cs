using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TaskWebAPI.Models;

namespace TaskWebAPI.Controllers
{    
    public class TaskController : ApiController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["TaskDB"].ConnectionString.ToString();
        // GET: api/Task
        public List<Task> Get()
        {
            List<Task> taskList = new List<Task>();
            DataTable dt = new DataTable();            
            using (SqlConnection con=new SqlConnection (connectionString))
            {
                
                con.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM TASK",con))
                {
                    adapter.Fill(dt);
                }
            }

            foreach (DataRow dr in dt.Rows)
            {
                Task temp = new Task();
                DateTime tempdate = new DateTime();
                temp.taskID = Convert.ToInt32(dr[0]);
                temp.parentTask = (dr[1].ToString().Length > 0) ? dr[1].ToString() : "";
                temp.task = dr[2].ToString();
                tempdate = Convert.ToDateTime(dr[3]);
                temp.startDate = tempdate.ToShortDateString();
                tempdate = Convert.ToDateTime(dr[4]);
                temp.endDate = tempdate.ToShortDateString();
                temp.priority= Convert.ToInt32(dr[5]);
                temp.isTaskComplete = Convert.ToBoolean(dr[6]);
                taskList.Add(temp);
            }
            return taskList;
        }

        // GET: api/Task/5
        public Task Get(int id)
        {
            DataTable dt = new DataTable();
            Task selectedTask = new Task();
            using (SqlConnection con = new SqlConnection(connectionString))
            {

                con.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM TASK WHERE TASK_ID="+id, con))
                {
                    adapter.Fill(dt);
                }
            }

            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                DateTime tempdate = new DateTime();
                selectedTask.taskID = Convert.ToInt32(dr[0]);
                selectedTask.parentTask = (dr[1].ToString().Length > 0) ? dr[1].ToString() : "";
                selectedTask.task = dr[2].ToString();
                tempdate = Convert.ToDateTime(dr[3]);
                selectedTask.startDate = tempdate.ToShortDateString();
                tempdate = Convert.ToDateTime(dr[4]);
                selectedTask.endDate = tempdate.ToShortDateString();
                selectedTask.priority = Convert.ToInt32(dr[5]);
                selectedTask.isTaskComplete = Convert.ToBoolean(dr[6]);
            }            
            return selectedTask;
        }

        // POST: api/Task
        public void Post(Task task)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO Task(Parent_ID,Task,Start_Date,End_Date,Priority) VALUES(@parentTaskID,@task,@startDate,@endDate,@priority)";
                SqlCommand cmd = new SqlCommand(sqlQuery,con);
                con.Open();                
                cmd.Parameters.AddWithValue("@parentTaskID", task.parentTask);
                cmd.Parameters.AddWithValue("@task", task.task);
                cmd.Parameters.AddWithValue("@startDate", task.startDate);
                cmd.Parameters.AddWithValue("@endDate", task.endDate);
                cmd.Parameters.AddWithValue("@priority", task.priority);

                int rowcount = cmd.ExecuteNonQuery();                
            }

            if (task.parentTask != null && task.parentTask != "")
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string sqlQuery = "INSERT INTO ParentTask(Parent_Task) VALUES(@parentTask)";
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@parentTask", task.parentTask);

                    int rowcount = cmd.ExecuteNonQuery();
                }
            }            
        }

        // PUT: api/Task/5
        [HttpPost]
        [Route("api/UpdateTask")]
        public void UpdateTask(Task task)
        {
            using (SqlConnection con=new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE TASK SET Task=@task, Parent_ID=@parentTaskID, Start_Date=@startDate, End_Date=@endDate, Priority=@priority WHERE Task_ID=" + task.taskID;
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                cmd.Parameters.AddWithValue("@parentTaskID", task.parentTask);
                cmd.Parameters.AddWithValue("@task", task.task);
                cmd.Parameters.AddWithValue("@startDate", task.startDate);
                cmd.Parameters.AddWithValue("@endDate", task.endDate);
                cmd.Parameters.AddWithValue("@priority", task.priority);
                int rowcount = cmd.ExecuteNonQuery();
            }            
        }

        // DELETE: api/Task/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("api/CompleteTask")]
        public void CompleteTask(Task task)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE TASK SET isTaskComplete=1 WHERE Task_ID=" + task.taskID;
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();                                
                int rowcount = cmd.ExecuteNonQuery();
            }
        }
    }
}

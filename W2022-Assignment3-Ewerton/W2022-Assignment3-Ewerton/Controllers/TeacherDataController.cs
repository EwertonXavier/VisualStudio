using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using W2022_Assignment3_Ewerton.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;
// Upgrade proposal: Function QuerySchoolDB( string query)
// I had created a class to query the school database and return result as a Array String(other project, same assignment).
// I thought about working to transform the results into a JSON and then creating objects using them.
// The advantage would be a reusable function to query the SchoolDB receiving a query as parameter
// Maybe I am going to work on this in the future.
namespace W2022_Assignment3_Ewerton.Controllers
{
    /// <summary>
    /// API to retrieve Teacher data from SchoolDB database
    /// </summary>
    public class TeacherDataController : ApiController
    {
        /// <summary>
        /// Describe all information related to teacherid stored inside the teachers table
        /// </summary>
        /// <example>GET:api/TeacherData/Describe/5 </example>
        /// <param name="teacherid"></param>
        /// <returns>Teacher teacher</returns>
        [HttpGet]
        [Route("api/TeacherData/Describe/{teacherid}")]
        public Teacher Describe(int teacherid = 0)
        {
            //creating db context
            SchoolDbContext dbCtx = new SchoolDbContext();

            //creating connection to schoolDB 
            MySqlConnection conn = dbCtx.AccessDatabase();
            
            //opening connection
            conn.Open();

            //creating command
            MySqlCommand cmd = conn.CreateCommand();

            //writing command query which select all information from teacher table using parameter teacher to filter rows
            cmd.CommandText = "SELECT * FROM teachers t WHERE teacherid = @key";

            //create parameter and prepares command
            cmd.Parameters.AddWithValue("@key",teacherid);
            cmd.Prepare();

            

            //execute command
            MySqlDataReader teachers = cmd.ExecuteReader();

            Teacher teacher = new Teacher(); //object to represent teacher data

            if (teachers.Read()) //I assumed the query will only bring one result. If more rows are return, only the first will be read. 
            {
                teacher.FName = teachers["teacherfname"].ToString();
                teacher.LName = teachers["teacherlname"].ToString();
                teacher.Id = Convert.ToInt32(teachers["teacherid"]);
                teacher.HireDate = Convert.ToDateTime(teachers["hiredate"]);
                teacher.Salary = Convert.ToDouble(teachers["salary"]);
                teacher.EmployeeNumber = teachers["employeenumber"].ToString();//besides the name saying "number" it s not a number :D
            }
            else
            {//case there is no string returned
                teacher.Anonymize();
            }
            conn.Close(); // close connection with DB
            return teacher;
        }


        /// <summary>
        /// Fetch all teachers info from teachers table
        /// </summary>
        /// <returns>A list<Teacher> with all attributes()</returns>
        [HttpGet]
        [Route("api/TeacherData/List/")]
        public List<Teacher> List()
        {
            //creating db context
            SchoolDbContext dbCtx = new SchoolDbContext();

            //creating connection to schoolDB 
            MySqlConnection conn = dbCtx.AccessDatabase();

            //opening connection
            conn.Open();

            //creating command
            MySqlCommand cmd = conn.CreateCommand();

            //writing command query which select all information from teacher table using parameter teacher to filter rows
            cmd.CommandText = "SELECT * FROM teachers t;";

            //execute command store rows in reachers
            MySqlDataReader teachers = cmd.ExecuteReader();

            //list to store all Intances of Teachers created from db
            List<Teacher> teacherList = new List<Teacher>();

            while (teachers.Read()) //reads each row from query
            {
                //object to represent teacher data
                Teacher teacher = new Teacher();
                teacher.FName = teachers["teacherfname"].ToString();
                teacher.LName = teachers["teacherlname"].ToString();
                teacher.Id = Convert.ToInt32(teachers["teacherid"]);
                teacher.HireDate = Convert.ToDateTime(teachers["hiredate"]);
                teacher.Salary = Convert.ToDouble(teachers["salary"]);
                teacher.EmployeeNumber = teachers["employeenumber"].ToString();//besides the name saying "number" it s not a number :D
                teacherList.Add(teacher); // adds the created teacher to the list
            }


            //Closing connection
            conn.Close();
            return teacherList;
        }

        /// <summary>
        /// Method to Create a new teacher in teachers table
        /// </summary>
        /// <example> 
        /// POST /api/teacherdata/new 
        /// type x-www-form-urlencoded 
        ///</example>
        /// <param name="teacher">
        /// FName:'Ewerton'
        /// LName:'Xavier de Freitas'
        /// HireDate:03/18/2022
        /// EmployeeNumber:'T9123'
        /// Salary:45.78</param>
        /// <returns>message "Success" if successfull or "Failed" if it encountered an error </returns>
        [HttpPost]
        public string AddTeacher([FromBody] Teacher teacher) //[FroBody] prepares controller to receive properties from body request. //I was using postman to test it
        {
            //create a connection with DB
            MySqlConnection conn = new SchoolDbContext().AccessDatabase();

            //open connection
            conn.Open();

            //create command
            MySqlCommand cmd = conn.CreateCommand();

            //write command 
            Debug.WriteLine(teacher.HireDate);
            string hireDate = teacher.HireDate.ToString("MM/dd/yyyy");//transform datetime to string using parameter format (couldn't get to make datetime to mysql date work)
            cmd.CommandText = "INSERT INTO teachers(teacherfname,teacherlname, employeenumber, hiredate, salary) VALUES ( "+"@teacherFName"+"," + "@teacherLName" + ", " + "@employeeNumber" + ", STR_TO_DATE(@hiredate, '%m/%d/%Y'), " + "@salary" + "); ";

            //create parameters
            cmd.Parameters.AddWithValue("@teacherFName", teacher.FName);
            cmd.Parameters.AddWithValue("@teacherLName", teacher.LName);
            cmd.Parameters.AddWithValue("@employeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@hiredate", hireDate);
            cmd.Parameters.AddWithValue("@salary", teacher.Salary);
            cmd.Prepare();

            //execute command
            int reader = cmd.ExecuteNonQuery();
            conn.Close();//closes command
            if (reader>0)//check how many rows were affected. The idea would be: check if there if the number of rows affected is greater than 0. To be careful we could use =1
            {
                return "SUCCESS";
               // return Request.CreateResponse(HttpStatusCode.OK); //creates  200 http response. Apparently, this is not being used anymore
            }

            else return "FAILED";//Request.CreateResponse(HttpStatusCode.InternalServerError); //creates 500 http response.// Because all key from DB can be null these is never going to be executed
        }


        /// <summary>
        /// Function to find a teacher based on the employeeId
        /// </summary>
        /// <param name="employeeNumber"></param>
        /// <returns>teacherid if employeeNumber exist in DB.
        /// -1 otherwise</returns>
        [HttpGet]
        [Route("api/TeacherData/Find/{employeeNumber}")]
        public int Find(string employeeNumber)
        {
            //teacherId
            int teacherId = -1;
            //create a connection with DB
            MySqlConnection conn = new SchoolDbContext().AccessDatabase();

            //open connection
            conn.Open();

            //create command
            MySqlCommand cmd = conn.CreateCommand();

            //add query
            cmd.CommandText = "SELECT teacherid FROM teachers t WHERE employeenumber = @employeeNumber";

            //add parameters to the query
            cmd.Parameters.AddWithValue("@employeeNumber", employeeNumber);

            //prepares query
            cmd.Prepare();

            

            //reads DB
            MySqlDataReader teacherFound = cmd.ExecuteReader();



            if (teacherFound.Read())
            {
                //close connection + return
                return Convert.ToInt32(teacherFound["teacherId"]);
            }


            else return teacherId;
        
        }



        /// <summary>
        /// Deletes a teacher from DB.
        /// </summary>
        /// <param name="teacherid"></param>
        /// <returns>SUCCESS in case deletion happened. FAILED IN CASE DELETION WASN'T COMPLETEDE</returns>
        [HttpPost]
        public string Remove(int teacherid)
        {
            //teacherId
            string returningMessage = "";
            //create a connection with DB
            MySqlConnection conn = new SchoolDbContext().AccessDatabase();

            Debug.Write("Teacher id " + teacherid);
            //open connection
            conn.Open();

            //create command
            MySqlCommand cmd = conn.CreateCommand();

            //add query
            cmd.CommandText = "DELETE  FROM teachers WHERE teacherid = @teacherid;";

            //add parameters to the query
            cmd.Parameters.AddWithValue("@teacherid", teacherid);

            //prepares query
            cmd.Prepare();


            //executes 
            int result = cmd.ExecuteNonQuery();

            //close connection
            conn.Close();
            if (result == 1)
            {
                returningMessage = "SUCCESS";

            }
            else { returningMessage = "FAILED"; }
            


            return returningMessage;
        }

        /// <summary>
        /// Functions to update teacher o School database
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns>Success if teacher was updated or </returns>
        public string UpdateTeacher(Teacher teacher)
        {

            //creating db context
            SchoolDbContext dbCtx = new SchoolDbContext();

            //creating connection to schoolDB 
            MySqlConnection conn = dbCtx.AccessDatabase();

            //opening connection
            conn.Open();

            //creating command
            MySqlCommand cmd = conn.CreateCommand();

            //writing command query which select all information from teacher table using parameter teacher to filter rows
            cmd.CommandText = "UPDATE teachers SET teacherfname=@teacherfname, teacherlname=@teacherlname, salary=@salary  WHERE teacherid = @teacherid";

            //create parameter and prepares command
            cmd.Parameters.AddWithValue("@teacherfname", teacher.FName);
            cmd.Parameters.AddWithValue("@teacherlname", teacher.LName);
            cmd.Parameters.AddWithValue("@salary", teacher.Salary);
            cmd.Parameters.AddWithValue("@teacherid", teacher.Id);
            cmd.Prepare();

            //executes 
            int result = cmd.ExecuteNonQuery();

            //close connection
            conn.Close();

            //return
            if (result == 1)
            {
                return "SUCCESS";

            }
            else return "FAILED";
        }



    }
}



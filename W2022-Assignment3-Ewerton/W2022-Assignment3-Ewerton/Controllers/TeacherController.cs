using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using W2022_Assignment3_Ewerton.Models;
namespace W2022_Assignment3_Ewerton.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Serves the Teacher/List view
        /// </summary>
        /// <example>Get: Teacher/List</example>
        /// <returns>List.cshtml</returns>
        public ActionResult List()
        {
            //instantiate a controller and calls list API method
            List<Teacher> teachers= new TeacherDataController().List();

            return View(teachers);
        }

        /// <summary>
        /// uses provided id to retrieve information at TeacherDataController then sends info to Show.cshtml
        /// </summary>
        /// <param name="id"></param>
        /// <example>Get: Teacher/Show/5</example>
        /// <returns>Show.cshtml</returns>
        //PS:From my research, we should use id instead of author id due to routeConfig.cs.
        //   But for me is a little strange.
        [Route("Teacher/Show/{id}")]
        public ActionResult Show(int id = 0)
        {
            //create controller
            TeacherDataController controller = new TeacherDataController();
            //use controller to access API
            Teacher teacherDetails = controller.Describe(id);
            //sends teacherDetails to the Show View
            return View(teacherDetails);
        }   
        
        /// <summary>
        /// Contains form to create a new teacher.
        /// </summary>
        /// <returns>View.New.cshtml</returns>
        [HttpGet]
        [Route("Teacher/New/")]
        public ActionResult New()
        {
             return View();
        }


        /// <summary>
        /// Receives info needed to create a new teacher and calls TeacherDataController to insert on the database
        /// </summary>
        /// <param name="teacherFName"></param>
        /// <param name="teacherLName"></param>
        /// <param name="employeeId"></param>
        /// <param name="hiredate"></param>
        /// <param name="salary"></param>
        /// <returns> View.New() </returns>
        [HttpPost]
        [Route("Teacher/New/")]
        public ActionResult New(string teacherFName,string teacherLName, string employeeId, DateTime hiredate,double salary)
        {
            Teacher newTeacher = new Teacher();
            newTeacher.FName = teacherFName;
            newTeacher.LName = teacherLName;
            newTeacher.EmployeeNumber = employeeId;
            newTeacher.HireDate=hiredate;
            newTeacher.Salary = salary;
            TeacherDataController controller = new TeacherDataController();
            string message = controller.AddTeacher(newTeacher);
            ViewBag.Message = message;
            if (message == "SUCCESS")
            {
                newTeacher.Id = controller.Find(employeeId);
                ViewBag.teacherId = newTeacher.Id;
            }
            return View();
        }


        /// <summary>
        /// Deletes a teacher from DB using TeacherDataController
        /// </summary>
        /// <param name="teacherid"></param>
        /// <returns>View.List if succeeded View.Show if failed </returns>
        [HttpPost]
        public ActionResult Remove(int id)
        {

            TeacherDataController controller = new TeacherDataController();

            string result = controller.Remove(id);
            if (result == "SUCCESS") { 
                return RedirectToAction("List");
            }
             
            return RedirectToAction("Show", id); //we could create something to warn the user here
                
            
        }



        //[HttpGet]
        //[Route("Teacher/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            Teacher teacher = new TeacherDataController().Describe(id);
            return View(teacher);
                
            
        }



    }
}
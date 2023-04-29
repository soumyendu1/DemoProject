using DemoProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;

namespace DemoProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoDBContext _db;

        public HomeController(DemoDBContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Logins logins)
        {
            DbConnection? conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            DataTable dt = new();
            using (Microsoft.Data.SqlClient.SqlCommand cmd = (Microsoft.Data.SqlClient.SqlCommand)conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Login] WHERE [Username] = '" + logins.Username + "' AND [Password] = '" + logins.Password + "'";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 0;
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            conn.Close();

            if (dt.Rows.Count > 0)
            {
                return RedirectToAction("Employee");
            }
            return View(logins);
        }

        public IActionResult Employee()
        {
            DbConnection? conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            DataTable dt = new();
            using (Microsoft.Data.SqlClient.SqlCommand cmd = (Microsoft.Data.SqlClient.SqlCommand)conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Employees]";
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd);
                da.Fill(dt);
                List<Employees> emps = new List<Employees>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Employees emp = new Employees();
                    emp.EmpId = Convert.ToInt32(dt.Rows[i]["Id"]);
                    emp.EmpName = Convert.ToString(dt.Rows[i]["Emp_Name"]);
                    emp.EmpCode = Convert.ToString(dt.Rows[i]["Emp_Code"]);
                    emp.Mobile = Convert.ToString(dt.Rows[i]["Mobile"]);
                    emps.Add(emp);
                }
                ViewBag.EmpList = emps;
            }
            conn.Close();
            return View();
        }

        [HttpPost]
        public IActionResult Employee(Employees employees)
        {
            DbConnection? conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using (Microsoft.Data.SqlClient.SqlCommand cmd = (Microsoft.Data.SqlClient.SqlCommand)conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [Employees] VALUES ('" + employees.EmpName + "','" + employees.EmpCode + "','" + employees.Mobile + "')";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            ModelState.Clear();
            return View();
        }

        public IActionResult EmpDataEdit(int Id)
        {
            DbConnection? conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            DataTable dt = new();
            List<Employees> emps = new List<Employees>();
            using (Microsoft.Data.SqlClient.SqlCommand cmd = (Microsoft.Data.SqlClient.SqlCommand)conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Employees] WHERE [Id] = " + Id;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd);
                da.Fill(dt);
                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Employees emp = new Employees();
                    emp.EmpId = Convert.ToInt32(dt.Rows[i]["Id"]);
                    emp.EmpName = Convert.ToString(dt.Rows[i]["Emp_Name"]);
                    emp.EmpCode = Convert.ToString(dt.Rows[i]["Emp_Code"]);
                    emp.Mobile = Convert.ToString(dt.Rows[i]["Mobile"]);
                    emps.Add(emp);
                }
            }
            conn.Close();
            var infoFromEmps = emps.Select(e => new
            {
                e.EmpId,
                e.EmpName,
                e.EmpCode,
                e.Mobile
            }).ToList();

            return Content(JsonConvert.SerializeObject(infoFromEmps), "application/json");
        }


        [HttpPost]
        public IActionResult EditEmp(int id, Employees employees)
        {
            DbConnection? conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using (Microsoft.Data.SqlClient.SqlCommand cmd = (Microsoft.Data.SqlClient.SqlCommand)conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE [Employees] SET [Emp_Name] = '" + employees.EmpName + "', [Emp_Code] = '" + employees.EmpCode + "', [Mobile] = '" + employees.Mobile + "' WHERE [Id] = " + id;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            return RedirectToAction("Employee");
        }
    }
}

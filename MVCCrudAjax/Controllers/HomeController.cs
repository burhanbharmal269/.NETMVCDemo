using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCCrudAjax.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace MVCCrudAjax.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public List<Employee> ListAll()
        {
            string connectionString = _config.GetConnectionString("Default");

            List<Employee> emplist = new List<Employee>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("SP_GetAllEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    emplist.Add(new Employee
                    {
                        EmployeeID = Convert.ToInt32(reader["EmployeeId"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Code = reader["Code"].ToString(),
                        EmailID = reader["EmailID"].ToString(),
                        Mobile = reader["Mobile"].ToString(),
                        Salary = reader["Salary"].ToString(),
                    });
                }
                return emplist;
            }
        }

        [ValidateAntiForgeryToken]
        public int AddEmployee(Employee emp)
        {
            string connectionString = _config.GetConnectionString("Default");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Check if email exists in the database
                SqlCommand checkEmailCmd = new SqlCommand("SELECT * FROM Employee WHERE EmailID = @EmailID", con);
                checkEmailCmd.Parameters.AddWithValue("@EmailID", emp.EmailID);
                int emailCount = 0;
                
                try
                {
                    object result = checkEmailCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        emailCount = Convert.ToInt32(result);
                    }
                    //emailCount = (int)checkEmailCmd.ExecuteScalar();
                     
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                

                if (emailCount > 0)
                {
                    // Email already exists, return a negative value (-1) as an error indicator
                    if(emp.EmployeeID ==  emailCount)
                    {
                        SqlCommand cmd = new SqlCommand("SP_InsertOrUpdateEmployee", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", emp.EmployeeID);
                        cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                        cmd.Parameters.AddWithValue("@Code", emp.Code);
                        cmd.Parameters.AddWithValue("@EmailID", emp.EmailID);
                        cmd.Parameters.AddWithValue("@Mobile", emp.Mobile);
                        cmd.Parameters.AddWithValue("@Salary", emp.Salary);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {

                    //Add new
                    SqlCommand cmd = new SqlCommand("SP_InsertOrUpdateEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", emp.EmployeeID);
                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@Code", emp.Code);
                    cmd.Parameters.AddWithValue("@EmailID", emp.EmailID);
                    cmd.Parameters.AddWithValue("@Mobile", emp.Mobile);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected;
                }

                
            }
        }

        public int AddAccount(Account acc)
        {
            string connectionString = _config.GetConnectionString("Default");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand checkEmailCmd = new SqlCommand("SELECT * FROM Accounts WHERE EmailID = @EmailID", con);
                checkEmailCmd.Parameters.AddWithValue("@EmailID", acc.EmailID);
                int emailCount = 0;

                try
                {
                    object result = checkEmailCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        emailCount = Convert.ToInt32(result);
                    }
                    //emailCount = (int)checkEmailCmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (emailCount > 0)
                {
                    // Email already exists, return a negative value (-1) as an error indicator
                        return -1;
                    
                }
                else
                {

                    //Add new
                    SqlCommand com = new SqlCommand("SP_AddAccount", con);
                    com.CommandType = CommandType.StoredProcedure;

                    //com.Parameters.AddWithValue("@Id", acc.Id);
                    com.Parameters.AddWithValue("@FirstName", acc.FirstName);
                    com.Parameters.AddWithValue("@LastName", acc.LastName);
                    com.Parameters.AddWithValue("@EmailID", acc.EmailID);
                    com.Parameters.AddWithValue("@Password", Encrypt(acc.Password));

                    int i = com.ExecuteNonQuery();
                    return i;
                }

                
            }
        }

        public int CheckCredential(Account acc)
        {
            string connectionString = _config.GetConnectionString("Default");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Check if email exists in the database
                SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE EmailID = @EmailID AND Password = @Password", con);
                checkEmailCmd.Parameters.AddWithValue("@EmailID", acc.EmailID);
                checkEmailCmd.Parameters.AddWithValue("@Password", Encrypt(acc.Password));
                int emailCount = 0;

                try
                {
                    object result = checkEmailCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        emailCount = Convert.ToInt32(result);
                    }
                    if (emailCount > 0)
                    {
                      
                        // Store user ID in the session
                        HttpContext.Session.SetString("EmailID", acc.EmailID.ToString());

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return emailCount;

            }
        }

        private string Encrypt(string clearText)
        {
            if (clearText == null)
            {
                return string.Empty; // Return empty string if clearText is null
            }
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        [ValidateAntiForgeryToken]
        public int DeleteEmployee(int ID)
        {
            string connectionString = _config.GetConnectionString("Default");

            int i;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("SP_DeleteEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", ID);
                i = com.ExecuteNonQuery();

            }
            return i;
        }

        #region Commented add
        //[ValidateAntiForgeryToken]
        //public int AddEmployee(Employee emp)
        //{
        //    string connectionString = _config.GetConnectionString("Default");

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();

        //        // Check if email exists in the database
        //        SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE EmailID = @EmailID", con);
        //        checkEmailCmd.Parameters.AddWithValue("@EmailID", emp.EmailID);
        //        int emailCount = (int)checkEmailCmd.ExecuteScalar();

        //        if (emailCount > 0)
        //        {

        //            // Email already exists, return a negative value (-1) as an error indicator
        //            return -1;
        //        }

        //        SqlCommand com = new SqlCommand("SP_InsertEmployeeOrUpdateEmployee", con);
        //        com.CommandType = CommandType.StoredProcedure;

        //        com.Parameters.AddWithValue("@Id", emp.EmployeeID);
        //        com.Parameters.AddWithValue("@FirstName", emp.FirstName);
        //        com.Parameters.AddWithValue("@LastName", emp.LastName);
        //        com.Parameters.AddWithValue("@Code", emp.Code);
        //        com.Parameters.AddWithValue("@EmailID", emp.EmailID);
        //        com.Parameters.AddWithValue("@Mobile", emp.Mobile);
        //        com.Parameters.AddWithValue("@Salary", emp.Salary);


        //        int i = com.ExecuteNonQuery();
        //        return i;
        //    }

        //}

        //[ValidateAntiForgeryToken]
        //public int UpdateEmployee(Employee emp)
        //{
        //    string connectionString = _config.GetConnectionString("Default");

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();


        //        SqlCommand com = new SqlCommand("SP_InsertEmployeeOrUpdateEmployee", con);
        //        com.CommandType = CommandType.StoredProcedure;

        //        com.Parameters.AddWithValue("@Id", emp.EmployeeID);
        //        com.Parameters.AddWithValue("@FirstName", emp.FirstName);
        //        com.Parameters.AddWithValue("@LastName", emp.LastName);
        //        com.Parameters.AddWithValue("@Code", emp.Code);
        //        com.Parameters.AddWithValue("@EmailID", emp.EmailID);
        //        com.Parameters.AddWithValue("@Mobile", emp.Mobile);
        //        com.Parameters.AddWithValue("@Salary", emp.Salary);


        //        int i = com.ExecuteNonQuery();
        //        return i;
        //    }

        //}
        #endregion Commented add


        public ActionResult signup()
        {
            return View();
        }
       
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("EmailID") == null)
            {
                return View("Privacy");
            }

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Privacy","Home");
        }
        public JsonResult AddAcc(Account acc)
        {
            return Json(AddAccount(acc));
        }
        public  ActionResult CheckSignIn(Account acc)
        {
            return Json(CheckCredential(acc));
            
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public JsonResult List()
        {
            return Json(ListAll());
        }
        public ActionResult Welcome()
        {
            if (HttpContext.Session.GetString("EmailID") == null)
            {
                return View("Privacy");
            }

            return View();
        }
        public JsonResult Add(Employee emp)
        {
            return Json(AddEmployee(emp));
        }
        public JsonResult GetbyID(int ID)
        {
            var Employee = ListAll().Find(x => x.EmployeeID.Equals(ID));
            return Json(Employee);
        }
        //public JsonResult Update(Employee emp)
        //{
        //    return Json(UpdateEmployee(emp));
        //}

        [ValidateAntiForgeryToken]
        public JsonResult Delete(int ID)
        {
            return Json(DeleteEmployee(ID));
        }
    }
}
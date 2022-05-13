using CrudOperationUsingADODOTNET.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudOperationUsingADODOTNET.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
         
            List<Customers> CustomerModel = new List<Customers>();
            string constr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            string query = "select Customers.CustomerId,Customers.Name,Customers.Country,Customers.Date,Customers.Class,CountryMaster.CountryName,StateMaster.StateName,CityMaster.CityName from Customers Inner join CountryMaster on Customers.CountryId = CountryMaster.CountryId Inner join StateMaster on Customers.StateId = StateMaster.StateId Inner join CityMaster on Customers.CityId = CityMaster.CityId";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while(sdr.Read())
                        {
                            CustomerModel.Add(new Customers
                            {
                                CustomerId = Convert.ToInt32(sdr["CustomerId"]),
                                Name = Convert.ToString(sdr["Name"]),
                                Country = Convert.ToString(sdr["Country"]),
                                Class = Convert.ToString(sdr["Class"]),
                                Date = sdr["Date"]!= null ?  Convert.ToDateTime(sdr["Date"]) : new DateTime(),
                                CountryName = Convert.ToString(sdr["CountryName"]),
                                StateName = Convert.ToString(sdr["StateName"]),
                                CityName = Convert.ToString(sdr["CityName"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            if(CustomerModel.Count == 0)
            {
                CustomerModel.Add(new Customers());
            }
            PopulateItems("SELECT CountryName,CountryId,0 FROM CountryMaster");
                return View(CustomerModel);
        }
        [HttpPost]
        public ActionResult Country()
        {
            var item = PopulateItems("SELECT CountryName,CountryId,0 FROM CountryMaster");
            return Json(item);
        }
        [HttpPost]
        public ActionResult State(string CountryId)
        {
            if(CountryId != null)
            {
                var query = "SELECT StateName,StateId,CountryId,0 FROM StateMaster where CountryId=" + CountryId;
                var item = PopulateItems(query);
                return Json(new { Data= item , Status = true} );
            }
            return Json(new { Data = "", Status = false });

        }
        [HttpPost]
        public ActionResult City(string StateId)
        {
            if(StateId != null)
            {
                var query = "select CityName,CityId,StateId,0 from CityMaster where StateId=" + StateId;
                var item = PopulateItems(query);
                return Json(new { Data = item, Status = true });
            }
            return Json(new { Data = "", Status = false });
        }
        private static List<ListItem> PopulateItems(string query)
        {
            string constr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            List<ListItem> items = new List<ListItem>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ListItem
                            {
                                Text = sdr[0].ToString(),
                                Value = Convert.ToInt32(sdr[1]),
                                ParentId = Convert.ToInt32(sdr[2])
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        [HttpPost]
        public ActionResult InsertCustomer(Customers Customer)
        {
            List<ListItem> items = new List<ListItem>();
            if (ModelState.IsValid)
            {
                try
                {
                    string constr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand("Insert into Customers values(@Name,@Country,@Class,@Date,@CountryId,@StateId)", con))
                        {
                            {
                                cmd.Parameters.AddWithValue("@Name", Customer.Name);
                                cmd.Parameters.AddWithValue("@Country", Customer.Country);
                                cmd.Parameters.AddWithValue("@Class", Customer.Class);
                                cmd.Parameters.AddWithValue("@Date", Customer.Date);
                                cmd.Parameters.AddWithValue("@CountryId", Customer.CountryId);
                                cmd.Parameters.AddWithValue("@StateId", Customer.StateId);
                                con.Open();
                                Customer.CustomerId = Convert.ToInt32(cmd.ExecuteScalar());
                                con.Close();
                            }
                        }

                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return View(Customer);
            }
                return Json(items);
        }
        [HttpPost]
        public JsonResult UpdateCustomer(string CustomerId,string Name,string Country,string Class,DateTime Date,int CountryId)
        {
            string constr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using(SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Customers SET Name=@Name, Country=@Country,Class=@Class,Date=@Date,CountryId=@CountryId WHERE CustomerId=@CustomerId",con))
                {

                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@Country", Country);
                    cmd.Parameters.AddWithValue("@Class", Class);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@CountryId", CountryId);
                    cmd.Parameters.AddWithValue("@CustomerId", Convert.ToInt32(CustomerId));
                    con.Open();
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    
                }
            }
            return Json("Record Updated");
        }
        [HttpPost]
        public ActionResult DeleteCustomer(int CustomerId)
        {
            string constr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Customers WHERE CustomerId=@CustomerId",con))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return Json("Record Deleted");
        }

        public class ListItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public int ParentId { get; set; }
        }
    }
}
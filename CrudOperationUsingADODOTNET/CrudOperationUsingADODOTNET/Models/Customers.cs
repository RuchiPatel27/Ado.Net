using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrudOperationUsingADODOTNET.Models
{
    public class Customers
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage ="please enter your name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="please enter your country")]
        public string Country { get; set; }
        [Required(ErrorMessage ="please select the class")]
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
}
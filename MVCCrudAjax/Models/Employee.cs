﻿using Microsoft.AspNetCore.Mvc;

namespace MVCCrudAjax.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public string? EmailID { get; set; }
        public string? Mobile { get; set; }
        public string? Salary { get; set; }

    }
}

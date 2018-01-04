using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CodePlex.Styles
{
	public class Employee
	{
		/// <summary>
		/// Gets or sets the employee ID.
		/// </summary>
		/// <value>The employee ID.</value>
		public int EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>The first name.</value>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>The last name.</value>
		public string LastName { get; set; }
	}

	public class Employees
	{
		/// <summary>
		/// Gets the employees.
		/// </summary>
		/// <returns>ObservableCollection of employees</returns>
		public static ObservableCollection<Employee> GetEmployees()
		{
			ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

            employees.Add(new Employee { EmployeeID = 1, FirstName = "Bill", LastName = "Gates" });
            employees.Add(new Employee { EmployeeID = 2, FirstName = "Steve", LastName = "Ballmer" });
            employees.Add(new Employee { EmployeeID = 3, FirstName = "S.", LastName = "Somasegar" });
            employees.Add(new Employee { EmployeeID = 4, FirstName = "Scott", LastName = "Guthrie" });

			return employees;
		}
	}
}

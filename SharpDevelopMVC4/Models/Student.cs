/*
 * Created by SharpDevelop.
 * User: Gabs
 * Date: 31/07/2019
 * Time: 3:08 pm
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SharpDevelopWebApi.Models
{
	/// <summary>
	/// Description of Student.
	/// </summary>
	public class Student : Person
	{
		public string Course { get; set; }
		public string SchoolLastAttended { get; set; }
		
	}
}

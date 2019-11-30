/*
 * Created by SharpDevelop.
 * User: Gabs
 * Date: 29/07/2019
 * Time: 3:29 pm
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SharpDevelopWebApi.Models
{
	/// <summary>
	/// Description of Contact.
	/// </summary>
	public class Contact
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
}

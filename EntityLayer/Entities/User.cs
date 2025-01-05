using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
	public class User 
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; } = "";

		[Required(ErrorMessage = "Surname is required.")]
		public string SurName { get; set; } = "";

		[Required(ErrorMessage = "Email is required.")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "Username is required.")]
		public string UserName { get; set; } = "admin@example.com";

		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; } = "";

		[Required(ErrorMessage = "Re-entering password is required.")]
		public string RePassword { get; set; } = "";

		public string Role { get; set; } = "User";
	}
}

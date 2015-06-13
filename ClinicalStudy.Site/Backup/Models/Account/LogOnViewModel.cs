using System;

namespace ClinicalStudy.Site.Models.Account {
	public class LogOnViewModel {
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Role { get; set; }
		public byte[] Photo { get; set; }
	}
}

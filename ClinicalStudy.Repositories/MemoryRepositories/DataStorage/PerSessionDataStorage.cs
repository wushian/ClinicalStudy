using System;
using System.Collections.Generic;
using System.Web;

//using ClinicalStudy.Repositories.DemoData;

namespace ClinicalStudy.Repositories.MemoryRepositories.DataStorage {
	public class PerSessionDataStorage : IDataStorage {
		public const string SessionKey = "{0}SessionData";

		public IList<T> GetData<T>() {
			var session = HttpContext.Current.Session;
			if (session == null)
				throw new InvalidOperationException("There is no open session");
			string key = string.Format(SessionKey, typeof (T).Name);
			List<T> data = session[key] as List<T>;
			if (data == null) {
				throw new InvalidOperationException("Demo data was not initialized");
			}
			return data;
		}

		public void SaveData<T>(List<T> data) {
			var session = HttpContext.Current.Session;
			if (session == null)
				throw new InvalidOperationException("There is no open session");
			string key = string.Format(SessionKey, typeof (T).Name);
			session[key] = data;
		}
	}
}

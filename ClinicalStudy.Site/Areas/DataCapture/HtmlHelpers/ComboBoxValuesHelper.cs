using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.Enums.Display;
using DevExpress.Web;

namespace ClinicalStudy.Site.Areas.DataCapture.HtmlHelpers {
	public static class ComboBoxValuesHelper {
		public static ListEditItemCollection SexValues {
			get { return GetCollection<Gender>(); }
		}


		public static ListEditItemCollection RaceValues {
			get { return GetCollection<Race>(); }
		}

		public static ListEditItemCollection OutcomeValues {
			get { return GetCollection<AdverseEventOutcome>("<-- Please select -->"); }
		}

		public static ListEditItemCollection IntensityValues {
			get { return GetCollection<AdverseEventIntensity>("<-- Please select -->"); }
		}


		public static ListEditItemCollection RelationshipToInvestigationalDrugValues {
			get { return GetCollection<AdverseEventRelanshionship>("<-- Please select -->"); }
		}

		public static TrackBarItemCollection HappinessValues {
			get {
				var collection = new TrackBarItemCollection();
				var values = GetKeysAndValues<HappinessLevel>();
				collection.AddRange(values.Select(pair => new TrackBarItem(pair.Value.ToString(), pair.Key, pair.Value.ToString())).ToList());
				return collection;
			}
		}

		public static ListEditItemCollection GetCollection<T>() where T : struct {
			return GetCollection<T>(null);
		}


		public static ListEditItemCollection GetCollection<T>(string caption)
			where T : struct {
			var collection = new ListEditItemCollection();
			var values = GetKeysAndValues<T>();
			if (!string.IsNullOrEmpty(caption))
				collection.Add(caption, 0);
			collection.AddRange(values.Select(pair => new ListEditItem(pair.Value.ToString(), pair.Key)).ToList());
			return collection;
		}

		public static List<KeyValuePair<int, string>> GetKeysAndValues<T>()
			where T : struct {
			var values = Enum.GetValues(typeof (T));
			var list = new List<KeyValuePair<int, string>>();
			foreach (var value in values) {
				list.Add(new KeyValuePair<int, string>((int) value, EnumHelper.GetDescription((Enum) value)));
			}
			return list;
		}
	}
}

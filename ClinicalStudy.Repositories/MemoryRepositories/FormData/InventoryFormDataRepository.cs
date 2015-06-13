using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class InventoryFormDataRepository : GenericFormDataRepository<InventoryFormData>, IInventoryFormDataRepository {
		public InventoryFormDataRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(InventoryFormData stored, InventoryFormData updated) {
			UpdateQuestionData(stored.QuantityShipped, updated.QuantityShipped);
			UpdateQuestionData(stored.BatchNumber, updated.BatchNumber);
			UpdateQuestionData(stored.ReceiptDate, updated.ReceiptDate);
			UpdateQuestionData(stored.ShipDate, updated.ShipDate);

			UpdateMedicationUsageData(stored.MedicationUsage, updated.MedicationUsage);

			base.MapEntity(stored, updated);
		}

		private void UpdateMedicationUsageData(List<RepeatableInventoryData> storedMedication,
		                                       List<RepeatableInventoryData> updatedMedication) {
			if (storedMedication == null)
				storedMedication = new List<RepeatableInventoryData>();

			if (updatedMedication == null)
				updatedMedication = new List<RepeatableInventoryData>();

			//this if code for in-memory repositories only
			//EF is able to take care about it itself
			var list = (from form in GetAll()
			            from gr in form.MedicationUsage
			            select gr);
			int muMaxId = list.Count() > 0 ? list.Select(x => x.Id).Max() : 0;

			//Assign new Ids
			foreach (var newItem in updatedMedication.Where(x => x.Id == 0)) {
				newItem.Id = ++muMaxId;
			}

			//Remove deleted entities
			storedMedication.RemoveAll(x => !updatedMedication.Select(y => y.Id).Contains(x.Id));

			//Add newly created entities
			storedMedication.AddRange(updatedMedication.Where(x => !storedMedication.Select(y => y.Id).Contains(x.Id)));

			foreach (var storedData in storedMedication) {
				var updatedData = updatedMedication.Where(x => x.Id == storedData.Id).FirstOrDefault();
				if (updatedData != null) {
					UpdateQuestionData(storedData.DateUsed, updatedData.DateUsed);
					UpdateQuestionData(storedData.QuantityUsed, updatedData.QuantityUsed);
				}
			}
		}
	}
}

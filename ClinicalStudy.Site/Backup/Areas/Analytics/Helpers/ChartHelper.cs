using System.Drawing;
using DevExpress.XtraCharts;

namespace ClinicalStudy.Site.Areas.Analytics.Helpers {
	public static class ChartHelper {
		public const string StudyProgressPaletteName = "ClinicalStudyProgressPalette";
		private static Palette studyProgressPalette;

		static ChartHelper() {
			CreateStudyProgressPalette();
		}

		public static Palette StudyProgressPalette {
			get { return studyProgressPalette; }
		}

		private static void CreateStudyProgressPalette() {
			var entry1 = new PaletteEntry(Color.FromArgb(124, 186, 180), Color.FromArgb(127, 192, 185));
			var entry2 = new PaletteEntry(Color.FromArgb(146, 199, 226), Color.FromArgb(117, 181, 214));
			var entry3 = new PaletteEntry(Color.FromArgb(183, 140, 155), Color.FromArgb(200, 164, 176));
			var entry4 = new PaletteEntry(Color.FromArgb(242, 202, 132), Color.FromArgb(246, 211, 149));
			var entry5 = new PaletteEntry(Color.FromArgb(167, 202, 116), Color.FromArgb(172, 206, 118));

			studyProgressPalette = new Palette(StudyProgressPaletteName,
				new[] {entry1, entry2, entry3, entry4, entry5});
		}
	}
}

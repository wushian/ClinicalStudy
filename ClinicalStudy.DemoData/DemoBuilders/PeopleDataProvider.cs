using System;

namespace ClinicalStudy.DemoData.DemoBuilders {
	internal static class PeopleDataProvider {
		private static Random RandomGenerator;
		public const int PatientPerDoctorBase = 4;

		static PeopleDataProvider() {
			unchecked {
				RandomGenerator = new Random((int) DateTime.Now.Ticks);
			}
		}

		public static bool IsMale() {
			return RandomGenerator.Next(100)%2 == 0;
		}

		public static string GetRandomLastName(bool isMale) {
			var names = Surnames;
			return names[RandomGenerator.Next(names.Length)];
		}

		public static string GetRandomFirstName(bool isMale) {
			string[] names;
			if (isMale) {
				names = MaleFirstNames;
			}
			else {
				names = FemaleFirstNames;
			}

			return names[RandomGenerator.Next(names.Length)];
		}

		public static DemoPatientState GetNewDemoPatientState() {
			var dice = RandomGenerator.Next(100);
			if (dice < 75)
				return DemoPatientState.AllVisits;
			if (dice < 90)
				return DemoPatientState.Day1Only;
			if (dice < 98)
				return DemoPatientState.BaselineOnly;
			return DemoPatientState.NotEnrolled;
		}

		#region Names and Surnames

		public static string[] MaleFirstNames = new string[] {
			"Jacob",
			"Michael",
			"Joshua",
			"Matthew",
			"Daniel",
			"Christopher",
			"Andrew",
			"Ethan",
			"Joseph",
			"William",
			"Anthony",
			"David",
			"Alexander",
			"Nicholas",
			"Ryan",
			"Tyler",
			"James",
			"John",
			"Jonathan",
			"Noah",
			"Brandon",
			"Christian",
			"Dylan",
			"Samuel",
			"Benjamin",
			"Zachary",
			"Nathan",
			"Logan",
			"Justin",
			"Gabriel",
			"Jose",
			"Austin",
			"Kevin",
			"Elijah",
			"Caleb",
			"Robert",
			"Thomas",
			"Jordan",
			"Cameron",
			"Jack",
			"Hunter",
			"Jackson",
			"Angel",
			"Isaiah",
			"Evan",
			"Isaac",
			"Mason",
			"Luke",
			"Jason",
			"Gavin",
			"Jayden",
			"Aaron",
			"Connor",
			"Aiden",
			"Aidan",
			"Kyle",
			"Juan",
			"Charles",
			"Luis",
			"Adam",
			"Lucas",
			"Brian",
			"Eric",
			"Adrian",
			"Nathaniel",
			"Sean",
			"Alex",
			"Carlos",
			"Bryan",
			"Ian",
			"Owen",
			"Jesus",
			"Landon",
			"Julian",
			"Chase",
			"Cole",
			"Diego",
			"Jeremiah",
			"Steven",
			"Sebastian",
			"Xavier",
			"Timothy",
			"Carter",
			"Wyatt",
			"Brayden",
			"Blake",
			"Hayden",
			"Devin",
			"Cody",
			"Richard",
			"Seth",
			"Dominic",
			"Jaden",
			"Antonio",
			"Miguel",
			"Liam",
			"Patrick",
			"Carson",
			"Jesse",
			"Tristan"
		};

		public static string[] FemaleFirstNames = new string[] {
			"Emily",
			"Madison",
			"Emma",
			"Olivia",
			"Hannah",
			"Abigail",
			"Isabella",
			"Samantha",
			"Elizabeth",
			"Ashley",
			"Alexis",
			"Sarah",
			"Sophia",
			"Alyssa",
			"Grace",
			"Ava",
			"Taylor",
			"Brianna",
			"Lauren",
			"Chloe",
			"Natalie",
			"Kayla",
			"Jessica",
			"Anna",
			"Victoria",
			"Mia",
			"Hailey",
			"Sydney",
			"Jasmine",
			"Julia",
			"Morgan",
			"Destiny",
			"Rachel",
			"Ella",
			"Kaitlyn",
			"Megan",
			"Katherine",
			"Savannah",
			"Jennifer",
			"Alexandra",
			"Allison",
			"Haley",
			"Maria",
			"Kaylee",
			"Lily",
			"Makayla",
			"Brooke",
			"Mackenzie",
			"Nicole",
			"Addison",
			"Stephanie",
			"Lillian",
			"Andrea",
			"Zoe",
			"Faith",
			"Kimberly",
			"Madeline",
			"Alexa",
			"Katelyn",
			"Gabriella",
			"Gabrielle",
			"Trinity",
			"Amanda",
			"Kylie",
			"Mary",
			"Paige",
			"Riley",
			"Jenna",
			"Leah",
			"Sara",
			"Rebecca",
			"Michelle",
			"Sofia",
			"Vanessa",
			"Jordan",
			"Angelina",
			"Caroline",
			"Avery",
			"Audrey",
			"Evelyn",
			"Maya",
			"Claire",
			"Autumn",
			"Jocelyn",
			"Ariana",
			"Nevaeh",
			"Arianna",
			"Jada",
			"Bailey",
			"Brooklyn",
			"Aaliyah",
			"Amber",
			"Isabel",
			"Danielle",
			"Mariah",
			"Melanie",
			"Sierra",
			"Erin",
			"Molly",
			"Amelia"
		};

		public static string[] Surnames = new string[] {
			"Smith",
			"Johnson",
			"Williams",
			"Jones",
			"Brown",
			"Davis",
			"Miller",
			"Wilson",
			"Moore",
			"Taylor",
			"Anderson",
			"Thomas",
			"Jackson",
			"White",
			"Harris",
			"Martin",
			"Thompson",
			"Garcia",
			"Martinez",
			"Robinson",
			"Clark",
			"Rodriguez",
			"Lewis",
			"Lee",
			"Walker",
			"Hall",
			"Allen",
			"Young",
			"Hernandez",
			"King",
			"Wright",
			"Lopez",
			"Hill",
			"Scott",
			"Green",
			"Adams",
			"Baker",
			"Gonzalez",
			"Nelson",
			"Carter",
			"Mitchell",
			"Perez",
			"Roberts",
			"Turner",
			"Phillips",
			"Campbell",
			"Parker",
			"Evans",
			"Edwards",
			"Collins",
			"Stewart",
			"Sanchez",
			"Morris",
			"Rogers",
			"Reed",
			"Cook",
			"Morgan",
			"Bell",
			"Murphy",
			"Bailey",
			"Rivera",
			"Cooper",
			"Richardson",
			"Cox",
			"Howard",
			"Ward",
			"Torres",
			"Peterson",
			"Gray",
			"Ramirez",
			"James",
			"Watson",
			"Brooks",
			"Kelly",
			"Sanders",
			"Price",
			"Bennett",
			"Wood",
			"Barnes",
			"Ross",
			"Henderson",
			"Coleman",
			"Jenkins",
			"Perry",
			"Powell",
			"Long",
			"Patterson",
			"Hughes",
			"Flores",
			"Washington",
			"Butler",
			"Simmons",
			"Foster",
			"Gonzales",
			"Bryant",
			"Alexander",
			"Russell",
			"Griffin",
			"Diaz",
			"Hayes"
		};

		#endregion

		public static int GetPatientForDoctorNumber() {
			var dice = RandomGenerator.Next(100);
			if (dice < 75)
				return PatientPerDoctorBase;
			if (dice < 85)
				return PatientPerDoctorBase - 1;
			if (dice < 95)
				return PatientPerDoctorBase + 1;
			return PatientPerDoctorBase + 2;
		}
	}


	internal enum DemoPatientState {
		NotEnrolled,
		BaselineOnly,
		Day1Only,
		AllVisits
	}
}

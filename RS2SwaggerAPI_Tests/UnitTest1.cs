using NUnit.Framework;

namespace RS2SwaggerAPI_Tests
{
	public class Tests
	{
		private string RS2_URL = "https://axctl.creteschools.org:55459/v1.0/";
		private string RS2_UserName = "apiuser";
		private string RS2_Password = "8VP.XC}yih*=s=v";
		private string RS2_PublicKey = "691F96B26D2B1C074A047ACB60A14857";
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}


		[Test]
		public void TestSelectAllBadges()
		{
			RS2SwaggerAPI.RS2_ws ws = new RS2SwaggerAPI.RS2_ws(RS2_URL, RS2_UserName, RS2_Password, RS2_PublicKey);
			dynamic[] AllBadges = ws.SelectAll("Cardholders");

		}

		[Test]
		public void TestSelectSingleBadge()
		{
			RS2SwaggerAPI.RS2_ws ws = new RS2SwaggerAPI.RS2_ws(RS2_URL, RS2_UserName, RS2_Password, RS2_PublicKey);
			dynamic AllBadges = ws.Select("Badges", new RS2SwaggerAPI.Criteria("LastName", "Louch"));
		}

		[Test]
		public void TestSelectSingleCardholderUUID()
		{
			RS2SwaggerAPI.RS2_ws ws = new RS2SwaggerAPI.RS2_ws(RS2_URL, RS2_UserName, RS2_Password, RS2_PublicKey);
			dynamic AllBadges = ws.Select("Cardholders", "c92feb3b-d604-4ada-be8e-ea609efd35c4");
		}

		[Test]
		public void CreateDefaultObject()
		{
			RS2SwaggerAPI.RS2_ws ws = new RS2SwaggerAPI.RS2_ws(RS2_URL, RS2_UserName, RS2_Password, RS2_PublicKey);
			dynamic AllBadges = ws.CreateDefault("Cardholders");
		}

		public void CreateObject()
		{
			RS2SwaggerAPI.RS2_ws ws = new RS2SwaggerAPI.RS2_ws(RS2_URL, RS2_UserName, RS2_Password, RS2_PublicKey);
			dynamic blankUser = ws.CreateDefault("Cardholders");

			blankUser.LastName = "Shredding";
			blankUser.FirstName = "PaperTiger";

		}
	}
}
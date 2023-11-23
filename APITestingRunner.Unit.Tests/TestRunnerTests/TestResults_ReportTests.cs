using FluentAssertions;

namespace APITestingRunner.Unit.Tests.TestRunnerTests {

  [TestClass]
  public class TestResulsReportTests {

    [TestMethod]
    public void TestPassingNullResponseResults_inErrors_logged() {
      var logger = new Logger();
      var runner = new TestRunner(logger);
      runner.PopulateResultsData(1, null, "url");
      var results = runner.GetResults();

      Assert.IsNotNull(results);
      _ = results.Should().HaveCount(0);
      _ = runner.Errors.Should().HaveCount(1);
      _ = runner.Errors.First().Should().Be("Failed to populate result for url");
    }

    [TestMethod]
    public void Test() {
      var logger = new Logger();
      var runner = new TestRunner(logger);
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      var results = runner.GetResults();

      Assert.IsNotNull(results);
      _ = results.Should().HaveCount(1);
      _ = results[0].Should().NotBeNull();
      _ = results[0].NumberOfResults.Should().Be(1);
      _ = results[0].StatusCode.Should().Be(200);
    }

    [TestMethod]
    public void TestResuls_ReportTests_ShouldAddCorrectlyMultipleResults() {
      var logger = new Logger();
      var runner = new TestRunner(logger);
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      var results = runner.GetResults();

      Assert.IsNotNull(results);
      _ = results.Should().HaveCount(1);
      _ = results[0].Should().NotBeNull();
      _ = results[0].NumberOfResults.Should().Be(3);
      _ = results[0].StatusCode.Should().Be(200);
    }

    [TestMethod]
    public void TestResuls_ReportTests_ShouldAddCorrectlyMultipleRecordForDifferentResults() {
      var logger = new Logger();
      var runner = new TestRunner(logger);
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.OK), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError), "url");
      runner.PopulateResultsData(1, new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError), "url");
      var results = runner.GetResults();

      Assert.IsNotNull(results);
      _ = results.Should().HaveCount(4);
      _ = results[0].Should().NotBeNull();
      _ = results[0].NumberOfResults.Should().Be(2);
      _ = results[0].StatusCode.Should().Be(200);

      //bad request
      _ = results[1].Should().NotBeNull();
      _ = results[1].NumberOfResults.Should().Be(3);
      _ = results[1].StatusCode.Should().Be(400);

      //not found
      _ = results[2].Should().NotBeNull();
      _ = results[2].NumberOfResults.Should().Be(1);
      _ = results[2].StatusCode.Should().Be(404);

      // internal server error
      _ = results[3].Should().NotBeNull();
      _ = results[3].NumberOfResults.Should().Be(2);
      _ = results[3].StatusCode.Should().Be(500);
    }
  }
}
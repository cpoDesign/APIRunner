namespace APITestingRunner.Unit.Tests
{
    [TestClass]
    public class IndividualActionsTests
    {
        [TestMethod]
        public void IndividualActions_SetLogger_PassNull_ShouldReturnNull()
        {
            _ = Assert.ThrowsException<ArgumentNullException>(() => new ApiTesterRunner(null));
        }
    }
}
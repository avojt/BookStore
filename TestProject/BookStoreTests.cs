using BookStore;

namespace BookTest
{
    [TestClass]
    public class BookStoreTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            string[,] parentName = ",";
            Array arr = 4.55;
            BooksStore t_book = new();

            // Act
            t_book.GetBooks(parentName, arr);

            // Assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        }
    }
}
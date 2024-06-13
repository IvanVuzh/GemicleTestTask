using Xunit;
using Shouldly;
using GemicleTestTaskDataReader.CsvReader;
using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskUnitTests
{
    public class CvsReaderTest
    {
        [Fact]
        public void ReadCsvFile_ShouldReturnCorrectCustomers_WhenFileIsValid()
        {
            // given
            var filePath = "customers.csv";
            var expectedCustomers = new List<Customer>
            {
                new Customer { Id = 1, Age = 25, Gender = "Male", City = "New York", Deposit = 100, NewCustomer = true },
                new Customer { Id = 2, Age = 30, Gender = "Female", City = "Los Angeles", Deposit = 150, NewCustomer = false }
            };

            File.WriteAllText(filePath, "CUSTOMER_ID,Age,Gender,City,Deposit,NewCustomer\n1,25,Male,New York,100,1\n2,30,Female,Los Angeles,150,0");

            // when
            var reader = new CsvReader(filePath);
            var customers = reader.GetUsers();

            // then
            customers.ShouldBeEquivalentTo(expectedCustomers);

            File.Delete(filePath);
        }

        [Fact]
        public void ReadCsvFile_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // given
            var filePath = "notFound.csv";

            // when
            var reader = new CsvReader(filePath);

            // then
            Should.Throw<FileNotFoundException>(() => reader.GetUsers());
        }

        [Fact]
        public void ReadCsvFile_ShouldThrowAggregateException_WhenFileHasInvalidData()
        {
            // given
            var filePath = "invalidCustomers.csv";
            File.WriteAllText(filePath, "CUSTOMER_ID,Age,Gender,City,Deposit,NewCustomer\n1,25,Male,New York,invalid deposit,1\n2,invalid age,Female,Los Angeles,150,0");

            // when 
            var reader = new CsvReader(filePath);

            // then
            var exception = Should.Throw<AggregateException>(() => reader.GetUsers());
            exception.InnerExceptions.Count.ShouldBe(2);

            File.Delete(filePath);
        }

        [Fact]
        public void ReadCsvFile_ShouldSkipEmptyLines()
        {
            // given
            var filePath = "customersWithEmptyLines.csv";
            var expectedCustomers = new List<Customer>
            {
                new Customer { Id = 1, Age = 25, Gender = "Male", City = "New York", Deposit = 100, NewCustomer = true }
            };

            File.WriteAllText(filePath, "CUSTOMER_ID,Age,Gender,City,Deposit,NewCustomer\n1,25,Male,New York,100,1\n\n");

            // when
            var reader = new CsvReader(filePath);
            var customers = reader.GetUsers();

            // then
            customers.ShouldBeEquivalentTo(expectedCustomers);

            File.Delete(filePath);
        }
    }
}

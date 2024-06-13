using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskDataReader.CsvReader
{
    public class CsvReader
    {
        private readonly string _filePath;

        public CsvReader(string filepath)
        {
            _filePath = filepath;
        }

        public List<Customer> GetUsers()
        {
            var customers = new List<Customer>();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("The csv file can not be found.", _filePath);
            }

            var lines = File.ReadAllLines(_filePath).Skip(1).ToList();

            var errors = new List<string>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                var values = lines[i].Split(',');

                if (values.Length != 6)
                {
                    errors.Add($"Invalid number of columns in line: {i + 2}");
                    continue;
                }

                try
                {
                    var customer = new Customer
                    {
                        Id = int.Parse(values[0]),
                        Age = int.Parse(values[1]),
                        Gender = values[2],
                        City = values[3],
                        Deposit = int.Parse(values[4]),
                        NewCustomer = values[5] == "1",
                    };

                    customers.Add(customer);
                }
                catch (FormatException fe)
                {
                    errors.Add($"Error parsing line: {i + 2}. Error: {fe.Message}");
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException("There were errors parsing the csv file:", errors.Select(e => new Exception(e)));
            }

            return customers;
        }
    }
}

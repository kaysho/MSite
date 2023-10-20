using System.Diagnostics;

namespace MSite.Core.Test
{
    public class MSiteUnitTest
    {
        public static List<Customer> GenerateSampleCustomers()
        {
            List<Customer> customers = new List<Customer>
        {
            new Customer { Name = "Customer 1", X = 10, Y = 20 },
            new Customer { Name = "Customer 2", X = 90, Y = 24 },
            new Customer { Name = "Customer 3", X = 34, Y = 63 },
            new Customer { Name = "Customer 4", X = 67, Y = 1 },
            new Customer { Name = "Customer 5", X = 24, Y = 84 },
            new Customer { Name = "Customer 6", X = 51, Y = 44 },
            new Customer { Name = "Customer 7", X = 97, Y = 92 },
            new Customer { Name = "Customer 8", X = 77, Y = 13 },
            new Customer { Name = "Customer 9", X = 35, Y = 39 },
            new Customer { Name = "Customer 10", X = 85, Y = 29 }
        };

            return customers;
        }

        public static IEnumerable<object[]> GetDepartmentFromDataGenerator()
        {
            yield return new object[] { new Department
        {
            Name = "Purchasing",
            Location = "Top floor",
            Members = new List<Person>
            {
                new Person { Surname = "Smith", Forename = "John", Title = Title.MR },
                new Person { Surname = "Jones", Forename = "Steve", Title = Title.MR },
                new Person { Surname = "Bradshaw", Forename = "Lisa", Title = Title.MRS }
            }
        }, "Purchasing" };

            yield return new object[] { new Department
        {
            Name = "Sales",
            Location = "Bottom floor",
            Members = new List<Person>
            {
                new Person { Surname = "Bradshaw", Forename = "Lisa", Title = Title.MRS },
                new Person { Surname = "Thompson", Forename = "Joanne", Title = Title.MISS },
                new Person { Surname = "Johnson", Forename = "David", Title = Title.MR }
            }
        }, "Sales" };
        }

        [Fact]
        public void GetSurnamesInDepartmentSuccessfully()
        {
            //Arrange
            var departmentName = "Sales";
            //Act
            var surnamesInDepartment = GetSurnamesInDepartment(departmentName);
            //Assert
            Assert.True(surnamesInDepartment.Count != 0);
        }

        [Fact]
        public void GetSurnamesInDepartmentThatDoesNotExistWithError()
        {
            //Arrange
            var departmentName = "Saless";
            //Act
            var ex = Assert.Throws<ArgumentException>(() => GetSurnamesInDepartment(departmentName));
            //Assert
            Assert.Equal("Department does not exist", ex.Message);
        }

        [Fact]
        public void GetSurnamesInDepartmentSuccessfullyWithOneNameConfirmed()
        {
            //Arrange
            var departmentName = "Sales";
            //Act
            var surnamesInDepartment = GetSurnamesInDepartment(departmentName);
            //Assert
            Assert.True(surnamesInDepartment.Contains("Bradshaw"));
        }

        [Theory]
        [MemberData(nameof(GetDepartmentFromDataGenerator))]
        public void TestGetSurnamesInDepartment(Department department, string departmentName)
        {
            // Act
            IList<string>? surnames = null;

            surnames = GetSurnamesInDepartment(departmentName);

            // Assert
            if (department != null)
            {
                var expectedSurnames = department.Members.Select(person => person.Surname).ToList();
                Assert.Equal(expectedSurnames, surnames);
            }
            else
            {
                Assert.Null(surnames);
            }
        }

        [Fact]
        public void TestGetUserInMultipleDepartmentSuccessfully()
        {
            //Arrange
            var departmentData = GetDepartmentListFromData();

            //Act
            var departments = GetDepartmentsForPerson("Bradshaw", "Lisa");

            // Assert
            Assert.Collection(departments,
                departmentName => Assert.Equal("Purchasing", departmentName),
                departmentName => Assert.Equal("Sales", departmentName));
        }

        [Fact]
        public void TestGetUserInOnlyOneDepartmentSuccessfully()
        {
            //Arrange
            var departmentData = GetDepartmentListFromData();

            //Act
            var departments = GetDepartmentsForPerson("Thompson", "Joanne");

            // Assert
            Assert.True(departments.Count == 1);
        }

        [Fact]
        public void TestGetUserInOnlyTwoDepartmentsWithoutSurname()
        {
            //Arrange
            var departmentData = GetDepartmentListFromData();

            // Act 
            var ex = Assert.Throws<ArgumentException>(() => GetDepartmentsForPerson("", "Joanne"));

            //Assert
            Assert.Equal("Surname cannot be empty", ex.Message);
        }

        [Fact]
        public void TestGetUserInOnlyTwoDepartmentsWithoutForename()
        {
            //Arrange
            var departmentData = GetDepartmentListFromData();

            // Act 
            var ex = Assert.Throws<ArgumentException>(() => GetDepartmentsForPerson("Thompson", ""));

            //Assert
            Assert.Equal("Forename cannot be empty", ex.Message);
        }


        [Fact]
        public void FindShortestRoute_StartAt0_0()
        {
            var customers = GenerateSampleCustomers();
            int startX = 0;
            int startY = 0;

            var shortestRoute = FindShortestRoute(startX, startY);

            int totalDistance = CalculateTotalDistance(shortestRoute, startX, startY);
            Debug.WriteLine($"Starting At {startX} and {startY}");
            Debug.WriteLine($"Total Distance: {totalDistance}");
            Debug.WriteLine("Customer Route:");

            foreach (var customer in customers)
            {
                Debug.WriteLine($"{customer.Name} (X: {customer.X}, Y: {customer.Y})");
            }

            Assert.True(shortestRoute.Count != 0);
        }
        public static List<Department> GetDepartmentListFromData()
        {
            var departmentData = GetDepartmentFromDataGenerator(); // Assuming GetDepartmentFromDataGenerator is accessible

            List<Department> departments = new List<Department>();

            foreach (var data in departmentData)
            {
                if (data.Length == 2 && data[0] is Department && data[1] is string)
                {
                    var department = data[0] as Department;
                    department.Name = data[1].ToString();
                    departments.Add(department);
                }
                else
                {
                    throw new ArgumentException("Invalid data format.");
                }
            }

            return departments;
        }

        public IList<string> GetSurnamesInDepartment(string departmentName)
        {
            var department = GetDepartments().FirstOrDefault(n => n.Name == departmentName);
            if (department == null)
            {
                throw new ArgumentException("Department does not exist");
            }
            if (department.Members == null)
            {
                throw new ArgumentException("Department does not have members");
            }
            // Retrieve the surnames of all members in the department
            var surnames = department.Members.Select(person => person.Surname).ToList();
            return surnames;
        }

        public List<Department> GetDepartments()
        {
            var departments = new List<Department>
    {
        new Department
        {
            Name = "Purchasing",
            Location = "Top floor",
            Members = new List<Person>
            {
                new Person { Surname = "Smith", Forename = "John", Title = Title.MR },
                new Person { Surname = "Jones", Forename = "Steve", Title = Title.MR },
                new Person { Surname = "Bradshaw", Forename = "Lisa", Title = Title.MRS }
            }
        },
        new Department
        {
            Name = "Sales",
            Location = "Bottom floor",
            Members = new List<Person>
            {
                new Person { Surname = "Bradshaw", Forename = "Lisa", Title = Title.MRS },
                new Person { Surname = "Thompson", Forename = "Joanne", Title = Title.MISS },
                new Person { Surname = "Johnson", Forename = "David", Title = Title.MR }
            }
        }
    };

            return departments;
        }

        public List<Customer> FindShortestRoute(int startX, int startY)
        {
            var customers = GenerateSampleCustomers();
            // Calculate the distances between the starting point and all customers.
            var distances = new Dictionary<int, int>();
            for (int i = 0; i < customers.Count; i++)
            {
                var distanceX = Math.Abs(customers[i].X - startX);
                var distanceY = Math.Abs(customers[i].Y - startY);
                distances[i] = distanceX + distanceY;
            }

            // Generate all possible permutations of customer orderings.
            var customerIndices = Enumerable.Range(0, customers.Count).ToList();
            var permutations = GetPermutations(customerIndices);

            // Find the shortest route among all permutations.
            int shortestDistance = int.MaxValue;
            List<Customer> shortestRoute = null;

            foreach (var permutation in permutations)
            {
                int totalDistance = distances[permutation[0]];
                for (int i = 1; i < permutation.Count; i++)
                {
                    totalDistance += distances[permutation[i]];
                    totalDistance += distances[permutation[i - 1]];
                }

                if (totalDistance < shortestDistance)
                {
                    shortestDistance = totalDistance;
                    shortestRoute = permutation.Select(i => customers[i]).ToList();
                }
            }

            return shortestRoute;
        }

        private List<List<int>> GetPermutations(List<int> items)
        {
            if (items.Count == 0)
                return new List<List<int>> { new List<int>() };

            var permutations = new List<List<int>>();

            for (int i = 0; i < items.Count; i++)
            {
                var subItems = new List<int>(items);
                subItems.RemoveAt(i);
                var subPermutations = GetPermutations(subItems);

                foreach (var subPermutation in subPermutations)
                {
                    subPermutation.Insert(0, items[i]);
                    permutations.Add(subPermutation);
                }
            }

            return permutations;
        }

        private int CalculateTotalDistance(List<Customer> route, int startX, int startY)
        {
            int totalDistance = 0;
            int currentX = startX;
            int currentY = startY;

            foreach (var customer in route)
            {
                totalDistance += Math.Abs(customer.X - currentX) + Math.Abs(customer.Y - currentY);
                currentX = customer.X;
                currentY = customer.Y;
            }

            return totalDistance;
        }

        private IList<string> GetDepartmentsForPerson(string surname, string forename)
        {
            var departments = GetDepartments();
            if (departments == null || departments.Count == 0)
            {
                throw new ArgumentException("Departments cannot be empty");
            }
            if (surname == null || surname == string.Empty)
            {
                throw new ArgumentException("Surname cannot be empty");
            }
            if (forename == null || forename == string.Empty)
            {
                throw new ArgumentException("Forename cannot be empty");
            }
            var departmentNames = new List<string>();

            foreach (var department in departments)
            {
                if (department.Members.Any(person =>
                    person.Surname == surname && person.Forename == forename))
                {
                    departmentNames.Add(department.Name);
                }
            }

            return departmentNames;
        }
    }
}
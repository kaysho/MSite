namespace MSite.Core
{
    public class Department
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<Person> Members { get; set; }
    }
}

namespace Authentication_Service.Models
{
    public class Dummy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dummy(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public override string ToString()
        {
            return $"Dummy(Id: {Id}, Name: {Name}, Description: {Description})";
        }
    }
}

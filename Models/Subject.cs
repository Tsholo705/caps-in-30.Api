namespace caps_in_30.Api.Models
{
    public class Subject
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Category> Categories { get; set; } = new();
    }
}
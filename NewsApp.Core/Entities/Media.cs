namespace NewsApp.Core.Entities
{
    public class Media
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; } = new();
    }
}

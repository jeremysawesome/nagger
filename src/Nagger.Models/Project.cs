namespace Nagger.Models
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public SupportedRemoteRepository? AssociatedRemoteRepository { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

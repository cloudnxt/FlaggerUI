namespace Gates.Shared.Data
{
    public class AppModel : Entity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Namespace { get; set; }
        public string? Image { get; set; }
        public string? OldImages { get; set; } = "";
        public string? Replicas { get; set; }
        public string? ContainerPorts { get; set; }
        public bool GatesEnabled { get; set; } = true;
    }


    public class AppDetailModel : AppModel
    {

       
    }

    public class ImageDetails 
    {
        public string Image { get; set; }
        public DateTime Applied { get; set; }

    }
}

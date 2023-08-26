using System.ComponentModel.DataAnnotations;

namespace Gates.Shared.Data
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Updated { get; set; } = DateTime.Now;
    }
}

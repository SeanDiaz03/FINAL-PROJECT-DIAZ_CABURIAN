namespace HelpDesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public List<Remark> Remarks { get; set; } = new();
    }

}

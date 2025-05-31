namespace HelpDesk.Models
{
    public class Remark
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string MadeBy { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }

}

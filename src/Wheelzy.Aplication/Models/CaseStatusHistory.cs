using Wheelzy.Application.Enums;

namespace Wheelzy.Application.Models
{
    public class CaseStatusHistory
    {
        public long Id { get; set; }
        public int CaseId { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime? StatusDateUtc { get; set; } // Requerido si Status == PickedUp
        public string ChangedBy { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Wheelzy.Application.Enums;

namespace Wheelzy.API.Requests
{
    public class ChangeCaseStatusRequest
    {
        [Required]
        public int? OrderId { get; init; }

        [Required]
        public CaseStatus? NewStatus { get; init; }

        [Required]
        public DateTime? StatusDateUtc { get; init; }
    }
}

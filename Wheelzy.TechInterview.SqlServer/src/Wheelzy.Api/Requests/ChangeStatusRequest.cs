using System.ComponentModel.DataAnnotations;

public record ChangeStatusRequest([Required] string Code, DateTime? StatusDate);

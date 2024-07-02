using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePlayerOnline.ViewModels;

public record class PlayerOnlineFormViewModel
{
	/// <example>0.0</example>
	[Required]
	public decimal Balance { get; init; }

	/// <example>0.0</example>
	[Required]
	public decimal FreeBalance { get; init; }

	/// <example>0.0</example>
	[Required]
	public decimal OutstandingBalance { get; init; }
}

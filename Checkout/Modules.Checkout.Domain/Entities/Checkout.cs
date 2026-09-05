using Modules.Checkout.Domain.Enums;

namespace Modules.Checkout.Domain.Entities;

public sealed class Checkout
{
	private Checkout() { }

	public Guid Id { get; private init; }
	public string UserId { get; private init; } = null!;
	public string OrderId { get; private init; } = null!;
	public string ShipmentNumber { get; private set; } = null!;
	public decimal Total { get; private init; }
	public CheckoutStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private init; }
	public DateTimeOffset? CompletedAt { get; private set; }

	public static Checkout Create(string userId, string orderId, decimal total)
		=> new()
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			OrderId = orderId,
			Total = total,
			Status = CheckoutStatus.Pending,
			CreatedAt = DateTimeOffset.UtcNow
		};

	public void MarkPaid() => Status = CheckoutStatus.Paid;

	public void Complete(string shipmentNumber)
	{
		ShipmentNumber = shipmentNumber;
		Status = CheckoutStatus.Completed;
		CompletedAt = DateTimeOffset.UtcNow;
	}

	public void Fail() => Status = CheckoutStatus.Failed;
}

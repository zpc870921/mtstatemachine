﻿namespace mtstatemachine.CourierActivities
{
    public interface PaymentArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }
}

using MassTransit;

namespace mtstatemachine.CourierActivities
{
    public class PaymentActivity : IActivity<PaymentArguments, PaymentLog>
    {
        public PaymentActivity()
        {

        }
        public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
        {
            await Task.Delay(100);
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArguments> context)
        {
            var orderid = context.Arguments.OrderId;
            var amount = context.Arguments.Amount;
            var cardnumber = context.Arguments.CardNumber;
            if(string.IsNullOrWhiteSpace(cardnumber))
            {
                throw new ArgumentNullException(nameof(cardnumber));
            }
            if (cardnumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("invalid cardnumber");
            }
            await Task.Delay(300);
            return context.CompletedWithVariables(new { AuthorizedCode ="77777"});
        }
    }
}

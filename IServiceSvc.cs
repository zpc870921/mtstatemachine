namespace mtstatemachine
{
    public interface IServiceSvc
    {
        string Hello(string msg);
    }

    public class ServiceSvc : IServiceSvc
    {
        public string Hello(string msg)
        {
           // Console.WriteLine("hello {msg}",msg);
            return "hello " + msg;
        }
    }
}

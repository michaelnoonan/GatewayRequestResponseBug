using System;
using Messages;
using NServiceBus;
using NServiceBus.Features;

namespace Site2
{
    class Program
    {
        static IBus Bus { get; set; }

        static void Main(string[] args)
        {
            SetLoggingLibrary.Log4Net();
            Configure.Features.Disable<SecondLevelRetries>();
            Configure.Features.Disable<TimeoutManager>();
            Bus = Configure.With()
                           .Log4Net()
                           .DefaultBuilder()
                           .InMemoryFaultManagement()
                           .RunGatewayWithInMemoryPersistence()
                           .UnicastBus()
                           .CreateBus()
                           .Start();

            Console.ReadLine();
        }
    }

    public class AskedAndAnswered : IHandleMessages<Question>
    {
        public IBus Bus { get; set; }

        public void Handle(Question message)
        {
            Bus.Reply(new Answer("The answer to your question is 42."));
        }
    }

}

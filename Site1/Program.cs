using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.MessageMutator;

namespace Site1
{
    internal class Program
    {
        private static IBus Bus { get; set; }

        private static void Main(string[] args)
        {
            SetLoggingLibrary.Log4Net();
            Configure.Features.Disable<TimeoutManager>();
            Configure.Features.Disable<SecondLevelRetries>();
            Bus = Configure.With()
                           .Log4Net()
                           .DefaultBuilder()
                           .InMemoryFaultManagement()
                           .RunGatewayWithInMemoryPersistence()
                           .UnicastBus()
                           .CreateBus()
                           .Start();

            Thread.Sleep(3000);
            var task = AskQuestion();
            task.ContinueWith(_ => Console.WriteLine("All done."));
            Console.ReadLine();
        }

        private static async Task AskQuestion()
        {
            const string question = "What is the meaning of life, the universe and everything?";
            Console.WriteLine(question);
            var answer =
                await
                Bus.SendToSites(new[] {"Site2"}, new Question(question))
                   .Register(cr => cr.Messages.OfType<Answer>().FirstOrDefault());
            Console.WriteLine(answer.TheAnswerToYourQuestionIs);
        }
    }

    public class RepairCorrelationIdIncomingMessageMutator : IMutateIncomingMessages, INeedInitialization
    {
        public IBus Bus { get; set; }

        public void Init()
        {
            // BUG: To work around this bug we can register this mutator
            //Configure.Instance.Configurer.ConfigureComponent<RepairCorrelationIdIncomingMessageMutator>(DependencyLifecycle.InstancePerCall);
        }

        public object MutateIncoming(object message)
        {
            var correlationId = Bus.CurrentMessageContext.Headers[Headers.CorrelationId];
            if (correlationId.EndsWith("\\0"))
            {
                Console.WriteLine("Recieved message with malformed CorrelationId. Id:'{0}' CorrelationId:'{1}'",
                               Bus.GetMessageHeader(message, Headers.MessageId),
                               Bus.GetMessageHeader(message, Headers.CorrelationId));
                Bus.CurrentMessageContext.Headers[Headers.CorrelationId] =
                    Bus.CurrentMessageContext.Headers[Headers.CorrelationId].Replace("\\0", "");
                Console.WriteLine("Replaced message Id:'{0}' CorrelationId:'{1}'",
                               Bus.GetMessageHeader(message, Headers.MessageId),
                               Bus.GetMessageHeader(message, Headers.CorrelationId));
            }
            return message;
        }
    }
}

using NServiceBus;

namespace Messages
{
    public class Answer : IMessage
    {
        public string TheAnswerToYourQuestionIs { get; protected set; }

        public Answer(string theAnswerToYourQuestionIs)
        {
            TheAnswerToYourQuestionIs = theAnswerToYourQuestionIs;
        }
    }
}
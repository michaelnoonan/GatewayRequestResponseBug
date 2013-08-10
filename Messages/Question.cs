using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Messages
{
    public class Question : ICommand
    {
        public Question(string theQuestionYouWantToAsk)
        {
            TheQuestionYouWantToAsk = theQuestionYouWantToAsk;
        }

        public string TheQuestionYouWantToAsk { get; protected set; }
    }
}

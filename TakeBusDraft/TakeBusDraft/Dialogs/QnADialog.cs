using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TakeBusDraft.Services;

namespace TakeBusDraft.Dialogs
{
    [Serializable]
    public class QnADialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(CommonProcess);
        }

        private async Task CommonProcess(IDialogContext context, IAwaitable<object> result)
        {
            var searcher = new QnAMakerSearcher();
            var answer=searcher.Search(context?.Activity?.AsMessageActivity()?.Text);

            if( answer.Score == 0 )
            {               
                await context.PostAsync(Resources.AnswerNotFound);
                context.Done(false);
            }
            else if(answer.Answer==Resources.EndMessage)
            {
                context.Done(false);
            }
            else
            {
                await context.PostAsync(answer.Answer);
                context.Done(true);
            }

        }
    }
}
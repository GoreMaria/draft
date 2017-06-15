using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TakeBusDraft.Services;
using Microsoft.Bot.Connector;
using TakeBusDraft.Classes;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace TakeBusDraft.Dialogs
{
    [Serializable]
    public class QnADialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(QuestionReceivedProcess);
        }

        
        private string userQuestion;
        private List<QnAAnswer> kbAnswer;

        private async Task QuestionReceivedProcess(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            
            var response   = await result;
            userQuestion = response.Text; //user question
            var searcher =  new QnAMakerSearcher();
            kbAnswer= searcher.Search(userQuestion); 
       
            if( kbAnswer[0].Score == 0 )
            {               
                await context.PostAsync(Resources.AnswerNotFound);
                context.Done<object>(null); 
            }
                      
            else
            {
                await context.PostAsync(kbAnswer[0].Answer);

                var attach = new ThumbnailCard(title: Resources.FeedbackQuesion, subtitle: null, text: null,
                            buttons: new List<CardAction>
                            {
                            new CardAction(type: ActionTypes.PostBack, title:Resources.Yes, value: Resources.Yes),
                            new CardAction(type: ActionTypes.PostBack, title:Resources.No, value: Resources.No)
                            }).ToAttachment();

                var message = context.MakeMessage();
                message.Attachments.Add(attach);
                await context.PostAsync(message);

                context.Wait(FeedbackReceiver);
            }

        }

        private async Task FeedbackReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text == Resources.Yes)
            {
                TrainService(kbAnswer[0], userQuestion);

                await context.PostAsync(Resources.FeedbackThanksMessage);
                context.Done<object>(null);
            }
            else if (response.Text == Resources.No && kbAnswer.Count > 1) 
            {
                var message=context.MakeMessage();
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                for (int i = 0; i < kbAnswer.Count; i++)
                {
                    var answercard = new ThumbnailCard
                    (
                        title: Resources.PossibleAnswerCard,
                        text: kbAnswer[i].Answer,
                        buttons: new List<CardAction>
                        { new CardAction(type: ActionTypes.PostBack, title: Resources.Yes, value: i.ToString()) }
                    );

                    message.Attachments.Add(answercard.ToAttachment());
                }

                var noanswercard = new ThumbnailCard
                    (
                        title: Resources.NoAnswerCard,
                        buttons: new List<CardAction>
                        { new CardAction(type: ActionTypes.PostBack, title: Resources.Yes, value: Resources.AnswerNotFound) }
                    );

                message.Attachments.Add(noanswercard.ToAttachment());

                await context.PostAsync(message);
                context.Wait(UsefullAnswerReceiver);
            }       
        }

        private async Task UsefullAnswerReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
           
            var response = await result;

            if (response.Text == Resources.AnswerNotFound)
            {
               await context.PostAsync(Resources.NoAnswerCardMessage);

               context.Done<object>(null);
            }
            else
            {
                var idAnswer = int.Parse(response.Text);
                TrainService(kbAnswer[idAnswer], userQuestion);

              await context.PostAsync(Resources.FeedbackThanksMessage);
              context.Done<object>(null);
            }
          
        }

        public async void TrainService(QnAAnswer answer, string lastQuestion)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ServiceKeys.qnamakerSubscriptionKey);

            var uri = $"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{ServiceKeys.knowledgebaseId}/train";

            HttpResponseMessage response;

            // Request params
            string lastAnswerQ = answer.Questions[0];
            string lastAnswerA = answer.Answer;

            string body = "{\"feedbackRecords\": [{\"userId\": \"" + Guid.NewGuid() + "\",\"userQuestion\": \"" + lastQuestion + "\",\"kbQuestion\": \"" + lastAnswerQ + "\",\"kbAnswer\": \"" + lastAnswerA + "\"}]}";
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpRequestMessage req = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
                req.Content = content;
                response = await client.SendAsync(req);
            }           
        }
    }
}
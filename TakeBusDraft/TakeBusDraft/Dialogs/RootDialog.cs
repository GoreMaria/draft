using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using TakeBusDraft.Services;
using TakeBusDraft.Classes;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Web;
using System.Text.RegularExpressions;

namespace TakeBusDraft.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync); 
        }

        // welcome message
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            // return our first welcome to the user
            await context.PostAsync(Resources.WelcomeMessage); // send message to user
           
            // create message
            var message = PopulateMenuMessage(context.MakeMessage(), title:Resources.IntroMessage);
            //post message to user
            await context.PostAsync(message);
            context.Wait(MenuReplyReceiver);
        }

        // create menu for initial message
        private IMessageActivity PopulateMenuMessage(IMessageActivity message, string title=null, string subtitle=null)
        {
            // create menu by ThumbnailCard
            var menu = new ThumbnailCard(title: title, subtitle: subtitle, text: Resources.TryOptions,
                buttons: new List<CardAction>
                {
                    new CardAction(type: ActionTypes.PostBack,  title:Resources._commonDialogCommand, value:Resources._commonDialogCommand),
                    new CardAction(type: ActionTypes.PostBack,  title:Resources._supportDialogCommand, value:Resources._supportDialogCommand),
                    new CardAction(type: ActionTypes.PostBack,  title:Resources._cancelDialogCommand, value: Resources._cancelDialogCommand)
                });

            // add menu to message
            message.Attachments.Add(menu.ToAttachment());
            return message;
        }

        private async Task MenuReplyReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if(response.Text==Resources._commonDialogCommand)
            {
               await context.Forward(new BuyTicketDialog(),DialogEnded,response,context.CancellationToken);
            }
            else if(response.Text==Resources._supportDialogCommand)
            {
                await context.PostAsync(Resources.CallCenter);
                await context.PostAsync(Resources.HelpMessage);
                context.Wait(MenuReplyReceiver);

            }
            else if(response.Text==Resources._cancelDialogCommand)
            {
                await context.PostAsync(Resources.EndMessage);
                context.Done<object>(null);
            }
            else {
                // use regular expressions, or luis

                if (response.Text.Contains("хорошо") || response.Text.Contains("спасибо") || response.Text.Contains("до свидания") || response.Text.Contains("ничем"))
                {
                    await context.PostAsync(Resources.ThankYouMessage);
                   // context.Done<object>(null);
                }
                else
                {
                    await context.Forward(new QnADialog(), DialogEnded, response, context.CancellationToken);

                }

                }
        }


        private async Task DialogEnded(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Resources.HelpMessage);
            context.Wait(MenuReplyReceiver);

        }

        

    }
}
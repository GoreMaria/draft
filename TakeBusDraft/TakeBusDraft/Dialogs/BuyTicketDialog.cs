using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using TakeBusDraft.Services;
using TakeBusDraft.Classes;

namespace TakeBusDraft.Dialogs
{
    [Serializable]
    public class BuyTicketDialog : IDialog
    { 
        public async Task StartAsync(IDialogContext context)
        {
           context.Wait(PricingProcess);
        }

        private Ticket newTicket;

        private async Task PricingProcess(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Resources.From);
            newTicket = new Ticket();
            context.Wait(FromCityReceiver);
        }

        private async Task FromCityReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            // TODO:check if response is not a city

            newTicket.From = response.Text;

            await context.PostAsync(Resources.To);
            context.Wait(ToCityReceiver);

        }

        private async Task ToCityReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            // TODO: check if response is not a date

            newTicket.To = response.Text;

            await context.PostAsync(Resources.Date);
            context.Wait(DateReceiver);
        }

        private async Task DateReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            // TODO: Show available time from db

            newTicket.Date = response.Text;

            await context.PostAsync(Resources.Time);
            context.Wait(TimeReceiver);
        }

        private async Task TimeReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            newTicket.Time = response.Text;

            var price = newTicket.CalculatePrice();

            var ticketReceipt = new ReceiptCard(title: Resources.TicketTitle,
                                       facts: new List<Fact>
                                              {
                                                 new Fact(Resources.FromField, newTicket.From),
                                                 new Fact(Resources.ToField, newTicket.To),
                                                 new Fact(Resources.DateField, newTicket.Date + Resources.Year),
                                                 new Fact(Resources.TimeField, newTicket.Time) }
                                       //       },
                                       //total: price.ToString() + Resources.Currency,
                                       //buttons: new List<CardAction>
                                       //         {
                                       //             new CardAction(ActionTypes.PostBack, title: Resources.Buy, value: Resources.Buy), // or OpenUrl
                                       //             new CardAction(ActionTypes.PostBack, title: Resources.Save, value: Resources.Save)
                                       //         }
                                       );

            var message = context.MakeMessage();
            message.Attachments.Add(ticketReceipt.ToAttachment());
            await context.PostAsync(message);

            context.Wait(ReсeiptAnswerReceiver);


        }



        private async Task ReсeiptAnswerReceiver(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text == Resources.BuyReply)
                //stub
                await context.PostAsync(Resources.BuyReply);
            else
                //stub
                await context.PostAsync(Resources.SaveReply);
        
            context.Done<object>(null);
        }
    }
}
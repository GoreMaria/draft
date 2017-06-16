# How to create trainable web-site faq assistant using Microsoft Bot Framework and QnA Maker

This repository contains solution that impements a simple trainable faq bot-assistant. 

Bot contonuously trained itself according to feedbacks provided from users. If an answer doesn't match user's expectation, the bot provides a top 3 answers from knowledge base and aks to chose appropriate. The result sends back to knowledge base.

Test bot [prototype in Telegram](https://t.me/tbus_bot). (Ru)

### Solution overview:
- QnA Maker gets and parses simple FAQ page in QnA pairs. 
- *Searcher* class connects to knowledge base hosted on qnamker service and posts the request
- Response contains top-3 answers that matched with user question based on "score" field
- When user tap "Yes" button bot provides "Thank you for feedback" message. 
- When user tap "No" bot shows AttachementLayout with type "Carousel" where ThumbnailCards created and added dynamicly from list of answers. And user can choose appropriate or inform that no answer was helpful.-
- *Train* method patch knowledge base according to user feedback. When user chose from "carousel" of answers code saved the id of chosen answer and post it to Train method.

### To test this bot with your FAQ locally:
1. Clone the repository  
2. Go to [qnmaker.ai](http://qnamaker.ai/)
2. Put link on your FAQ page or upload call-center script
3. Test, train and publish
4. Put *knowledgebaseId* and *qnamakerSubscriptionKey* keys generated in previous steps in ServiceKeys.cs

### To test this bot with your FAQ in different channels: 
1. Use steps from previos section
2. Go to [Azure](portal.azure.com) portal
3. Cretate Web App
4. Go to [dev.botframework.com](dev.botframework.com) and register your bot 
5. Put Microsoft App ID & Microsoft App Password from previous step to WebConfig
6. Deploy your bot, e.g. using *deployment options* tab to connect your repo

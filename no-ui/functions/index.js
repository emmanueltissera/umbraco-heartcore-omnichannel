// See https://github.com/dialogflow/dialogflow-fulfillment-nodejs
// for Dialogflow fulfillment library docs, samples, and to report issues
'use strict';

const functions = require('firebase-functions');
const { WebhookClient } = require('dialogflow-fulfillment');
const { Card, Suggestion } = require('dialogflow-fulfillment');

process.env.DEBUG = 'dialogflow:debug'; // enables lib debugging statements

exports.dialogflowFirebaseFulfillment = functions.https.onRequest((request, response) => {
    const agent = new WebhookClient({ request, response });
    console.log('Dialogflow Request headers: ' + JSON.stringify(request.headers));
    console.log('Dialogflow Request body: ' + JSON.stringify(request.body));

    function welcome(agent) {
        agent.add(`Lord Lamington sends his greetings. How can I help you today?`);
        agent.add(new Suggestion('Specials, please.'));
    }

    function fallback(agent) {
        agent.add(`I didn't understand`);
        agent.add(`I'm sorry, can you try again?`);
    }

    function specialsTodayHandler(agent, specialsData) {
        var specialMessage, specialTitle, specialImage;
        if (specialsData.length == 0) {
            specialMessage = "Oops! Our specials are all gone for the day. You can try tomorrow!";
            specialTitle = "Sorry!";
            specialImage = "";
        } else {
            let specialTodayType = specialsData[0].title;
            let specialTodayQty = specialsData[0].quantity;
            specialMessage = `We have got ` + specialTodayType + ` on special today. There are only ` + specialTodayQty + ` left. So you better hurry if you want one!`;
            specialTitle = specialTodayType + ` on special today`;
            specialImage = specialsData[0].image;
        }
        agent.add(specialMessage);
        agent.add(new Card({
            title: specialTitle,
            imageUrl: specialImage,
            text: specialMessage
        }));
    }

    function specialsTodayInvoker(agent) {
        const rp = require('request-promise-native');
        var options = {
            method: 'POST',
            uri: 'https://cdn.umbraco.io/content/filter',
            headers: {
                'umb-project-alias': 'emmanuels-tidy-otter',
                'culture': 'en-AU',
                'umb-nocache': 'true',
                'Content-Type': 'application/json'
            },
            body: {
                'contentTypeAlias': 'product',
                'properties': [{
                    'alias': 'isOnSpecialToday',
                    'value': '1',
                    'match': 'LIKE'
                }]
            },
            json: true // Automatically parses the JSON string in the response
        };

        return rp(options)
            .then(data => {
                var specials = [];
                if (data._totalItems > 0) {
                    data._embedded.content.map(function(item) {
                        if (item.isOnSpecialToday && item.quantity > 0) {
                            var datum = { title: item.title, description: item.description, image: item.image._url, quantity: item.quantity };
                            specials.push(datum);
                        }
                    });
                }
                console.log('Data:', specials);
                specialsTodayHandler(agent, specials);
            })
            .catch(function(err) {
                agent.add("Oops! We had issues retrieving data. Please try again later.");
            });
    }


    // Run the proper function handler based on the matched Dialogflow intent name
    let intentMap = new Map();
    intentMap.set('Default Welcome Intent', welcome);
    intentMap.set('Default Fallback Intent', fallback);
    intentMap.set('SpecialsToday', specialsTodayInvoker);
    agent.handleRequest(intentMap);
});
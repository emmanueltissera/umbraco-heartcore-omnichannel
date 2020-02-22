## Setup a Google assitant agent step by step:

1.  Go to [https://dialogflow.cloud.google.com/](https://dialogflow.cloud.google.com/)
    
2.  Create a new agent
    
3.  Go to the settings of the new agent and in the “Export and Import” section, Import from zip the file found here - Heartcore-lamington-Rnd.zip
    
4.  Once you have imported it, go to the Fulfilment section and enable the Inline Editor (Powered by Cloud Functions for Firebase)
    
5.  Copy the code from index.js and package.js files and deploy
    
6.  Once the deployment is complete, you should be able to test the application in the Google Assistant Test console or any other Google devices you may have.

**IMPORTANT:** *In order to call external APIs such as Umbraco Heartcore, you need to have a paid firebase package. You would need to go to the Usage and Billing section in your Firebase project and change it to use a Pay as you Go plan.*

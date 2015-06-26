A Practical Guide To Big Data - Demo Code
===================

This code and instruction guide contain the basic steps to get started with Big Data in Microsoft's Azure platform. It utilizes the following technologies:

* Microsoft Azure's HDInsight Hadoop cluster
* node.js
* npm packages Twit, socket.io, rotating-log, and the Express webserver
* the HDInsight .NET SDK
* Hive, the Hadoop querying language
* Power Query, Microsoft's ETL tool embedded within Microsoft Excel

The basic coding steps in this demo were modified from Aidan Casey's awesome HadoopTweetAnalyser project. Thanks, Aidan!

## Demo Outline

1. Sign up for Microsoft Azure and provision a storage account and HDInsight Cluster.
2. Sign up for a Twitter developer API access key to be able to access and capture streaming tweets.
3. Configure and execute our Node.JS webserver to capture user tweets triggered by various keywords.
4. Using Azure Powershell, push our tweets and a custom keyword dictionary to Azure Blob Storage.
5. Submit Hive jobs both through Powershell and using the HDInsight .NET SDK.
6. Analyze the results of our data crunching in Excel via Power Query.

# Signing up for Microsoft Azure

1. Visit https://azure.microsoft.com and sign up for the free trial. As of June 2015, Microsoft provides Azure users with $200 worth of Azure credits during their first month. The estimated cost to run this demo is $2.
2. Log in to your new Azure account and you'll find yourself at the Azure Management Portal home page.

![Azure Portal Main Page](/../screenshots/screenshots/AzurePortal.JPG?raw=true "Azure Portal main page")

3. Add a Storage Account. Select "NEW" at the bottom of the portal, then Data Services -> Storage -> Create. Provide a unique subdomain for your URL, and be sure to choose an Affinity Group near your location for this demo.
4. Once your Storage Account has been provisioned, add your new HDInsight Cluster. Again select "NEW" at the bottom of the portal, then Data Services -> HDInsight -> Hadoop. Choose another unique subdomain for your cluster's URL, choose "2 data nodes" for the cluster size, a password to access the cluster via HTTP, and for the Storage Account choose the Storage Account you created in step 3.

![Create a new HD Insight Cluster](/../screenshots/screenshots/NewHDInsightCluster.JPG?raw=true "Create a new HD Insight Cluster")

While the cluster is provisioning, we can move on to the next part of the tutorial.

# Signing up for a Twitter developer API

1. Sign up for a Twitter account. You can use your existing account or create a new one solely to maintain your apps if you desire.
2. Once logged in to Twitter, visit https://apps.twitter.com/app/new. Fill out the information in the form. For the purposes of this demo, you can enter anything in the Website field as we're not actually building a production app.
3. Once your app has been created, select it from the main screen at https://apps.twitter.com. From within the App settings page, choose the "Keys and Access Tokens" tab. Stay on this page, you'll need these in the next part of the tutorial.

# Installing and executing our Node.JS Tweet Aggregator

1. First, download and install node.js at https://nodejs.org/download/.
2. Once installed, open a Command Prompt and navigate to the Tweet Aggregator folder from this codebase.
3. At the prompt, enter 'npm install'. This will download all of the necessary packages to run our webserver.
4. Open the file 'config.json' within the Tweet Aggregator folder. Fill in the following information:
  * From Twitter, the consumer_key, consumer_secret, access_token, and access_token_secret.
  * 
4. Now enter 'node server.js'. If successful, you should see something like this in the command console:

![Node webserver is running](/../screenshots/screenshots/RunningNodeServer.JPG?raw=true "A successfully running Node.JS webserver")

5. Open a web browser and visit http://localhost:8080 . 



Configure config.JSON with:
 * Your Twitter API credentials
 * Log file destination
 * Max log file size (default is 5MB)
 * Keyword watchlist
 

npm install express, rotating-log, socket-io, twit
 
In a console, run node server.js, visit localhost:8080, capture as many tweets as you want. Ctrl +  C to kill web server.

#Azure Powershell

Add-AzureAccount
Set-Subscription
Set-AzureBlobStorageContent - File yourtweets -Blob Tweets/n
Set-AzureBlobStorageContent - File youridentifiers - Blob Identifiers/0

#HDInsight Client

Generate certificate, use Certificate Manager to give it a friendly name.

Configure App.Config with:
 * Azure Storage Account name
 * Azure subscription ID
 * HDFS cluster name
 * Local certificate friendly name
 
If this is your first time running, uncomment the line CreateTables to create the Hive external tables pointing to your Tweets and Identifiers directory.

# Power Query

Open Excel, Azure -> Azure Blob Storage

Supply URL of storage account and account key.
AAALoadTweets/stdout -> Binary -> Close and Load.

Bonus task: Modify identifiers file, re-run Hive job, refresh Power Query.

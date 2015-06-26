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
2. Configure and execute our Node.JS webserver to capture user tweets triggered by various keywords.
3. Using Azure Powershell, push our tweets and a custom keyword dictionary to Azure Blob Storage.
4. Submit Hive jobs both through Powershell and using the HDInsight .NET SDK.
5. Analyze the results of our data crunching in Excel via Power Query.

# Signing up for Microsoft Azure

1. Visit https://azure.microsoft.com and sign up for the free trial. As of June 2015, Microsoft provides Azure users with $200 worth of Azure credits during their first month. The estimated cost to run this demo is $2.
2. 

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

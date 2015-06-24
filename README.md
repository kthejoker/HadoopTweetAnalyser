HadoopTweetAnalyser
===================
A node.js twitter stream aggregator and Hadoop file processor.



# Basic Usage

# Tweet Aggregator

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

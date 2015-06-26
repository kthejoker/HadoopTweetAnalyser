A Practical Guide To Big Data - Demo Code
===================

This guide was prepared in conjunction with a presentation given to the Houston BI Users Meetup group on July 24, 2015. The PowerPoint presentation is included in the codebase.

This code and instruction guide contain the basic steps to get started with Big Data in Microsoft's Azure platform. It utilizes the following technologies:

* Microsoft Azure's HDInsight Hadoop cluster
* node.js
* npm packages Twit, socket.io, rotating-log, and the Express webserver
* Twitter's developer APIs
* Azure Powershell
* the HDInsight .NET SDK
* Hive, the Hadoop querying language
* Power Query, Microsoft's ETL tool embedded within Microsoft Excel

The basic coding steps in this demo were modified from Aidan Casey's awesome HadoopTweetAnalyser project. Thanks, Aidan!

<h2 color="red">Warning</h2>

When you have completed this demo and any additional testing you wish to perform, it is imperative that you delete your HDInsight cluster, as it is billed by its ontime not its usage time. A 2 node cluster costs roughly $15 a day - that bill will rack up fast if you forget to do this. You can also prevent this by setting hard limits on your Azure billing preferences.

## Demo Outline

1. Sign up for Microsoft Azure and provision a storage account and HDInsight Cluster.
2. Sign up for a Twitter developer API access key to be able to access and capture streaming tweets.
3. Configure and execute our Node.JS webserver to capture tweets for analysis.
4. Using Azure Powershell, push our tweets and a custom keyword dictionary to Azure Blob Storage.
5. Submit Hive jobs both through Powershell and using the HDInsight .NET SDK.
6. Analyze the results of our data crunching in Excel via Power Query.

## Signing up for Microsoft Azure

1. Visit https://azure.microsoft.com and sign up for the free trial. As of June 2015, Microsoft provides Azure users with $200 worth of Azure credits during their first month. The estimated cost to run this demo is $2.
2. Log in to your new Azure account and you'll find yourself at the Azure Management Portal home page.

![Azure Portal Main Page](/../screenshots/screenshots/AzurePortal.JPG?raw=true "Azure Portal main page")

3. Add a Storage Account. Select "NEW" at the bottom of the portal, then Data Services -> Storage -> Create. Provide a unique subdomain for your URL, and be sure to choose an Affinity Group near your location for this demo. Wait until the Storage Account has been provisioned.
4. Add your new HDInsight Cluster. Again select "NEW" at the bottom of the portal, then Data Services -> HDInsight -> Hadoop. Choose "2 data nodes" for the cluster size, and remember the password you choose here, as you'll need it later in the demo.

![Create a new HD Insight Cluster](/../screenshots/screenshots/NewHDInsightCluster.JPG?raw=true "Create a new HD Insight Cluster")

While the cluster is provisioning, we can move on to the next part of the tutorial.

## Signing up for a Twitter developer API

1. Sign up for a Twitter account. You can use your existing account or create a new one solely to maintain your apps if you desire.
2. Visit https://apps.twitter.com/app/new. Fill out the information in the form. For the purposes of this demo, you can enter anything in the Website field as we're not actually building a production app.
3. Select your new app from the main screen at https://apps.twitter.com. From within the App settings page, choose the "Keys and Access Tokens" tab. Stay on this page, you'll need these in the next part of the tutorial.

## Installing and executing our Node.JS Tweet Aggregator

Our aggregator will work as a two-pass system. First, we will enter keywords to identify Twitter users we are interested in analyzing. Then we will capture the previous 200 tweets of these users to perform basket analysis on these users. In short, we'll try to find other posting patterns among the users who post about the keywords we choose.

1. First, download and install node.js at https://nodejs.org/download/.
2. Once installed, open a Command Prompt and navigate to the Tweet Aggregator folder from this codebase.
3. At the prompt, enter <code>npm install</code>. This will download all of the necessary packages to run our webserver.
4. Rename the file <em>sample.config.json</em> within the Tweet Aggregator folder to <em>config.json</em>. Fill in the following information:
  * From Twitter, the <code>consumer_key</code>, <code>consumer_secret</code>, <code>access_token</code>, and <code>access_token_secret</code>.
  * The <code>log_directory</code> for where to store the generated Tweet logs.
  * The <code>tweet_keywqrds</code> you wish to track in Tweets, separated by commas.
4. Now enter into the console <code>node server.js</code>. If successful, you should see something like this in the command console:

![Node webserver is running](/../screenshots/screenshots/RunningNodeServer.JPG?raw=true "A successfully running Node.JS webserver")

5. Open a web browser and visit http://localhost:8080 . If you've chosen fairly popular keywords, you should begin to see matching tweets emitting both into the console and the browser window, as well as logs generating in your log directory.
6. You can let this run as long as you wish, but for the purposes of this demo, 3-5 minutes should provide enough user tweets to analyze.
7. When you are satisfied with your results, return to the console and hit Ctrl + C to stop the web server.


As a last step, you may optionally create a text file, <em>Identifiers.txt</em>, which contains a custom set of keywords, lowercase, one per row to filter your basket analysis. (An example file is provided in the Tweet Aggregator folder.)

## Uploading our tweets and custom keyword dictionary to our HDInsight Cluster

1. Install Azure Powershell ( https://github.com/Azure/azure-powershell/releases ) and open the program.
2. At the prompt enter <code>Add-AzureAccount</code>. Re-enter your Azure credentials in the popup to connect your Powershell session to your Azure account.
3. Next enter <code>Set-AzureSubscription -SubscriptionName "<b>(your subscription name)</b>" -CurrentStorageAccount "<b>(your Storage Account name)</b>"</code>. You can find your Subscription Name under the Settings tab of your Azure Management Portal.

![Azure Settings](/../screenshots/screenshots/AzureSettings.JPG?raw=true "Azure Settings")

4. Now we'll upload our two files (Tweets.log and Identifiers.txt) to our HDInsight Cluster. (Technical note: When creating an HDInsight Cluster, the Storage Account adds a container with the same name as the cluster to itself.) The two commands to enter are

<code>Set-AzureStorageBlobContent -Container <b>(your cluster name)</b> -File <b>(C:\path\to\Tweets.log)</b> -Blob Tweets\1
Set-AzureStorageBlobContent -Container <b>(your clutser name)</b> -File <b>(C:\path\to\Identifiers.txt)</b> -Blob Identifiers\1</code>

Now we're ready to turn our files into Hive tables and run Hive jobs against them to retrieve our data.

## Creating Hive tables and submitting Hive jobs manually

It's possible to run the scripts to create the Hive tables and submit Hive jobs to produce our data directly through Azure Powershell using the 'Invoke-Hive' command. You can also visit the HTTP site for your cluster at

<code>https://(your cluster name).azurehdinsight.net/</code>

(When prompted, the username is <code>admin</code> and the password is the one you entered when creating the cluster in Azure.)

And from there use the Hive Editor tab to submit and monitor jobs. Here are the Hive queries you need:

Creating tables:

<code>DROP TABLE tweets; CREATE EXTERNAL TABLE tweets( id_str string, created_at string, retweet_count string, tweetText string, userName string, userId string, screenName string, countryCode string, placeType string, placeName string, placeType1 string, coordinates array<string>) ROW FORMAT DELIMITED FIELDS TERMINATED BY '\t' COLLECTION ITEMS TERMINATED BY ',' STORED AS TEXTFILE location 'wasb://<b>(your cluster name)</b>@<b>(your storage account)</b>.blob.core.windows.net/Tweets';

DROP TABLE identifiers; CREATE EXTERNAL TABLE identifiers(identifier string) ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' STORED AS TEXTFILE location 'wasb://<b>(your cluster name)</b>@<b>(your storage account)</b>.blob.core.windows.net/Identifiers'</code>

Generating all word counts from Tweets:

<code>select Z.X.ngram[0] as word, Z.X.estfrequency as tweetCount  from (select explode(word_map) as X from ( SELECT context_ngrams(sentences(lower(tweetText)), array(null), 1000) as word_map FROM tweets ) Y) Z</code>

Generating data with the identifiers filter:

<code>select identifiers.identifier, Z.X.estfrequency as tweetCount  from (select explode(word_map) as X from ( SELECT context_ngrams(sentences(lower(tweetText)), array(null), 1000) as word_map FROM tweets ) struct) Z join identifiers on identifiers.identifier = Z.X.ngram[0]</code>

(Technical note: If you do run these through the online Hive Editor, you can't set the output file and therefore must note down the job GUID to find the output in Power Query later.)

## Running Hive jobs via the HDInsight .NET SDK

### Generating and uploading personal certificate

First, we must generate a personal certificate to allow our application to submit Hive jobs to your Azure portal on our behalf. 

1. Locate the Visual Studio Command Prompt application on your computer. This may also be called the Developer Command Prompt depending on the version of Visual Studio you have. There should be a shortcut in your Visual Studio program folder under your Start Menu. In Windows 8 you can search for it by hitting the Windows button, pressing Ctrl + Tab, and hitting V.
2. You must run the VS COmmand Prompt as an Administrator in order to generate certificates.
3. Run the following command, substituiting any certificate name for HDInsightDemo if you wish:

<code>makecert -sky exchange -r -n "CN=HDInsightDemo" -pe -a sha1 -len 2048 -ss My "HDInsightDemo.cer"</code>

![Certificate Generated](/../screenshots/screenshots/GenerateCertificate.JPG?raw=true "Generate Certificate")

4. Run Certificate Manager (<code>Start -> Run -> certmgr.msc</code>) and locate your certificate (<code>Personal -> Certificates</code>). Right click your certificate and select Properties. Enter a Friendly Name for the certificate, and note this down for use in our .NET application.
5. Return to the Azure Management Portal. Under Settings, choose the Management Certificates tab, and upload your certificate to Azure.

![Azure Certificate Upload](/../screenshots/screenshots/AzureCertificate.JPG?raw=true "Azure Certificate")

Now we're ready to submit Hive jobs via applications without our direct credentials.

### Configuring and running our .NET Application

1. Open the HDInsightClient solution from the demo codebase in Visual Studio.
2. In the App.config file, fill in the values for the <code>AzureSubscriptionID</code>, <code>ClusterName</code>, 
    <code>AzurePublisherCertName</code> (i.e. the Friendly Name from the steps above), and <code>AzureStorageAccount</code>.
3. Build the solution to retrieve the associated NuGet packages required to compile.
4. For the first execution, uncomment the <code>CreateTables()</code> call in <code>Main()</code>. (Once you create the tables, you can re-comment this line out.)
5. Start the application.
6. You can monitor the Hive jobs on the server by visiting the cluster website <code>https://(your cluster name).azurehdinsight.net/</code> and choosing the Job History tab.
7. After approximately 2-3 minutes, the two jobs should be complete and (fingers crossed!) you can see the results of the select query in your console window for the application.

## Accessing the Data in Power Query

1. If you haven't already, install Power Query ( http://tinyurl.com/pqdownload ). Make sure you choose the correct version (32 bit or 64 bit) for your version of Excel.
2. Open Excel and go to the new Power Query tab.
3. In the Get External Data section, choose Azure -> From Microsoft Azure Blob Storage.

![Choosing Azure in Power Query](/../screenshots/screenshots/PowerQueryAzure.JPG?raw=true "Choosing Azure in Power Query")

4. Enter the URL of your storage account ( https://(your storage account).blob.core.windows.net/ ).
5. When prompted, enter your Access Key. You can find this in the Azure Management Portal. Select Storage and then choose Manage Access Keys at the bottom of the screen. A window will pop up where you can copy your Access Key to paste back into Excel.
6. Power Query now presents you with the Navigator window. First, choose your HDInsight cluster's storage container from the left panel. Then choose the "Edit" button in the lower right corner to go to the Power Query editor window.

![Choosing Azure in Power Query](/../screenshots/screenshots/PowerQueryNavigator.JPG?raw=true "Choosing Azure in Power Query")

7. Within the editor window, find the row where the value of the Name column is <code>AAALoadTweets/stdout</code>. Select the corresponding Binary link under the Content column to retrieve our generated dataset.
8. Choose Close and Load from the top menu to pull the data into Excel as a table.

## Next Steps

Now that you're a Big Data expert, here are some more tasks to try!

1. Change the Node.JS server code to pull only incoming Tweets from the United Kingdom.
2. Change the keywords in Identifiers.txt. Re-upload the file to Azure, re-run the .NET application, and refresh Excel.
3. Change the Hive query to return only the hashtags contained in the Tweets.
4. Change the Hive query to only return words following the phrase "I love" in a Tweet. (Hint: context_ngrams )
4. Write a Powershell script file to perform all the Powershell steps from this demo; add a step invoking a Hive query to create the external tables.
5. Try to automate the entire workflow within the .NET solution.
6. Use Sqoop to bring the data into a SQL Server database.
6. Look into other Big Data tools like HBase, Pig, Storm, Oozie, and Tez. Become familiar with their use cases.
7. Try using the geographic coordinates from our Tweets to produce a Map chart in Power View.

Practical Big Data Questions

1. How are the four Vs represented in this demo?
2. What sorts of decisions might this kind of analysis serve as an input to?
3. How might you structure the final data to help with those decisions?
4. How much data is required to begin generating valid sample sizes?
5. What's the signal-to-noise ratio?
6. What sort of enterprise data could this integrate with?
7. How might you replicate this data analysis within a traditional BI architecture? 
8. Who in my company is interested in social and customer data?

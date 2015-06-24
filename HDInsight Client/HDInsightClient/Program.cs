using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using Microsoft.WindowsAzure.Management.HDInsight;
using Microsoft.Hadoop.Client;

namespace SubmitHiveJob
{
    
    class Program
    {
        private static void WaitForJobCompletion(JobCreationResults jobResults, IJobSubmissionClient client)
        {
            JobDetails jobInProgress = client.GetJob(jobResults.JobId);
            while (jobInProgress.StatusCode != JobStatusCode.Completed && jobInProgress.StatusCode != JobStatusCode.Failed)
            {
                Console.WriteLine("waiting...");
                jobInProgress = client.GetJob(jobInProgress.JobId);
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }


        public static void LoadTweets(JobSubmissionCertificateCredential creds, AzureSettings settings)
        {

              var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "Load tweets to external table",
                StatusFolder = "/AAALoadTweets",

                Query = "select identifiers.identifier, Z.X.estfrequency as tweetCount  from (select explode(word_map) as X from ( SELECT context_ngrams(sentences(lower(tweetText)), array(null), 1000) as word_map FROM tweets ) struct) Z join identifiers on identifiers.identifier = Z.X.ngram[0]"
            };


            // Submit the Hive job
            var jobClient = JobSubmissionClientFactory.Connect(creds);
            var jobResults = jobClient.CreateHiveJob(hiveJobDefinition);

            WaitForJobCompletion(jobResults, jobClient);


            // Print the Hive job output
            var stream = jobClient.GetJobOutput(jobResults.JobId);

            var reader = new StreamReader(stream);
            Console.WriteLine(reader.ReadToEnd());


        }

        public static void CreateTables(JobSubmissionCertificateCredential creds, AzureSettings settings)
        {

            var storagePath = String.Format("wasb://{0}@{1}.blob.core.windows.net/", settings.ClusterName, settings.StorageAccount);
            var TweetInPath = storagePath + @"/Tweets";
            var IdentifiersInPath = storagePath + @"/Identifiers";

            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "Create external tables",
                StatusFolder = "/AAACreateTables",

                Query = "DROP TABLE tweets; CREATE EXTERNAL TABLE tweets( id_str string, created_at string, retweet_count string, tweetText string, userName string, userId string, screenName string, countryCode string, placeType string, placeName string, placeType1 string, coordinates array<string>)" +
                "ROW FORMAT DELIMITED FIELDS TERMINATED BY '\t' COLLECTION ITEMS TERMINATED BY ',' STORED AS TEXTFILE location '" + TweetInPath + "';" +
                "DROP TABLE identifiers; CREATE EXTERNAL TABLE identifiers(identifier string)" +
                "ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' STORED AS TEXTFILE location '" + IdentifiersInPath + "';" 
            };


            // Submit the Hive job
            var jobClient = JobSubmissionClientFactory.Connect(creds);
            var jobResults = jobClient.CreateHiveJob(hiveJobDefinition);

            WaitForJobCompletion(jobResults, jobClient);


            // Print the Hive job output
            var stream = jobClient.GetJobOutput(jobResults.JobId);

            var reader = new StreamReader(stream);
            Console.WriteLine(reader.ReadToEnd());


        }


       
        private static JobSubmissionCertificateCredential GenerateJobSubmissionCert(AzureSettings settings)
        {
            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.FriendlyName == settings.LocalCertName);
            var creds = new JobSubmissionCertificateCredential(new Guid(settings.SuscriptionId), cert, settings.ClusterName);
            return creds;
        }


       

        static void Main(string[] args)
        {

            string subscriptionId = ConfigurationManager.AppSettings["AzureSubscriptionID"];
            string clusterName = ConfigurationManager.AppSettings["ClusterName"];
            string certfriendlyname = ConfigurationManager.AppSettings["AzurePublisherCertName"];
            string storageAccount = ConfigurationManager.AppSettings["AzureStorageAccount"];

            var settings = new AzureSettings
                                         {
                                             ClusterName = clusterName,
                                             LocalCertName = certfriendlyname,
                                             SuscriptionId = subscriptionId,
                                             StorageAccount = storageAccount
                                         };

            var creds = GenerateJobSubmissionCert(settings);

            Console.WriteLine("START " + DateTime.Now.ToLongTimeString());
            //CreateTables(creds, settings);
            LoadTweets(creds, settings);
             
            Console.WriteLine("END " + DateTime.Now.ToLongTimeString());

            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();


        }
    }
}

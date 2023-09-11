
# SendNotifications

## Description
- The Application Reads data from Database using an Timer Triggered Azure Function and then writes converts the data to ServiceBusMessage and writes the data to the specified ServiceBus Queue. 

## Running the Application Locally

### Local.Settings.Json
- The Following should be the contents of local.settings.json

      {
        "IsEncrypted": false,
        "Values": {
          "AzureWebJobsStorage": "UseDevelopmentStorage=true",
          "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    
          // SQL Server connection string
          "Values:SQLConnectionString": "<your-sql-server-connection-string>",
    
          // Service Bus connection string
          "Values:ServiceBusConnectionString": "<your-service-bus-connection-string-with-sharedAccessKey>",
          "Values:ServiceBusQueueName": "<queueName>"
        }
      }


- local.settings.example.json is also included in the project for reference, kindly copy the local.settings.example.json and replace the connection strings and queue name to run the application locally

### Database
If you want to setup the database for this code base then please follow the steps as mentioned below:

- Create Table with the following script

        SET ANSI_NULLS ON
        GO
        SET QUOTED_IDENTIFIER ON
        GO
        CREATE TABLE [dbo].[UserNotification](
    	    [RecordId] [int] IDENTITY(1,1) NOT NULL,
    	    [UserId] [nvarchar](50) NOT NULL,
    	    [UserName] [nvarchar](50) NOT NULL,
    	    [Email] [nvarchar](100) NOT NULL,
    	    [DataValue] [nvarchar](max) NOT NULL,
    	    [NotificationFlag] [tinyint] NOT NULL,
    	    [CreatedDateTime] [datetime] NOT NULL
    	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    	GO
    	ALTER TABLE [dbo].[UserNotification] ADD  CONSTRAINT [PK_UserNotification] PRIMARY KE
    	CLUSTERED 
    	(
    	  [RecordId] ASC
    	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    	GO
    	ALTER TABLE [dbo].[UserNotification] ADD  CONSTRAINT [DEFAULT_UserNotification_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
    	GO
- Create Stored Procedure with the following script

        SET  ANSI_NULLS  ON
        GO
        SET  QUOTED_IDENTIFIER  ON
        GO
        CREATE  PROCEDURE [dbo].[GetRemainingUserDataNotification]
        @LastExecutedDateTime DATETIME
        AS
        SET  NOCOUNT  ON;
        SELECT [RecordId]
        ,[UserId]
        ,[UserName]
        ,[Email]
        ,[DataValue]
        ,[NotificationFlag]
        FROM dbo.UserNotification
        Where CreatedDateTime > @LastExecutedDateTime
        GO

- Please note that if you change the name of Stored-Procedure, Table or Columns then you will have to explicitly update it in the code. 




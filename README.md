Meter Reading App

Description

This tool is designed to process and validate CSV files containing meter readings. The application checks for duplicate records, validates reading formats, and filters out invalid account IDs based on specific criteria. Once validated, the valid records are saved to a database for further processing.
Assumptions

Before using this tool, here are some key assumptions I made while developing the application:

    Duplicate Entries: A "duplicate entry" is defined as a record where the AccountId, MeterReadingDateTime, and MeterReadValue are exactly the same. The tool assumes that a "duplicate" means the entry already exists in the database (not just in the CSV file). If a record with identical values already exists in the database, it will not be reloaded.

    Reading Value Format: Reading values are expected to be in the format NNNNN (a 5-digit number). Any rows with MeterReadValue values that exceed 5 digits will be invalidated and removed.

    Valid Account IDs: The application will only process entries for specific valid AccountIds. Entries with an account ID not on the valid list will be excluded from processing.

How to Run the Program

To get started with the tool, follow these steps:
Prerequisites

    Visual Studio: Ensure that you have Visual Studio installed with support for .NET applications.
    SQL Server: You need to have a local SQL Server instance running.
    Restore Database: In the solution directory, you'll find a backup file MeterReadingApp.bak. This file contains the necessary database schema and data. You will need to restore this backup to your local SQL Server instance.

Steps to Run

    Open the Solution: Open the solution in Visual Studio.
    Run the Application: Press F5 or select Run to start the application using IIS Express.
    Swagger Interface: Once the application is running, Swagger will automatically load in your browser. This provides a convenient interface for interacting with the API.
    Upload the CSV: Use the Swagger interface to upload your CSV file containing meter readings. The tool will validate the file and display the results.

Validation Logic

The tool validates meter readings against the following rules:

    Duplicate Checks: No duplicate records will be processed. A duplicate is defined as a record where the AccountId, MeterReadingDateTime, and MeterReadValue are identical to an existing entry in the database.

    Meter Reading Length: The MeterReadValue should be a 5-digit value (i.e., between 00000 and 99999). Any row with a value that exceeds this length will be discarded.

    Valid Account IDs: Only records with AccountIds from the pre-defined list of valid account IDs will be considered. All other account IDs will be ignored.

    When an account has an existing record, any row with a value that is older than the existing record will be discarded.


Additional Comments

    Further Clarification Needed: If this task had been assigned in a real-world setting, I would have sought additional clarification from the product owner or client before proceeding. Some assumptions made during development, such as the definition of "duplicate entries" or the expected format of the MeterReadValue, should ideally have been clarified upfront to ensure the correct functionality.

    Performance Considerations: If given more time, I would have revisited the approach for querying the database, considering performance improvements such as using stored procedures or other optimized database access patterns. This would be particularly important for large datasets to reduce load times and improve efficiency.

Future Improvements

    Optimized Querying: As mentioned, a performance optimization strategy, such as using stored procedures or indexed queries, could help scale this solution for larger datasets.
    Logging Enhancements: I would implement more detailed logging, especially for edge cases or errors, to make debugging easier and improve visibility into the application's operations.
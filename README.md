## QA-checks
This Web API project serves as a system for managing production orders and QA checks. It allows users to perform CRUD (create, read, update, delete) operations on orders, creating QA checks
and generating report PDF file with the information about order and ongoing process.

### Prerequisites
To run this project, you'll need the following:

.NET 7 SDK installed on your machine, an IDE such as Visual Studio or Visual Studio Code installed and SQL Server.

Clone this repository to your local machine:<br/>
<b>````git clone https://github.com/IhorZhylchuk/QA-checks.git````</b>
<br/>
<br/>
Navigate to the project directory and restore dependencies:
<br/>
<b>```cd YOUR_REPOSITORY```</b><br/>
<b>```dotnet restore```</b><br/>

Open the project in IDE, in <b>```appsettings.json```</b> enter your server name, run the following commands:<br/>
<b>```add-migration MIGRATION_NAME```</b> then run <br/>
<b>```update-database```</b><br/>

You can run Api by pressing the "Play" button in your IDE or by running the following command in the terminal:
<br/>
<b>```dotnet run```</b>
<br/><br/>

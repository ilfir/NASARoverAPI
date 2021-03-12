# NASARover App


Items included in the solution:
1. NASA API integration. API key and url are set in appsettings.
2. Added docker support. Current project is configured to run on docker.
3. Downloading files locally. Files are downloaded to a docker volume specified in appsettings.
4. Ability to initiate downloading of files via API and Web UI. 
5. Ability to view locally downloaded files in UI. 
6. Two unit tests added. More needed, this is just a start.

Functionality included in UI: 
1. Ability to initiate download. 
2. Ability to view files. 

Functionality in API: 
1. Initiate download. 
2. View metadata for downloaded files. 

Application settings in appsettings.json: 
- ApiKey - used in all API calls to NASA endpoints. 
- ApiUrl - base url for all calls except when downloading images. 
- localFolder - base folder for downloading.
- rovers - list of rover names to query.

Notes:
- When downloading files, metadata for the files is also saved on local volume with limited information for displaying list of images in UI.
- Folder structure for image files: [base volume]/[date]/[rover name]/. For metadata: [base volume]/metadata.
- Since there was no API endpoint to list the rovers, I included the list of rovers in the appsettings. 
- I am currently not floating the message for one of the dates being invalid to UI/API, work is needed there. 
- Only two small unit tests are added. Need to add more. 
- Developed using non-windows environment. Visual Studio and Postman on Mac were used. Good learning exercise for myself. 
- Postman collection is available here for API endpoints: https://www.getpostman.com/collections/f6c82db1906612d3e59e

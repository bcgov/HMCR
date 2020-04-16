# HMCR

As part of the Highway Maintenance Contract Renewal (HMCR) process the business area defined the reporting requirements for the Maintenance Contractors (MCs). The contracts outlined the fields that have to be reported on and the format that it needs to be reported in.

This presents a problem on how to best collect the data being provided by the MCs and ensure its quality, so that it can be put to use for the Program and Ministry needs. Through this project the program wants to meet the need to automate the data gathering, as well as data validation process then capture the successfully validated data in a database which can then be immediately available to HQ and District Offices.

## Prerequisites

- .Net Core 3.1 SDK
- Node.JS v10.0 or newer
- Microsoft SQL Server 2017 or newer

## Dependencies

- Working KeyCloak Realm with BC Gov IDIR and BCeID
- Ministry of Transportation and Infrastructure GeoServer access
- IDIR service account with access to BC Gov BceID WebService

## Local Development

### Configuration

Use the following steps to configure the local development environment

1. Clone the repository

   ```
   git clone https://github.com/bcgov/HMCR.git
   ```

2. Create the HMR_DEV database in MS SQL Server

   - Delete all existing tables
   - Run scripts in `database/V01.1` directory
   - Apply incremental scripts `(V14.1 to Vxx.x)` in ascending order
   - Create the first admin user in `HMR_SYSTEM_USER` table and assign the `SYSTEM_ADMIN` role in the `HMR_USER_ROLE` table

3. Configure API Server settings

   - Copy `api/Hmcr.API/appsettigns.json` to `api/Hmcr.API/appsettigns.Development.json`
   - Update the placeholder values with real values, eg., replace the `<app-id>` with actual KeyCloak client id in the `{ "JWT": { "Audience": "<app-id>" } }` field
   - Update the connection string to match the database
   - Make note of or update the port for the API Server in Visual Studio or through the `properties/launchSettings.json` file.

4. Configure Hangfire Server settings

   - Copy `api/Hmcr.Hangfire/appsettigns.json` to `api/Hmcr.Hangfire/appsettigns.Development.json`
   - Update the placeholder values with real values, eg., replace the `<ServiceAccount:User>` with actual IDIR service account in the `{ "ServiceAccount": { "User": "<ServiceAccount:User>" } }` field
   - Update the connection string to match the database

5. Configure the React development settings

   - Create the `client/.env.development.local` file and add the following content

   ```
    # use port value from step 3
    REACT_APP_API_HOST=http://localhost:<api-port>

    REACT_APP_SSO_HOST=https://sso-dev.pathfinder.gov.bc.ca/auth
    REACT_APP_SSO_CLIENT=<client-id>
    REACT_APP_SSO_REALM=<realm-id>

    REACT_APP_DEFAULT_PAGE_SIZE_OPTIONS=25,50,100,200
    REACT_APP_DEFAULT_PAGE_SIZE=25

    # Optional, default port is 3000
    # PORT=3001
   ```

   - Replace the placeholder values

### Run

Use the following steps to run the local development environment

1. Run the API Server

   - F5 in Visual Studio
   - Or from console

   ```
   cd api/Hmcr.Api
   dotnet restore
   dotnet build
   dotnet run
   ```

2. Run the Hangfire Server. _It's only neccessary to run the Hangfire Server if debugging Hangfire jobs_

   - F5 in Visual Studio
   - Or from console

   ```
   cd api/Hmcr.Api
   dotnet restore
   dotnet build
   dotne
   ```

3. Run the React frontend
   ```
   cd client
   npm install
   npm start
   ```

## OpenShift Deployment

Refer to [this document](openshift/README.md) for OpenShift Deployment and Pipeline related topics

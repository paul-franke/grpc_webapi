# grpc_webapi: A microservice supporting both WebApi and gRPC


gRPC_webAPI is a microservice that bridges the REST with  gRPC protocol. A typical use case for this microservice is when you want to allow direct external access via REST to a hybrid gRPC internal landscape.

## Features
- gRPC
- WebApi
- HTTP2
- TLS
- Containerized

## Caveats
- No authorization nor authentication has been implemented.
- This is a first release. Microservice has been developed on windows, so expect some platform dependencies still to be present.
- This code is meant as a starter, so not production ready.
- During install you will need to install a self signed development certificate for docker support. This is one of features I will reconsider for a next release. 

## Setup

### Docker-setup
1. Clone repo
2. Start docker desktop
3. Execute (on windows):

        ./certificate.bat    
        docker-compose up


### Using Visual-Studio
1. Clone repo
2. Open solution file
3. Select profile: Docker or local:
4. Run

## Description of the sample app: A tiny session_manager
To demo this microservice there is functionality implemented for a tiny session_manager. 

It's the session_manager's job to setup a session. The external environment is serviced by the WebApi. The Internal environment is serviced by gRPC.

It's the app's responsibility to do the bookkeeping of the session information. As such the session_manager hands out a token (GUID) for a to be created session. The creation and control of actual sessions should be done by another microservice using that token, all in line with the Single responsibility principle.  

The tiny session_manager uses at this moment an in memory cache. This will be persisted to a Postgres database in a future release. 

### Design session_manager

Please see the file: ./design/SessionManagement.drawio

### WebApi functionality
- Request a session to be started.
- Request a running session to stopped.  
- Status queries for the session

### gRPC functionality
gRPC is in charge of the internal communication.
- Communicate with "internal" processes hosting the sessions.

## Testing the microservice

### WebApi Swagger

Swagger allows interaction with all endpoints. 
- Docker: https://localhost:39771/swagger
- Visual-Studio: browser is started automatically.

### gRPC grpcurl

Dependent upon your preferences you can use Postman or grpcurl. In the tools-directory there is a win64-version included of grpcurl. 

Below there will be the grpcurl-commands given to communicate with the microservice.

Note: change your port in accordance with your run parameters.

- GetSessionStatus

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\"}" localhost:443 grpc.SessionManagerServerDefinition/GetSessionStatus

- GetSessionAllow

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\"}" localhost:443 grpc.SessionManagerServerDefinition/GetSessionAllow


- SetSessionStatus

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\", , \"Status\" : <int32> }" localhost:443 grpc.SessionManagerServerDefinition/SetSessionStatus

- SetSessionAllow

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\", \"Attended\" : true, \"UnAttended\" : true }" localhost:443 grpc.SessionManagerServerDefinition/SetSessionAllow



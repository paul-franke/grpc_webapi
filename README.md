# grpc_webapi: A microservice supporting both WebApi and gRPC


## Server
gRPC_WebApi is a microservice that services both the REST and gRPC protocol. A typical use case for this microservice is when you want to allow direct external access via REST to a hybrid gRPC internal landscape.

## Clients
Also included with this repo are 2 fully functional clients i.e., a gRPC-client and a Rest-client. These clients are using a `BackGroundService` to communicate with the server on a timed basis. 

These projects can be used as a starter for interacting with your own customized server.

### Certificate pinning

Both clients showcase certificate pinning. <https://owasp.org/www-community/controls/Certificate_and_Public_Key_Pinning>

In docker certificate pinning is turned on and fully functional, for Visual Studio the certificate thumbprints need to be copied into the `appsettings.json`. 

## Features
- gRPC
- WebApi
- HTTP2
- TLS
- Containerized
- Certificate pinning
- A Rest and a gRPC client using this service

## Caveats
- No authorization nor authentication has been implemented.
- Microservice has been developed on windows, so expect some platform dependencies still to be present.
- This code is meant as a starter, so not production ready.

## Setup

### Generic
1. Clone repo
2. Execute in windows cmd-box:    
* goto repo root
* cd rss
* certificate.bat


### Docker-setup

1. Start docker desktop
2. docker-compose up

Note: For docker hosting, Certificate pinning is on by default.

### Visual-Studio
1. Open solution file
2. Run
3. Optional: Certificate Thumbprints, see below under "Customizing the Thumbprints".

Note: For Visual Studio hosting, Certificate pinning is off by default.

## Description of the sample app: A tiny session_manager
To demo this microservice there is functionality implemented for a tiny session_manager. 

It's the app's responsibility to do the bookkeeping of the session information. As such the session_manager hands out a token (GUID) for a to be created session. The creation and control of actual sessions should be done by another microservice using that token, all in line with the Single responsibility principle.  

The tiny session_manager uses at this moment an in memory cache. This will be persisted to a Postgres database in a future release. 

### Design session_manager

Please see the file: `./design/SessionManagement.drawio`

### WebApi functionality
- Request a session to be started.
- Request a running session to stopped.  
- Status queries for the session

### gRPC functionality
- Communicate with "internal" processes hosting the sessions.

## Testing the microservice

### WebApi Swagger

Swagger allows interaction with all endpoints. 
- Docker: https://localhost:443/swagger

### gRPC grpcurl

Dependent upon your preferences you can use Postman or grpcurl. In the tools-directory there is a win64-version included of grpcurl. 

Below the grpcurl-commands are given to communicate with the microservice.


- GetSessionStatus

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\"}" localhost:443 grpc.SessionManagerServerDefinition/GetSessionStatus

- GetSessionAllow

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\"}" localhost:443 grpc.SessionManagerServerDefinition/GetSessionAllow


- SetSessionStatus

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\", \"Status\" : <int32> }" localhost:443 grpc.SessionManagerServerDefinition/SetSessionStatus

- SetSessionAllow

        grpcurl -d "{ \"Guid\" : \"<your session Guid>\", \"Attended\" : true, \"UnAttended\" : true }" localhost:443 grpc.SessionManagerServerDefinition/SetSessionAllow

## The two Clients

### Generic

Both clients are BackgroundServices i.e., implementations of the .Net class `BackgroundService`. In the `ExecuteAsync` method there is an infinite loop that only ends when they `CancellationToken` signals to do so. Outside the infinite loop is a communication-client constructed, by the `RestHttpClientFactory` respectively `SessionManagerServerDefinitionClient` class. In the loop this constructed class is used to communicate with the server. In the factory classes the specifics are being implemented such as certificate pinning.

### Configuration

Configure by setting the values in the `appsettings.json` and/or in the `docker-compose.yml` file.

Both clients allow to following settings:
```    
    public class GrpcConfigurationOptions
    {

        public string? LeafCertificateThumbString { get; set; }
        public string? IntermediateCertificateThumbString { get; set; }
        public bool CertificatePinning { get; set; }
        public string? url { get; set; }
        public int GrpcClientTimeOut {  get; set; }
    }
```

You can set the thumbprint for a Leaf and an Intermediate certificate. To use pinning, the boolean `CertificatePinning` must be set to `true`, if set to false standard certificate verification will be done. The `url` is the address of the server and `ClientTimeOut` is the maximum wait time after which a request is aborted.

### Actual processing

The actual processing in the clients are small request to the server, just to showcase the functionality. 

### Customizing the Thumbprints

#### Docker

In Docker they are pre-filled when the certificate is created. 

#### Visual Studio

In Visual Studio the thumbprints must be filled/supplied manually. 

Please, complete the `appsettings.json` for both clients with the values of the `repo root\rss\.env` file. 

## Docker

Docker spins up the the server `rss_base` and the two test client `grpc_client` and `rest_client`.  Besides these servers also Postgres is being spun up as a preparation for a implementation of persistency.

## Certificate handling

To allow all clients and the server to use TLS seamlessly some actions were needed. 
The purpose is to let the three-way handshake succeed, see for an explanation <https://cabulous.medium.com/tls-1-2-andtls-1-3-handshake-walkthrough-4cfd0a798164> 

### Certificate creation

The batchfile `certificate.bat` creates a developer certificate and saves the certificate as `./certificate/aspnetapp.pfx`. This certificate is used by the server as part of the Visual Studio solution and by the server when running as a container in Docker. 

 `certificate.bat` also saves the thumbprint of the certificate to the file `.env`. Docker retrieves these values during startup:

```
    environment:
      - GrpcConfigurationOptions__LeafCertificateThumbString=${LEAF}
      - GrpcConfigurationOptions__IntermediateCertificateThumbString=${INTERMEDIATE}
```


### Server side certificate 

As described above the certificate is accessible as `./certificate/aspnetapp.pfx`.

### Client side certificate

Docker/Linux and Windows differ here substantially.

#### Windows

 `certificate.bat` loads the certificate in the certificate store and marks it as a trusted certificate thereby allowing `https` traffic. 

#### Docker

This is still an open issue. The certificate is still to be be added to the trusted certificates. However, since pinning is on by default, full functionality is available.

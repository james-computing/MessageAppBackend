# MessageAppBackend

This is a project for the backend for a real-time messaging app, to put in practice multiple technologies used for backend development. The following technologies were used in this project:

<ul>
  <li> C#, .NET; </li>
  <li> REST APIs with ASP.NET Core; </li>
  <li> ASP.NET Core SignalR for real-time communication; </li>
  <li> Json Web Tokens (JWT) for authentication and authorization; </li>
  <li> Microsoft SQL Server for a relational database; </li>
  <li> Dapper to access the database with finer control; </li>
  <li> Stored procedures, to make database execution faster; </li>
  <li> Docker for containerization; </li>
  <li> Kubernetes for deploying the application containers in a cluster; </li>
  <li> Apache Kafka as messaging queue; </li>
  <li> Strimzi operator to run a Kafka cluster in Kubernetes; </li>
  <li> NGINX Gateway Fabric to make a gateway for the application in Kubernetes; </li>
  <li> xUnit for testing the application. </li>
</ul>

# Overview

The app is made for different clients to communicate with each other in real-time. The communication is done inside rooms that the users must create and join. The basic flow of the app is as follows:

<ul>
  <li> The user registers itself; </li>
  <li> The user login to get a JWT for authentication, which is required to use the app; </li>
  <li> The user can create new rooms, to which it joins automatically; </li>
  <li> Other users can be invited to join a room; </li>
  <li> Messages can be sent to a room in real-time through a SignalR connection; </li>
  <li> Messages from a room can be received by a client in real-time if it is connected via SignalR. Otherwise, the messages can be loaded by a REST API.</li>
</ul>

For web apps, real-time communication is done with WebSockets. Specifically for ASP.NET Core, SignalR can be used instead, which has WebSockets underneath it. Real-time communication is important mainly for a client to receive messages. I expect that the client will be connected to the server through SignalR while using the app. Being already connected, it makes sense to use such connection to send messages. For this reason, I've chosen that **SignalR will be used both to send and receive messages in real-time**, not only to receive messages. Other functionality of the app doesn't require real-time communication and is done by REST APIs.

# Architecture

The app is structured in multiple services that implement different functionality. The services are the following:

<ul>
  <li> Auth: the authentication service; </li>
  <li> REST: a REST API to manage rooms and its messages; </li>
  <li> MessageRealTime: a web app that uses ASP.NET Core SignalR for real-time communication; </li>
  <li> KafkaConsumer: a web app with a Kafka consumer, to consume messages produced by other services. </li>
</ul>

# Auth service

The entry point of the app is the Auth service. A user must first register using the Auth service, then it must login to get a JWT. It receives an access token for authentication and a refresh token. The access token expires faster than the refresh token. The refresh token has the only purpose of letting the user request a new access token without having to login again. After the refresh token expires, the user must login another time. The Auth service is responsible for managing the users, so it is used both to register a new user and deleting it.

All other services that a user can access require authentication, so the user must have a valid JWT to access them.

# REST service

The REST service have all the REST APIs that are outside the Auth service. It is responsible for interactions between client and server that doesn't require real-time communication. The service has two controllers, Message and Rooms. The Message controller is responsible for managing users massages. Loading messages, editing or deleting them can be done with the Message controller. The Rooms controller is responsible for managing the rooms. This includes creating and deleting rooms, inviting users to rooms, joining rooms and removing users from rooms.

# MessageRealTime service

The MessageRealTime service is responsible for stablishing real-time communication between client and server. Since most functionality of the app doesn't require real-time communication, this service is only responsible for letting a client send messages to a room and receive messages from rooms in real-time. The message sent by a client is first saved in a relational database, which automatically generates an id for the message. This id let users identify the message and ask for editing of deleting a message. The client who sent the message also receives a confirmation that the message was sent together with the id generated for the message.

There is a single url that the client can access for this service, which it uses to send a message. The client doesn't request messages from this service, rather they are pushed automatically to the clients in real-time. If a client isn't connected, it won't receive messages by this service, but the messages are stored in a database. To get these messages, the REST service must be used.

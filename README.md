# MessageAppBackend

This is a project of the backend for a real-time messaging app, to put in practice multiple technologies used for backend development. The following technologies were used in this project:

<ul>
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

# Basic idea

The app is made for different clients to communicate with each other in real-time. The communication is done inside rooms that the users must create and join. The basic flow of the app is as follows:

<ul>
  <li> The user registers itself; </li>
  <li> The user login to get a JWT for authentication, which is required to use the app; </li>
  <li> The user can create new rooms, to which it joins automatically; </li>
  <li> Other users can be invited to join a room; </li>
  <li> Messages can be sent to a room in real-time with a SignalR connection; </li>
  <li> Messages from a room can be received by a client in real-time if it is connected via SignalR; </li>
</ul>

For web apps, real-time communication is done with WebSockets. Specifically for ASP.NET Core, SignalR can be used instead, which has WebSockets underneath it. Real-time communication is important mainly for a client to receive messages. I expect that the client will be connected to the server through SignalR while a frontend app is being used (a frontend isn't implemented in this project). Therefore, I would expect the client to already have a SignalR connection when it is sending a message to a room. For this reason, I've chosen that **SignalR will be used both to send and receive messages in real-time**. Other functionality of the app doesn't require real-time communication and is done by REST APIs.

# Architecture

Communication between users happens inside rooms that they must create and join.

The app has multiple services:

<ul>
  <li> Auth: the authentication service; </li>
  <li> REST: a REST API to manage room
</ul>

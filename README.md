# MassTransit3Demo
A simple MassTransit3 demo uses RabbitMQ, Autofac, Serilog

4 projects

1. MassTransit3Demo.Core
Includes all consumers, middlewares and settings

2. MassTransit3Demo.Messages
Includes all messages to be sent over the queue

3. MassTransit3Demo.Service
A Topshelf Windows Service to handle messages

4. MassTransit3Demo.Web
A single page application

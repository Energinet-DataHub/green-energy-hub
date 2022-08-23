# System Overview

## External communication flow

The system is based on asynchrous message exchange. An actor can invoke actions by sending a message. The message is routed to a domain, from here it will be validated and executed upon.

Outbound messages is written to a queue. Actors are responsible for peeking and dequeuing messages.

## Internal communication flow

All communication within the system is based on publish/subscriber or request/reply pattern. We are using a message broker component to handle the communication.

![image](https://user-images.githubusercontent.com/16430/186102456-c0fc5d64-f05e-47c2-b232-5c7e6e6a9493.png)

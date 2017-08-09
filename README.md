# Sample-Cloud-Deployment-Engine
An implementation of a simple cloud deployment engine in C# that uses event-driven programming. The application consists of:

· A Cloud deployment engine, who is given YAML specifications representing the services that need to be deployed;

· A Cloud provider, that handles the creation and the destruction of the services the engine needs to deploy;

· A Recursive descent parser that parses YAML specifications

The Cloud Deployment Engine (CDE) is used to deploy applications to the Cloud Provider (CP). The CP can be asked either to create or destroy an agent of a given kind. The CDE reads a YAML specification that lists the services to be deployed, the relations among them and the events that may occur during their lifetime. An event can be raised by the CDE when all the preconditions (specified in the "after" clause) are satisfied. Actions triggered by events are executed asynchronously, and results are communicated back to the CDE by the agents. 

The CDE can receive additional specifications after the first one: to handle these situations, when given a new description, it compares the current state with the new one and performs the actions needed to reach that state (e.g. create more instances of an agent, additional relations or events).

In this project, there are two possible kinds of agents, "moodle" and "database", which are implemented respectively by the classes "Moodle" and "Mysql". 

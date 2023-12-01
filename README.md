# kafka-protobuf-example
An example of a Kafka Producer / Consumer using Protobuf

* Run `docker-composeup`
* Go to your Kafka UI at `localhost:8080`
* Go to the Schema Registry tab and add the following schema with the name `SimpleMessage`:
```
syntax = "proto3";
package kafka-protobuf-example;

message SimpleMessage {
  string content = 1;
  string date_time = 2;
}
```
You'll see a copy of this in the project `protobufs` folder.

At this point we'll have a couple options
* We can use [protobuf-net](https://github.com/protobuf-net/protobuf-net)
* We'd have to use the website https://protogen.marcgravell.com/ to generate the C# code
* Or we could use the `protogen` tool
* `dotnet tool run protogen **/*.proto --csharp_out=.`
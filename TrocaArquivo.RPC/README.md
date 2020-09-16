# grpc tool command

```
protoc -I=./protos -I=/home/$USER/.nuget/packages/google.protobuf.tools/3.12.3/tools/ --csharp_out=./generated_code --grpc_out=./generated_code ./protos/*.proto --plugin=protoc-gen-grpc=/home/$USER/.nuget/packages/grpc.tools/2.30.0/tools/linux_x64/grpc_csharp_plugin
```

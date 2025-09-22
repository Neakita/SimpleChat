using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleChat.API.Contracts;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthenticationRequest))]
[JsonSerializable(typeof(AuthenticationResponse))]
public sealed partial class ContractsSourceGenerationContext : JsonSerializerContext;
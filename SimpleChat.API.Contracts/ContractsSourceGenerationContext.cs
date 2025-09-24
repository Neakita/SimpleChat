using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleChat.API.Contracts;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthenticationRequest))]
[JsonSerializable(typeof(AuthenticationResponse))]
[JsonSerializable(typeof(RefreshAccessTokenResponse))]
[JsonSerializable(typeof(IEnumerable<UserInfoResponse>))]
[JsonSerializable(typeof(IEnumerable<DirectMessageResponse>))]
[JsonSerializable(typeof(SendDirectMessageRequest))]
public sealed partial class ContractsSourceGenerationContext : JsonSerializerContext;
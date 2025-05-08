using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Owl.Enums;
using Owl.Models;
using Owl.Models.Requests;
using Serilog;
using Environment = Owl.Models.Environment;

namespace Owl.Importers;

internal record struct InsomniaV4Export
{
	[JsonPropertyName("_type")] public string Type { get; init; }
	[JsonPropertyName("__export_format")] public uint ExportFormat { get; init; }
	[JsonPropertyName("__export_date")] public DateTime ExportDate { get; init; }
	[JsonPropertyName("__export_source")] public string ExportSource { get; init; }
	public InsomniaV4Resource[] Resources { get; init; }
}

internal record struct InsomniaV4Header
{
	public string Name { get; init; }
	public string Value { get; init; }
}

internal record struct InsomniaV4ResourceBody
{
	public string MimeType { get; init; }
	public string Text { get; init; }
}

internal record struct InsomniaV4Parameter
{
	[JsonPropertyName("_id")] public string Id { get; init; }
	public string Name { get; init; }
	public string Value { get; init; }
	public string Description { get; init; }
	public bool Disabled { get; init; }
}

internal record struct InsomniaV4ResourceAuthentication
{
	public string? Type { get; init; }
	public string? Token { get; init; }
}

internal record struct InsomniaV4Resource
{
	[JsonPropertyName("_id")] public string Id { get; init; }
	public string ParentId { get; init; }
	public long Modified { get; init; }
	public long Created { get; init; }
	public string Url { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }

	[JsonConverter(typeof(InsomniaV4ResourceMethodConverter))]
	public HttpRequestMethod Method { get; init; }

	public InsomniaV4ResourceBody Body { get; init; }
	public InsomniaV4Parameter[] Parameters { get; init; }
	public InsomniaV4Header[] Headers { get; init; }
	public InsomniaV4ResourceAuthentication Authentication { get; init; }
	public double MetaSortKey { get; init; }

	public bool IsPrivate { get; init; }

	// TODO: Type this properly
	public object[] PathParameters { get; init; }
	public bool SettingStoreCookies { get; init; }
	public bool SettingSendCookies { get; init; }
	public bool SettingDisableRenderRequestBody { get; init; }
	public bool SettingEncodeUrl { get; init; }
	public bool SettingRebuildPath { get; init; }
	public string SettingFollowRedirects { get; init; }

	[JsonPropertyName("_type")]
	[JsonConverter(typeof(InsomniaV4ResourceTypeConverter))]
	public InsomniaV4ResourceType Type { get; init; }
}

internal enum InsomniaV4ResourceType
{
	Environment,
	Request,
	RequestGroup,
	Workspace,
	CookieJar,
	UnitTestSuite,
}

internal class InsomniaV4ResourceMethodConverter : JsonConverter<HttpRequestMethod>
{
	private static readonly Dictionary<string, HttpRequestMethod> Map = new()
	{
		{ "GET", HttpRequestMethod.Get },
		{ "POST", HttpRequestMethod.Post },
		{ "PUT", HttpRequestMethod.Put },
		{ "DELETE", HttpRequestMethod.Delete },
		{ "PATCH", HttpRequestMethod.Patch },
		{ "OPTION", HttpRequestMethod.Option },
		{ "CONNECT", HttpRequestMethod.Connect },
		{ "HEAD", HttpRequestMethod.Head },
		{ "TRACE", HttpRequestMethod.Trace },
	};

	public override HttpRequestMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && Map.TryGetValue(reader.GetString(), out var result))
		{
			return result;
		}

		throw new JsonException($"Unsupported HttpRequestMethod method type: {reader.GetString()}");
	}

	public override void Write(Utf8JsonWriter writer, HttpRequestMethod method, JsonSerializerOptions options)
	{
		var stringValue = Map.FirstOrDefault(x => x.Value == method).Key;
		if (stringValue is not null)
		{
			writer.WriteStringValue(stringValue);
			return;
		}

		throw new JsonException($"Cannot serialize resource type: {method}");
	}
}

internal class InsomniaV4ResourceTypeConverter : JsonConverter<InsomniaV4ResourceType>
{
	private static readonly Dictionary<string, InsomniaV4ResourceType> TypeMap = new()
	{
		{ "environment", InsomniaV4ResourceType.Environment },
		{ "request", InsomniaV4ResourceType.Request },
		{ "request_group", InsomniaV4ResourceType.RequestGroup },
		{ "workspace", InsomniaV4ResourceType.Workspace },
		{ "cookie_jar", InsomniaV4ResourceType.CookieJar },
		{ "unit_test_suite", InsomniaV4ResourceType.UnitTestSuite },
	};

	public override InsomniaV4ResourceType Read(ref Utf8JsonReader reader, Type typeToConvert,
		JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && TypeMap.TryGetValue(reader.GetString(), out var result))
		{
			return result;
		}

		throw new JsonException($"Unknown InsomniaV4ResourceType resource type: {reader.GetString()}");
	}

	public override void Write(Utf8JsonWriter writer, InsomniaV4ResourceType value, JsonSerializerOptions options)
	{
		var stringValue = TypeMap.FirstOrDefault(x => x.Value == value).Key;
		if (stringValue != null)
		{
			writer.WriteStringValue(stringValue);
			return;
		}

		throw new JsonException($"Cannot serialize resource type: {value}");
	}
}

internal class InsomniaV4Importer
{
	// TODO: Should return Owl structure, probably even something like ICollection or something, throw on failure
	public OwlCollection Parse(string content)
	{
		Log.Debug("Parsing insomniaV4 content.");
		// TODO: Check for null
		var export = JsonSerializer.Deserialize<InsomniaV4Export>(content,
			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

		OwlCollection collection = new OwlCollection();

		//TODO: refactor
		MapExport(export, ref collection);

		Log.Debug("Finished parsing insomniaV4 export.");

		return collection;
	}

	// TODO: Should return bool and out Owl structure, probably even something like ICollection or something, return false on failure
	public void TryParse(string content)
	{
	}

	private List<RequestHeader> MapHeaders(InsomniaV4Header[] headers)
	{
		List<RequestHeader> result = [];
		result.AddRange(headers.Select(MapHeader));

		return result;
	}

	private static RequestHeader MapHeader(InsomniaV4Header header)
	{
		return new RequestHeader
		{
			IsEnabled = true,
			Key = header.Name,
			Value = header.Value
		};
	}

	private IList<IRequest> MapRequests(IEnumerable<InsomniaV4Resource> resources)
	{
		var collection = new List<IRequest>();
		foreach (var resource in resources)
		{
			switch (resource.Type)
			{
				case InsomniaV4ResourceType.Request:
					collection.Add(MapHttpRequest(resource));
					continue;

				default:
					Log.Error("Owl supports only request resource types as a children for this resource.");
					throw new ArgumentOutOfRangeException();
			}
		}

		return collection;
	}

	private IRequest MapHttpRequest(InsomniaV4Resource resource)
	{
		return new HttpRequest
		{
			Name = resource.Name,
			Method = resource.Method,
			Url = resource.Url,
			Body = resource.Body.Text,
			Headers = MapHeaders(resource.Headers),
			Parameters = MapParameters(resource.Parameters),
			Auth = MapAuth(resource.Authentication),
		};
	}

	private static RequestAuth? MapAuth(InsomniaV4ResourceAuthentication resource)
	{
		if (resource.Type is null) return null;

		return new RequestAuth
		{
			// TODO: Let's introduce Enum for the auth type
			Scheme = resource.Type,
			Token = resource.Token ?? string.Empty,
		};
	}

	private List<RequestParameter> MapParameters(InsomniaV4Parameter[] parameters)
	{
		List<RequestParameter> result = [];
		result.AddRange(parameters.Select(MapParameter));
		return result;
	}

	private static RequestParameter MapParameter(InsomniaV4Parameter resource)
	{
		return new RequestParameter
		{
			IsEnabled = resource.Disabled,
			Key = resource.Name,
			Value = resource.Value,
			Description = resource.Description,
		};
	}

	private void MapExport(InsomniaV4Export export, ref OwlCollection collection)
	{
		var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
		foreach (var resource in export.Resources)
		{
			switch (resource.Type)
			{
				case InsomniaV4ResourceType.Environment:
					// TODO: Add environment variables if such thing exists in Insomnia
					collection.Environments.Add(new Environment { Name = resource.Name });
					continue;

				case InsomniaV4ResourceType.Request:
					// All that has parent with "wrk_....." is "root level" with no real folder, and we can map it right away
					if (!resource.ParentId.StartsWith("wrk_"))
						continue;

					// TODO: Map other params properly
					// TODO: There are other types of "body" we need to map those properly
					collection.Requests.Add(MapHttpRequest(resource));
					continue;

				// TODO: We need to do this in recursion
				case InsomniaV4ResourceType.RequestGroup:
					// TODO: Apparently we need to map Ids first so we can later take other resources and affiliate them with this group
					var children = export.Resources.Where(x => x.ParentId == resource.Id);

					collection.Requests.Add(new GroupRequest
						{ Name = resource.Name, Children = MapRequests(children) });
					continue;

				default:
					Log.Error($"FIXME: INSOMNIAV4IMPORTER RESOURECE TYPE {resource.Name} NOT IMPLEMENTED");
					continue;
			}
		}
	}
}

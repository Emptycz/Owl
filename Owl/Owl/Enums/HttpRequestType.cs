using Owl.Attributes;

namespace Owl.Enums;

public enum HttpRequestType
{
	[DisplayName("GET")]
	Get,
	[DisplayName("POST")]
	Post,
	[DisplayName("PUT")]
	Put,
	[DisplayName("UPDATE")]
	Update,
	[DisplayName("DELETE")]
	Delete
}

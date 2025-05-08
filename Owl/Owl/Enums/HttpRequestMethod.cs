using Owl.Attributes;

namespace Owl.Enums;

/**
 * https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
 */
public enum HttpRequestMethod
{
	[Value("GET")]
	Get,
	[Value("HEAD")]
	Head,
	[Value("POST")]
	Post,
	[Value("PUT")]
	Put,
	[Value("DELETE")]
	Delete,
	[Value("CONNECT")]
	Connect,
	[Value("OPTION")]
	Option,
	[Value("TRACE")]
	Trace,
	[Value("PATCH")]
	Patch,
}

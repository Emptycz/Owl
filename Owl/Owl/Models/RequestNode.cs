using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Owl.Models;

public class RequestNode : INotifyPropertyChanged
{
    private string _method = "GET";
    private string _name = string.Empty;
    
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string? Url { get; set; }
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public HttpStatusCode? StatusCode { get; set; }

    public IEnumerable<RequestNode> Children { get; set; } = new List<RequestNode>();

    public string Method
    {
        get => _method;
        set
        {
            _method = value;
            OnPropertyChanged(nameof(Method));
        }
    }

    public RequestNode()
    {
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}